USE [diNo]
GO

/****** Object:  View [dbo].[vwNotenbogen]    Script Date: 14.03.2016 16:58:50 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER TABLE [dbo].[Schueler] ADD DNoteAllg decimal(5,1) NULL;
GO
