use diNo

SELECT Schueler.Id, Schueler.Name, Schueler.Vorname, Klasse.Bezeichnung, HjLeistung.Art
FROM HjLeistung RIGHT OUTER JOIN
Schueler ON HjLeistung.SchuelerId = Schueler.Id right outer join Klasse on Klasse.Id = Schueler.KlasseId
where Schueler.Schulart='F' and Klasse.Bezeichnung like '%12%'
and HjLeistung.Art is null