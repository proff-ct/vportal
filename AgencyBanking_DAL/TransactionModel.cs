using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencyBanking_DAL
{
   public class TransactionModel
    {
       public string Amount{get;set;}
       public string Posted{get;set;}
       public string DatePosted{get;set;}
       public string TimePosted{get;set;}
      
       public string Description{get;set;}
       public string AgentCode{get;set;}
       public string Location{get;set;}

       public string AgentName{get;set;}
       public string DepositerTelephoneNo{get;set;}
       public string  DeviceID{get;set;}
    }
}
