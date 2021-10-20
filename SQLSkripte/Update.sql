use diNo;

alter table Fach add Fachschaft varchar(50);
update Fach set Fachschaft = Bezeichnung;

-- Danach in Tabelle Fach die Fachschaften anpassen (in größere Gruppe zusammenfassen, so muss später auch das Unterverzeichnis für die Ablage der LNW heißen).