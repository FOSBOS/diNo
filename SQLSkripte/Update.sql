use diNo;

ALTER TABLE Schueler DROP COLUMN Punktesumme;
ALTER TABLE Schueler DROP COLUMN Bestanden;

EXEC sp_rename 'Schueler.DNoteAllg', 'DNoteFachgebHSR', 'COLUMN';

alter table Fach add WPFid int null;


-- neu ab 27.5.2020

alter table Schueler
	add AndereFremdspr2Art int not null default 0

alter table Schueler drop column Fremdsprache2, Wahlfach1, Wahlfach2, Wahlfach3, Wahlfach4, Wahlpflichtfach
