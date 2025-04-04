using Azure;
using CTMS.DataModel.Models;
using CTMS.EntityModel;
using CTMS.EntityModel.Models;
using CTMS.ExcelUtility.Services;
using CTMS.Share.Helpers;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SyncExcel.Services;
using Syncfusion.HtmlConverter;
using Syncfusion.Pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CTMS.Business.Services
{
    public class GeneratePdfService
    {
        private readonly BackendDBContext backendDBContext;
        private readonly ExcleService excleService;
        private readonly UploadFileService uploadFileService;
        NextGenerationSportsCTMSModel CTMSModel = new();

        public GeneratePdfService(BackendDBContext backendDBContext,
            ExcleService excleService, UploadFileService uploadFileService)
        {
            this.backendDBContext = backendDBContext;
            this.excleService = excleService;
            this.uploadFileService = uploadFileService;
        }

        public async Task<string> Make(Action<string> UpdateMessage, string code, string FullUrl)
        {
            string pageAll = $"All";
            //string page2 = "Genomics";
            //string page3 = "Blood";
            //string page4 = "pdfSummaryPage";
            //string page5 = "pdfSummaryPage";
            var allPages = new List<string> { "pdfSummaryPage", "Motion1", "Motion2",
                "Cardio", "Mental1", "Mental2", "Body1", "Body2", "Genomics1", "Genomics2",
                "Metabolite1", "Metabolite2", "Metabolite3", "Metabolite4",
           "Blood1", "Blood2", "Blood3","Recomm" };

            UpdateMessage($"讀取資料中...");
            var Athlete = await uploadFileService.GetAsync(code);
            CTMSModel = JsonConvert
            .DeserializeObject<NextGenerationSportsCTMSModel>(Athlete.ExcelData);

            UpdateMessage($"刪除舊資料...");
            string path = MagicObjectHelper.UploadTempPath;
            foreach (var item in allPages)
            {
                File.Delete(Path.Combine(path, GetTempPath(code, item)));
            }
            int total = allPages.Count;
            int count = 0;
            foreach (var item in allPages)
            {
                var msg = $"產生頁面 PDF {++count} / {total}";
                UpdateMessage(msg);

                await Task.Delay(100);
                await UrlToPdfAsync(UpdateMessage, msg, code, FullUrl, item);
                //await GetPdfFromApiAsync(code, FullUrl, item);
            }

            UpdateMessage($"合併 PDF...");
            var pdffilename = await Merge(code, allPages);
            return pdffilename;
        }
        async Task<string> Merge(string code, List<string> pages)
        {
            string path = MagicObjectHelper.UploadTempPath;
            string pathPdf = MagicObjectHelper.PdfFilesPath;
            string fileName1 = Path.Combine(path, GetTempPath(code, pages[0]));
            string fileNameAll = Path.Combine(path, GetTempPath(code, "All"));
            string fileNameCombin = Path.Combine(path, GetTempPath(code, "Combin"));
            string fileNamePdf = Path.Combine(pathPdf, GetTempPath(code, "Combin"));
            File.Copy(fileName1, fileNameAll, true);

            for (int i = 1; i < pages.Count; i++)
            {
                string fileName = Path.Combine(path, GetTempPath(code, pages[i]));
                //Due to platform limitations, the PDF file cannot be loaded from disk. However, you can merge multiple documents from stream using the following code snippet.
                //Creates a PDF document.
                using (PdfDocument finalDoc = new PdfDocument())
                {
                    using (FileStream stream1 = new FileStream(fileNameAll, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        using (FileStream stream2 = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                        {
                            //Creates a PDF stream for merging.
                            Stream[] streams = { stream1, stream2 };
                            //Merges PDFDocument.
                            PdfDocumentBase.Merge(finalDoc, streams);

                            //Save the document into stream.
                            using (MemoryStream stream = new MemoryStream())
                            {
                                finalDoc.Save(stream);
                                //Close the document.
                                finalDoc.Close(true);
                                //Disposes the streams.
                                stream2.Dispose();

                                // Write MemoryStream to fileNameCombin
                                if (File.Exists(fileNameCombin))
                                {
                                    File.Delete(fileNameCombin);
                                }
                                using (FileStream fileStream = new FileStream(fileNameCombin, FileMode.Create, FileAccess.Write))
                                {
                                    stream.WriteTo(fileStream);
                                }
                            }
                        }
                        stream1.Dispose();
                    }

                    File.Copy(fileNameCombin, fileNameAll, true);
                }
            }

            File.Copy(fileNameAll, fileNamePdf, true);

            return fileNamePdf;
        }

        async Task GetPdfFromApiAsync(string code, string FullUrl, string page)
        {
            string url = $"{FullUrl}/pdf?code={code}&FullUrl={FullUrl}&page={page}";
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                //var stream = await response.Content.ReadAsStreamAsync();
                //var path = MagicObjectHelper.UploadTempPath;
                //var fileName = Path.Combine(path, GetTempPath(code, page));
                //using (var fileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.Write))
                //{
                //    stream.CopyTo(fileStream);
                //}
            }
        }
        string GetTempPath(string code, string page)
        {
            return $"{code}_{page}.pdf"; ;
        }
        public async Task UrlToPdfAsync(Action<string> UpdateMessage, string msg, string code, string FullUrl, string page)
        {
            string path = MagicObjectHelper.UploadTempPath;
            string fileName = Path.Combine(path, GetTempPath(code, page));

            #region Blink
            //Initialize HTML to PDF converter.
            HtmlToPdfConverter htmlConverter = new HtmlToPdfConverter();
            BlinkConverterSettings settings = new BlinkConverterSettings();
            //Enable JavaScript 
            //settings.EnableJavaScript = true;
            //settings.MediaType = MediaType.Print;

            //Set PDF page margin 
            settings.Margin = new Syncfusion.Pdf.Graphics.PdfMargins { Top = 10, Left = 10, Right = 10, Bottom = 10 };
            settings.Orientation= PdfPageOrientation.Portrait;
            // Set blink view port size
            //settings.ViewPortSize = new Syncfusion.Drawing.Size(1280, 0);

            settings.AdditionalDelay = 2000;

            //Assign Blink converter settings to HTML converter.
            htmlConverter.ConverterSettings = settings;
            #endregion

            #region Cef
            //Initialize HTML to PDF converter.
            //HtmlToPdfConverter htmlConverter = new HtmlToPdfConverter(HtmlRenderingEngine.Cef);
            //CefConverterSettings cefConverterSettings = new CefConverterSettings();
            ////Set Blink viewport size.
            //cefConverterSettings.ViewPortSize = new Syncfusion.Drawing.Size(1920, 0);
            ////Assign Blink converter settings to HTML converter.
            //htmlConverter.ConverterSettings = cefConverterSettings;
            #endregion


            #region QtWebKit
            //HtmlToPdfConverter htmlConverter = new HtmlToPdfConverter(HtmlRenderingEngine.WebKit);
            #endregion

            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }
            //Convert URL to PDF document.
            var url = $"{FullUrl}/{page}/{code}";
            //url= "https://www.google.com";
            //UpdateMessage($"{msg} >>> 讀取網頁 {url}");
            UpdateMessage($"{msg} >>> 讀取網頁內容中...");
            using (PdfDocument document = htmlConverter.Convert(url))
            {
                //Create a filestream.
                using (FileStream fileStream = new FileStream(fileName, FileMode.CreateNew, FileAccess.Write, FileShare.Write))
                {
                    //UpdateMessage($"{msg} >>> 網頁 {url} 儲存為 PDF");
                    UpdateMessage($"{msg} >>> 網頁儲存為 PDF");
                    await Task.Delay(100);
                    //Save and close the PDF document.
                    document.Save(fileStream);
                    document.Close(true);
                }
            }
        }
    }
}
