using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallCenter_BLL.Utilities
{
  public static class Validators
  {
    public static bool ValidateObject<T>(T obj)
    {
      ValidationContext vc = new ValidationContext(obj);
      ICollection<ValidationResult> results = new List<ValidationResult>();
      // Will contain the results of the validation
      bool isValid = Validator.TryValidateObject(obj, vc, results, true);
      // Validates the object and its properties using the previously created context.
      // The variable isValid will be true if everything is valid
      // The results variable contains the results of the validation
      return isValid;
    }
  }
}
