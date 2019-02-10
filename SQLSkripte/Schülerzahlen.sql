use diNo;

select Klasse.Bezeichnung,count(Schueler.id)
From Schueler,Klasse
Where Klasse.Id = Schueler.KlasseId
and Schueler.Status=0
Group by Klasse.Bezeichnung