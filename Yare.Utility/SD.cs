namespace Yare_WebApplication.Data.Utility;

public static class SD
{
    //User Roles 
    public const string Role_User_Customer = "Customer";
    public const string Role_MasterAdmin = "Master Admin";
    public const string Role_Admin = "Admin";
    public const string Role_Employee = "Employee";

    //Stock Status
    public const string OutOfStockStatus = "Out of stock";
    public const string RunningLowStockStatus = "Running low";
    public const string InStockStatus = "In stock";

    //Order Status
    public const string StatusApproved = "Approved";
    public const string StatusPending = "Pending";
    public const string StatusInProcess = "Processing";
    public const string StatusShipped = "Shipped";
    public const string StatusCancelled = "Cancelled";
    public const string StatusRefunded = "Refunded";

    //Payment Status
    public const string PaymentStatusPending = "Pending";
    public const string PaymentStatusApproved = "Approved";
    public const string PaymentStatusRejected = "Rejected";

    //Stripe Api Session
    public const string SessionCart = "SessionShoppingCart";
}
