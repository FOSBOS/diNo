use diNo;

ALTER TABLE Fach ADD schuelerfach_id NVARCHAR(40);
ALTER TABLE Fach ADD schule_fach_id NVARCHAR(40);

--insert into GlobaleStrings values (29,'MailAdresseTest','stefan.hein@fos-sonthofen-cloud.de')
--ALTER TABLE Schueler ADD CONSTRAINT chk_schueler_geschlecht CHECK (Geschlecht IN ('M','W','D'));
--ALTER TABLE Schueler ADD asv_id NVARCHAR(40);


-- bereits im Produktivsystem eingespielte Änderungen:
-- ALTER TABLE KlasseKurs ADD PRIMARY KEY (KursId,KlasseId)
-- ALTER TABLE dbo.Schueler ADD MailSchule NVARCHAR(50) NULL DEFAULT ''