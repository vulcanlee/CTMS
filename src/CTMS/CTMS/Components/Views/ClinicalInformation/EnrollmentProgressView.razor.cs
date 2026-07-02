using CTMS.Business.Services;
using Microsoft.AspNetCore.Components;

namespace CTMS.Components.Views.ClinicalInformation;

public partial class EnrollmentProgressView
{
    [Inject]
    private EnrollmentProgressService EnrollmentProgressService { get; set; } = default!;

    private bool isLoaded = false;

    /// <summary>
    /// 總計收案數的統計基準日（民國年，例：115.07.02）
    /// </summary>
    private string AsOfDate => $"{DateTime.Today.Year - 1911}.{DateTime.Today:MM.dd}";

    protected override async Task OnInitializedAsync()
    {
        EnrollmentProgressService.Build();
        await EnrollmentProgressService.RefreshAsync();
        isLoaded = true;
    }
}
