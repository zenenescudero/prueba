using ArzyzWeb.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace ArzyzWeb.Controllers
{
    public class BaseController : Controller
    {
        public static readonly string pathTemp = Path.GetFullPath("Temp");
        public static readonly string pathFiles = Path.GetFullPath("Files");
        public static readonly string pathPhotos = Path.GetFullPath("PhotosUsers");
        public static readonly string pathTemplates = Path.GetFullPath("Templates");
        protected OneMitigationData.OneMitigationContext _context = new OneMitigationData.OneMitigationContext();
        protected string base_company_id;
        protected string base_customer_id;


        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
            base.Dispose(disposing);
        }
        public UserLoginModel CurrentUser
        {
            get
            {
                return new UserLoginModel()
                {
                    id = HttpContext.User.FindFirst("id")?.Value,
                    name = HttpContext.User.FindFirst("name_user")?.Value,
                    email = HttpContext.User.FindFirst("email_user")?.Value,
                    user_type = HttpContext.User.FindFirst("user_type")?.Value,
                    company_id = HttpContext.User.FindFirst("company_id")?.Value,
                    customer_id = HttpContext.User.FindFirst("customer_id")?.Value,
                    language = HttpContext.User.FindFirst("language")?.Value,
                    language_name = HttpContext.User.FindFirst("language_name")?.Value,
                    logo = HttpContext.User.FindFirst("logo")?.Value,
                    company_name = HttpContext.User.FindFirst("company_name")?.Value,
                    reports = HttpContext.User.FindFirst("reports")?.Value,
                };
            }
        }    
    }
}
