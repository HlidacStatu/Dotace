require 'active_support/all'
require 'oj'
require 'roo-xls'

FILENAME = 'data/Udelene_investicni_pobidky.xls'
DATASET_URL = 'https://www.czechinvest.org/cz/Sluzby-pro-investory/Investicni-pobidky'
DATA_DIR = '../../data/czechinvest/out'

DEFAULT_SHEET = 'PROJEKTY'
START_ROW = 3
HEADERS = %i[num name ico sector cz_nace investing_subject group_country investee_country eur_investment usd_investment czk_investment created_work_positions investment_taxes investment_work_positions investment_requalification investment_land investment_capital public_support public_support_ceil district region nuts_ii request_year decision_day decision_month decision_year msp cancel cancel_reason invested_without_incentive]
CZECH_MONTHS = %w[leden únor březen duben květen červen červenec srpen září říjen listopad prosinec]

def create_dirs
  FileUtils.mkdir_p(DATA_DIR)
end

def parse_date(day, month, year)
  if month.is_a?(Date)
    month.to_s
  else
    day_i = day.to_i
    Date.new(year, CZECH_MONTHS.index(month.strip) + 1, day_i.zero? ? 1 : day_i).to_s
  end
end

def transform_row(donation_id, row)
  donation_date = parse_date(row[:decision_day], row[:decision_month], row[:decision_year])
  donation_size = row[:czk_investment] * 1_000_000.0
  ceil = row[:public_support_ceil].to_i
  if ceil.zero?
    ceil = nil
  else
    ceil *= 1_000_000
  end

  {
    podpisDatum: donation_date,
    idDotace: donation_id,
    idProjektu: nil,
    nazevProjektu: row[:investing_subject],
    kodProjektu: nil,
    dotaceCelkem: donation_size,
    pujckaCelkem: 0,
    verejnaPodpora: row[:public_support],
    stropVerejnePodpory: ceil,
    zadostZrusena: row[:cancel].present?,
    rozhodnuti: [{
      datum: donation_date,
      castkaPozadovana: donation_size,
      castkaRozhodnuta: donation_size,
      zdrojFinanci: 'CZ',
      poskytovatelNazev: nil,
      poskytovatelIco: nil,
      jePujcka: false,
    }],
    program: {
      id: nil,
      nazev: row[:investing_subject],
      kod: nil,
      url: nil,
    },
    prijemce: {
      ico: row[:ico].presence,
      prijemceObchodniJmeno: nil,
      jmenoPrijemce: row[:name],
      prijemceOkresNazev: row[:district],
      prijemceObecNazev: nil,
      prijemcePSC: nil,
    },
    zdrojUrl: DATASET_URL,
    zdrojNazev: 'czechinvest',
  }
end

def generate_donation_id(company_name, donation_id)
  "CZECHINVEST-#{Digest::MD5.hexdigest(company_name)}-#{donation_id}"
end

def parse_rows
  sheet = Roo::Spreadsheet.open(FILENAME)
  sheet.default_sheet = DEFAULT_SHEET

  date = sheet.row(1)[1].match(/\d+\. \w+ \d{4}/).to_a.first
  p "Soubor investic ze dne: #{date}"

  end_row = sheet.column(1)[START_ROW..].index(&:nil?)

  donation_counts = Hash.new { |h, k| h[k] = 0 }

  ((START_ROW + 1)..end_row).map do |row_idx|

    parsed_row = HEADERS.zip(sheet.row(row_idx)).to_h
    company_name = parsed_row[:name]
    donation_counts[company_name] += 1

    donation_id = generate_donation_id(company_name, donation_counts[company_name])

    transform_row(donation_id, parsed_row)
  end
end

def out_filename
  "#{DATA_DIR}/czechinvest.json"
end

def transform_to_hlidac(force: false)
  return if !force && File.exist?(out_filename)

  parsed_rows = parse_rows
  File.open(out_filename, 'w') { |f| f.write(Oj.dump(parsed_rows), mode: :compat) }
end

def main(force: false)
  create_dirs
  transform_to_hlidac(force: force)
end

main
