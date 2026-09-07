use diNo;

SELECT 
		Klasse.Bezeichnung
	  ,[Name]    
      ,[Rufname]
      ,[AndereFremdspr2Note]
      ,[AndereFremdspr2Text]
      ,[AndereFremdspr2Art]
	  ,Fach.Bezeichnung	  
  FROM Schueler, Fach, Klasse
  where AndereFremdspr2Note is not null and Status=0
  and Fach.Id=Schueler.AndereFremdspr2Fach
  and Klasse.Id=Schueler.KlasseId
  and Klasse.JgStufe=11
  order by Klasse.Bezeichnung, Name