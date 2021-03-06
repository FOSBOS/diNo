/****** Skript für SelectTopNRows-Befehl aus SSMS ******/
SELECT 
      [SchulischeVorbildung], 
	  [Ausbildungsrichtung],
	  SUM([Status]) AS Ausgetreten,
      Count([Status]) AS Anzahl,
	  100 * SUM([Status]) / Count([Status]) AS Quote
  FROM [diNo].[dbo].[Schueler]
  GROUP BY [SchulischeVorbildung], [Ausbildungsrichtung]
  ORDER BY SchulischeVorbildung DESC