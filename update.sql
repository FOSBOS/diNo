USE [diNo]
GO

/****** Object:  Table [dbo].[GlobaleKonstanten]    Script Date: 10.02.2016 18:48:46 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[GlobaleKonstanten](
	[Schuljahr] [int] NULL,
	[aktZeitpunkt] [int] NULL
) ON [PRIMARY]

GO

INSERT INTO [dbo].[GlobaleKonstanten]
           ([Schuljahr]
           ,[aktZeitpunkt])
     VALUES
           (2015
           ,3)
GO

