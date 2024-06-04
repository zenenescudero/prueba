namespace ArzyzWeb.Models
{
    public class UserResetPass
    {
        public string id { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string confirm_password { get; set; }
        public string temp_password { get; set; }
        public string language { get; set; }

    }
}
