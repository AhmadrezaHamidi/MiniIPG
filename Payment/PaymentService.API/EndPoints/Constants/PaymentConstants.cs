namespace PaymentService.API.EndPoints.Constants
{
    public static class PaymentConstants
    {
        // Routes
        public static class Routes
        {
            public const string BaseRoute = "api/v{version:apiVersion}/Payment";
            public const string GetToken = "/get-token";
            public const string Verify = "/verify";
            public const string UpdateStatus = "/update-status";
        }

        // Endpoint Names
        public static class Names
        {
            public const string GetToken = "GetPaymentToken";
            public const string Verify = "VerifyPayment";
            public const string UpdateStatus = "UpdatePaymentStatus";
        }

        // Documentation
        public static class Docs
        {
            public static class GetToken
            {
                public const string Summary = "دریافت توکن پرداخت";
                public const string Description = "این سرویس برای ایجاد توکن پرداخت جدید استفاده می‌شود";
            }

            public static class Verify
            {
                public const string Summary = "تایید تراکنش";
                public const string Description = "این سرویس برای تایید نهایی تراکنش استفاده می‌شود";
            }

            public static class UpdateStatus
            {
                public const string Summary = "به‌روزرسانی وضعیت تراکنش";
                public const string Description = "این سرویس برای به‌روزرسانی وضعیت تراکنش از درگاه بانک استفاده می‌شود";
            }
        }
    }
}
