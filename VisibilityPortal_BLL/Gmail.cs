﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisibilityPortal_BLL
{

    public class GMAILConfig
    {
        public static string username { get; set; }
        public static string password { get; set; }
        public static string host { get; set; }
        public static int port { get; set; }
        public static bool GmailSSL { get; set; }

        static GMAILConfig()
        {
            username = "coretec.mobility.team@gmail.com";
            password = "!Team@123";
            host = "smtp.gmail.com";
            port = 587;
            GmailSSL = true;
        }
    }
}
