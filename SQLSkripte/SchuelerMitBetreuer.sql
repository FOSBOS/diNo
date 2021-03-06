/****** Skript für SelectTopNRows-Befehl aus SSMS ******/
USE [diNo]
SELECT 
	  Schueler.Id
      ,Name
      ,Vorname
	  ,Bezeichnung
  FROM Schueler,Klasse
  WHERE Schueler.KlasseId = Klasse.Id and (Bezeichnung like 'F11%' or Bezeichnung like '%13%')
  ORDER BY Bezeichnung,Name