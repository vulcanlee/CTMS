﻿using AutoMapper;
using CTMS.AdapterModels;
using CTMS.Business.Helpers;
using CTMS.Business.Services;
using CTMS.DataModel.Interfaces;
using CTMS.DataModel.Models;
using CTMS.EntityModel;
using CTMS.Services;
using CTMS.Share.Helpers;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc.Razor;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Maps;
using Syncfusion.Blazor.Navigations;

namespace CTMS.ViewModels;

public class MyUserViewModel
{
    #region Constructor
    public MyUserViewModel(MyUserService CurrentService,
       BackendDBContext context, IMapper Mapper,
       TranscationResultHelper transcationResultHelper,
       RolePermissionService rolePermissionService)
    {
        this.CurrentService = CurrentService;
        this.context = context;
        mapper = Mapper;
        TranscationResultHelper = transcationResultHelper;
        this.rolePermissionService = rolePermissionService;

        #region 工具列按鈕初始化
        Toolbaritems.Add(new ItemModel()
        {
            Id = ButtonIdHelper.ButtonIdAdd,
            Text = "新增",
            TooltipText = "新增",
            Type = ItemType.Button,
            PrefixIcon = "mdi mdi-plus-thick",
            Align = ItemAlign.Left,
        });
        Toolbaritems.Add(new ItemModel()
        {
            Id = ButtonIdHelper.ButtonIdRefresh,
            Text = "重新整理",
            TooltipText = "重新整理",
            PrefixIcon = "mdi mdi-refresh",
            Align = ItemAlign.Left,
        });
        Toolbaritems.Add("Search");
        #endregion
    }
    #endregion

    #region Property
    public bool IsShowEditRecord { get; set; } = false;
    public MyUserAdapterModel CurrentRecord { get; set; } = new MyUserAdapterModel();
    public MyUserAdapterModel CurrentNeedDeleteRecord { get; set; } = new MyUserAdapterModel();
    public EditContext LocalEditContext { get; set; }
    public string EditRecordDialogTitle { get; set; } = "";
    public bool ShowAontherRecordPicker { get; set; } = false;
    private bool isShowConfirm { get; set; } = false;

    #region 訊息說明之對話窗使用的變數
    public ConfirmBoxModel ConfirmMessageBox { get; set; } = new ConfirmBoxModel();
    public MessageBoxModel MessageBox { get; set; } = new MessageBoxModel();
    #endregion
    #endregion

    #region Field
    bool isNewRecordMode;
    private readonly MyUserService CurrentService;
    private readonly BackendDBContext context;
    private readonly IMapper mapper;
    private readonly RolePermissionService rolePermissionService;
    DataModel.Interfaces.IRazorPage thisView;
    IDataGrid dataGrid;
    public List<object> Toolbaritems = new List<object>();
    #endregion

    #region Method

    #region DataGrid 初始化
    public void Setup(DataModel.Interfaces.IRazorPage razorPage, IDataGrid dataGrid)
    {
        thisView = razorPage;
        this.dataGrid = dataGrid;
    }
    #endregion

    #region 工具列事件 (新增)
    public void ToolbarClickHandler(Syncfusion.Blazor.Navigations.ClickEventArgs args)
    {
        if (args.Item.Id == ButtonIdHelper.ButtonIdAdd)
        {
            CurrentRecord = new MyUserAdapterModel();
            #region 針對新增的紀錄所要做的初始值設定商業邏輯
            CurrentRecord.Status = true;
            CurrentRecord.IsAdmin = false;
            CurrentRecord.RolePermission = rolePermissionService.InitializePermissionSetting();
            #endregion
            EditRecordDialogTitle = "新增紀錄";
            isNewRecordMode = true;
            IsShowEditRecord = true;
        }
        else if (args.Item.Id == ButtonIdHelper.ButtonIdRefresh)
        {
            dataGrid.RefreshGrid();
        }
    }
    #endregion

    #region 記錄列的按鈕事件 (修改與刪除)
    public async Task OnCommandClicked(CommandClickEventArgs<MyUserAdapterModel> args)
    {
        MyUserAdapterModel item = args.RowData as MyUserAdapterModel;
        if (args.CommandColumn.ButtonOption.IconCss == ButtonIdHelper.ButtonIdEdit)
        {
            CurrentRecord = item.Clone();
            EditRecordDialogTitle = "修改紀錄";
            IsShowEditRecord = true;
            isNewRecordMode = false;

        }
        else if (args.CommandColumn.ButtonOption.IconCss == ButtonIdHelper.ButtonIdDelete)
        {
            CurrentNeedDeleteRecord = item;

            #region 檢查關聯資料是否存在
            var checkedResult = await CurrentService
                .BeforeDeleteCheckAsync(CurrentNeedDeleteRecord);
            await Task.Delay(100);
            if (checkedResult.Success == false)
            {
                MessageBox.Show("400px", "200px", "警告",
                    checkedResult.Message, MessageBox.HiddenAsync);
                await Task.Yield();
                await thisView.NeedRefreshAsync();
                return;
            }
            #endregion

            #region 刪除這筆紀錄
            await Task.Yield();
            var checkTask = ConfirmMessageBox.ShowAsync("400px", "200px", "警告",
                 "確認要刪除這筆紀錄嗎?", ConfirmMessageBox.HiddenAsync);
            await thisView.NeedRefreshAsync();
            var checkAgain = await checkTask;
            if (checkAgain == true)
            {
                var verifyRecordResult = await CurrentService.DeleteAsync(CurrentNeedDeleteRecord.Id);
                await TranscationResultHelper.CheckDatabaseResult(MessageBox, verifyRecordResult);
                dataGrid.RefreshGrid();
            }
            #endregion
        }
    }
    #endregion

    #region 修改紀錄對話窗的按鈕事件
    public void OnEditContestChanged(EditContext context)
    {
        LocalEditContext = context;
    }

    public void OnRecordEditCancel()
    {
        IsShowEditRecord = false;
    }

    public async Task OnRecordEditConfirm()
    {
        #region 進行 Form Validation 檢查驗證作業
        if (LocalEditContext.Validate() == false)
        {
            return;
        }
        #endregion

        #region 檢查資料完整性
        if (isNewRecordMode == true)
        {
            if (string.IsNullOrEmpty(CurrentRecord.PasswordPlaintext))
            {
                MessageBox.Show("400px", "200px", "警告",
                    "密碼不能為空白", MessageBox.HiddenAsync);
                await thisView.NeedRefreshAsync();
                return;
            }
            var checkedResult = await CurrentService
                .BeforeAddCheckAsync(CurrentRecord);
            if (checkedResult.Success == false)
            {
                MessageBox.Show("400px", "200px", "警告",
                    checkedResult.Message, MessageBox.HiddenAsync);
                await thisView.NeedRefreshAsync();
                return;
            }
            CurrentRecord.Salt = Guid.NewGuid().ToString();
            CurrentRecord.Password =
                PasswordHelper.GetPasswordSHA(CurrentRecord.Salt, CurrentRecord.PasswordPlaintext);
        }
        else
        {
            var checkedResult = await CurrentService
                .BeforeUpdateCheckAsync(CurrentRecord);
            if (checkedResult.Success == false)
            {
                MessageBox.Show("400px", "200px", "警告",
                    checkedResult.Message, MessageBox.HiddenAsync);
                await thisView.NeedRefreshAsync();
                return;
            }
            if (string.IsNullOrEmpty(CurrentRecord.PasswordPlaintext) == false)
            {
                CurrentRecord.Password =
                    PasswordHelper.GetPasswordSHA(CurrentRecord.Salt, CurrentRecord.PasswordPlaintext);
            }
        }
        #endregion

        if (IsShowEditRecord == true)
        {
            if (isNewRecordMode == true)
            {
                var verifyRecordResult = await CurrentService.AddAsync(CurrentRecord);
                await TranscationResultHelper.CheckDatabaseResult(MessageBox, verifyRecordResult);
                dataGrid.RefreshGrid();
            }
            else
            {
                var verifyRecordResult = await CurrentService.UpdateAsync(CurrentRecord);
                await TranscationResultHelper.CheckDatabaseResult(MessageBox, verifyRecordResult);
                dataGrid.RefreshGrid();
            }
            IsShowEditRecord = false;
        }
    }
    #endregion

    #region 開窗選取紀錄使用到的方法
    public void OnOpenPicker()
    {
        ShowAontherRecordPicker = true;
    }

    public void OnPickerCompletion(RoleViewAdapterModel e)
    {
        if (e != null)
        {
            CurrentRecord.RoleViewId = e.Id;
            CurrentRecord.RoleViewName = e.Name;
            CurrentRecord.RoleView = e;
        }
        ShowAontherRecordPicker = false;
    }
    #endregion

    #region 排序搜尋事件
    public int DefaultSorting { get; set; } = -1;
    public TranscationResultHelper TranscationResultHelper { get; }
    #endregion

    #region 啟用/停用
    public async Task DisableIt(MyUserAdapterModel item)
    {
        if (item.Account.ToLower() == MagicObjectHelper.開發者帳號)
        {
            MessageBox.Show("400px", "200px", "警告",
                "開發者帳號不可以被停用", MessageBox.HiddenAsync);
            return;
        }
        await CurrentService.DisableIt(item);
        dataGrid.RefreshGrid();
    }
    public async Task EnableIt(MyUserAdapterModel item)
    {
        await CurrentService.EnableIt(item);
        dataGrid.RefreshGrid();
    }
    #endregion

    #region 使用者的政策調整
    public void OnResetForceLogoutDatetime()
    {
    }
    #endregion
    #endregion
}
