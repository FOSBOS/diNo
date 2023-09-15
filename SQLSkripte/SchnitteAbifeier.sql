-- Export als csv, dann im Notepad alle . durch , ersetzen und mit Excel aufmachen
-- Alternativ direkte Datenanbindung im Excel ??
-- direkt nach Excel kopieren funktioniert nicht, deshalb entweder abtippen oder über Notepad kopieren, dort . durch , ersetzen, dann in Excel

use diNo

--select avg(DNote) as Note
--select floor(DNote) as Note, count(*) as Anzahl
--select Klasse.JgStufe, avg(DNote) as Note
--select Schueler.Schulart, avg(DNote) as Note
--select Schueler.Geschlecht, avg(DNote) as Note
select Schueler.Ausbildungsrichtung, avg(DNote) as Note
--select Schueler.Bekenntnis, avg(DNote) as Note

from Schueler, Klasse
where
DNote is not null
and Status=0
and Schueler.KlasseId = Klasse.Id
and Schueler.Id not in
( select SchuelerId
  from Vorkommnis
  where Art>20 and Art<32

)
--group by floor(DNote)
--group by Klasse.JgStufe
--group by Schueler.Schulart
group by Schueler.Ausbildungsrichtung
--group by Geschlecht
--group by Bekenntnis