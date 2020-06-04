use diNo;

ALTER TABLE Schueler DROP COLUMN Punktesumme;
ALTER TABLE Schueler DROP COLUMN Bestanden;

EXEC sp_rename 'Schueler.DNoteAllg', 'DNoteFachgebHSR', 'COLUMN';

alter table Fach add WPFid int null;


-- neu ab 27.5.2020

alter table Schueler add AndereFremdspr2Art int not null default 0
alter table Schueler add AndereFremdspr2Fach int null
ALTER TABLE Schueler ADD CONSTRAINT FK_Schueler_AndereFremdspr2Fach FOREIGN KEY (AndereFremdspr2Fach) REFERENCES Fach (ID);

--update Schueler set AndereFremdspr2Fach=11 where AndereFremdspr2Text like 'Französisch%';
--update Schueler set AndereFremdspr2Fach=166 where AndereFremdspr2Text like 'Latein%'

alter table Schueler drop column Fremdsprache2, Wahlfach1, Wahlfach2, Wahlfach3, Wahlfach4, Wahlpflichtfach
