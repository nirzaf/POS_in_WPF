namespace PosModels.Types
{
    public enum Permissions
    {
        None = -0x01,
        // Ticket and Register Permissions
        LateCancel = 0x00,           // Byte 0, Bit 0
        Void = 0x01,                 // Byte 0, Bit 1
        Cashout = 0x02,              // Byte 0, Bit 2
        RegisterStart = 0x03,        // Byte 0, Bit 3
        RegisterPayout = 0x04,       // Byte 0, Bit 4
        RegisterDrop = 0x05,         // Byte 0, Bit 5
        RegisterNoSale = 0x06,       // Byte 0, Bit 6
        RegisterRefund = 0x07,       // Byte 0, Bit 7
        RegisterReturn = 0x08,       // Byte 1, Bit 0
        RegisterReport = 0x09,       // Byte 1, Bit 1
        RegisterClose = 0x0A,        // Byte 1, Bit 2
        DriverDispatch = 0x0B,       // Byte 1, Bit 3
        RegisterDiscounts = 0x0C,    // Byte 1, Bit 4
        RegisterDeposit = 0x0D,      // Byte 1, Bit 5
        DriverBankrolling = 0x0E,    // Byte 1, Bit 6
        UseAnyRegisterDrawer = 0x0F, // Byte 1, Bit 7
        ChangeTicketOwner = 0x10,    // Byte 2, Bit 0
        // 0x11    // Byte 2, Bit 1
        // 0x12    // Byte 2, Bit 2
        // 0x13    // Byte 2, Bit 3
        // 0x14    // Byte 2, Bit 4
        // 0x15    // Byte 2, Bit 5
        // 0x16    // Byte 2, Bit 6
        // 0x17    // Byte 2, Bit 7

        // Reports and Maintenance
        ReportsMenu = 0x18,                  // Byte 3, Bit 0
        SystemMaintenance = 0x19,            // Byte 3, Bit 1
        EmployeeMaintenance = 0x1A,          // Byte 3, Bit 2
        EmployeeScheduleMaintenance = 0x1B,  // Byte 3, Bit 3
        EmployeeTimesheetMaintenance = 0x1C, // Byte 3, Bit 4
        CustomerMaintenance = 0x1D,          // Byte 3, Bit 5
        VendorMaintenance = 0x1E,            // Byte 3, Bit 6
        InventoryAdjustments = 0x1F,         // Byte 3, Bit 7
        CommandShell = 0x20,                 // Byte 4, Bit 0
        StartOfDay = 0x21,                   // Byte 4, Bit 1
        EndOfDay = 0x22,                     // Byte 4, Bit 2
        ManagerAlerts = 0x23,                // Byte 4, Bit 3
        // 0x24    // Byte 4, Bit 4
        // 0x25    // Byte 4, Bit 5
        // 0x26    // Byte 4, Bit 6
        // 0x27    // Byte 4, Bit 7
        // 0x28    // Byte 5, Bit 0
        // 0x29    // Byte 5, Bit 1
        // 0x2A    // Byte 5, Bit 2
        // 0x2B    // Byte 5, Bit 3
        // 0x2C    // Byte 5, Bit 4
        // 0x2D    // Byte 5, Bit 5
        // 0x2E    // Byte 5, Bit 6
        // 0x2F    // Byte 5, Bit 7
        
        // Override Options
        OverrideDeliveryRestriction = 0x30,   // Byte 6, Bit 0
        
        // Other Permissions
        ExitProgram = 0x38    // Byte 7, Bit 0        

        // There is room for upto 64(0x3F) permissions
    }
}
