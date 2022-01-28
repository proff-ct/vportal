using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisibilityPortal_Dataspecs.OTPAuth
{
    namespace Models
    {
        public interface IOTP
        {
            string Code { get; set; }
            string UserId { get; set; }
            DateTime CreatedAt { get; set; }
            OTP_STATUS Status { get; set; }
        }

        public enum OTP_STATUS
        {
            Pending,
            Expired,
            Verified
        }

        public interface IDB_OTP
        {
            string Code { get; set; }
            string UserId { get; set; }
            DateTime CreatedAt { get; set; }
            DateTime LastUpdatedAt { get; set; }
            string Status { get; set; }
            string RequestingDeviceParameters { get; set; }
            string VerifyingDeviceParameters { get; set; }
        }
    }
    namespace Functions
    {
        public interface IBL_OTPAuth
        {
            string GenerateOTP(string userId);
            bool ValidateOTP(string userId, string otp, out string validationMsg);
        }
    }
    
}
