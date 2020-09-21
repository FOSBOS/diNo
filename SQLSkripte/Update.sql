use diNo;

alter table Seminarfachnote drop column ThemaKurz
EXEC sp_rename 'Seminarfachnote.ThemaLang', 'Thema', 'COLUMN';

alter table Klasse alter column Schulart tinyint;