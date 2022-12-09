use diNo;

SELECT Klasse.Bezeichnung, Schueler.Name, Schueler.Rufname, Vorkommnis.Bemerkung, Schueler.Geburtsdatum
FROM Schueler, Klasse, Vorkommnis
WHERE Schueler.KlasseId = Klasse.Id
AND Schueler.Id = Vorkommnis.SchuelerId
AND Vorkommnis.Art = 11 --OR Vorkommnis.Art = 17 --Probezeit nicht bestanden oder verlängert
order by Bezeichnung,Name