use diNo
go

/****** Skript für SelectTopNRows-Befehl aus SSMS ******/
SELECT KLasse.Bezeichnung,
Name
      ,[Vorname]              

  FROM Schueler,Klasse
  WHERE LRSStoerung=1 and Schueler.KlasseId = Klasse.Id
  order by Bezeichnung,Name