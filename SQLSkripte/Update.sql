use diNo;

alter table Fach add Fachschaft varchar(50);
update Fach set Fachschaft = Bezeichnung;

-- Danach in Tabelle Fach die Fachschaften anpassen (in gr��ere Gruppe zusammenfassen, so muss sp�ter auch das Unterverzeichnis f�r die Ablage der LNW hei�en).