use [diNo]

-- nächste Jahr raus:
drop table BerechneteNote
SET IDENTITY_INSERT Kurs ON


-- Lehrer manuell korrigieren

update GlobaleKonstanten set Schuljahr=YEAR(CURRENT_TIMESTAMP), aktZeitpunkt=1, Sperre=0


delete from Fpa

delete from Note
DBCC CHECKIDENT (Note, RESEED, 0);

delete from HjLeistung;
DBCC CHECKIDENT (HJLeistung, RESEED, 0);

delete from KlasseKurs
delete from SchuelerKurs
delete from Kurs
DBCC CHECKIDENT (Kurs, RESEED, 0);

delete from Vorkommnis
DBCC CHECKIDENT (Vorkommnis, RESEED, 0);

delete from SchuelerKurs
delete from Seminarfachnote
delete from Punktesumme

delete from Schueler

delete from Klasse
DBCC CHECKIDENT (Klasse, RESEED, 0);

