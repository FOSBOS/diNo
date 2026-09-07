use diNo;

ALTER TABLE Fach ADD schuelerfach_id NVARCHAR(40);
ALTER TABLE Fach ADD schule_fach_id NVARCHAR(40);

-- Umbau Anschriften/Wiederholungen auf 1:n-Tabellen (ASV-Import)
-- Keine Datenmigration nötig, da die Umstellung zum neuen Schuljahr erfolgt und Schueler ohnehin per
-- ErzeugeInitialzustand.sql geleert und aus ASV neu importiert wird.
CREATE TABLE SchuelerAnschrift (
    Id INT IDENTITY PRIMARY KEY,
    SchuelerId INT NOT NULL REFERENCES Schueler(Id),
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
    SchuelerId INT NOT NULL REFERENCES Schueler(Id),
    Schuljahr NVARCHAR(10) NULL,
    Jahrgangsstufe NVARCHAR(10) NULL,
    Grund NVARCHAR(80) NULL
);

-- Erst ausführen, nachdem der neue Code (Schueler.cs, ASVImporter.cs, UI) deployed ist:
--ALTER TABLE Schueler DROP COLUMN AnschriftStrasse, AnschriftPLZ, AnschriftOrt, AnschriftTelefonnummer;
--ALTER TABLE Schueler DROP COLUMN NachnameEltern1, VornameEltern1, AnredeEltern1, VerwandtschaftsbezeichnungEltern1;
--ALTER TABLE Schueler DROP COLUMN NachnameEltern2, VornameEltern2, AnredeEltern2, VerwandtschaftsbezeichnungEltern2;
--ALTER TABLE Schueler DROP COLUMN Wiederholung1Jahrgangsstufe, Wiederholung1Grund, Wiederholung2Jahrgangsstufe, Wiederholung2Grund;

--insert into GlobaleStrings values (29,'MailAdresseTest','stefan.hein@fos-sonthofen-cloud.de')
--ALTER TABLE Schueler ADD CONSTRAINT chk_schueler_geschlecht CHECK (Geschlecht IN ('M','W','D'));
--ALTER TABLE Schueler ADD asv_id NVARCHAR(40);


-- bereits im Produktivsystem eingespielte Änderungen:
-- ALTER TABLE KlasseKurs ADD PRIMARY KEY (KursId,KlasseId)
-- ALTER TABLE dbo.Schueler ADD MailSchule NVARCHAR(50) NULL DEFAULT ''