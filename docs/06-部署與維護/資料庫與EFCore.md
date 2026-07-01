# 資料庫與 EF Core 維護指南

本文件整併自原 `docs/系統維護用文件/` 下的 `EFCore.md`、`Migration History.md`、`SQLite.md`，說明 CTMS 的資料庫工具、Migration 操作與歷史紀錄。

## 1. 資料庫概觀

- ORM：Entity Framework Core 9.0.3。
- 資料庫：SQLite（`BackendDB.db`）為主，另支援 SQL Server LocalDB（`MSSQLLocalDB`）作為替代連線。
- DbContext：`BackendDBContext`（位於 `CTMS.EntityModel` 專案）。
- Entity 與 Migration 皆放在 `CTMS.EntityModel` 專案，啟動專案為 `CTMS`。

## 2. SQLite 工具

檢視／編輯 `BackendDB.db` 可使用 DB Browser for SQLite：

- 專案首頁：<https://github.com/sqlitebrowser/sqlitebrowser>
- Windows 下載：<https://sqlitebrowser.org/dl/#windows>

## 3. Migration 常用指令

> 以下指令在 Visual Studio 的 Package Manager Console 執行；對應的 `dotnet ef` CLI 版本附在後面。

### 新增 Migration

```shell
Add-Migration <名稱> -Context BackendDBContext -Project CTMS.EntityModel -StartupProject CTMS
```

```shell
dotnet ef migrations add <名稱> --project CTMS.EntityModel --startup-project CTMS
```

### 更新資料庫

```shell
Update-Database -Context BackendDBContext -StartupProject CTMS -Project CTMS.EntityModel
```

### 移除最後一個 Migration

```shell
Remove-Migration -Context BackendDBContext -Project CTMS.EntityModel -StartupProject CTMS
```

```shell
dotnet ef migrations remove --project CTMS.EntityModel --startup-project CTMS
```

### 產生移轉 SQL 指令稿

```shell
Script-Migration -Project CTMS.EntityModel -StartupProject CTMS
```

### 參數說明

- `-Context <String>`：指定要使用的 DbContext 類別（類別名稱或含命名空間的完整名稱）。若省略且有多個 Context 類別，則此參數為必填。
- `-Project <String>`：目標專案（放置 Migration 的專案）。省略時使用 Package Manager Console 的預設專案。
- `-StartupProject <String>`：啟動專案。省略時使用方案屬性中的啟動專案。
- `-Args <String>`：傳遞給應用程式的參數。
- `-Verbose`：顯示詳細輸出。

## 4. Migration 歷史

| 順序 | Migration 名稱 | 說明 |
| --- | --- | --- |
| 1 | `Init` | 初始資料庫結構 |
| 2 | `AddUser` | 新增使用者相關資料表 |

> 上表為早期紀錄，最新的 Migration 清單以 `CTMS.EntityModel/Migrations/` 目錄下的實際檔案為準。

## 5. 相關文件

- 解決方案分層與專案職責：[../02-架構/解決方案架構.md](../02-架構/解決方案架構.md)
- 領域資料模型：[../03-資料模型/領域資料模型.md](../03-資料模型/領域資料模型.md)
- 部署與環境設定：[部署與環境設定.md](部署與環境設定.md)
