use diNo;

SELECT Klasse.Bezeichnung, Schueler.Name, Schueler.Vorname, Vorkommnis.Bemerkung, Schueler.Geburtsdatum
FROM Schueler, Klasse, Vorkommnis
WHERE Schueler.KlasseId = Klasse.Id
AND Schueler.Id = Vorkommnis.SchuelerId
AND Vorkommnis.Art = 11
order by Bezeichnung,Name