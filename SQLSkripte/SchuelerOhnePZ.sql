select Bezeichnung, Schueler.Name,Vorname
from Schueler, Klasse
where Schueler.KlasseId = Klasse.Id and Schueler.ProbezeitBis is null
And (Bezeichnung like '%VK%' or Bezeichnung like 'B12%')
order by Bezeichnung,Name