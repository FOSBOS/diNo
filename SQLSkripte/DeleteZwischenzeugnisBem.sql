use diNo;

delete from Vorkommnis where Art=41 and SchuelerId in (
	select Id from Schueler,Klasse where Klasse.JgStufe<12 and Schueler.KlasseId=Klasse.Id 
)
