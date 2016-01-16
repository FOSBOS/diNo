
Use diNo

update Vorkommnisart set Bezeichnung = 'Rücktritt in Vorklasse' Where Id=4;
update Vorkommnisart set Bezeichnung = 'Vorrückungserlaubnis nicht erhalten' Where Id=5;
insert into Vorkommnisart (Bezeichnung) Values ('Mitteilung über Gefährdung');
insert into Vorkommnisart (Bezeichnung) Values ('Mitteilung über starke Gefährdung');
insert into Vorkommnisart (Bezeichnung) Values ('Bei weiterem Absinken der Leistungen gefährdet');
insert into Vorkommnisart (Bezeichnung) Values ('Gefahr der Abweisung');
insert into Vorkommnisart (Bezeichnung) Values ('Die Jgst. darf nicht mehr wiederholt werden.');
insert into Vorkommnisart (Bezeichnung) Values ('Zwischenzeugnis');
insert into Vorkommnisart (Bezeichnung) Values ('kein Zwischenzeugnis');
insert into Vorkommnisart (Bezeichnung) Values ('Jahreszeugnis');
insert into Vorkommnisart (Bezeichnung) Values ('kein Jahreszeugnis');
insert into Vorkommnisart (Bezeichnung) Values ('Fachabiturzeugnis');
insert into Vorkommnisart (Bezeichnung) Values ('Zeugnis über fachgebundene Hochschulreife');
insert into Vorkommnisart (Bezeichnung) Values ('Zeugnis über allgemeine Hochschulreife');
insert into Vorkommnisart (Bezeichnung) Values ('Englisch Sprachkenntnisse Niveaustufe B2');
insert into Vorkommnisart (Bezeichnung) Values ('Bemerkung');
insert into Vorkommnisart (Bezeichnung) Values ('Französisch wurde nur bis zur SAP besucht.');

Alter Table Schueler Add BetreuerId int null;
Alter Table Schueler Add DNote decimal(5,1) null;