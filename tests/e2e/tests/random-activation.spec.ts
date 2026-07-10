import { test, expect } from './fixtures';
import type { Page } from '@playwright/test';
import { waitForBlazor, selectHospital } from './helpers';

/**
 * 驗證「隨機表啟用規則」（受試者編號與隨機分組.md §5.7）：
 *   1. 新增受試者後，其 SubjectNo「不會」出現在隨機表任一分頁。
 *   2. 開啟 BasicClinical，填「癌別（EC）＋癌症分期（FIGO=IB→Early）」並儲存後，
 *      SubjectNo「才會」出現在對應分頁（高榮 EC Early），且該列 Treatment 已回填。
 *
 * 這支測試補上既有 create-patient / random 測試的涵蓋缺口——先前測試只驗
 * 「分頁存在、有列數」，從未走「填臨床資訊後才上表」這條實際啟用路徑。
 *
 * 注意：以個案列的「修改」鍵開啟 BasicClinical（走 SPA 導覽、用正確的 Code，
 * 而非以 SubjectNo 直接組 URL）。會實際寫入 BackendDB.db / RandomListRuntime.json，
 * 請以 scripts/Run-E2E.ps1 執行（自動備份/還原）。
 */

/** 展開個案清單篩選並以醫院名稱搜尋。 */
async function filterByHospital(page: Page, hospital: string): Promise<void> {
  await page.locator('button.btn-filter').click();
  const search = page.locator('input[placeholder="搜尋受測者編號"]');
  await search.fill(hospital);
  await search.blur();
  await page.locator('span.search-icon').click();
}

/** 前往 /Random 並點入指定分頁，等待其成為作用中分頁。 */
async function gotoRandomTab(page: Page, tabLabel: string): Promise<void> {
  await page.goto('/Random');
  await waitForBlazor(page);
  const tab = page.locator('.mud-tab', { hasText: tabLabel }).first();
  await tab.click();
  await expect(tab).toHaveClass(/mud-tab-active/);
}

test.describe('隨機表啟用規則：填癌別+分期後 SubjectNo 才上隨機表', () => {
  test('高雄榮民總醫院：建案未上表 → 設 EC + FIGO IB → 出現在「高榮 EC Early」', async ({ page }) => {
    test.info().annotations.push({
      type: '操作步驟',
      description:
        '建立高榮病患並取得 SubjectNo → 確認 /Random「高榮 EC Early」尚無此編號 → 由個案列「修改」開啟 BasicClinical，設癌別 EC、FIGO 分期 IB（Early）並儲存 → 回「高榮 EC Early」確認此編號已出現且 Treatment 非空',
    });

    // 1. 建立高榮病患
    await page.goto('/Browser');
    await waitForBlazor(page);
    await page.locator('button.btn-add').click();
    const dialog = page.locator('.e-dialog', { hasText: '新增病患資料' });
    await expect(dialog).toBeVisible();
    await selectHospital(page, dialog, '高雄榮民總醫院');
    await dialog.getByRole('button', { name: '確認' }).click();
    await expect(page.getByText('受測者資料已成功新增。')).toBeVisible();
    await page.getByRole('button', { name: '確定' }).click();

    // 2. 篩選出高榮病患，取得其 SubjectNo（KSVGH####）
    await filterByHospital(page, '高雄榮民總醫院');
    const createdRow = page.getByRole('row', { name: /KSVGH\d{4}/ }).first();
    await expect(createdRow).toBeVisible();
    const subjectNo = (await createdRow.innerText()).match(/KSVGH\d{4}/)?.[0];
    expect(subjectNo, '應能從個案列取得 KSVGH#### 編號').toBeTruthy();

    // 3. 尚未填臨床資訊 → 「高榮 EC Early」分頁不應出現此 SubjectNo
    await gotoRandomTab(page, '高榮 EC Early');
    await expect(
      page.locator('table.sample-table tbody tr', { hasText: subjectNo! }),
      '建案後尚未填癌別/分期，SubjectNo 不應出現在隨機表',
    ).toHaveCount(0);

    // 4. 回個案清單，點該病患列的「修改」開啟 BasicClinical（SPA 導覽、用正確 Code）
    await page.goto('/Browser');
    await waitForBlazor(page);
    await filterByHospital(page, '高雄榮民總醫院');
    const row = page.getByRole('row', { name: new RegExp(subjectNo!) }).first();
    await expect(row).toBeVisible();
    await row.locator('button.btn-edit').filter({ hasText: '修改' }).click();

    // 5. 進入 BasicClinical，確認載入該病患後按「編輯」
    await expect(page.getByText(subjectNo!, { exact: false }).first()).toBeVisible();
    await page.locator('span.edit-icon').filter({ hasText: '編輯' }).click();

    // 5a. 癌別列（label：臨床資訊 癌別）→ 選 EC
    const ecRow = page.locator('tr', { has: page.getByText('臨床資訊 癌別', { exact: true }) });
    await ecRow.locator('.e-input-group').first().click();
    await page.getByRole('option', { name: 'EC', exact: true }).click();

    // 5b. 癌症分期列（label：癌症分期(2023 FIGO)）→ 選 IB（EC 清單中 IB 屬 Early）
    const figoRow = page.locator('tr', { has: page.getByText('癌症分期(2023 FIGO)', { exact: true }) });
    await figoRow.locator('.e-input-group').first().click();
    await page.getByRole('option', { name: 'IB', exact: true }).click();

    // 5c. 儲存
    await page.locator('button.save-button').click();
    await waitForBlazor(page);

    // 6. 回「高榮 EC Early」→ 此 SubjectNo 應已出現，且該列 Treatment（Dr/AI）非空
    await gotoRandomTab(page, '高榮 EC Early');
    const assignedRow = page.locator('table.sample-table tbody tr', { hasText: subjectNo! });
    await expect(assignedRow, '填妥癌別+分期並儲存後，SubjectNo 應出現在隨機表').toHaveCount(1);
    await expect(assignedRow.first()).toContainText(/Dr|AI/);
  });
});
