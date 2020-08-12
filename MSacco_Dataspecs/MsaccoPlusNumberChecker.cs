using MSacco_Dataspecs.MSSQLOperators;
using System;
using System.Collections.Generic;

namespace MSacco_Dataspecs.Feature.MsaccoPlusNumberChecker
{
  namespace Models
  {
    public interface IMsaccoPlusNumberChecker
    {
      int Id { get; set; }
      string PhoneNumber { get; set; }
      string DeviceId { get; set; }
      string Comments { get; set; }
      DateTime DateLinked { get; set; }
      DateTime LastUpdated { get; set; }
    }

    public interface IMsaccoPlusNumberCheckerViewModel
    {
      int Id { get; set; }
      string PhoneNumber { get; set; }
      string Comments { get; set; }
      DateTime LastUpdated { get; set; }
      DateTime DateLinked { get; set; }
    }
  }
  namespace Functions
  {
    public interface IBL_MsaccoPlusNumberChecker
    {
      IEnumerable<Models.IMsaccoPlusNumberChecker> GetMsaccoPlusRegisteredMemberDevicesListForClient(string corporateNo, out int lastPage, bool paginate = false, IPaginationParameters pagingParams = null);

      Models.IMsaccoPlusNumberChecker GetMsaccoPlusRegisteredMemberDeviceRecordForClient(string corporateNo, string MemberPhoneNo);
      bool ResetMsaccoPlusMemberDeviceForClient(string corporateNo, string memberPhoneNo, out string resetMessage);
    }
  }
}