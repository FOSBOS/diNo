use diNo;

SELECT
	  [Bezeichnung] AS Klasse 
      ,[Name] as Nachname
      ,[Rufname] AS Vorname
      ,[AnschriftPLZ] as PLZ
      ,[AnschriftOrt] as Ort
      ,[AnschriftStrasse] as Straße
      ,[AnschriftTelefonnummer] as Telefon
	  ,case when [Geschlecht]='M' then 'H' else 'F' end as Anrede
	       
  FROM [dbo].[Schueler],[dbo].[Klasse]
  WHERE Schueler.KlasseId = Klasse.Id 
  AND (Status=0) AND Bezeichnung not like '%Rest'

  ORDER BY Bezeichnung,Ausbildungsrichtung, Name