using AutoMapper;
using CTMS.AdapterModels;
using CTMS.DataModel.Dtos;
using CTMS.DataModel.Models;
using CTMS.EntityModel.Models;

namespace CTMS.Helper;

public class AutoMapping : Profile
{
    public AutoMapping()
    {
        #region Blazor AdapterModel
        CreateMap<MyUserAdapterModel, CurrentUser>();

        #region Project
        CreateMap<Project, ProjectAdapterModel>();
        CreateMap<ProjectAdapterModel, Project>();
        CreateMap<Project, ProjectDto>();
        CreateMap<ProjectDto, Project>();
        CreateMap<ProjectAdapterModel, ProjectDto>();
        CreateMap<ProjectDto, ProjectAdapterModel>();
        #endregion

        #region RoleView
        CreateMap<RoleView, RoleViewAdapterModel>();
        CreateMap<RoleViewAdapterModel, RoleView>();
        CreateMap<RoleView, RoleViewDto>();
        CreateMap<RoleViewDto, RoleView>();
        CreateMap<RoleViewAdapterModel, RoleViewDto>();
        CreateMap<RoleViewDto, RoleViewAdapterModel>();
        #endregion

        #region RoleViewDetail
        CreateMap<RoleViewDetail, RoleViewDetailAdapterModel>();
        CreateMap<RoleViewDetailAdapterModel, RoleViewDetail>();
        CreateMap<RoleViewDetail, RoleViewDetailDto>();
        CreateMap<RoleViewDetailDto, RoleViewDetail>();
        CreateMap<RoleViewDetailAdapterModel, RoleViewDetailDto>();
        CreateMap<RoleViewDetailDto, RoleViewDetailAdapterModel>();
        #endregion

        #region MyUser
        CreateMap<MyUser, MyUserAdapterModel>();
        CreateMap<MyUserAdapterModel, MyUser>();
        CreateMap<MyUser, MyUserDto>();
        CreateMap<MyUserDto, MyUser>();
        CreateMap<MyUserAdapterModel, MyUserDto>();
        CreateMap<MyUserDto, MyUserAdapterModel>();
        #endregion

        #region Patient
        CreateMap<Patient, PatientAdapterModel>();
        CreateMap<PatientAdapterModel, Patient>();
        CreateMap<Patient, PatientDto>();
        CreateMap<PatientDto, Patient>();
        CreateMap<PatientAdapterModel, PatientDto>();
        CreateMap<PatientDto, PatientAdapterModel>();
        #endregion
        #endregion
    }
}
