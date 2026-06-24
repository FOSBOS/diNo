use diNo;

SELECT 
	Klasse.Bezeichnung
	  ,Name     
      ,Rufname
      ,AndereFremdspr2Note
	  ,Fach.Bezeichnung	  
  FROM Schueler,Klasse,Fach
  where Klasse.Id=Schueler.KlasseId and
  Fach.Id = Schueler.AndereFremdspr2Fach and
  AndereFremdspr2Note is not null and Status=0
  and Klasse.JgStufe=13
  and Schueler.Id not in
( select SchuelerId
  from Vorkommnis
  where Art>20 and Art<32 or Art=47
)
  order by Klasse.Bezeichnung,Name

