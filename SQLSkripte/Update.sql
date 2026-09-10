use diNo;


-- Umbau Anschriften/Wiederholungen auf 1:n-Tabellen (ASV-Import)
CREATE TABLE SchuelerAnschrift (
    Id INT IDENTITY PRIMARY KEY,
    SchuelerId INT NOT NULL REFERENCES Schueler(Id) ON DELETE CASCADE,
    AnschriftWessen NVARCHAR(2) NOT NULL,   -- ASV-Code: 1=Erz.-berecht., 2=weitere/r Erz.-berecht., 3=Schüler, 4=weitere Anschrift
    Anschriftstyp NVARCHAR(2) NULL,
    NachnamePerson NVARCHAR(60) NULL,       -- nur befüllt wenn AnschriftWessen <> 3 (Erziehungsberechtigte)
    VornamePerson NVARCHAR(60) NULL,
    AnredePerson NVARCHAR(20) NULL,
    Verwandtschaftsbezeichnung NVARCHAR(40) NULL,
    Strasse NVARCHAR(60) NULL,
    Hausnummer NVARCHAR(10) NULL,
    PLZ NVARCHAR(10) NULL,
    Ort NVARCHAR(60) NULL,
    Telefonnummer NVARCHAR(40) NULL,
    Mobilnummer NVARCHAR(40) NULL,
    Email NVARCHAR(80) NULL,
    HauptAnsprechpartner BIT NULL,
    Auskunftsberechtigt BIT NULL
);

CREATE TABLE SchuelerWiederholung (
    Id INT IDENTITY PRIMARY KEY,
    SchuelerId INT NOT NULL REFERENCES Schueler(Id) ON DELETE CASCADE,
    Schuljahr NVARCHAR(10) NULL,
    Jahrgangsstufe NVARCHAR(10) NULL,
    Grund NVARCHAR(80) NULL
);

ALTER TABLE Schueler DROP COLUMN AnschriftStrasse, AnschriftPLZ, AnschriftOrt, AnschriftTelefonnummer,Email;
ALTER TABLE Schueler DROP COLUMN NachnameEltern1, VornameEltern1, AnredeEltern1, VerwandtschaftsbezeichnungEltern1;
ALTER TABLE Schueler DROP COLUMN NachnameEltern2, VornameEltern2, AnredeEltern2, VerwandtschaftsbezeichnungEltern2;
ALTER TABLE Schueler DROP COLUMN Wiederholung1Jahrgangsstufe, Wiederholung1Grund, Wiederholung2Jahrgangsstufe, Wiederholung2Grund;
ALTER TABLE Schueler DROP COLUMN Notfalltelefonnummer, MittlereReifeDeutschnote, MittlereReifeMathenote, MittlereReifeEnglischnote;
ALTER TABLE Schueler ADD FranzB1 BIT NOT NULL default 0, SpanischB1 BIT NOT NULL default 0;

