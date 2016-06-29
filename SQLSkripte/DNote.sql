use [diNo]

select Bezeichnung,Name,Rufname,DNote,DNoteAllg
From Schueler,Klasse
Where schueler.KlasseId = Klasse.Id and DNote is not null
--and Dnote < 2
--Order by Dnote,Name
order by Bezeichnung,Name