use [diNo]

select Klasse.Bezeichnung, Schueler.Name, Schueler.Rufname, Fpa.Gesamt
From Schueler,Klasse,Fpa
Where schueler.KlasseId = Klasse.Id and Schueler.Id = Fpa.SchuelerId
and Schueler.Status=0 and left(Bezeichnung,3)='F11' --and Fpa.Gesamt<5


-- für 1. Hj die Variable Erfolg durch Erfolg1Hj ersetzen

