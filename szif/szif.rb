require 'active_support/all'
require 'typhoeus'
require 'nokogiri'
require 'byebug'
require 'oj'

DATA_DIR = '../../data/szif/out'
PARALLEL_REQUESTS = 500
YEARS = {
  '2014' => 683,
  '2015' => 690,
  '2016' => 641,
  '2017' => 650,
  '2018' => 661,
}

def create_dirs
  YEARS.keys.each { |year| FileUtils.mkdir_p("#{DATA_DIR}/#{year}") }
end

def decorate_path(path)
  "https://www.szif.cz/cs/seznam-prijemcu-dotaci#{path}"
end

# Nokogiri::HTML(response.body).css('.container-table td a').map { |link| decorate_path(link.attr('href')) }
def parse_links(request)
  links = request.response.body.scan(/<a href="(\?[^>]*)">/).flatten.map { |path| decorate_path(path) }

  if links.blank?
    raise "empty links with response url: #{request.url} [#{request.response.body}]"
  end

  links
end

def fetch_parallel_urls(urls, parallel: PARALLEL_REQUESTS)
  parallel ||= PARALLEL_REQUESTS

  working_urls = urls
  valid_requests = []

  idx = 0

  while working_urls.present?
    p "Fetching #{working_urls.size} urls (batch #{idx})"
    sleep(1) if idx > 0
    idx += 1

    p 'preparing hydra'
    hydra = Typhoeus::Hydra.new(max_concurrency: parallel)
    url_requests = working_urls.map { |url| request = Typhoeus::Request.new(url); hydra.queue(request); [url, request] }

    p 'running hydra'
    hydra.run

    working_urls = []

    p 'iterating requests'
    url_requests.each do |url, request|
      if request.response.body.blank? || request.response.body.include?('<p class="messageManagerERROR">')
        working_urls << url
      else
        valid_requests << request
      end
    end
  end

  valid_requests
end

def list_file_path(year)
  "#{DATA_DIR}/links_#{year}.csv"
end

def check_link_files(force: false)
  YEARS.each do |year, max_page|
    link_file_path = list_file_path(year)
    next if File.exist?(link_file_path) && !force

    urls = (1..max_page).map { |page_num| "https://www.szif.cz/cs/seznam-prijemcu-dotaci?asc=asc&year=#{year}&page=#{page_num}" }
    requests = fetch_parallel_urls(urls)

    links = requests.flat_map(&method(:parse_links)).uniq

    File.open(link_file_path, 'w') { |f| f.write(links.join("\n")) }
  end
end

def individual_file(year, id)
  "#{DATA_DIR}/#{year}/#{id}.json"
end

def parse_individual_row(row)
  %i[year fund_name provision resources_cz resources_eu resources_total].zip(row.css('td').map(&:text).map(&:strip)).to_h
end

def parse_individual(year, request)
  individual_id = request.url.match(/ji=(\d+)&/)[1]
  individual_file = individual_file(year, individual_id)

  return if File.exists?(individual_file)

  nokogiri = Nokogiri::HTML(request.response.body)
  container_tables = nokogiri.css('.container-table')

  name = nokogiri.at_css('.section h3').text
  district = nokogiri.at_xpath("//div[@class = 'section']/div[contains(., 'okres')]").text

  data = {
    name: name,
    district: district,
  }

  if container_tables.size == 0
    data.merge!(intervention_finances: nokogiri.css('section tbody tr').map do |row|
      year, fund_name, provision, resources = row.css('td').map(&:text).map(&:strip)

      %i[year fund_name provision resources_cz resources_eu resources_total].zip([year, fund_name, provision, resources, '0,00', resources]).to_h
    end)
  else
    data.merge!(finances: container_tables[0].css('tbody tr').map(&method(:parse_individual_row)))
    data.merge!(temporary_finances: container_tables[1].css('tbody tr').map(&method(:parse_individual_row))) if container_tables.size > 1
  end


  File.open(individual_file, 'w').write(Oj.dump(data))
end

def url_from_individual_file(year, individual_file)
  "https://www.szif.cz/cs/seznam-prijemcu-dotaci?ji=#{individual_file.gsub(/\.json$/, '')}&opatr=&year=#{year}&portalAction=detail"
end

def existing_individual_files(year)
  Dir["#{DATA_DIR}/#{year}/*.json"]
end

def download_yearly_files(year)
  all_files = File.read(list_file_path(year)).split("\n")
  existing_files = existing_individual_files(year).map { |file| url_from_individual_file(year, file.split('/').last) }

  return if (all_files - existing_files).empty?

  p "downloading year #{year}"

  group_size = 100

  p "groups count: #{((all_files - existing_files).size / group_size) + 1}"

  (all_files - existing_files).in_groups_of(group_size, false).each_with_index do |file_group, group_idx|
    p "parsing group #{group_idx}"
    fetch_parallel_urls(file_group).each { |request| parse_individual(year, request) }
  end
end

def download_individual_files
  YEARS.keys.each do |year|
    download_yearly_files(year)
  end
end

def merged_yearly_file(year)
  "#{DATA_DIR}/#{year}.json"
end

def clean_up_temporary_finances(temporary_finances)
  temporary_finances
    .select { |row| row[:year].present? }
    .map { |row| row.merge(resources_eu: '0,00', resources_total: row[:resources_cz]) }
end

def merge_files_for_year(year, force: false)
  return if !force && File.exist?(merged_yearly_file(year))

  p "creating yearly files for year #{year}"
  data = []

  existing_individual_files(year).map do |file|
    individual = Oj.load(File.read(file))
    individual.merge!(temporary_finances: clean_up_temporary_finances(individual[:temporary_finances])) if individual[:temporary_finances].present?
    individual.merge!(szif_id: file.split('/').last.gsub(/\.json$/, ''))
    data << individual
  end

  File.open(merged_yearly_file(year), 'w').write(Oj.dump(data, mode: :compat))
end

def merge_yearly_files(force: false)
  YEARS.keys.each do |year|
    merge_files_for_year(year, force: force)
  end
end

def file_for_hlidac
  "#{DATA_DIR}/hlidac.json"
end

def parse_number(value)
  value.gsub(/[[:space:]]/, '').tr(',', '.').to_f
end

def parse_district(district)
  if district.include?(', ')
    obec, okres = district.split(', ')
  else
    obec = nil
    okres = district
  end

  [obec&.strip, okres.gsub('okres ', '').strip]
end

def parse_shared_individual(year, row)
  obec, okres = parse_district(row[:district].gsub(/[[:space:]]/, ' '))

  {
    podpisDatum: "#{year}-01-01",
    prijemce: {
      ico: nil,
      prijemceObchodniJmeno: nil,
      jmenoPrijemce: row[:name],
      prijemceOkresNazev: okres,
      prijemceObecNazev: obec,
      prijemcePSC: nil,
    },
    zdrojUrl: url_from_individual_file(year, row[:szif_id]),
    zdrojNazev: 'szif',
  }
end

def non_zero?(value)
  value.present? && !value.zero?
end

def parse_finances(year, row, parsed_individual, finances, slug)
  (finances || []).map.with_index(1) do |finance_row, donation_idx|
    donation_id = "SZIF-#{year}#{slug.present? ? "-#{slug}" : ''}-#{row[:szif_id]}-#{donation_idx}"

    cz_value = parse_number(finance_row[:resources_cz])
    eu_value = parse_number(finance_row[:resources_eu])
    total_value = parse_number(finance_row[:resources_total])

    decisions = []
    if non_zero?(cz_value)
      decisions << {
        datum: "#{year}-01-01",
        castkaPozadovana: cz_value,
        castkaRozhodnuta: cz_value,
        zdrojFinanci: 'CZ',
        poskytovatelNazev: finance_row[:fund_name],
        poskytovatelIco: nil,
        jePujcka: false,
      }
    end

    if non_zero?(eu_value)
      decisions << {
        datum: "#{year}-01-01",
        castkaPozadovana: eu_value,
        castkaRozhodnuta: eu_value,
        zdrojFinanci: 'EU',
        poskytovatelNazev: finance_row[:fund_name],
        poskytovatelIco: nil,
        jePujcka: false,
      }
    end

    parsed_individual.merge(
      idDotace: donation_id,
      idProjektu: '',
      nazevProjektu: finance_row[:provision],
      kodProjektu: nil,
      dotaceCelkem: total_value,
      pujckaCelkem: 0,
      rozhodnuti: decisions,
      program: {
        id: nil,
        nazev: finance_row[:fund_name],
        kod: nil,
        url: nil,
      },
    )
  end
end

def transform_individual_file_to_hlidac(year, row)
  individual = parse_shared_individual(year, row)

  hlidac_rows = []

  hlidac_rows += parse_finances(year, row, individual, row[:finances], nil)
  hlidac_rows += parse_finances(year, row, individual, row[:temporary_finances], 'TMP')
  hlidac_rows += parse_finances(year, row, individual, row[:intervention_finances], 'INT')

  hlidac_rows
end

def transform_year_file_to_hlidac(year)
  Oj.load(File.read(merged_yearly_file(year)), symbol_keys: true).map do |file|
    transform_individual_file_to_hlidac(year, file)
  end.flatten
end

def transform_to_hlidac(force: false)
  return if !force && File.exist?(file_for_hlidac)

  rows = YEARS.keys.map(&method(:transform_year_file_to_hlidac)).flatten

  File.open(file_for_hlidac, 'w') { |f| f.write(Oj.dump(rows, mode: :compat)) }
end

def main
  create_dirs
  check_link_files
  download_individual_files
  merge_yearly_files

  transform_to_hlidac
end

main
