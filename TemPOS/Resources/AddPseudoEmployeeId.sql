if Exists(select * from sys.columns where Name = N'TicketDiscountEmployeeId'  
            and Object_ID = Object_ID(N'TicketDiscount'))
begin
    ALTER TABLE TicketDiscount DROP COLUMN TicketDiscountEmployeeId
end

GO
if NOT Exists(select * from sys.columns where Name = N'TicketDiscountPseudoEmployeeId'  
            and Object_ID = Object_ID(N'TicketDiscount'))
begin
    ALTER TABLE TicketDiscount ADD TicketDiscountPseudoEmployeeId int
end

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[AddTicketDiscount] 
(
	@TicketDiscountYear smallint,
	@TicketDiscountDiscountId int,
	@TicketDiscountTicketId int,
	@TicketDiscountTicketItemId int,
	@TicketDiscountAmount float,
	@TicketDiscountPseudoEmployeeId int
)
AS
BEGIN
	INSERT INTO TicketDiscount (TicketDiscountYear, TicketDiscountDiscountId, TicketDiscountTicketId, TicketDiscountTicketItemId, TicketDiscountAmount, TicketDiscountPseudoEmployeeId)
	VALUES (@TicketDiscountYear, @TicketDiscountDiscountId, @TicketDiscountTicketId, @TicketDiscountTicketItemId, @TicketDiscountAmount, @TicketDiscountPseudoEmployeeId)
	
	return SCOPE_IDENTITY()
END
GO
