CREATE PROCEDURE [dbo].[AddTicketItem]
(
	@TicketItemYear smallint,
	@TicketItemTicketId int,
	@TicketItemItemId int,
	@TicketItemQuantity smallint,
	@TicketItemPrice float,
	@TicketItemOrderTime datetime,
	@TicketItemPreparedTime datetime,
	@TicketItemParentTicketItemId int,
	@TicketItemId int OUTPUT
)
AS
EXTERNAL NAME PointOfSale.TicketItem.AddTicketItem