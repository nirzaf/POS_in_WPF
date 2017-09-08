if NOT Exists(select * from sys.columns where Name = N'ItemButtonImage'  
            and Object_ID = Object_ID(N'Item'))
begin
    ALTER TABLE Item ADD ItemButtonImage varbinary(max) NULL
end