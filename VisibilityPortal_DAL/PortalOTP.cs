using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisibilityPortal_Dataspecs.OTPAuth.Models;

namespace VisibilityPortal_DAL
{
    public class PortalOTP : IDB_OTP
    {
        public static string DBName => "VisibilityPortal";
        public static string DBTableName => "OTP_AUTH";
        public string Code { get; set; }
        public string UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdatedAt { get; set; }
        public string Status { get; set; }
        public string RequestingDeviceParameters { get; set; }
        public string VerifyingDeviceParameters { get; set; }
    }
}
