using CTMS.DataModel.Models;
using CTMS.EntityModel;
using CTMS.EntityModel.Models;
using CTMS.Share.Helpers;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SyncExcel.Services;

namespace CTMS.Business.Services;

public class BrowsePatientService
{
    private readonly BackendDBContext backendDBContext;

    public BrowsePatientService(BackendDBContext backendDBContext)
    {
        this.backendDBContext = backendDBContext;
    }

    public async Task<List<Patient>> GetAllAsync()
    {
        return await backendDBContext.Patient
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Patient> GetByCodeAsync(string code)
    {
        var patient = await backendDBContext.Patient
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Code == code);
        return patient;
    }

}
