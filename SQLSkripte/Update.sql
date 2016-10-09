USE [diNo]
GO

/****** Object:  View [dbo].[vwNotenbogen]    Script Date: 07.10.2016 19:41:14 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[vwNotenbogen]
AS
SELECT        dbo.Klasse.Bezeichnung, dbo.Lehrer.Nachname AS Klassenleiter, dbo.Seminarfachnote.Gesamtnote, dbo.Seminarfachnote.ThemaLang, 
                         dbo.Seminarfachnote.ThemaKurz, dbo.Schueler.Id, dbo.Schueler.Name, dbo.Schueler.Vorname, dbo.Schueler.KlasseId, dbo.Schueler.Rufname, 
                         dbo.Schueler.Geschlecht, dbo.Schueler.Geburtsdatum, dbo.Schueler.Geburtsort, dbo.Schueler.Bekenntnis, dbo.Schueler.AnschriftPLZ, dbo.Schueler.AnschriftOrt, 
                         dbo.Schueler.AnschriftStrasse, dbo.Schueler.AnschriftTelefonnummer, dbo.Schueler.Ausbildungsrichtung, dbo.Schueler.Fremdsprache2, 
                         dbo.Schueler.ReligionOderEthik, dbo.Schueler.Wahlpflichtfach, dbo.Schueler.Wahlfach1, dbo.Schueler.Wahlfach2, dbo.Schueler.Wahlfach3, dbo.Schueler.Wahlfach4, 
                         dbo.Schueler.Wiederholung1Jahrgangsstufe, dbo.Schueler.Wiederholung2Jahrgangsstufe, dbo.Schueler.Wiederholung1Grund, dbo.Schueler.Wiederholung2Grund, 
                         dbo.Schueler.ProbezeitBis, dbo.Schueler.Austrittsdatum, dbo.Schueler.SchulischeVorbildung, dbo.Schueler.BeruflicheVorbildung, dbo.Schueler.LRSStoerung, 
                         dbo.Schueler.LRSSchwaeche, dbo.Schueler.LRSBisDatum, dbo.Schueler.NachnameEltern1, dbo.Schueler.VornameEltern1, dbo.Schueler.AnredeEltern1, 
                         dbo.Schueler.VerwandtschaftsbezeichnungEltern1, dbo.Schueler.NachnameEltern2, dbo.Schueler.VornameEltern2, dbo.Schueler.AnredeEltern2, 
                         dbo.Schueler.VerwandtschaftsbezeichnungEltern2, dbo.Schueler.EintrittJahrgangsstufe, dbo.Schueler.EintrittAm, dbo.Schueler.EintrittAusSchulnummer, 
                         dbo.Schueler.Email, dbo.Schueler.Notfalltelefonnummer, dbo.Schueler.DNote, dbo.Schueler.Status, dbo.Schueler.AndereFremdspr2Note, 
                         dbo.Schueler.AndereFremdspr2Text, dbo.Schueler.DNoteAllg, dbo.Schueler.Schulart
FROM            dbo.Seminarfachnote RIGHT OUTER JOIN
                         dbo.Lehrer RIGHT OUTER JOIN
                         dbo.Schueler INNER JOIN
                         dbo.Klasse ON dbo.Schueler.KlasseId = dbo.Klasse.Id ON dbo.Lehrer.Id = dbo.Klasse.KlassenleiterId ON dbo.Seminarfachnote.SchuelerId = dbo.Schueler.Id

GO



alter table FpaNoten add Stelle1Hj nvarchar(256) null;  
alter table FpaNoten add Stelle2Hj nvarchar(256) null;
