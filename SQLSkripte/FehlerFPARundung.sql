/****** Skript für SelectTopNRows-Befehl aus SSMS ******/
use diNo;

; With probl as (
SELECT 
		Name,Vorname,Ausbildungsrichtung,
      [SchuelerId]
	  , Betrieb
      ,[Anleitung]
      ,[Vertiefung]
      ,[Gesamt]
	  ,Round((Betrieb*2+Anleitung+Vertiefung)/4.0,0) as Erg
  FROM fpa,Schueler
  Where Gesamt is not null and SchuelerId = Schueler.Id
  )
Select * from probl
  Where Erg<>Gesamt