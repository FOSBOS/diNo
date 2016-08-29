-- Datei muss als UTF-16 abgespeichert sein!

BULK INSERT Vorkommnisart	
    FROM 'D:\Claus\Notenverwaltung\Vorkommnisarten.csv'
    WITH
    (
    FIRSTROW = 1,
    FIELDTERMINATOR = ';',  --CSV field delimiter
    ROWTERMINATOR = '\n',   --Use to shift the control to next row    
    TABLOCK
    )