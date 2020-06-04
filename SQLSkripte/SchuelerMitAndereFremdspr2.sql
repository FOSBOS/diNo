use diNo;

SELECT Id
      ,[Name]
      ,[KlasseId]
      ,[Rufname]
      ,[AndereFremdspr2Note]
      ,[AndereFremdspr2Text]
      ,[AndereFremdspr2Art]
	  ,[AndereFremdspr2Fach]
	  , Status
  FROM [diNo].[dbo].[Schueler]
  where AndereFremdspr2Note is not null and Status=0