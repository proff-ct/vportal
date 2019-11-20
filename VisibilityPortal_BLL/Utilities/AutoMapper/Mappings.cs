using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.Configuration;
using VisibilityPortal_BLL.Models;
using VisibilityPortal_BLL.Models.ViewModels;
using VisibilityPortal_DAL;

namespace VisibilityPortal_BLL.Utilities.AutoMapper
{
  public static class Mappings
  {
    public static void RegisterMappings()
    {
      Mapper.Initialize(
        config => {
          config.CreateMap<Sacco, CoreTecClient>().ReverseMap();
          config.CreateMap<CoreTecClient, CoretecClientWithModule>().ReverseMap();
          config.CreateMap<PortalModuleForClient, CoretecClientModuleViewModel>().ReverseMap();
          config.CreateMap<PortalModule, PortalModuleViewModel>().ReverseMap();
        });
    }
    public static MapperConfiguration GetMapConfig()
    {
      return new MapperConfiguration(config => {
        config.CreateMap<Sacco, CoreTecClient>().ReverseMap();
        config.CreateMap<CoreTecClient, CoretecClientWithModule>().ReverseMap();
        config.CreateMap<PortalModuleForClient, CoretecClientModuleViewModel>().ReverseMap();
        config.CreateMap<PortalModule, PortalModuleViewModel>().ReverseMap();
      });
    }
    public static Action<IMapperConfigurationExpression> MapperConfigExpr = new Action<IMapperConfigurationExpression>(
      config =>
      {
        config.CreateMap<Sacco, CoreTecClient>().ReverseMap();
        config.CreateMap<CoreTecClient, CoretecClientWithModule>().ReverseMap();
        config.CreateMap<PortalModuleForClient, CoretecClientModuleViewModel>().ReverseMap();
        config.CreateMap<PortalModule, PortalModuleViewModel>().ReverseMap();
      });
    }
  }
