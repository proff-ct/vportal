using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Validators.ValidationAttributes
{
    public class ValidateCoreTecEmailDomain : ValidationAttribute
  {
    private string _domain
    {
      get
      {
        return "@coretec.co.ke";
      }
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
      if (value != null)
      {
        string email = value.ToString();
        if (email.Substring(email.IndexOf('@')) == _domain)
        {
          return ValidationResult.Success;
        }
        else
        {
          return new ValidationResult("Please enter a valid CoreTec email address");
        }
      }
      else
      {
        return new ValidationResult("" + validationContext.DisplayName + " is required");
      }
    }

  }
}
