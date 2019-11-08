﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using CallCenter_BLL.Models;
using CallCenter_BLL.ViewModels;
using CallCenter_DAL;

namespace CallCenter_BLL.Utilities.AutoMapper
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
        });
    }
  }
}
