using CTMS.Business.Events;
using CTMS.DataModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTMS.Business.Helpers;

public class TranscationResultHelper
{
    public TranscationResultHelper(IEventAggregator eventAggregator)
    {
        EventAggregator = eventAggregator;
    }

    public IEventAggregator EventAggregator { get; }
    public MessageBoxModel MessageBox { get; set; }
    /// <summary>
    /// 訊息對話窗設定
    /// </summary>
    public async Task CheckDatabaseResult(MessageBoxModel messageBox,
        VerifyRecordResult verifyRecordResult)
    {
        MessageBox = messageBox;
        if (verifyRecordResult.Success == false)
        {
            string message;
            if (verifyRecordResult.Exception == null)
            {
                message = $"{verifyRecordResult.Message}";
            }
            else
            {
                message = $"{verifyRecordResult.Message}，例外異常:{verifyRecordResult.Exception.Message}";
                if(verifyRecordResult.Exception.InnerException != null)
                {
                    message += $"，內部例外異常:{verifyRecordResult.Exception.InnerException.Message}";
                }
            }
            MessageBox.Show("400px", "200px", "發生例外異常", message, MessageBox.HiddenAsync);
        }
        await Task.Yield();
    }
}
