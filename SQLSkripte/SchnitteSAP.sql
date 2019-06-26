use diNo;

-- Export als csv, dann im Notepad alle . durch , ersetzen und mit Excel aufmachen
-- Alternativ direkte Datenanbindung im Excel ??

SELECT        Kurs.Bezeichnung AS Kursbez, Lehrer.Kuerzel, Count(*) as Anzahl, AVG(CONVERT(decimal(10,2),Note.Punktwert)) As Schnitt
FROM            Fach INNER JOIN
                         Kurs ON Fach.Id = Kurs.FachId INNER JOIN
                         Lehrer ON Kurs.LehrerId = Lehrer.Id INNER JOIN
                         Note ON Kurs.Id = Note.KursId                        
WHERE        (Note.Notenart = 8)
group by Kurs.Bezeichnung, Lehrer.Kuerzel,Kurs.Id
ORDER BY Kurs.Bezeichnung