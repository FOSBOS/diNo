USE [diNo]
GO

/****** Object:  Table [dbo].[GlobaleKonstanten]    Script Date: 12.02.2016 14:47:28 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[GlobaleKonstanten](
	[Id] [int] NOT NULL,
	[Schuljahr] [int] NULL,
	[aktZeitpunkt] [int] NULL,
	[Sperre] [int] NULL,
 CONSTRAINT [PK_GlobaleKonstanten] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

INSERT INTO [dbo].[GlobaleKonstanten]
           ([Id]
			,[Schuljahr]
           ,[aktZeitpunkt]
		   ,[Sperre])
     VALUES
           (1
		   ,2015
           ,3
		   ,0)
GO

ALTER TABLE [dbo].[Schueler] ADD [Status] [int] NULL DEFAULT ((0))
GO

Update Schueler set Status = 0
UPDATE [dbo].[Schueler] SET [Status] = 1 WHERE [Austrittsdatum] is not null;
GO

CREATE VIEW [dbo].[vwVorkommnis]
AS
SELECT        dbo.Vorkommnis.*, dbo.Vorkommnisart.Bezeichnung
FROM            dbo.Vorkommnis INNER JOIN
                         dbo.Vorkommnisart ON dbo.Vorkommnis.Art = dbo.Vorkommnisart.Id

GO