use diNo
select Klasse.Bezeichnung,Name,Schueler.Rufname,Note.Punktwert
from Schueler,Note,Kurs,Fach,Klasse
where Schueler.Id = Note.SchuelerId
and Schueler.KlasseId = Klasse.Id
and Kurs.Id = Note.KursId
and Fach.Id = Kurs.FachId
and Note.Notenart = 8 -- SAP
and Fach.Kuerzel='D'
order by Punktwert desc

