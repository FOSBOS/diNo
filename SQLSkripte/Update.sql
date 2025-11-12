use diNo;


-- bereits im Produktivsystem eingespielte Änderungen:
-- ALTER TABLE Schueler ADD CONSTRAINT chk_schueler_geschlecht CHECK (Geschlecht IN ('M','W','D'));
-- ALTER TABLE dbo.Schueler ADD MailSchule NVARCHAR(50) NULL DEFAULT ''