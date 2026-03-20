using CTMS.DataModel.Models;
using CTMS.Share.Helpers;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Inputs;

namespace CTMS.Components.Views.Commons;

public partial class UploadManualAnnotationView
{
    [Parameter]
    public bool OpenPicker { get; set; } = false;
    [Parameter]
    public string SubjectNo { get; set; }
    [Parameter]
    public EventCallback<string> OnConfirmCallback { get; set; }

    ConfirmBoxModel ConfirmMessageBox { get; set; } = new ConfirmBoxModel();
    MessageBoxModel MessageBox { get; set; } = new MessageBoxModel();

    SfUploader upload1;

    string DialogTitle = "上傳 人工標註 檔案";
    public bool ShowMessageBox { get; set; } = false;
    public string MessageBoxBody { get; set; } = "";
    public string MessageBoxTitle { get; set; } = "";

    protected override async Task OnInitializedAsync()
    {
        await GetData();
    }

    async Task GetData()
    {
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
        }
    }

    async Task OnPickerCancel()
    {
        OpenPicker = false;
        await OnConfirmCallback.InvokeAsync(null);
    }

    public async Task UploaderChange(UploadChangeEventArgs args)
    {
        await Task.Delay(100);
        var sourceDicomFileName = "";
        var sourceDicomPath = "";
        var targetImageFileName = "";
        var targetImagePath = "";
        foreach (var uploadFile in args.Files)
        {
            var file = uploadFile.File;
            try
            {
            }
            catch (Exception ex)
            {
            }
            using (var inputStream = file.OpenReadStream(20 * 1024 * 1024))
            {
                sourceDicomFileName = $"{file.Name}";

                // 將 inputFileName 內容複製到指定 path 目錄下，檔案名稱要相同
                sourceDicomPath = Path.Combine(MagicObjectHelper.UploadTempPath, sourceDicomFileName);
                using (var fileStream = new FileStream(sourceDicomPath, FileMode.Create, FileAccess.Write))
                {
                    await inputStream.CopyToAsync(fileStream);
                }
            }

            OpenPicker = false;
            await OnConfirmCallback.InvokeAsync(sourceDicomPath);
            break;
        }
        await Task.Delay(100);
        StateHasChanged();
    }
}
