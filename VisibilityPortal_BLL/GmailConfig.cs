using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisibilityPortal_BLL
{

    public static class GMAILConfig
    {
        public static string username { get; set; }
        public static string password { get; set; }
        public static string host { get; set; }
        public static int port { get; set; }
        public static bool GmailSSL { get; set; }

        static GMAILConfig()
        {
            //username = "coretec.mobility.team@gmail.com";
            username = "coretec.msacco@gmail.com";
            password = "!Team@123";
            host = "smtp.gmail.com";
            port = 587;
            GmailSSL = true;
        }
    }
    public static class OutlookConfig
    {
        public static string username { get; set; }
        public static string password { get; set; }
        public static string host { get; set; }
        public static int port { get; set; }
        public static string EncryptionMethod { get; set; }

        static OutlookConfig()
        {
            username = "msacco@coretec.co.ke";
            password = "!Team@123";
            host = "smtp.office365.com";
            port = 587;
            EncryptionMethod = "STARTTLS";
        }
    }
}
