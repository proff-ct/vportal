using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using MSacco_BLL.Models;
using MSacco_BLL.ViewModels;
using MSacco_DAL;
using MSacco_Dataspecs.Feature.MSACCOBankTransfer.Models;
using MSacco_Dataspecs.Feature.MsaccoPlusNumberChecker.Models;
using MSacco_Dataspecs.Feature.MsaccoTIMSINumberChecker.Models;
using MSacco_Dataspecs.Models;

namespace MSacco_BLL.Utilities.AutoMapper
{
  public static class Mappings
  {
    public static void RegisterMappings()
    {
      Mapper.Initialize(
        config => {
          config.CreateMap<MSaccoSalaryAdvance, LoanListViewModel>().ReverseMap();
          config.CreateMap<MSaccoSalaryAdvance, LoansPlusGuarantors>();
          config.CreateMap<LinkMonitoring, LinkMonitoringViewModel>();
          config.CreateMap<LinkDowntime, LinkDowntimeViewModel>();
          config.CreateMap<LinkMonitoring, LinkStatusPlusDowntime>();
          config.CreateMap<LinkStatusPlusDowntime, LinkStatusPlusDowntimeViewModel>();
          config.CreateMap<VirtualRegistrationNewIPRS, WauminiIPRSLookupViewModel>();
          config.CreateMap<VirtualRegistrationNewIPRS, VirtualRegistrationViewModel>();
          config.CreateMap<IMsaccoPlusNumberChecker, IMsaccoPlusNumberCheckerViewModel>();
          config.CreateMap<IMSACCO_TIMSI_RESET_DB_LOG, IMSACCO_TIMSI_RESET_ViewModel>();
          config.CreateMap<IBankTransfer, IBankTransferViewModel>();
          config.CreateMap<IMobileWithdrawals_DarajaDB, IMobileWithdrawals_SACCODB>(MemberList.Source)
          .ForMember(m => m.Exported_To_Saf, opt => opt.Ignore())
          .ForMember(m => m.Verified, opt => opt.Ignore());
        });
    }
    public static MapperConfiguration GetMapConfig()
    {
      return new MapperConfiguration(config =>
      {
        config.CreateMap<MSaccoSalaryAdvance, LoanListViewModel>().ReverseMap();
        config.CreateMap<MSaccoSalaryAdvance, LoansPlusGuarantors>();
        config.CreateMap<LinkMonitoring, LinkMonitoringViewModel>();
        config.CreateMap<LinkDowntime, LinkDowntimeViewModel>();
        config.CreateMap<LinkMonitoring, LinkStatusPlusDowntime>();
        config.CreateMap<LinkStatusPlusDowntime, LinkStatusPlusDowntimeViewModel>();
        config.CreateMap<VirtualRegistrationNewIPRS, WauminiIPRSLookupViewModel>();
        config.CreateMap<VirtualRegistrationNewIPRS, VirtualRegistrationViewModel>();
        config.CreateMap<IMsaccoPlusNumberChecker, IMsaccoPlusNumberCheckerViewModel>();
        config.CreateMap<IMSACCO_TIMSI_RESET_DB_LOG, IMSACCO_TIMSI_RESET_ViewModel>();
        config.CreateMap<IBankTransfer, IBankTransferViewModel>();
        config.CreateMap<IMobileWithdrawals_DarajaDB, IMobileWithdrawals_SACCODB>(MemberList.Source)
          .ForMember(m => m.Exported_To_Saf, opt => opt.Ignore())
          .ForMember(m => m.Verified, opt => opt.Ignore());
      });
    }

    public static Action<IMapperConfigurationExpression> MapperConfigExpr = new Action<IMapperConfigurationExpression>(
      config =>
    {
      config.CreateMap<MSaccoSalaryAdvance, LoanListViewModel>().ReverseMap();
      config.CreateMap<MSaccoSalaryAdvance, LoansPlusGuarantors>();
      config.CreateMap<LinkMonitoring, LinkMonitoringViewModel>();
      config.CreateMap<LinkDowntime, LinkDowntimeViewModel>();
      config.CreateMap<LinkMonitoring, LinkStatusPlusDowntime>();
      config.CreateMap<LinkStatusPlusDowntime, LinkStatusPlusDowntimeViewModel>();
      config.CreateMap<VirtualRegistrationNewIPRS, WauminiIPRSLookupViewModel>();
      config.CreateMap<VirtualRegistrationNewIPRS, VirtualRegistrationViewModel>();
      config.CreateMap<IMsaccoPlusNumberChecker, IMsaccoPlusNumberCheckerViewModel>();
      config.CreateMap<IMSACCO_TIMSI_RESET_DB_LOG, IMSACCO_TIMSI_RESET_ViewModel>();
      config.CreateMap<IBankTransfer, IBankTransferViewModel>();
      config.CreateMap<IMobileWithdrawals_DarajaDB, IMobileWithdrawals_SACCODB>(MemberList.Source)
          .ForMember(m => m.Exported_To_Saf, opt => opt.Ignore())
          .ForMember(m => m.Verified, opt => opt.Ignore());
    });
  }
}
