if NOT Exists(select * from sys.columns where Name = N'TicketItemFireTime'  
            and Object_ID = Object_ID(N'TicketItem'))
begin
    ALTER TABLE TicketItem ADD TicketItemFireTime datetime
end