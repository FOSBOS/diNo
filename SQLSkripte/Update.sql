use diNo;

ALTER TABLE Schueler DROP COLUMN Punktesumme;
ALTER TABLE Schueler DROP COLUMN Bestanden;

EXEC sp_rename 'Schueler.DNoteAllg', 'DNoteFachgebHSR', 'COLUMN';

