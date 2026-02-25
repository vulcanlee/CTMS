using CTMS.AdapterModels;
using CTMS.DataModel.Models.ClinicalInformation;
using CTMS.Services;
using Microsoft.AspNetCore.Components;

namespace CTMS.Components.Pages.AdminTools;

public partial class ExportPage
{
    [Inject]
    public PatientService PatientService { get; set; }
    PatientAdapterModel patientAdapterModel = new();
    public PatientData patientData { get; set; }= new();
    public async Task ExportData()
    {

        patientAdapterModel = await PatientService.GetAsync("ddabf6f3-08a4-4ed4-8031-9f05fdffc3c3");
        patientData.FromJson(patientAdapterModel.JsonData);
    }
}
