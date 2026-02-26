using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace CTMS.Components.Views.ClinicalInformation;

public partial class DashboardView
{
    [Inject]
    private IJSRuntime JS { get; set; } = default!;
    [Inject]
    private NavigationManager NavigationManager { get; set; } = default!;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await JS.InvokeVoidAsync("initDashboardCharts");
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
