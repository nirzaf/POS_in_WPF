   M S B A M L   `   `   `  ����T  PTemPOS, Version=1.0.4709.35932, Culture=neutral, PublicKeyToken=b60a724858027fa2)clr-namespace:PointOfSalePointOfSale  5   D  5clr-namespace:PointOfSale.TypesPointOfSale.Types  6i  R NPosControls, Version=1.0.0.0, Culture=neutral, PublicKeyToken=b60a724858027fa2>.clr-namespace:PosControls;assembly=PosControlsPosControls 6�  4    .PointOfSale.IngredientEditorPreparationControl   6   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     6G   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     6�   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     6�   @d2http://schemas.microsoft.com/expression/blend/2008     6  (myclr-namespace:PointOfSale     6D  -tclr-namespace:PointOfSale.Types     6i  ?ctrl.clr-namespace:PosControls;assembly=PosControls     6�  .��5      ]� -   5        $��mainPane��5   #   ��. .+�5      &   PosControls.ConfigurationManager  -   5           Name$  Configuration��5       � 5      $	��405��6    � RowDefinitions 5      �� 5   
   $��40 �6   �� 5   
   $	��30* �6   �� 5   
   $
��235* �6   �� 5	   
   $	��80* �6    � ColumnDefinitions 5      �� 5   
   $
��300* �6   �� 5   
   $
��150* �6   �� 5   
   $
��150* �6   ��5      �� 5   
    �� ElementName$ Configuration��6    �� Path$$ BindableControlBackgroundBrush�6.   .+�5      �� $��0,0,5,0q�6   $��2��6    �� 5      $��1��6   $��2��6   $��2��63   �� 5      -   5         ��Name$ label1��5         PointOfSale.Types.Strings    IngredientEditorAvailable#��Z 6   $��Center=�6W   $��14(�6r   $��1��6�   �� 5      -   5        $ label2��5         IngredientEditorCurrent#��Z 6   $��Center=�6U   $��14(�6p   $��1��6~   $��2��6�   $��1��6�   �� 5      -   5        $��stackPanelPromptAmount��5       �� Orientation$ 
Horizontal=�61   $��Center=�6J   .+�5   
     PosControls.PushCheckBox  -   5        	  Name$	 checkBoxEnabled��5      $��25��65   $��25��6@   $��5,0,0,0q�6L   6]   �� 5   
   
   IngredientEditorPrepared#��Z
 6   $��14(�6H   $��Normal"�6V   $��Center=�6j   5   	   #  PosControls.DragScrollListBox  5      -   5          Name$! listBoxAvailableIngredients��5   1   $��5,5,10,5q�6   $��2��6V   6c     5      -   5        $ listBoxItemIngredients��5   *   $��5q�6   $��1��6J   $��2��6Z   $��2��6n   6{   �� 5      $ 
Horizontal=�6   $��1��6,   $��2��6<   $��Center=�6P   .+�5   
   �� -	   5        $ label3��5      "   IngredientEditorRecipeYield#��Z 6    $��Center=�6[   $��14(�6v     PosControls.CustomTextBox  5   
   -
   5          Name$ buttonYield��5         
PromptType$ FloatNumeric=�62      UseContextMenuEditing . $��30��6i   $��5q�6u      Text$	 0.0��6�   $��75��6�   $��14(�6�   6�   �� 5    
   -   5        $ 
labelUnits��5          IngredientEditorUnits#��Z 6$   $��Center=�6Y   $��14(�6t   5!   	   !  PosControls.TextBlockButton  5"      -   5          Name$ 	buttonAdd��5"      $��3��60   $��5q�6=   $   IngredientEditorAddIngredient   Text# Z 6H   $��85��6�   $��70��6�      FontSize$ 14Q�6�   6�   �� 5#      $ 
Horizontal=�6   $��Center=�6,   $��3��6I   $��2��6V   $��1��6j   .+�5$   
     -   5        $ buttonAmount��5$      $��5q�65   %   IngredientEditorEditIngredient# Z 6@   $��85��6{   $��70��6�   $ 14Q�6�   6�     5%   
   -   5        $ buttonRemove��5%      $��5q�65   '    IngredientEditorRemoveIngredient# Z 6@   $��85��6}   $��70��6�   $ 14Q�6�   6�   5&   	   �� 5'      -   5        $ labelReadOnly��5'      $��3��6%   $��3��62   $��Center=�6F   $��Center=�6c   $��12(�6~   Q   JPreparationCanNotBeModifiedDuringADayOfOperationCompleteAnEndOfDayToModify#��Z 6�   $y�	Collapsed=�6�   5(      5)      5*      