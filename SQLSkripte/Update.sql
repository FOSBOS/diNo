use diNo;
go

insert into Fach (Bezeichnung, Kuerzel,Sortierung, Typ, NichtNC) values ('Fachreferat', 'FR', 301, 3, 0);
-- ergänzen Sortierung FpA = 305, Sem = 310

--delete from HjLeistung where Art > 4;

insert into HjLeistung (SchuelerId, FachId,Art,Punkte,JgStufe, Status)
SELECT SchuelerId
	  ,74
      ,Halbjahr
      ,Gesamt
	  ,11
	  ,0
FROM Fpa 
Where Gesamt is not null;

