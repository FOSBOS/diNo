use diNo;

ALTER TABLE dbo.Schueler ADD
	LRSZuschlagMin int NULL,
	LRSZuschlagMax int NULL
	
ALTER TABLE dbo.Schueler
	DROP COLUMN SonderfallNur2Hj