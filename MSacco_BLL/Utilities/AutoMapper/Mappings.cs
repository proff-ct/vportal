using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using MSacco_BLL.Models;
using MSacco_BLL.ViewModels;
using MSacco_DAL;
using MSacco_Dataspecs.Feature.MsaccoPlusNumberChecker.Models;

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
    });
  }
}
