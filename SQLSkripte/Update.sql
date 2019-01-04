USE [diNo]
GO

/****** Object:  Table [dbo].[Punktesumme]    Script Date: 04.01.2019 14:28:30 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Punktesumme](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[SchuelerId] [int] NOT NULL,
	[Art] [int] NOT NULL,
	[Punktzahl] [int] NOT NULL,
	[Anzahl] [int] NOT NULL
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[Punktesumme]  WITH CHECK ADD  CONSTRAINT [FK_Punktesumme_Schueler] FOREIGN KEY([SchuelerId])
REFERENCES [dbo].[Schueler] ([Id])
GO

ALTER TABLE [dbo].[Punktesumme] CHECK CONSTRAINT [FK_Punktesumme_Schueler]
GO


