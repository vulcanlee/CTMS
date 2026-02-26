using CTMS.Business.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace CTMS.Components.Views.ClinicalInformation;

public partial class DashboardView
{
    [Inject]
    private IJSRuntime JS { get; set; } = default!;
    [Inject]
    private NavigationManager NavigationManager { get; set; } = default!;
    [Inject]
    private DashboardService DashboardService { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        DashboardService.Build();
        await DashboardService.RefreshAsync();

    }
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            // 將 DashboardViewModel 傳遞給 JavaScript
            await JS.InvokeVoidAsync("initDashboardCharts", DashboardService.Dashboard);
        }
    }

    async Task ToggleTheme()
    {
        await JS.InvokeVoidAsync("toggleTheme");
        //await Task.Delay(500); 
        NavigationManager.NavigateTo("/Dashboard", true);
    }

    async Task OnGotoBrowser()
    {
        NavigationManager.NavigateTo("/Browser", true);
    }
}
