use diNo;

ALTER TABLE dbo.Schueler ADD
	LRSZuschlagMin int NOT NULL DEFAULT 0,
	LRSZuschlagMax int NOT NULL DEFAULT 0
	
--ALTER TABLE dbo.Schueler DROP COLUMN SonderfallNur2Hj