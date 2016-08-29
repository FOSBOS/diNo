use [diNo]

-- Lehrer manuell korrigieren

update GlobaleKonstanten set Schuljahr=2106, aktZeitpunkt=1, Sperre=0

delete from BerechneteNote
DBCC CHECKIDENT (BerechneteNote, RESEED, 0); -- AutoIncrement zurücksetzen

delete from FpANoten

delete from Note
DBCC CHECKIDENT (Note, RESEED, 0);

delete from KlasseKurs
delete from SchuelerKurs
delete from Kurs
DBCC CHECKIDENT (Kurs, RESEED, 0);

delete from Vorkommnis
DBCC CHECKIDENT (Vorkommnis, RESEED, 0);

delete from SchuelerKurs
delete from Seminarfachnote

delete from Schueler

delete from Klasse
DBCC CHECKIDENT (Klasse, RESEED, 0);

