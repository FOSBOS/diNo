USE [diNo]
GO

/****** Object:  View [dbo].[vwNotenbogen]    Script Date: 14.03.2016 16:58:50 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[vwNotenbogen]
AS
SELECT        dbo.Klasse.Bezeichnung, dbo.Lehrer.Name AS Klassenleiter, dbo.Seminarfachnote.Gesamtnote, dbo.Seminarfachnote.ThemaLang, 
                         dbo.Seminarfachnote.ThemaKurz, dbo.Schueler.Id, dbo.Schueler.Name, dbo.Schueler.Vorname, dbo.Schueler.KlasseId, dbo.Schueler.Rufname, 
                         dbo.Schueler.Geschlecht, dbo.Schueler.Geburtsdatum, dbo.Schueler.Geburtsort, dbo.Schueler.Bekenntnis, dbo.Schueler.AnschriftPLZ, dbo.Schueler.AnschriftOrt, 
                         dbo.Schueler.AnschriftStrasse, dbo.Schueler.AnschriftTelefonnummer, dbo.Schueler.Ausbildungsrichtung, dbo.Schueler.Fremdsprache2, 
                         dbo.Schueler.ReligionOderEthik, dbo.Schueler.Wahlpflichtfach, dbo.Schueler.Wahlfach1, dbo.Schueler.Wahlfach2, dbo.Schueler.Wahlfach3, dbo.Schueler.Wahlfach4, 
                         dbo.Schueler.Wiederholung1Jahrgangsstufe, dbo.Schueler.Wiederholung2Jahrgangsstufe, dbo.Schueler.Wiederholung1Grund, dbo.Schueler.Wiederholung2Grund, 
                         dbo.Schueler.ProbezeitBis, dbo.Schueler.Austrittsdatum, dbo.Schueler.SchulischeVorbildung, dbo.Schueler.BeruflicheVorbildung, dbo.Schueler.LRSStoerung, 
                         dbo.Schueler.LRSSchwaeche, dbo.Schueler.LRSBisDatum, dbo.Schueler.NachnameEltern1, dbo.Schueler.VornameEltern1, dbo.Schueler.AnredeEltern1, 
                         dbo.Schueler.VerwandtschaftsbezeichnungEltern1, dbo.Schueler.NachnameEltern2, dbo.Schueler.VornameEltern2, dbo.Schueler.AnredeEltern2, 
                         dbo.Schueler.VerwandtschaftsbezeichnungEltern2, dbo.Schueler.EintrittJahrgangsstufe, dbo.Schueler.EintrittAm, dbo.Schueler.EintrittAusSchulnummer, 
                         dbo.Schueler.Email, dbo.Schueler.Notfalltelefonnummer, dbo.Schueler.DNote, dbo.Schueler.Status
FROM            dbo.Seminarfachnote RIGHT OUTER JOIN
                         dbo.Lehrer INNER JOIN
                         dbo.Schueler INNER JOIN
                         dbo.Klasse ON dbo.Schueler.KlasseId = dbo.Klasse.Id ON dbo.Lehrer.Id = dbo.Klasse.KlassenleiterId ON dbo.Seminarfachnote.SchuelerId = dbo.Schueler.Id

GO

ALTER TABLE [dbo].[Lehrer] ADD Vorname NVARCHAR(50) NULL;
ALTER TABLE [dbo].[Lehrer] ADD Nachname NVARCHAR(50) NULL;
UPDATE Lehrer SET Nachname = substring(Name, CHARINDEX(' ', NAME, 1) + 1, 256),
Vorname = substring(Name, 1, CHARINDEX(' ', NAME, 1) - 1);

GO


BEGIN TRANSACTION
GO
CREATE TABLE dbo.Rolle
	(
	Id int NOT NULL,
	Bezeichnung nvarchar(50) NOT NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.Rolle ADD CONSTRAINT
	PK_Rolle PRIMARY KEY CLUSTERED 
	(
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE Rolle SET (LOCK_ESCALATION = TABLE)
GO
COMMIT

INSERT INTO Rolle Values (1, 'Administrator');
INSERT INTO Rolle Values (2, 'Schulleitung');
INSERT INTO Rolle Values (3, 'Sekretariat');
INSERT INTO Rolle Values (4, 'Seminarfach');
INSERT INTO Rolle Values (5, 'FpA Wirtschaft');
INSERT INTO Rolle Values (6, 'FpA Sozial');
INSERT INTO Rolle Values (7, 'FpA Technik');
INSERT INTO Rolle Values (8, 'FpA Agrar');

BEGIN TRANSACTION
SET QUOTED_IDENTIFIER ON
SET ARITHABORT ON
SET NUMERIC_ROUNDABORT OFF
SET CONCAT_NULL_YIELDS_NULL ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.Rolle SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.Lehrer SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
BEGIN TRANSACTION
GO
CREATE TABLE dbo.LehrerRolle
	(
	LehrerId int NOT NULL,
	RolleId int NOT NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.LehrerRolle ADD CONSTRAINT
	FK_LehrerRolle_Lehrer FOREIGN KEY
	(
	LehrerId
	) REFERENCES dbo.Lehrer
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  CASCADE 
	
GO
ALTER TABLE dbo.LehrerRolle ADD CONSTRAINT
	FK_LehrerRolle_Rolle FOREIGN KEY
	(
	RolleId
	) REFERENCES dbo.Rolle
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.LehrerRolle SET (LOCK_ESCALATION = TABLE)
GO
COMMIT

