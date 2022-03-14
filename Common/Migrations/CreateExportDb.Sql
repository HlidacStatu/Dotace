START TRANSACTION;

CREATE SCHEMA IF NOT EXISTS exportready;

CREATE TABLE IF NOT EXISTS exportready.dotace (
    id text NOT NULL,
    iddotace text NOT NULL,
    datumpodpisu timestamp with time zone NULL,
    datumaktualizace timestamp with time zone NULL,
    kodprojektu text NULL,
    nazevprojektu text NULL,
    duplicita text NULL,
    rozhodnuti jsonb NOT NULL,
    chyba jsonb NULL,
    programnazev text NULL,
    programkod text NULL,
    prijemceico text NULL,
    prijemceobchodnijmeno text NULL,
    prijemcehlidacjmeno text NULL,
    prijemcejmeno text NULL,
    prijemceroknarozeni integer NULL,
    prijemceobec text NULL,
    prijemceokres text NULL,
    prijemcepsc text NULL,
    prijemceulice text NULL,
    zdrojnazev text NOT NULL,
    zdrojurl text NOT NULL,
    CONSTRAINT pk_dotace PRIMARY KEY (id)
);

COMMIT;