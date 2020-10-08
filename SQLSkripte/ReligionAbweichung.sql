use diNo;

select Klasse.Bezeichnung,Name,Rufname, Bekenntnis, ReligionOderEthik
from Schueler, Klasse
where Schueler.KlasseId = Klasse.Id and
Bekenntnis<>ReligionOderEthik and ReligionOderEthik<>'Eth'
order by Bezeichnung,Name