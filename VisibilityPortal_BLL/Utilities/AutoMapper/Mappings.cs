using AutoMapper;
using System;
using Utilities.PortalApplicationParams;
using VisibilityPortal_BLL.Models;
using VisibilityPortal_BLL.Models.ASP_Identity;
using VisibilityPortal_BLL.Models.ASP_Identity.IdentityModels;
using VisibilityPortal_BLL.Models.ViewModels;
using VisibilityPortal_DAL;
using VisibilityPortal_Dataspecs.OTPAuth.Models;

namespace VisibilityPortal_BLL.Utilities.AutoMapper
{
    public static class Mappings
    {
        public static void RegisterMappings()
        {
            Mapper.Initialize(
              config =>
              {
                  config.CreateMap<Sacco, CoreTecClient>().ReverseMap();
                  config.CreateMap<CoreTecClient, CoretecClientWithModule>().ReverseMap();
                  config.CreateMap<CoreTecClient, CoretecClientWithUsers>().ReverseMap();
                  config.CreateMap<PortalModuleForClient, CoretecClientModuleViewModel>().ReverseMap();
                  config.CreateMap<PortalModule, PortalModuleViewModel>().ReverseMap();
                  config.CreateMap<PortalModuleForClient, PortalModuleForClientViewModel>();
                  config.CreateMap<PortalUserRole, PortalRole>().ReverseMap();
                  config.CreateMap<PortalUserRole, ActiveUserParams.UserRoles>();
                  config.CreateMap<PortalUserRole, PortalUserRoleViewModel>().ReverseMap();
                  config.CreateMap<PortalRole, ActiveUserParams.UserRoles>().ReverseMap();
                  config.CreateMap<ApplicationUser, ApplicationUserViewModel>();
                  config.CreateMap<EditPortalUserViewModel, AddUserRoleViewModel>();
                  config.CreateMap<IDB_OTP, IOTP>()
              .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (OTP_STATUS)Enum.Parse(typeof(OTP_STATUS), src.Status)));
              });
        }
        public static MapperConfiguration GetMapConfig()
        {
            return new MapperConfiguration(config =>
            {
                config.CreateMap<Sacco, CoreTecClient>().ReverseMap();
                config.CreateMap<CoreTecClient, CoretecClientWithModule>().ReverseMap();
                config.CreateMap<CoreTecClient, CoretecClientWithUsers>().ReverseMap();
                config.CreateMap<PortalModuleForClient, CoretecClientModuleViewModel>().ReverseMap();
                config.CreateMap<PortalModule, PortalModuleViewModel>().ReverseMap();
                config.CreateMap<PortalModuleForClient, PortalModuleForClientViewModel>();
                config.CreateMap<PortalUserRole, PortalRole>().ReverseMap();
                config.CreateMap<PortalUserRole, ActiveUserParams.UserRoles>();
                config.CreateMap<PortalUserRole, PortalUserRoleViewModel>().ReverseMap();
                config.CreateMap<PortalRole, ActiveUserParams.UserRoles>().ReverseMap();
                config.CreateMap<ApplicationUser, ApplicationUserViewModel>();
                config.CreateMap<EditPortalUserViewModel, AddUserRoleViewModel>();
                config.CreateMap<IDB_OTP, IOTP>()
                  .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (OTP_STATUS)Enum.Parse(typeof(OTP_STATUS), src.Status)));
            });
        }
        public static Action<IMapperConfigurationExpression> MapperConfigExpr = new Action<IMapperConfigurationExpression>(
          config =>
          {
              config.CreateMap<Sacco, CoreTecClient>().ReverseMap();
              config.CreateMap<CoreTecClient, CoretecClientWithModule>().ReverseMap();
              config.CreateMap<CoreTecClient, CoretecClientWithUsers>().ReverseMap();
              config.CreateMap<PortalModuleForClient, CoretecClientModuleViewModel>().ReverseMap();
              config.CreateMap<PortalModule, PortalModuleViewModel>().ReverseMap();
              config.CreateMap<PortalModuleForClient, PortalModuleForClientViewModel>();
              config.CreateMap<PortalUserRole, PortalRole>().ReverseMap();
              config.CreateMap<PortalUserRole, ActiveUserParams.UserRoles>();
              config.CreateMap<PortalUserRole, PortalUserRoleViewModel>().ReverseMap();
              config.CreateMap<PortalRole, ActiveUserParams.UserRoles>().ReverseMap();
              config.CreateMap<ApplicationRole, PortalApplicationRoleViewModel>()
          .ForMember(dest => dest.RoleId, opt => opt.MapFrom(src => src.Id))
          .ReverseMap();
              config.CreateMap<ApplicationUser, ApplicationUserViewModel>();
              config.CreateMap<EditPortalUserViewModel, AddUserRoleViewModel>();
              config.CreateMap<IDB_OTP, IOTP>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (OTP_STATUS)Enum.Parse(typeof(OTP_STATUS), src.Status)));
          });
    }
}
