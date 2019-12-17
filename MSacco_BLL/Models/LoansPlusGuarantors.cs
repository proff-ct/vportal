using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MSacco_DAL;

namespace MSacco_BLL.Models
{
  public class LoansPlusGuarantors : MSaccoSalaryAdvance
  {
    public virtual List<Guarantors> Guarantors { get; set; }
  }
}
