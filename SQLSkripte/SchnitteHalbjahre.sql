use diNo;

-- Export als csv, dann im Notepad alle . durch , ersetzen und mit Excel aufmachen
-- Alternativ direkte Datenanbindung im Excel ??

SELECT        Fach.Bezeichnung as Fachbez, HjLeistung.Art +1 as Halbjahr, Klasse.Bezeichnung AS Klassenbez, count(*) as Anzahl, AVG(CONVERT(decimal(10,2),HjLeistung.Punkte)) as Schnitt
FROM            HjLeistung INNER JOIN
                         Fach ON HjLeistung.FachId = Fach.Id INNER JOIN
                         Schueler ON HjLeistung.SchuelerId = Schueler.Id INNER JOIN
                         Klasse ON Schueler.KlasseId = Klasse.Id
WHERE        (HjLeistung.JgStufe > 11)  And (HjLeistung.Art<2) and Fach.typ < 2 --AND (HjLeistung.Status < 3)
group by Fach.Bezeichnung, HjLeistung.Art, Klasse.Bezeichnung
Having count(*) > 9
order by Fach.Bezeichnung, Klasse.Bezeichnung,Art