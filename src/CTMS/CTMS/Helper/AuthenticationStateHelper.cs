using AutoMapper;
using CTMS.AdapterModels;
using CTMS.Business.Services;
using CTMS.DataModel.Models;
using CTMS.ExcelUtility.Extensions;
using CTMS.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CTMS.Business.Helpers
{
    public class AuthenticationStateHelper
    {
        private readonly ILogger<AuthenticationStateHelper> logger;
        private readonly IMapper mapper;
        private readonly MyUserService myUserService;
        private readonly CurrentUserService currentUserService;
        private readonly RolePermissionService rolePermissionService;

        public AuthenticationStateHelper(ILogger<AuthenticationStateHelper> logger,
            IMapper mapper, MyUserService myUserService,
            CurrentUserService currentUserService,
            RolePermissionService rolePermissionService)
        {
            this.logger = logger;
            this.mapper = mapper;
            this.myUserService = myUserService;
            this.currentUserService = currentUserService;
            this.rolePermissionService = rolePermissionService;
        }
        public async Task<bool> Check(AuthenticationStateProvider authStateProvider,
            NavigationManager NavigationManager)
        {
            var authState = await authStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            if (user.Identity is not null && user.Identity.IsAuthenticated)
            {
                var id = user.Claims
                    .FirstOrDefault(c => c.Type == ClaimTypes.Sid)?.Value.ToInt();
                if (id is not null)
                {
                    var myuser = await myUserService.GetAsync(id.Value);
                    CurrentUser currentUser = mapper.Map<CurrentUser>(myuser);

                    RolePermission rolePermission = rolePermissionService.InitializePermissionSetting();
                    if(myuser.RoleView == null)
                    {
                        NavigationManager.NavigateTo("/Auths/Logout", true, true);
                        return false;
                    }
                    List<string> permissions = JsonSerializer
                        .Deserialize<List<string>>(myuser.RoleView.TabViewJson);
                    rolePermissionService
                        .SetPermissionInput(rolePermission, permissions);
                    currentUser.RoleJson = myuser.RoleView.TabViewJson;


                    currentUserService.CurrentUser.CopyFrom(currentUser);
                    currentUser.IsAuthenticated = true;
                    return true;
                }
                else
                {
                    NavigationManager.NavigateTo("/Auths/Logout", true, true);
                    return false;
                }
            }
            else
            {
                NavigationManager.NavigateTo("/Auths/Logout", true, true);
                return false;
            }
        }

        public async Task<List<ProjectAdapterModel>> GetProjectListAsync(AuthenticationStateProvider authStateProvider)
        {
            List<ProjectAdapterModel> result = new();
            var authState = await authStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            if (user.Identity is not null && user.Identity.IsAuthenticated)
            {
                var id = user.Claims
                    .FirstOrDefault(c => c.Type == ClaimTypes.Sid)?.Value.ToInt();
                if (id is not null)
                {
                    result = await myUserService.GetProjectsAsync(id.Value);
                }
            }
            return result;
        }

        public bool CheckIsAdmin()
        {
            return currentUserService.CurrentUser.IsAdmin;
        }

        public bool CheckAccessPage(string name)
        {
            var result = currentUserService.CurrentUser.RoleList.Contains(name);
            return result;
        }
    }
}
