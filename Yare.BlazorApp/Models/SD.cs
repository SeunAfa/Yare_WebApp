namespace Yare.BlazorApp.Models;

public static class SD
{
    // User Roles
    public const string Role_Customer = "Customer";
    public const string Role_MasterAdmin = "Master Admin";
    public const string Role_Admin = "Admin";
    public const string Role_Employee = "Employee";

    // Stock Status
    public const string OutOfStock = "Out of stock";
    public const string RunningLow = "Running low";
    public const string InStock = "In stock";

    // Order Status
    public const string StatusApproved = "Approved";
    public const string StatusPending = "Pending";
    public const string StatusInProcess = "Processing";
    public const string StatusShipped = "Shipped";
    public const string StatusCancelled = "Cancelled";
    public const string StatusRefunded = "Refunded";

    // Payment Status
    public const string PaymentStatusPending = "Pending";
    public const string PaymentStatusApproved = "Approved";
    public const string PaymentStatusRejected = "Rejected";

    // LocalStorage keys
    public const string CartKey = "yare_cart";
    public const string UserKey = "yare_user";
    public const string OrdersKey = "yare_orders";
}
