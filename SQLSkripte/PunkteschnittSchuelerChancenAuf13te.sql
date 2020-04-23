/****** Skript für SelectTopNRows-Befehl aus SSMS ******/
SELECT [SchuelerId], Name, Vorname, Klasse.Bezeichnung AS Klasse
	 ,ROUND(AVG(CAST ([Punkte] AS DECIMAL(10,2))), 2) AS Punkteschnitt
  FROM [diNo].[dbo].[HjLeistung], [diNo].[dbo].Schueler, [diNo].[dbo].Klasse, [diNo].[dbo].[Fach]
  WHERE HjLeistung.SchuelerId = Schueler.Id
  AND Schueler.KlasseId = Klasse.Id
  AND HjLeistung.FachId = Fach.Id
  AND (HjLeistung.JgStufe = 12 OR (HjLeistung.JgStufe = 11 AND Art = 1))
  AND Schueler.Schulart = 'F'
  AND Klasse.Bezeichnung LIKE '%12%'
  AND Fach.Typ <= 2
  GROUP BY SchuelerId, Name, Vorname, Klasse.Bezeichnung
  ORDER BY Klasse.Bezeichnung, Schueler.Name