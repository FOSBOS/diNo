USE [diNo]
GO
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


alter table Schueler add AlteSO bit null default 1;
GO
update Schueler set AlteSO=1;

alter table Schueler add Punktesumme int null;
alter table Schueler add Bestanden int null;
GO

drop view vwNotenbogen
go

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
                         dbo.Schueler.AndereFremdspr2Text, dbo.Schueler.DNoteAllg, dbo.Schueler.Schulart, dbo.Schueler.AlteSO, dbo.Schueler.Punktesumme, dbo.Schueler.Bestanden
FROM            dbo.Seminarfachnote RIGHT OUTER JOIN
                         dbo.Lehrer RIGHT OUTER JOIN
                         dbo.Schueler INNER JOIN
                        dbo.Klasse ON dbo.Schueler.KlasseId = dbo.Klasse.Id ON dbo.Lehrer.Id = dbo.Klasse.KlassenleiterId ON dbo.Seminarfachnote.SchuelerId = dbo.Schueler.Id
GO


CREATE TABLE [dbo].[HjLeistung](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SchuelerId] [int] NOT NULL,
	[FachId] [int] NOT NULL,
	[Art] [tinyint] NOT NULL,
	[Punkte] [tinyint] NOT NULL,
	[Einbringen] [bit] NOT NULL,
	[Punkte2Dez] [decimal](5, 2) NULL,
	[SchnittMdl] [decimal](5, 2) NULL,
 CONSTRAINT [PK_HjLeistung] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[HjLeistung] ADD  CONSTRAINT [DF_HjLeistung_Einbringen]  DEFAULT ((0)) FOR [Einbringen]
GO

ALTER TABLE [dbo].[HjLeistung]  WITH CHECK ADD  CONSTRAINT [FK_HjLeistung_FachId] FOREIGN KEY([FachId])
REFERENCES [dbo].[Fach] ([Id])
GO

ALTER TABLE [dbo].[HjLeistung] CHECK CONSTRAINT [FK_HjLeistung_FachId]
GO

ALTER TABLE [dbo].[HjLeistung]  WITH CHECK ADD  CONSTRAINT [FK_HjLeistung_SchuelerId] FOREIGN KEY([SchuelerId])
REFERENCES [dbo].[Schueler] ([Id])
GO

ALTER TABLE [dbo].[HjLeistung] CHECK CONSTRAINT [FK_HjLeistung_SchuelerId]
GO

ALTER TABLE HjLeistung
ADD CONSTRAINT uc_HjLeistung UNIQUE (SchuelerId,FachId,Art)
Go
