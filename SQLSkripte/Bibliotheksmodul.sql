use diNo;

-- Bibliotheksmodul: Buecher-Stammdaten, Ausleihtabelle, Standardbuchungen je Schuelerkonstellation

CREATE TABLE Buecher (
    Id INT IDENTITY PRIMARY KEY,
    Fach NVARCHAR(20) NULL,              -- Fachkuerzel, wie in Buecher.Fach der alten Access-DB (z.B. "E4", "M3/2")
    Titel NVARCHAR(255) NOT NULL,
    ISBN NVARCHAR(50) NULL,
    Autor NVARCHAR(255) NULL,
    Verlag NVARCHAR(100) NULL,
    Preis DECIMAL(10,2) NULL,
    Bestand INT NOT NULL DEFAULT 0,      -- Gesamtzahl vorhandener Exemplare an der Schule
    Aktiv BIT NOT NULL DEFAULT 1         -- inaktive Buecher verschwinden aus Auswahllisten/Standardsaetzen
);

CREATE TABLE SchuelerBuecher (          -- Ausleihtabelle
    Id INT IDENTITY PRIMARY KEY,
    SchuelerId INT NOT NULL REFERENCES Schueler(Id) ON DELETE CASCADE,
    BuchId INT NOT NULL REFERENCES Buecher(Id),
    AusgegebenAm DATE NOT NULL DEFAULT GETDATE(),
    RueckgabeAm DATE NULL                -- NULL = noch ausgeliehen
);

CREATE TABLE BuchStandardsatz (          -- Standardbuchungen je Schuelerkonstellation
    Id INT IDENTITY PRIMARY KEY,
    Jahrgangsstufe TINYINT NOT NULL,      -- Werte wie Klasse.JgStufe (8/9/10/11/12/13)
    Schulart TINYINT NULL,                -- wie Klasse.Schulart (1=FOS, 2=BOS), NULL = beide
    Zweig NVARCHAR(1) NULL,               -- wie Klasse.Data.Zweig ('S','T','W','U'), NULL = alle Zweige
    BuchId INT NOT NULL REFERENCES Buecher(Id)
);

-- neue Rolle "Bibliothek" fuer Sonderpersonen, die Buch-Stammdaten/Standardsaetze pflegen und die
-- schulweite Sammel-Einbuchung durchfuehren duerfen (zusaetzlich zu Admin/Sekretariat). Id muss zum
-- C#-Enum Rolle.Bibliothek in Zugriff.cs passen. Ids 9-13 sind in dieser DB bereits belegt
-- (IV, FVKSTU, FVKW, BVKSTW, Dummy) und im C#-Enum nicht abgebildet - daher Id 14.
INSERT INTO Rolle (Id, Bezeichnung) VALUES (14, 'Bibliothek');

-- Freischalt-Option fuer den Bibliothek-Reiter: 0 = nur Admins/Rolle Bibliothek sehen ihn,
-- 1 = alle Lehrer sehen ihn (per Checkbox "Bibliothek anzeigen" in der Administration umschaltbar).
ALTER TABLE GlobaleKonstanten ADD BibliothekAnzeigen INT NOT NULL DEFAULT 0;
