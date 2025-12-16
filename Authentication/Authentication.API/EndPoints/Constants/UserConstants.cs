namespace Authentication.API.EndPoints.Constants;
    public static class UserConstants
    {
        public static class Routes
        {
            public const string BaseRoute = "api/v{version:apiVersion}/User";

            public const string Login = "/login";
            public const string RefreshToken = "/refresh-token";
            public const string GetAll = "";
            public const string GetById = "/{id:int}";
            public const string Register = "/register";
            public const string RegisterAdminUser = "/register-admin-user";
            public const string Update = "/update";
            public const string Delete = "/{userId:int}";
        }

        public static class Names
        {
            public const string Login = "UserLogin";
            public const string RefreshToken = "UserRefreshToken";
            public const string GetAll = "GetAllUsers";
            public const string GetById = "GetUserById";
            public const string Register = "RegisterUser";
            public const string RegisterAdminUser = "RegisterAdminUser";
            public const string Update = "UpdateUser";
            public const string Delete = "DeleteUser";
        }

        public static class Docs
        {
            public static class Login
            {
                public const string Summary = "ورود کاربر";
                public const string Description = "احراز هویت کاربر و دریافت توکن";
            }

            public static class RefreshToken
            {
                public const string Summary = "تمدید توکن";
                public const string Description = "دریافت توکن جدید با استفاده از refresh token";
            }

            public static class GetAll
            {
                public const string Summary = "دریافت لیست همه کاربران";
                public const string Description = "فقط برای ادمین";
            }

            public static class GetById
            {
                public const string Summary = "دریافت کاربر بر اساس شناسه";
                public const string Description = "فقط برای ادمین";
            }

            public static class Register
            {
                public const string Summary = "ثبت‌نام کاربر عادی";
                public const string Description = "ایجاد کاربر جدید با نقش user";
            }

            public static class RegisterAdminUser
            {
                public const string Summary = "ثبت‌نام کاربر ادمین";
                public const string Description = "ایجاد کاربر جدید با نقش admin (فقط ادمین)";
            }

            public static class Update
            {
                public const string Summary = "به‌روزرسانی اطلاعات کاربر";
                public const string Description = "به‌روزرسانی پروفایل کاربر";
            }

            public static class Delete
            {
                public const string Summary = "حذف کاربر";
                public const string Description = "حذف کاربر بر اساس شناسه (فقط ادمین)";
            }
        }
    }
