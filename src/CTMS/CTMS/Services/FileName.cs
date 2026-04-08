using LisServiceReference;
using PipServiceReference;

namespace CTMS.Services
{
    public class FileName
    {
        public async Task Get()
        {
            BusinessLogicClient businessLogicClient = new BusinessLogicClient();
            businessLogicClient.SYSPOWERGetLabdataByChartNoAsync("123456", "2024-01-01", "2024-12-31");

            PipServiceClient pipServiceClient = new PipServiceClient();
            GetPatientMedicationsDT dataTable = new();
            //dataTable.
            //pipServiceClient.GetPatientMedicationsAsync()
        }
    }
}
