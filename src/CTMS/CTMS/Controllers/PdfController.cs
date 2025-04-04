using CTMS.Business.Services;
using CTMS.Share.Helpers;
using Microsoft.AspNetCore.Mvc;
using Syncfusion.HtmlConverter;
using Syncfusion.Pdf;

namespace CTMS.Controllers;

[ApiController]
[Route("[controller]")]
public class PdfController : ControllerBase
{
    private readonly ILogger<PdfController> _logger;
    private readonly GeneratePdfService generatePdfService;

    public PdfController(ILogger<PdfController> logger,
        GeneratePdfService generatePdfService)
    {
        _logger = logger;
        this.generatePdfService = generatePdfService;
    }

    [HttpGet]
    public async void Get(string code, string FullUrl, string page)
    {
        await generatePdfService.UrlToPdfAsync(UpdateMessage,"", code, FullUrl, page);

    }

    private void UpdateMessage(string message)
    {
        //_logger.LogInformation(message);
    }
}
