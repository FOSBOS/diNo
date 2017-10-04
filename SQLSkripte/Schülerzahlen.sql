use diNo;

select Klasse.Bezeichnung,count(Schueler.id)
From Schueler,Klasse
Where Klasse.Id = Schueler.KlasseId
Group by Klasse.Bezeichnung