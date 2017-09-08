if NOT Exists(select * from sys.columns where Object_ID = Object_ID(N'ItemGroup'))
begin
    CREATE TABLE ItemGroup
	(ItemGroupId int IDENTITY PRIMARY KEY NOT NULL, ItemGroupSourceItemId int NOT NULL,
	 ItemGroupTargetItemId int NOT NULL, ItemGroupTargetItemQuantity int NOT NULL)
end
GO

if NOT Exists(select * from sys.columns where Name = N'ItemIsGrouping'  
            and Object_ID = Object_ID(N'Item'))
begin
    ALTER TABLE Item ADD ItemIsGrouping bit NOT NULL DEFAULT 0
end
GO

if NOT Exists(select * from sys.columns where Name = N'TicketItemParentTicketItemId'  
            and Object_ID = Object_ID(N'TicketItem'))
begin
    ALTER TABLE TicketItem ADD TicketItemParentTicketItemId int DEFAULT NULL
end
GO

IF Exists(select * from sys.objects WHERE object_id = OBJECT_ID(N'AddItemGroup') AND type in (N'P', N'PC'))
BEGIN
	DROP PROCEDURE [dbo].[AddItemGroup] 
END
GO

IF Exists(select * from sys.objects WHERE object_id = OBJECT_ID(N'AddTicketItem') AND type in (N'P', N'PC'))
BEGIN
	DROP PROCEDURE [dbo].[AddTicketItem]
END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[AddItemGroup] 
(
	@ItemGroupSourceItemId int,
	@ItemGroupTargetItemId int,
	@ItemGroupTargetItemQuantity int
)
AS
BEGIN
	INSERT INTO ItemGroup (ItemGroupSourceItemId, ItemGroupTargetItemId, ItemGroupTargetItemQuantity)
	VALUES (@ItemGroupSourceItemId, @ItemGroupTargetItemId, @ItemGroupTargetItemQuantity)
	
	return SCOPE_IDENTITY()
END
GO

ALTER PROCEDURE [dbo].[AddItem] 
(
	@ItemCategoryId int,
	@ItemName text,
	@ItemShortName text,
	@ItemDefaultPrice float,
	@ItemItemOptionSetId3 int,
	@ItemItemOptionSetId2 int,
	@ItemItemOptionSetId1 int,
	@ItemPrintOptionSetId int,
	@ItemActive bit,
	@ItemTaxId int,
	@ItemPrepareTime time,
	@ItemIsReturnable bit,
	@ItemIsTaxExemptable bit,
	@ItemIsFired bit,
	@ItemIsOutOfStock bit,
	@ItemIsDiscontinued bit,
	@ItemIsGrouping bit
)
AS
BEGIN
	INSERT INTO Item (ItemCategoryId, ItemName, ItemShortName, ItemDefaultPrice, ItemItemOptionSetId3, ItemItemOptionSetId2, ItemItemOptionSetId1, ItemPrintOptionSetId, ItemActive, ItemTaxId, ItemPrepareTime, ItemIsReturnable, ItemIsTaxExemptable, ItemIsFired, ItemIsOutOfStock, ItemIsDiscontinued, ItemIsGrouping)
	VALUES (@ItemCategoryId, @ItemName, @ItemShortName, @ItemDefaultPrice, @ItemItemOptionSetId3, @ItemItemOptionSetId2, @ItemItemOptionSetId1, @ItemPrintOptionSetId, @ItemActive, @ItemTaxId, @ItemPrepareTime, @ItemIsReturnable, @ItemIsTaxExemptable, @ItemIsFired, @ItemIsOutOfStock, @ItemIsDiscontinued, @ItemIsGrouping)
	
	return SCOPE_IDENTITY()
END
GO
