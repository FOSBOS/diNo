use diNo;

select Klasse.Bezeichnung, Name,Vorname,DNote, DNoteAllg
From Schueler, Klasse
Where Klasse.Id = Schueler.KlasseId and
Schueler.Id in (
Select SchuelerId fROM Note, Kurs Where Notenart=9 and Note.KursId = kurs.Id and kurs.FachId <> 8)
Order by Bezeichnung, Name