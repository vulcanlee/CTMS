using CTMS.Components;
using CTMS.DataModel.Models;
using CTMS.EntityModel;
using CTMS.Share.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MudBlazor.Services;
using SyncExcel.Services;
using Syncfusion.Blazor;
using static System.Formats.Asn1.AsnWriter;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Http.Features;
using NLog.Web;
using NLog;
using Prism.Events;
using CTMS.Services;
using CTMS.Middlewares;
using CTMS.Business.Events;
using CTMS.Business.Services;
using CTMS.Helper;
using CTMS.ViewModels;
using CTMS.AdapterModels;
using CTMS.DataModel.Dtos;
using CTMS.EntityModel.Models;
using CTMS.Business.Helpers;

namespace CTMS
{
    public class Program
    {

        public static void Main(string[] args)
        {
            var logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
            try
            {
                var builder = WebApplication.CreateBuilder(args);

                builder.Logging.ClearProviders();
                builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                builder.Host.UseNLog();

                Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MzY3OTY0NkAzMjM4MmUzMDJlMzBNOEJGNGtlYWtnWFdNanVRTXpTa3JEVWJGVTBlVmZYdW5QbGNSQ21FbzZVPQ==");
                builder.WebHost.ConfigureKestrel(options =>
                {
                    // 這邊設定單次請求可允許的最長大小（下例為 200 MB）
                    options.Limits.MaxRequestBodySize = 200_000_000;
                });

                builder.Services.AddControllers();
                builder.Services.AddCascadingAuthenticationState();

                // FormOptions 最大多組件上傳大小設定
                builder.Services.Configure<FormOptions>(options =>
                {
                    // 這邊設定整個 multipart/form-data 的上限（下例為 200 MB）
                    options.MultipartBodyLengthLimit = 200_000_000;
                });
                // Add services to the container.
                builder.Services.AddRazorComponents()
                    .AddInteractiveServerComponents();
                builder.Services.AddMudServices();
                builder.Services.AddSyncfusionBlazor();

                #region EF Core 宣告
                var ctmsSettings = builder.Configuration
                    .GetSection(nameof(CTMSSettings))
                    .Get<CTMSSettings>();
                var SQLiteDefaultConnection = ctmsSettings.ConnectionStrings.SQLiteDefaultConnection;

                builder.Services.AddDbContext<BackendDBContext>(options =>
                    options.UseSqlite(SQLiteDefaultConnection),
                    ServiceLifetime.Transient);
                #endregion

                #region 加入使用 Cookie & JWT 認證需要的宣告
                builder.Services.Configure<CookiePolicyOptions>(options =>
                {
                    options.CheckConsentNeeded = context => true;
                    options.MinimumSameSitePolicy = Microsoft.AspNetCore.Http.SameSiteMode.None;
                });
                builder.Services.AddAuthentication(MagicObjectHelper.CookieScheme)
                    .AddCookie(MagicObjectHelper.CookieScheme, options =>
                    {
                        options.ExpireTimeSpan = TimeSpan.FromDays(2);
                        options.SlidingExpiration = true;  // 若使用者持續活動，會自動延長過期時間
                    });
                #endregion

                #region 註冊專案客制用的服務
                builder.Services.AddTransient<BrowseAthleteService>();
                builder.Services.AddTransient<ExcleService>();
                builder.Services.AddTransient<UploadFileService>();
                builder.Services.AddTransient<DownloadFileService>();
                builder.Services.AddTransient<RestoreFileService>();
                builder.Services.AddTransient<DownloadFileServiceV1>();
                builder.Services.AddTransient<RestoreFileServiceV1>();
                builder.Services.AddTransient<DeleteFileService>();
                builder.Services.AddTransient<GptService>();
                builder.Services.AddTransient<GeneratePdfService>();
                builder.Services.AddTransient<TranscationResultHelper>();
                builder.Services.AddTransient<AuthenticationStateHelper>();
                builder.Services.AddTransient<MigrationExamineService>();
                builder.Services.AddScoped<RequestInfoService>();
                builder.Services.AddScoped<CurrentUserService>();
                builder.Services.AddScoped<CurrentProject>();
                builder.Services.AddScoped<CurrentAthleteExamine>();
                builder.Services.AddTransient<RolePermissionService>();
                builder.Services.AddHttpContextAccessor();
                builder.Services.AddSingleton<RequestInformation>();
                builder.Services.AddScoped<IEventAggregator, EventAggregator>();

                builder.Services.AddTransient<MyUserServiceLogin>();

                #region MyUser
                builder.Services.AddTransient<MyUserService>();
                builder.Services.AddTransient<MyUserViewModel>();
                builder.Services.AddTransient<MyUserAdapterModel>();
                builder.Services.AddTransient<MyUser>();
                builder.Services.AddTransient<MyUserDto>();
                #endregion

                #region Project
                builder.Services.AddTransient<ProjectService>();
                builder.Services.AddTransient<ProjectViewModel>();
                builder.Services.AddTransient<ProjectAdapterModel>();
                builder.Services.AddTransient<Project>();
                builder.Services.AddTransient<ProjectDto>();
                #endregion

                #region RoleView
                builder.Services.AddTransient<RoleViewService>();
                builder.Services.AddTransient<RoleViewViewModel>();
                builder.Services.AddTransient<RoleViewAdapterModel>();
                builder.Services.AddTransient<RoleView>();
                builder.Services.AddTransient<RoleViewDto>();
                #endregion

                #region RoleViewDetail
                builder.Services.AddTransient<RoleViewDetailService>();
                builder.Services.AddTransient<RoleViewDetailViewModel>();
                builder.Services.AddTransient<RoleViewDetailAdapterModel>();
                builder.Services.AddTransient<RoleViewDetail>();
                builder.Services.AddTransient<RoleViewDetailDto>();
                #endregion

                #endregion

                #region 加入設定強型別注入宣告
                builder.Services.Configure<CTMSSettings>(builder.Configuration
                    .GetSection(MagicObjectHelper.CTMSSettings));
                #endregion

                #region AutoMapper 使用的宣告
                builder.Services.AddAutoMapper(c => c.AddProfile<AutoMapping>());
                #endregion

                var app = builder.Build();

                #region 資料庫的 Migration
                //if (!app.Environment.IsDevelopment())
                {
                    using var scope = app.Services.CreateScope();
                    using var dbContext = scope.ServiceProvider.GetRequiredService<BackendDBContext>();
                    var migrationExamineService = scope.ServiceProvider.GetRequiredService<MigrationExamineService>();
                    dbContext.Database.Migrate();

                    Project projectItemNew = null;
                    RoleView roleViewItemNew = null;

                    #region 是否有存在的專案定義
                    var projectItem = dbContext.Project
                        .FirstOrDefault(x => x.Name == MagicObjectHelper.預設專案);
                    if (projectItem == null)
                    {
                        projectItemNew = new Project()
                        {
                            Name = MagicObjectHelper.預設專案,
                        };
                        dbContext.Project.Add(projectItemNew);
                        dbContext.SaveChanges();
                    }

                    #endregion

                    #region 是否有存在的角色檢視定義
                    var roleViewItem = dbContext.RoleView
                        .FirstOrDefault(x => x.Name == MagicObjectHelper.預設角色);
                    RolePermissionService RolePermissionService = scope
                        .ServiceProvider
                        .GetRequiredService<RolePermissionService>();
                    var allPermissionJson = RolePermissionService
                        .GetRolePermissionAllNameToJson();
                    if (roleViewItem == null)
                    {
                        roleViewItemNew = new RoleView()
                        {
                            Name = MagicObjectHelper.預設角色,
                            TabViewJson = allPermissionJson
                        };
                        dbContext.RoleView.Add(roleViewItemNew);
                        dbContext.SaveChanges();
                    }
                    else
                    {
                        roleViewItem.TabViewJson = allPermissionJson;
                        dbContext.SaveChanges();
                    }
                    #endregion

                    #region 建立預設的 RoleViewProject
                    if (projectItemNew != null && roleViewItemNew != null)
                    {
                        var roleViewProjectItem = dbContext.RoleViewProject
                            .FirstOrDefault(x => x.ProjectId == projectItemNew.Id &&
                            x.RoleViewId == roleViewItemNew.Id);
                        if (roleViewProjectItem == null)
                        {
                            var roleViewProjectItemNew = new RoleViewProject()
                            {
                                ProjectId = projectItemNew.Id,
                                RoleViewId = roleViewItemNew.Id,
                            };
                            dbContext.RoleViewProject.Add(roleViewProjectItemNew);
                            dbContext.SaveChanges();
                        }
                    }
                    #endregion

                    #region 產生預設帳號
                    var support = dbContext.MyUser
                        .FirstOrDefault(x => x.Account == MagicObjectHelper.開發者帳號);

                    if (support == null)
                    {
                        support = new MyUser()
                        {
                            Account = MagicObjectHelper.開發者帳號,
                            //Password = MagicObjectHelper.開發者帳號,
                            Name = MagicObjectHelper.開發者帳號,
                            Email = MagicObjectHelper.開發者帳號,
                            IsAdmin = true,
                            Salt = Guid.NewGuid().ToString(),
                            Status = true,
                            RoleViewId = roleViewItemNew.Id,
                            RoleJson = "[]",
                        };
                        support.Password =
                            PasswordHelper.GetPasswordSHA(support.Salt, MagicObjectHelper.開發者帳號);

                        dbContext.MyUser.Add(support);
                        dbContext.SaveChanges();
                    }
                    else
                    {
                        support.Password =
                            PasswordHelper.GetPasswordSHA(support.Salt, MagicObjectHelper.開發者帳號);
                        support.IsAdmin = true;
                        if(roleViewItemNew!= null)
                            support.RoleViewId = roleViewItemNew.Id;
                        else
                            support.RoleViewId = roleViewItem.Id;
                        dbContext.SaveChanges();
                    }
                    #endregion

                    #region 更新沒有 RowView 的紀錄
                    //var Athletes = dbContext.Athlete
                    //    .AsNoTracking()
                    //    .Where(x => x.ProjectId == null)
                    //    .ToList();
                    //foreach (var item in Athletes)
                    //{
                    //    if(projectItemNew != null)
                    //        item.ProjectId = projectItemNew.Id;
                    //    else
                    //        item.ProjectId = projectItem.Id;
                    //    dbContext.Athlete.Update(item);
                    //}
                    //dbContext.SaveChanges();
                    #endregion

                    //migrationExamineService.MigrateExamineAsync().Wait();
                }
                #endregion

                app.UseMiddleware<RequestLoggingMiddleware>();

                // Configure the HTTP request pipeline.
                if (!app.Environment.IsDevelopment())
                {
                    app.UseExceptionHandler("/Error");
                    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                    app.UseHsts();
                }

                //app.UseHttpsRedirection();

                app.UseStaticFiles();
                app.UseStaticFiles(new StaticFileOptions
                {
                    FileProvider = new PhysicalFileProvider(
               Path.Combine(builder.Environment.ContentRootPath, "UploadFiles")),
                    RequestPath = "/UploadFiles"
                });
                app.UseStaticFiles(new StaticFileOptions
                {
                    FileProvider = new PhysicalFileProvider(
               Path.Combine(builder.Environment.ContentRootPath, "DownloadFiles")),
                    RequestPath = "/DownloadFiles"
                });
                app.UseStaticFiles(new StaticFileOptions
                {
                    FileProvider = new PhysicalFileProvider(
               Path.Combine(builder.Environment.ContentRootPath, "PdfFiles")),
                    RequestPath = "/PdfFiles"
                });
                app.UseAntiforgery();

                //app.UseAuthentication();
                //app.UseAuthorization();

                app.MapControllers();
                app.MapRazorComponents<App>()
                    .AddInteractiveServerRenderMode();

                app.Run();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Stopped program because of an exception");
                throw;
            }
            finally
            {
                LogManager.Shutdown();
            }
        }
    }
}
