use diNo;

select Bezeichnung, Count(Punkte) as Anzahl, sum(Punkte) as Summe
from Fach INNER JOIN HjLeistung ON Fach.Id = HjLeistung.FachId
where Art=3
group by FachId, Bezeichnung