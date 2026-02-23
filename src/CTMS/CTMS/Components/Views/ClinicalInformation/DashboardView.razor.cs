using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace CTMS.Components.Views.ClinicalInformation;

public partial class DashboardView
{
    [Inject]
    private IJSRuntime JS { get; set; } = default!;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await JS.InvokeVoidAsync("initDashboardCharts");
        }
    }
}
