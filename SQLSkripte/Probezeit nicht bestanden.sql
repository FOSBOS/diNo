SELECT Schueler.Name, Schueler.Vorname, Klasse.Bezeichnung
FROM Schueler, Klasse, Vorkommnis
WHERE Schueler.KlasseId = Klasse.Id
AND Schueler.Id = Vorkommnis.SchuelerId
AND Vorkommnis.Art = 1