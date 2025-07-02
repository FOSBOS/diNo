/****** Skript für SelectTopNRows-Befehl aus SSMS ******/
SELECT [Nachname]
		,[Vorname]
		,[Kuerzel]
      ,[Dienstbezeichnung]
      
    
  FROM [diNo].[dbo].[Lehrer]
  order by Nachname,Vorname
