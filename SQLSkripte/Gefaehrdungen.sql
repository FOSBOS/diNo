use diNo

SELECT (Left(Klasse.Bezeichnung,4)), Count (Schueler.Id)
FROM Schueler, Klasse, Vorkommnis
WHERE Schueler.KlasseId = Klasse.Id
AND Schueler.Id = Vorkommnis.SchuelerId
AND Vorkommnis.Art = 14
Group by (Left(Klasse.Bezeichnung,4))