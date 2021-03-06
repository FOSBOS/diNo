use diNo;

SELECT
      [Name]
      ,[Rufname] AS Vorname
	  ,[Bezeichnung] AS Klasse
      ,[Ausbildungsrichtung] AS Zweig
  FROM [dbo].[Schueler],[dbo].[Klasse]
  WHERE Schueler.KlasseId = Klasse.Id 
  AND (Status=0)
  AND (Klasse.Bezeichnung Like '%12%' or Klasse.Bezeichnung Like '%13%')
  ORDER BY Bezeichnung,Ausbildungsrichtung, Name