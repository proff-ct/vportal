using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisibilityPortal_BLL.Tests
{
    namespace OTPAuthBLL_Functions
    {
        [TestFixture]
        public class GenerateOTP
        {
            [Test]
            public void Returns_otp_code_as_string()
            {

            }
        }
        [TestFixture]
        public class ValidateOTP
        {
            [Test]
            public void Returns_true_if_supplied_otp_is_for_current_user_and_has_not_expired()
            {

            }
            [Test]
            public void Returns_false_if_supplied_otp_is_for_current_user_and_has_expired_and_sets_validationMsg()
            {

            }
        }
    }
}
