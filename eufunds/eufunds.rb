require 'active_support/all'
require 'creek'
require 'oj'
require 'typhoeus'
require 'nokogiri'

DATA_DIR = '../../data/eufunds/out'

OLD_SPREADSHEET_FILE = 'data/Prehled_projektu_20160603upraveno.xlsx'
OLD_KEYS = {
  program_name: 'A',
  project_id: 'D',
  project_name: 'E',
  receiver_name: 'G',
  receiver_id: 'H',
  receiver_type: 'I',
  signature_date: 'L',
  receiver_address: 'M',
  receiver_district_name: 'O',
  receiver_city_name: 'Q',
  total_resources: 'R',
  public_resources: 'S',
  eu_resources: 'T',
  public_received_resources: 'U',
  eu_received_resources: 'V',
  total_received_resources: 'W',
}

def file_for_hlidac
  "#{DATA_DIR}/hlidac.json"
end

def clean_donation_id(donation_id)
  donation_id.tr('/.', '-')
end

def parse_old_row(row)
  donation_id = clean_donation_id(row[OLD_KEYS[:project_id]])
  donation_id = "#{donation_id}-#{@donation_ids_counter[donation_id]}"
  @donation_ids_counter[donation_id] += 1

  {
    podpisDatum: row[OLD_KEYS[:signature_date]],
    prijemce: {
      ico: row[OLD_KEYS[:receiver_id]],
      prijemceObchodniJmeno: (row[OLD_KEYS[:receiver_type]]&.strip == 'Podnikající fyzická osoba tuzemská') ? nil : row[OLD_KEYS[:receiver_name]],
      jmenoPrijemce: (row[OLD_KEYS[:receiver_type]]&.strip == 'Podnikající fyzická osoba tuzemská') ? row[OLD_KEYS[:receiver_name]] : nil,
      prijemceOkresNazev: row[OLD_KEYS[:receiver_district_name]],
      prijemceObecNazev: row[OLD_KEYS[:receiver_city_name]],
      prijemcePSC: row[OLD_KEYS[:receiver_address]]&.match(/^\d+/)&.[](0),
    },
    zdrojUrl: nil,
    zdrojNazev: 'eurofondy',
    idDotace: "EU-OLD-#{donation_id}",
    idProjektu: row[OLD_KEYS[:project_id]],
    nazevProjektu: row[OLD_KEYS[:project_name]],
    kodProjektu: row[OLD_KEYS[:project_id]],
    dotaceCelkem: row[OLD_KEYS[:total_received_resources]],
    pujckaCelkem: 0,
    rozhodnuti: [{
      datum: row[OLD_KEYS[:signature_date]],
      castkaPozadovana: row[OLD_KEYS[:eu_resources]],
      castkaRozhodnuta: row[OLD_KEYS[:eu_received_resources]],
      zdrojFinanci: 'EU',
      poskytovatelNazev: nil,
      poskytovatelIco: nil,
      jePujcka: false,
    }, {
      datum: row[OLD_KEYS[:signature_date]],
      castkaPozadovana: row[OLD_KEYS[:public_resources]],
      castkaRozhodnuta: row[OLD_KEYS[:public_received_resources]],
      zdrojFinanci: 'CZ',
      poskytovatelNazev: nil,
      poskytovatelIco: nil,
      jePujcka: false,
    }],
    program: {
      id: nil,
      nazev: row[OLD_KEYS[:program_name]],
      kod: nil,
      url: nil,
    },
  }
end


def parse_old
  sheet = Creek::Book.new(OLD_SPREADSHEET_FILE).sheets[0]
  sheet.simple_rows.select { |row| row[OLD_KEYS[:project_id]].start_with?('CZ') }.map(&method(:parse_old_row))
end

NEW_KEYS = {
  donation_id: 'ID',
  program_id: 'ID_VYZVA',
  project_code: 'KOD',
  project_name: 'NAZ',
  date: 'DZRSKUT',
  receiver_name: 'ZAD/NAZ',
  receiver_id: 'ZAD/IC',
  district_name: 'ZAD/ADR/OKNAZEV',
  city_name: 'ZAD/ADR/OBNAZEV',
  area_code: 'ZAD/ADR/PSC',
  resources_used_total: 'PF/CZV',
  resources_eu: 'PF/EU',
  resources_cs: 'PF/CNV',
  resources_cs_private: 'PF/SN',
  resources_private: 'PF/S',
  resources_total: 'PF/CV',
}

def new_key(project, key, multi: false)
  nodes = project.xpath("./#{NEW_KEYS[key]}")
  multi ? nodes.map(&:text) : nodes.text
end

def parse_new_row(project)
  date_string = new_key(project, :date)
  date =
    begin
      Date.parse(date_string).to_s
    rescue ArgumentError
      nil
    end

  decisions = {
    'EU' => new_key(project, :resources_eu, multi: true).map(&:to_f).sum,
    'CZ' => new_key(project, :resources_cs, multi: true).map(&:to_f).sum,
    'CZ_PRIVATE' => new_key(project, :resources_cs_private, multi: true).map(&:to_f).sum,
    'PRIVATE' => new_key(project, :resources_private, multi: true).map(&:to_f).sum,
  }.map do |resources_key, resources|
    next nil if resources.blank? || resources.to_f.zero?

    {
      datum: date,
      castkaPozadovana: resources,
      castkaRozhodnuta: resources,
      zdrojFinanci: resources_key,
      poskytovatelNazev: nil,
      poskytovatelIco: nil,
      jePujcka: false,
    }
  end.compact

  donation_id = clean_donation_id(new_key(project, :project_code))
  donation_id = "#{donation_id}-#{@donation_ids_counter[donation_id]}"
  @donation_ids_counter[donation_id] += 1

  {
    podpisDatum: date,
    prijemce: {
      ico: new_key(project, :receiver_id),
      prijemceObchodniJmeno: nil,
      jmenoPrijemce: new_key(project, :receiver_name),
      prijemceOkresNazev: new_key(project, :district_name),
      prijemceObecNazev: new_key(project, :city_name),
      prijemcePSC: new_key(project, :area_code),
    },
    zdrojUrl: nil,
    zdrojNazev: 'eurofondy',
    idDotace: "EU-NEW-#{donation_id}",
    idProjektu: nil,
    nazevProjektu: new_key(project, :project_name),
    kodProjektu: new_key(project, :project_code),
    dotaceCelkem: new_key(project, :resources_used_total, multi: true).map(&:to_f).sum,
    pujckaCelkem: 0,
    rozhodnuti: decisions,
    program: {
      id: new_key(project, :program_id),
      nazev: new_key(project, :project_name),
      kod: nil,
      url: nil,
    },
  }
end

def parse_new
  xml_data = Typhoeus.get('https://ms14opendata.mssf.cz/SeznamProjektu.xml').body

  nokogiri = Nokogiri::XML(xml_data)
  nokogiri.remove_namespaces!

  projects = nokogiri.xpath('//PRJ')
  projects.map(&method(:parse_new_row))
end

def create_dirs
  FileUtils.mkdir_p(DATA_DIR)
end

def parse_files(force: false)
  return if !force && File.exist?(file_for_hlidac)

  @donation_ids_counter = Hash.new { |h, k| h[k] = 0 }

  old_rows = parse_old
  new_rows = parse_new

  p old_rows.size
  p new_rows.size

  File.open(file_for_hlidac, 'w') { |f| f.write(Oj.dump((old_rows + new_rows).uniq { |row| row[:idDotace] }, mode: :compat)) }
end

def main(force: false)
  create_dirs
  parse_files(force: force)
end

main
