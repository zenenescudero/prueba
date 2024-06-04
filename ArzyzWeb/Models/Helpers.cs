using System.IO;
using System.Text.RegularExpressions;
using System;
using System.Linq;
using System.Collections.Generic;
namespace ArzyzWeb.Models
{
    public static class Helpers
    {
        public const string formatDate = "yyyy-MM-dd";
        public const string formatGetDate = "current_date()";
        public const int pageSizeMax = int.MaxValue;
        private static Random random = new Random();
        public enum userType
        {
            SuperAdmin = 0,
            Admin = 1,
            User = 2,
            UserCustomer = 3
        }

        public enum table_names
        {
            sys_catalogs,
            sys_labels
        }
        public static string ConcatInValues(string text)
        {
            string inStatus = string.IsNullOrWhiteSpace(text) ? "" : "'" + string.Join("','", text.Split('|')) + "'";

            return inStatus;
        }
        public static string UserTypeTextId(userType typeU)
        {
            return ((int)typeU).ToString();
        }

        public static void ValidatePassword(string password)
        {
            var input = password;

            var hasNumber = new Regex(@"[0-9]+");
            var hasUpperChar = new Regex(@"[A-Z]+");
            var hasMiniMaxChars = new Regex(@".{8,}");
            var hasLowerChar = new Regex(@"[a-z]+");
            var hasSymbols = new Regex(@"[!@#$%^&*()_+=\[{\]};:<>|./?,-]");

            if (!hasLowerChar.IsMatch(input))
            {
                throw new OMxception(MessagesApp.sys_pass_rule_lower);
            }
            else if (!hasUpperChar.IsMatch(input))
            {
                throw new OMxception(MessagesApp.sys_pass_rule_upper);
            }
            else if (!hasMiniMaxChars.IsMatch(input))
            {
                throw new OMxception(MessagesApp.sys_pass_rule_min);
            }
            else if (!hasNumber.IsMatch(input))
            {
                throw new OMxception(MessagesApp.sys_pass_rule_num);
            }
            else if (!hasSymbols.IsMatch(input))
            {
                throw new OMxception(MessagesApp.sys_pass_rule_special);
            }

        }

        public static bool Email_OK(string email)
        {
            try
            {
                string expresion;
                expresion = "\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*";
                if (Regex.IsMatch(email, expresion))
                {
                    if (Regex.Replace(email, expresion, string.Empty).Length == 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }

            }
            catch (Exception)
            {
                return false;
            }
        }

      
        public static void LogRegister(string metodo, string cadena)
        {
            try
            {
                var archivo = "";
                archivo = Path.Combine(Directory.GetCurrentDirectory(), "Logs", "log" + DateTime.Now.ToString("yyyyMMdd") + ".txt");
                using (StreamWriter sw = File.AppendText(archivo))
                {
                    sw.WriteLine($"{DateTime.Now.ToString()} : {metodo} -> {cadena} ");
                }
            }
            catch (Exception ee)
            {
                Console.WriteLine(ee);
            }
        }

        public static void LogRegister(string metodo, Exception cadena)
        {
            try
            {
                var archivo = "";
                archivo = Path.Combine(Directory.GetCurrentDirectory(), "Logs", "log" + DateTime.Now.ToString("yyyyMMdd") + ".txt");
                using (StreamWriter sw = File.AppendText(archivo))
                {
                    sw.WriteLine($"{DateTime.Now.ToString()} : {metodo} -> {cadena.Message} InnerException: {cadena.InnerException?.Message} {cadena.StackTrace} ");
                }
            }
            catch (Exception ee)
            {
                Console.WriteLine(ee);
            }
        }
        public static string GetRandomString(int length)
        {
            const string characters = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
            return new string(Enumerable.Repeat(characters, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
                      
        public static List<int> MonthsMitigation(int initialMonth, int period)
        {
            List<int> months = new List<int>();

            DateTime initialDate = new DateTime(DateTime.Now.Year, initialMonth, 1);

            do
            {
                months.Add(initialDate.Month);
                initialDate = initialDate.AddMonths(period);
            }
            while (initialDate.Month != initialMonth);

            return months;
        }

        public static int GetPeriodNumber(string period)
        {
            return period switch
            {
                "sys_period_monthly" => 1,
                "sys_period_bimonthly" => 2,
                "sys_period_quarterly" => 3,
                "sys_period_semiannual" => 6,
                "sys_period_annual" => 12,
                _ => 1,
            };
        }

        public static string GenerateICalendarEvent(DateTime? eventStartDate, DateTime? eventEndDate,
            string eventName, string description, string eventLocation, string language_code)
        {
            string iCalendarContent = $"BEGIN:VCALENDAR{Environment.NewLine}" +
                                  "VERSION:2.0" + Environment.NewLine +
                                  $"PRODID:-//{ConstantsValues.APP_NAME}//NONSGML Calendar//{language_code.ToUpper()}" + Environment.NewLine +
                                  $"BEGIN:VEVENT{Environment.NewLine}" +
                                  $"DTSTART:{eventStartDate:yyyyMMdd}T000000{Environment.NewLine}" +
                                  $"DTEND:{eventEndDate:yyyyMMdd}T235959{Environment.NewLine}" +
                                  $"SUMMARY:{eventName.Replace("\n", " ")}{Environment.NewLine}" +
                                  $"DESCRIPTION:{description.Replace("\n", " ")}{Environment.NewLine}" +
                                  $"LOCATION:{eventLocation}{Environment.NewLine}" +
                                  "END:VEVENT" + Environment.NewLine +
                                  "END:VCALENDAR";

            return iCalendarContent;
        }


    }
}
