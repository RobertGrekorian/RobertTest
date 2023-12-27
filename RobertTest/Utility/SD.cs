namespace RobertTest.Utility
{
    public class SD
    {
        public enum ApiType
        {
            GET, POST, PUT, DELETE
        }

        public enum ContentType
        {
            Json,
            MultipartFormData
        }

        public static string AuthApiBase { get; set; }
        public const string RoleAdmin = "ADMIN";
        public const string RoleCustomer = "CUSTOMER";
        public const string TokenCookie = "JwtToken";
    }
}
