if NOT Exists(select * from sys.columns where Name = N'IngredientParQuantity'  
            and Object_ID = Object_ID(N'Ingredient'))
begin
    ALTER TABLE Ingredient ADD IngredientParQuantity float
end

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[AddIngredient] 
(
	@IngredientName text,
	@IngredientShortName text,
	@IngredientInventoryAmount float,
	@IngredientMeasurementUnit smallint,
	@IngredientCostPerUnit float,
	@IngredientParQuantity float
)
AS
BEGIN
	INSERT INTO Ingredient (IngredientName, IngredientShortName, IngredientInventoryAmount, IngredientMeasurementUnit, IngredientCostPerUnit, IngredientParQuantity)
	VALUES (@IngredientName, @IngredientShortName, @IngredientInventoryAmount, @IngredientMeasurementUnit, @IngredientCostPerUnit, @IngredientParQuantity)
	
	return SCOPE_IDENTITY()
END
