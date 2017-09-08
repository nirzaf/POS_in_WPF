using System.Collections.Generic;
using System.Reflection;

namespace TemPOS.Types
{
    [Obfuscation(Exclude = true)]
    public partial class Strings
    {
        public static readonly Dictionary<string, string> Spanish = new Dictionary<string, string>();

        public static void InitializeSpanish()
        {
            #region General
            Spanish.Add("Warning", "Advertencia");
            Spanish.Add("Error", "error");
            Spanish.Add("Notification", "notificación");
            Spanish.Add("DeleteConfirmation", "Eliminar confirmación");
            Spanish.Add("Help", "ayudar");
            Spanish.Add("Exception", "excepción");
            Spanish.Add("Yes", "sí");
            Spanish.Add("No", "no");
            Spanish.Add("All", "todos");
            Spanish.Add("None", "ninguno");
            Spanish.Add("Set", "establecer");
            Spanish.Add("Add", "añadir");
            Spanish.Add("Remove", "quitar");
            Spanish.Add("Increase", "aumentar");
            Spanish.Add("Decrease", "disminuir");
            #endregion

            #region Common
            Spanish.Add("Name", "nombre");
            Spanish.Add("Update", "actualizar");
            Spanish.Add("Description", "descripción");
            Spanish.Add("Amount", "cantidad");
            Spanish.Add("IsActive", "Es activo");
            Spanish.Add("CancelChanges", "Cancelar cambios");
            Spanish.Add("SelectAll", "Seleccionar todo");
            Spanish.Add("Ticket", "billete");
            Spanish.Add("TicketItem", "Venta de entradas artículo");
            Spanish.Add("Confirmation", "confirmación");
            Spanish.Add("FirstName", "Nombre");
            Spanish.Add("LastName", "apellido");
            Spanish.Add("MiddleInitial", "inicial del segundo nombre");
            Spanish.Add("AddressLine1", "Dirección (Línea 1)");
            Spanish.Add("AddressLine2", "Dirección (Línea 2)");
            Spanish.Add("City", "ciudad");
            Spanish.Add("State", "estado");
            Spanish.Add("PostalCode", "código postal");
            Spanish.Add("EMailAddress", "E-Mail");
            Spanish.Add("Password", "contraseña");
            Spanish.Add("IsEnabled", "está habilitado");
            Spanish.Add("Enabled", "Habilitado");
            Spanish.Add("Port", "puerto");
            Spanish.Add("Save", "ahorrar");

            // Ticket
            Spanish.Add("LateCancel", "Late Cancelar");
            Spanish.Add("Void", "Nulo");
            Spanish.Add("PrivilegedDiscounts", "Descuentos privilegiados");
            Spanish.Add("DeliveryDriverDispatching", "Chofer de Entrega Despacho");
            Spanish.Add("ChangeOwnerEmployee", "Cambiar Empleado Dueño");

            // Register
            Spanish.Add("Cashout", "Cashout");
            Spanish.Add("StartUp", "Start-Up");
            Spanish.Add("Payout", "pago");
            Spanish.Add("Drop", "caer");
            Spanish.Add("Deposit", "depósito");
            Spanish.Add("NoSale", "No Venta");
            Spanish.Add("Refund", "reembolso");
            Spanish.Add("Return", "volver");
            Spanish.Add("Report", "Informe");
            Spanish.Add("CloseOut", "Cerca de salida");
            Spanish.Add("DeliveryDriverBankrolling", "Entrega financiando controlador");
            Spanish.Add("UseAnyRegisterDrawer", "Utilice cualquier cajón Registrarse");

            // Manager
            Spanish.Add("ReportsMenu", "Menu");
            Spanish.Add("StartOfDay", "Comienzo del día");
            Spanish.Add("EndOfDay", "Fin del día");
            Spanish.Add("ManagerAlerts", "alertas del Administrador");
            Spanish.Add("SystemSetup", "Configuración del sistema");
            Spanish.Add("EmployeeSetup", "Empleado de configuración");
            Spanish.Add("EmployeeScheduling", "Employee Scheduling (Futuro)");
            Spanish.Add("EmployeeTimekeeping", "Empleado de hora normal");
            Spanish.Add("CustomerSetup", "Configuración del cliente (Futuro)");
            Spanish.Add("VendorSetup", "Configuración de proveedor (Futuro)");
            Spanish.Add("InventoryAdjustments", "Ajustes de inventario");
            Spanish.Add("OverrideDeliveryRestrictions", "Anular las restricciones de entrega (Futuro)");
            Spanish.Add("AdministrativeCommandConsole", "Consola de comandos de administración");
            Spanish.Add("ExitProgram", "Salir del programa");
            #endregion

            #region ChangePasswordControl
            Spanish.Add("PasswordIncorrect", "La contraseña antigua no es correcta.");
            Spanish.Add("NewPasswordsMismatch", "Sus nuevas contraseñas no coinciden.");
            Spanish.Add("NewPasswordToShort", "Usted nueva contraseña debe tener al menos cinco caracteres de longitud.");
            Spanish.Add("ChangePasswordWindowTitle", "Cambiar contraseña");
            Spanish.Add("ChangePasswordOldPassword", "Contraseña anterior");
            Spanish.Add("ChangePasswordNewPassword1", "Nueva contraseña");
            Spanish.Add("ChangePasswordNewPassword2", "New Password (Repetir)");
            #endregion

            #region Command Shell Strings
            Spanish.Add("ShellWindowTitle", "Command Console");
            Spanish.Add("ShellResetSystem", "¿Está seguro de que desea restablecer todo el sistema y salir de la aplicación?");
            Spanish.Add("ShellResetSystemTitle", "Confirmar restablecimiento del sistema");
            Spanish.Add("ShellDeleteLog", "¿Está seguro que desea eliminar el archivo de registro?");
            Spanish.Add("ShellNoLogFileExists", "No existe el archivo de registro");
            Spanish.Add("ShellHelpClearLine1", "Limpia la consola");
            Spanish.Add("ShellHelpClearLine2", "Comando: clear");
            Spanish.Add("ShellHelpLogLine1", "Eliminar o mostrar el archivo de registro");
            Spanish.Add("ShellHelpLogLine2", "Comando: log [delete | mostrar]");
            Spanish.Add("ShellHelpSqlQueryLine1", "Ejecutar una sentencia SQL que devuelve un conjunto de resultados");
            Spanish.Add("ShellHelpSqlQueryLine2", "Comando: sqlquery [sentencia SQL]");
            Spanish.Add("ShellHelpSqlNonQueryLine1", "Ejecutar una sentencia SQL sin devolver un conjunto de resultados");
            Spanish.Add("ShellHelpSqlNonQueryLine2", "Comando: sqlnonquery [sentencia SQL]");
            Spanish.Add("ShellSqlNonQueryResultSuccess", "Comando ejecutado satisfactoriamente");
            Spanish.Add("ShellSqlNonQueryResultFailed", "Error en el comando para ejecutar");
            Spanish.Add("ShellHelpStoreSettingLine1", "Establece u obtiene un valor de ajuste tienda");
            Spanish.Add("ShellHelpStoreSettingLine2", "Comando: storesetting [NAME] [DATA_TYPE] [VALUE]");
            Spanish.Add("ShellHelpStoreSettingLine3", "Tipos de datos: String (default), int, double, fecha y hora");
            Spanish.Add("ShellStoreSettingNotSet", "No existe un conjunto de valor para");
            Spanish.Add("ShellSeatingStatusOn", "Selección ocasión está encendido.");
            Spanish.Add("ShellSeatingStatusOff", "Ocasión selección está apagado.");
            Spanish.Add("ShellNoPrintersFound", "No se han encontrado impresoras");
            Spanish.Add("ShellKeyboardLockStatusOn", "Bloqueo del teclado está activado.");
            Spanish.Add("ShellKeyboardLockStatusOff", "Bloqueo del teclado está apagado.");
            Spanish.Add("ShellHelpUsage", "Uso:");
            #endregion

            #region CancelMadeUnmadeControl
            Spanish.Add("CancelControlReopen", "reabrir");
            Spanish.Add("CancelControlCancelMade", "Cancelar Hecho");
            Spanish.Add("CancelControlCancelUnmade", "Cancelar Unmade");
            Spanish.Add("CancelControlVoid", "Nulo");
            Spanish.Add("CancelControlDontCancel", "No cancelar");
            Spanish.Add("CancelControlDontRefund", "Cancelar reembolso");
            #endregion

            #region CategoryEditorControl
            Spanish.Add("CategoryEditorDisplayIndex", "Visualización de índice");
            Spanish.Add("CategoryEditorDoNotDisplay", "No mostrar");
            Spanish.Add("CategoryEditorNameInvalid", "Por favor ingrese un nombre válido");
            Spanish.Add("CategoryEditorDisplayIndexInvalid", "Por favor, seleccione un valor de índice de visualización");
            #endregion

            #region CouponCategorySelectionControl
            Spanish.Add("CategoryAllCategories", "Todas las categorías");
            Spanish.Add("CategorySelectedCategories", "Categorías seleccionadas");
            Spanish.Add("CategoryAddCategory", "Añadir categoría");
            Spanish.Add("CategoryRemoveCategory", "Eliminar Categoría");
            #endregion

            #region CouponEditorDetailsControl
            Spanish.Add("CouponEditorAmountAsPercentage", "Indicar la cantidad como porcentaje");
            Spanish.Add("CouponEditorMatching", "Tema y categoría a juego");
            Spanish.Add("CouponEditorMatchAll", "Aplicar a todos los elementos devueltos localizados en el Ticket");
            Spanish.Add("CouponEditorThirdPartyCompensation", "Compensación de Terceros");
            Spanish.Add("CouponEditorCouponValueLimit", "Valor del límite del Cupón");
            Spanish.Add("CouponEditorLimitPerTicket", "Limite el número de entradas por");
            Spanish.Add("CouponEditorCategory", "Categoría:");
            Spanish.Add("CouponEditorExcludeAllExceptFor", "Excluir todos, a excepción de ...");
            Spanish.Add("CouponEditorIncludeAllExceptFor", "Incluya todos, a excepción de ...");
            Spanish.Add("CouponEditorInvlaidLimitPerTicket", "Por favor, introduzca un límite válido por boleto, o deje el campo vacío.");
            Spanish.Add("CouponEditorInvalidAmountLimit","Por favor, introduzca un límite de cantidad cupón válido o deje el campo vacío.");
            Spanish.Add("CouponEditorInvalidName", "Por favor ingrese un nombre válido.");
            Spanish.Add("CouponEditorInvalidAmount", "Por favor, ingrese una cantidad válida.");
            Spanish.Add("CouponEditorInvalidType", "Selecciona si se trata de una cantidad fija o un porcentaje.");
            #endregion

            #region CouponItemSelectionControl
            Spanish.Add("ItemAllItems", "Todos los artículos");
            Spanish.Add("ItemSelectedItems", "elementos seleccionados");
            Spanish.Add("ItemAddItem", "Agregar elemento");
            Spanish.Add("ItemRemoveItem", "Quitar el producto");
            #endregion

            #region CouponMaintenanceControl
            Spanish.Add("CouponSetupNewCoupon", "Cupón Nuevo");
            Spanish.Add("CouponSetupDeleteCoupon", "Eliminar Cupón");
            Spanish.Add("CouponSetupUpdateCoupon", "Actualizar Cupón");
            #endregion

            #region DayOfOperationRangeSelectionControl
            Spanish.Add("DayOfOperationStartingDay", "A partir Día");
            Spanish.Add("DayOfOperationEndingDay", "Terminar el Día");
            Spanish.Add("DayOfOperationSelectSpecifiedDays", "Seleccione los días establecidos");
            Spanish.Add("DayOfOperationSelectThisYear", "Seleccione Este Año");
            #endregion

            #region DeviceSelectionControl
            Spanish.Add("DeviceSelectionTab1", "Bump Bares");
            Spanish.Add("DeviceSelectionTab2", "escáner de tarjetas");
            Spanish.Add("DeviceSelectionTab3", "Cajones de Efectivo");
            Spanish.Add("DeviceSelectionTab4", "Coin Despenser");
            Spanish.Add("DeviceSelectionTab5", "Impresoras de recibos");
            #endregion

            #region DiscountEditorControl
            Spanish.Add("DiscountEditorAmountAsPercentage", "Indicar la cantidad como porcentaje");
            Spanish.Add("DiscountEditorApplyTo", "Aplicar a");
            Spanish.Add("DiscountEditorRequiresPermission", "Requiere el permiso de descuento de Seguridad");
            Spanish.Add("DiscountEditorPromptForAmount", "Solicitar Cantidad");
            #endregion

            #region DiscountMaintenanceControl
            Spanish.Add("DiscountEditorAddDiscount", "Nuevo descuento");
            Spanish.Add("DiscountEditorDeleteDiscount", "Eliminar Descuento");
            Spanish.Add("DiscountEditorUpdateDiscount", "Actualización de Descuento");
            #endregion

            #region EmployeeClockInControl and EmployeeClockOutControl
            Spanish.Add("ClockInJobs", "Jobs");
            Spanish.Add("ClockIn", "Reloj-In");
            Spanish.Add("ClockOutDeclareTips", "Declarar Consejos");
            Spanish.Add("ClockOut", "Reloj de salida");
            Spanish.Add("ClockOutTips", "Tips");
            #endregion

            #region EmployeeEditorControl
            Spanish.Add("EmployeeEditorAddEmployee", "Añadir Empleado");
            Spanish.Add("EmployeeEditorEditJobs", "Editar opciones de empleo");
            Spanish.Add("EmployeeEditorTerminateEmployee", "terminar Empleado");
            Spanish.Add("EmployeeEditorRemoveEmployee", "Eliminar Empleado");
            Spanish.Add("EmployeeEditorUpdateEmployee", "Actualización de los Empleados");
            Spanish.Add("EmployeeEditorRehireEmployee", "recontratación del empleado");
            Spanish.Add("EmployeeEditorConfirmRehire", "¿Seguro de que deseas volver a contratar al empleado seleccionado?");
            Spanish.Add("EmployeeEditorCantTerminateSelf", "Usted no puede terminar!");
            Spanish.Add("EmployeeEditorConfirmTerminate", "¿Está seguro que desea despedir al empleado seleccionado?");
            Spanish.Add("EmployeeEditorTerminateFirst", "Un empleado debe ser terminado antes de ser eliminado");
            Spanish.Add("EmployeeEditorConfirmRemove", "¿Está seguro que desea eliminar el empleado seleccionado?");
            Spanish.Add("EmployeeEditorPasswordWarning", "Usted no ha configurado una contraseña para este empleado, no podrá iniciar sesión.");
            #endregion

            #region EmployeeEditorDetailsControl
            Spanish.Add("EmployeeEditorTaxId", "Identificación de Impuestos Federales (Número de Seguro Social)");
            Spanish.Add("EmployeeEditorTicketPermissions", "Permisos de entradas");
            Spanish.Add("EmployeeEditorRegisterPermissions", "Registrarse Permisos");
            Spanish.Add("EmployeeEditorManagerPermissions", "Permisos de Administrador");
            Spanish.Add("EmployeeEditorPhoneNumbers", "Números de teléfono");
            #endregion

            #region EmployeeJobEditorControl
            Spanish.Add("EmployeeEditorCanDeclareTips", "Puede declarar Consejos");
            Spanish.Add("EmployeeEditorCanTakeDeliveries", "Puede tomar entregas");
            #endregion

            #region EmployeeJobMaintenanceControl
            Spanish.Add("EmployeeJobEditorNewJob", "Nuevo Trabajo");
            Spanish.Add("EmployeeJobEditorUpdateJob", "Actualización de trabajo");
            #endregion

            #region EmployeeJobSelectionControl
            Spanish.Add("EmployeeJobEditorAddJob", "Añadir Job");
            Spanish.Add("EmployeeJobEditorEditPayRate", "Editar Sueldo");
            Spanish.Add("EmployeeJobEditorRemoveJob", "Retire Trabajo");
            Spanish.Add("EmployeeJobEditorPayRate", "Sueldo");
            Spanish.Add("EmployeeJobEditorSelectJobsFor", "Seleccione Trabajos de");
            #endregion

            #region ExitControl
            Spanish.Add("ExitLockWorkstation", "Bloquear estación de trabajo");
            Spanish.Add("ExitLogoffWindows", "Cierre de sesión de Windows");
            Spanish.Add("ExitShutdownWindows", "apagado de Windows");
            Spanish.Add("ExitRestartWindows", "Reinicie Windows");
            Spanish.Add("ExitHibernate", "hibernar");
            Spanish.Add("ExitSuspend", "suspender");
            Spanish.Add("ExitRestartProgram", "Reinicie Programa");
            Spanish.Add("ExitExitProgramAndSql", "Salir del programa y SQL");
            Spanish.Add("ExitExitProgram", "Salir del programa");
            Spanish.Add("ExitStoppingSql", "Detener SQL Services ...");
            Spanish.Add("ExitExitTemPos", "Salir tempos");
            #endregion

            #region FutureTimeEditControl
            Spanish.Add("FutureTimeSetTime", "Establecer hora");
            Spanish.Add("FutureTimeMakeNow", "Haga ahora");
            Spanish.Add("FutureTimeTooEarly", "No se puede establecer un tiempo futuro más temprano que hoy en");
            Spanish.Add("FutureTimeTooEarlyError", "Error demasiado temprano");
            Spanish.Add("FutureTime", "Temporal Futura");
            #endregion

            #region GeneralSettingsBrushSetupControl
            Spanish.Add("BrushesApplication", "aplicación");
            Spanish.Add("BrushesBordersEnabled", "Borders - Activado");
            Spanish.Add("BrushesBordersDisabled", "Fronteras - Desactivado");
            Spanish.Add("BrushesButtonEnabled", "Button - Habilitado");
            Spanish.Add("BrushesButtonDisabled", "Button - Discapacitados");
            Spanish.Add("BrushesButtonEnabledSelected", "Button - Habilitado y seleccionado");
            Spanish.Add("BrushesButtonDisabledSelected", "Button - Desactivado seleccionada y");
            Spanish.Add("BrushesCaret", "signo de intercalación");
            Spanish.Add("BrushesCheckBoxEnabled", "CheckBox - Activado");
            Spanish.Add("BrushesCheckBoxDisabled", "CheckBox - Discapacitados");
            Spanish.Add("BrushesCheckBoxEnabledSelected", "CheckBox - Activado seleccionada y");
            Spanish.Add("BrushesCheckBoxDisabledSelected", "CheckBox - Discapacitados y seleccionado");
            Spanish.Add("BrushesComboBoxEnabled", "ComboBox - Activado");
            Spanish.Add("BrushesComboBoxDisabled", "ComboBox - Desactivado");
            Spanish.Add("BrushesLabelEnabled", "Label - Habilitado");
            Spanish.Add("BrushesLabelDisabled", "Label - Discapacitados");
            Spanish.Add("BrushesListItemEnabled", "Lista de elementos - Habilitado");
            Spanish.Add("BrushesListItemDisabled", "Lista de elementos - Discapacitados");
            Spanish.Add("BrushesListItemEnabledSelected", "Lista de elementos - Activado seleccionada y");
            Spanish.Add("BrushesListItemDisabledSelected", "Lista de elementos - Discapacitados y seleccionado");
            Spanish.Add("BrushesRadioButtonEnabled", "RadioButton - Activado");
            Spanish.Add("BrushesRadioButtonDisabled", "RadioButton - Desactivado");
            Spanish.Add("BrushesRadioButtonEnabledSelected", "RadioButton - Activado seleccionada y");
            Spanish.Add("BrushesRadioButtonDisabledSelected", "RadioButton - Discapacitados y seleccionado");
            Spanish.Add("BrushesTabButtonEnabled", "Botón Tab - Habilitado");
            Spanish.Add("BrushesTabButtonDisabled", "Tab Button - Desactivado");
            Spanish.Add("BrushesTabButtonEnabledSelected", "Botón Tab - Habilitado y seleccionado");
            Spanish.Add("BrushesTabButtonDisabledSelected", "Botón Tab - Desactivado seleccionada y");
            Spanish.Add("BrushesTextBoxEnabled", "TextBox - Activado");
            Spanish.Add("BrushesTextBoxDisabled", "TextBox - Desactivado");
            Spanish.Add("BrushesWindowTitleBar", "Ventana de la barra de título");
            Spanish.Add("BrushesForegroundColors", "colores de primer plano");
            Spanish.Add("BrushesBackgroundColors", "colores de fondo");
            #endregion

            #region GeneralSettingsGeneralPreferencesControl
            Spanish.Add("SettingsClientMessageBroadcastServer", "Mensaje de cliente del servidor de difusión");
            Spanish.Add("SettingsTest", "prueba");
            Spanish.Add("SettingsGeneralOptions", "Opciones generales");
            Spanish.Add("SettingsUseSeating", "Use asientos");
            Spanish.Add("SettingsForceWasteOnVoid", "Fuerza de residuos en los huecos");
            Spanish.Add("SettingsRestrictKeyboard", "Restringir el teclado");
            Spanish.Add("SettingsWeatherConditions", "Estado del tiempo");
            Spanish.Add("SettingsPostalCode", "código postal");
            Spanish.Add("SettingsAutoLogoutOptions", "Auto-Salir Opciones");
            Spanish.Add("SettingsAutoLogoutTimeout", "Tiempo de espera (segundos)");
            Spanish.Add("SettingsAutoLogoutDisable", "inhabilitar");
            Spanish.Add("SettingsAutoLogoutDisableOrderEntry", "En la entrada de pedidos");
            Spanish.Add("SettingsAutoLogoutDisableDialogs", "Para las ventanas de diálogo");
            Spanish.Add("SettingsAutoLogoutDisablePasswordChange", "En los cambios de contraseña");
            #endregion

            #region GeneralSettingsUpdateControl
            Spanish.Add("SettingsOptions", "Opciones");
            Spanish.Add("SettingsAutoUpdate", "Auto-Update");
            Spanish.Add("SettingsServer", "servidor");
            Spanish.Add("SettingsVersionCheck", "Version Check");
            Spanish.Add("SettingsUpdateNow", "Actualizar ahora");
            Spanish.Add("UpdateDownloadError", "Error: la actualización descargado está dañado. Actualización se canceló");
            Spanish.Add("UpdateConnected", "conectado");
            Spanish.Add("UpdateFailedToConnect", "No se ha podido conectar al servidor de actualizaciones");
            Spanish.Add("UpdateDisconnected", "desconectado");
            Spanish.Add("UpdateAuthenticated", "Jurada");
            Spanish.Add("UpdateNewestVersion", "La versión más reciente es");
            Spanish.Add("UpdateReceived", "Actualizar recibida");
            Spanish.Add("UpdateTemposUpdater", "tempos Updater");
            #endregion

            #region IngredientAmountControl
            Spanish.Add("MeasurementUnit", "Unidad de Medida");
            Spanish.Add("UnmeasuredUnits", "Unidades no medidos");
            Spanish.Add("WeightPound", "Peso: Libra");
            Spanish.Add("WeightOunce", "Peso: Onza");
            Spanish.Add("WeightGram", "Peso: Gram");
            Spanish.Add("WeightMilligram", "Peso: Miligramo");
            Spanish.Add("WeightKilogram", "Peso: Kilogramo");
            Spanish.Add("VolumeGallon", "Volumen: Galón");
            Spanish.Add("VolumeQuart", "Volumen: Quart");
            Spanish.Add("VolumePint", "Volumen: Pint");
            Spanish.Add("VolumeCup", "Volumen: Copa");
            Spanish.Add("VolumeTablespoon", "Volumen: Cucharada");
            Spanish.Add("VolumeTeaspoon", "Volumen: Cucharita");
            Spanish.Add("VolumeLiter", "Volumen: litros");
            Spanish.Add("VolumeFluidOunce", "Volumen: Fluid Ounce");
            Spanish.Add("VolumeMilliliter", "Volumen: Milliliter");
            Spanish.Add("VolumeKiloliter", "Volumen: kilolitro");
            #endregion

            #region IngredientEditorDetailsControl
            Spanish.Add("IngredientEditorIncreaseByAmount", "Incrementar Por importe");
            Spanish.Add("IngredientEditorIncreaseByRecipe", "Incrementar Por Receta");
            Spanish.Add("IngredientEditorDecreaseByAmount", "Disminuir Por importe");
            Spanish.Add("IngredientEditorDecreaseByRecipe", "Disminuir Por Receta");
            Spanish.Add("IngredientEditorNoYieldError", "No se puede aumentar o disminuir por receta, hasta que haya especificado un rendimiento receta (que se encuentra en la pestaña preparación de ingredientes)");
            Spanish.Add("IngredientEditorConvert1", "¿Te gustaría convertir la cantidad de inventario");
            Spanish.Add("IngredientEditorConvert2", "(y el rendimiento receta)");
            Spanish.Add("IngredientEditorConvert3", "de las unidades de medida a las antiguas unidades de medida nuevos?");
            Spanish.Add("IngredientEditorUpdateInventory", "Actualizar inventario");
            Spanish.Add("IngredientEditorPrintedName", "Nombre Impreso");
            Spanish.Add("IngredientEditorInventoryAmount", "Monto de Inventario");
            Spanish.Add("IngredientEditorMeasuringUnit", "Unidad de Medida:");
            Spanish.Add("IngredientEditorCostPerUnit", "Costo por unidad");
            Spanish.Add("IngredientEditorParAmount", "Monto PAR");
            Spanish.Add("IngredientEditorIncrease", "aumentar");
            Spanish.Add("IngredientEditorDecrease", "disminuir");
            #endregion

            #region IngredientEditorPreparationControl
            Spanish.Add("IngredientEditorPrepared", "Este es un ingrediente preparada");
            Spanish.Add("IngredientEditorAvailable", "Ingredientes disponibles");
            Spanish.Add("IngredientEditorCurrent", "Ingredientes actuales");
            Spanish.Add("IngredientEditorRecipeYield", "Receta Rendimiento");
            Spanish.Add("IngredientEditorUnits", "Unidades");
            Spanish.Add("IngredientEditorAddIngredient", "Añadir los ingredientes");
            Spanish.Add("IngredientEditorEditIngredient", "Editar Importe");
            Spanish.Add("IngredientEditorRemoveIngredient", "Retire Ingrediente");
            Spanish.Add("IngredientEditorAmount", "monto:");
            Spanish.Add("IngredientEditorWarningUncheck", "Si no selecciona esta opción, los ingredientes utilizados para preparar este ingrediente ya no se asocia con la preparación de este ingrediente.");
            Spanish.Add("IngredientEditorEditAmount", "Editar Ingrediente Cantidad");
            Spanish.Add("IngredientEditorConfirmRemove", "¿Está seguro que desea eliminar el ingrediente seleccionado de la receta?");
            Spanish.Add("IngredientEditorEditRecipeYield", "Editar receta Rinde");
            #endregion

            #region InventoryEditorControl
            Spanish.Add("InventoryIncreaseByAmount", "Incrementar Por importe");
            Spanish.Add("InventoryIncreaseByRecipe", "Incrementar Por Receta");
            Spanish.Add("InventoryDecreaseByAmount", "Disminuir Por importe");
            Spanish.Add("InventoryDecreaseByRecipe", "Disminuir Por Receta");
            Spanish.Add("InventorySetAmount", "Indique la cantidad de inventario");
            Spanish.Add("InventoryError", "No se puede aumentar o disminuir por receta, hasta que haya especificado un rendimiento receta (que se encuentra en la pestaña preparación de ingredientes)");
            Spanish.Add("InventoryEdit", "Editar Inventario");
            #endregion

            #region ItemEditorControl
            Spanish.Add("ItemEditorErrorNoCategory", "No ha seleccionado una categoría para este artículo.");
            Spanish.Add("ItemEditorInvalidCategory", "Categoría no válido");
            Spanish.Add("ItemEditorErrorNoName", "Nombre del artículo está en blanco.");
            Spanish.Add("ItemEditorErrorExistingName", "Ese nombre de producto está siendo utilizado.");
            Spanish.Add("ItemEditorInvalidName", "Nombre no válido");
            #endregion

            #region ItemEditorDetailsControl
            Spanish.Add("ItemEditorInvalidPrice", "Precio no válido");
            Spanish.Add("ItemEditorInvalidTaxSetting", "Ajuste fiscal no válido");
            Spanish.Add("ItemEditorCategory", "categoría");
            Spanish.Add("ItemEditorFullName", "Nombre Completo");
            Spanish.Add("ItemEditorButtonName", "POS Nombre del botón");
            Spanish.Add("ItemEditorPrice", "precio");
            Spanish.Add("ItemEditorPrintDestination", "Destino de impresión automática");
            Spanish.Add("ItemEditorTax", "impuesto");
            Spanish.Add("ItemEditorIsReturnable", "Es Retornable");
            Spanish.Add("ItemEditorIsFired", "se dispara");
            Spanish.Add("ItemEditorIsTaxExemptable", "Puede gozar de exención fiscal");
            Spanish.Add("ItemEditorAvailableForSale", "Disponible a la Venta");
            Spanish.Add("ItemEditorIsOutOfStock", "Es fuera de stock");
            Spanish.Add("ItemEditorIsGrouping", "es la agrupación");
            Spanish.Add("ItemEditorJournal", "revista");
            Spanish.Add("ItemEditorReceipt", "recibo");
            Spanish.Add("ItemEditorKitchen1", "Cocina 1");
            Spanish.Add("ItemEditorKitchen2", "Cocina 2");
            Spanish.Add("ItemEditorKitchen3", "Cocina 3");
            Spanish.Add("ItemEditorBar1", "Bar 1");
            Spanish.Add("ItemEditorBar2", "Bar 2");
            Spanish.Add("ItemEditorBar3", "bar 3");
            #endregion

            #region ItemEditorGroupingControl
            Spanish.Add("ItemEditorQuantity", "Cantidad:");
            Spanish.Add("ItemEditorErrorDemo", "Sólo se pueden agrupar dos elementos de la versión demo.");
            Spanish.Add("ItemEditorDemoRestriction", "demo de Restricción");
            Spanish.Add("ItemEditorEditQuantity", "Editar Cantidad");
            Spanish.Add("ItemEditorConfirmRemove", "¿Está seguro que desea eliminar el elemento seleccionado de los elementos incluidos?");
            Spanish.Add("ItemEditorAvailableItems", "Artículos disponibles");
            Spanish.Add("ItemEditorIncludedItems", "elementos incluidos");
            Spanish.Add("ItemEditorErrorStartOfDay", "Agrupaciones artículo no puede ser modificado durante un día de la operación. Completar un fin-de-día para modificar.");
            Spanish.Add("ItemEditorAddItem", "Agregar elemento");
            Spanish.Add("ItemEditorEditQuantityButton", "Editar Cantidad");
            Spanish.Add("ItemEditorRemoveItem", "Quitar el producto");
            #endregion

            #region ItemEditorIngredientsControl
            Spanish.Add("ItemEditorAmount", "monto:");
            Spanish.Add("ItemEditorIngredientAmount", "Artículo Ingrediente Cantidad");
            Spanish.Add("ItemEditorEditIngredient", "Editar Elemento del artículo");
            Spanish.Add("ItemEditorConfirmIngredientRemove", "¿Está seguro que desea eliminar el ingrediente seleccionado de la receta?");
            Spanish.Add("ItemEditorAvailableIngredients", "Ingredientes disponibles");
            Spanish.Add("ItemEditorCurrentIngredients", "Ingredientes actuales");
            Spanish.Add("ItemEditorAddIngredient", "Añadir los ingredientes");
            Spanish.Add("ItemEditorEditIngredientAmount", "Editar Importe");
            Spanish.Add("ItemEditorRemoveIngredient", "Retire Ingrediente");
            #endregion

            #region ItemEditorOptionSetControl
            Spanish.Add("ItemEditorOptionSet1", "Conjunto de opciones 1");
            Spanish.Add("ItemEditorOptionSet2", "Conjunto de opciones 2");
            Spanish.Add("ItemEditorOptionSet3", "Conjunto de opciones 3");
            #endregion

            #region ItemEditorSpecialPricingControl
            Spanish.Add("ItemEditorListboxText1", ", De:");
            Spanish.Add("ItemEditorListboxText2", "a");
            Spanish.Add("ItemEditorDay", "día");
            Spanish.Add("ItemEditorStartTime", "Hora de inicio");
            Spanish.Add("ItemEditorEndTime", "Final del tiempo");
            Spanish.Add("ItemEditorMinDiscount", "Mínimo para el descuento");
            Spanish.Add("ItemEditorMaxDiscount", "Descuento máximo");
            Spanish.Add("ItemEditorDiscountPrice", "Ventajas económicas");
            #endregion

            #region ItemMaintenanceControl
            Spanish.Add("ItemSetupFind", "encontrar");
            Spanish.Add("ItemSetupEditItemOptions", "Editar Elemento Opciones");
            Spanish.Add("ItemSetupFindNext", "Buscar siguiente");
            Spanish.Add("ItemSetupView", "ver");
            Spanish.Add("ItemSetupItems", "Artículos");
            Spanish.Add("ItemSetupItemsInCategory", "Artículos en la categoría: \"");
            Spanish.Add("ItemSetupCategories", "Categorías");
            Spanish.Add("ItemSetupIngredients", "Ingredientes");
            Spanish.Add("ItemSetupItemOptionSets", "Conjuntos de elementos opcionales");
            Spanish.Add("ItemSetup", "elemento de configuración");
            Spanish.Add("ItemSetupUpdateItem", "actualizar el artículo");
            Spanish.Add("ItemSetupAddItem", "Agregar elemento");
            Spanish.Add("ItemSetupDeleteItem", "Eliminar elemento");
            Spanish.Add("ItemSetupUpdateCategory", "Actualizar Categoría");
            Spanish.Add("ItemSetupAddCategory", "Añadir categoría");
            Spanish.Add("ItemSetupDeleteCategory", "Eliminar categoría");
            Spanish.Add("ItemSetupUpdateIngredient", "Actualizar Ingrediente");
            Spanish.Add("ItemSetupAddIngredient", "Añadir los ingredientes");
            Spanish.Add("ItemSetupUpdateOptionSet", "Actualizar Conjunto de opciones");
            Spanish.Add("ItemSetupAddOptionSet", "Añadir Conjunto de opciones");
            Spanish.Add("ItemSetupDeleteOptionSet", "Eliminar Conjunto de opciones");
            Spanish.Add("ItemSetupNotifyNoAdditialItemsFound", "No hay elementos adicionales encontrados que contienen");
            Spanish.Add("ItemSetupSearchItems", "Buscar artículos");
            Spanish.Add("ItemSetupNotifyNoItemsFound", "No se encontraron elementos que contiene");
            Spanish.Add("ItemSetupConfirmDeleteOptionSet", "¿Está seguro que desea eliminar el conjunto de elementos de opción seleccionado?");
            Spanish.Add("ItemSetupConfirmDeleteCategory", "¿Está seguro que desea eliminar la categoría seleccionada y todos los artículos de la categoría?");
            Spanish.Add("ItemSetupConfirmDeleteItem", "¿Está seguro que desea eliminar el elemento seleccionado?");
            Spanish.Add("ItemSetupValidationError", "Error de validación");
            #endregion

        }
    }
}
