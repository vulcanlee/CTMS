import { test, expect } from './fixtures';
import { waitForBlazor } from './helpers';

/**
 * 驗證收案（新增病患）表單的「院別」下拉列出全部 6 家醫院，含三家新醫院。
 * 此下拉資料來源 DropDownListDataService.Get院別() 迭代 HospitalRegistry.All，
 * 因此柳營奇美（IsPrefixOwner=false）也會出現。
 */
const ALL_HOSPITALS = [
  '成大醫院',
  '奇美醫院',
  '郭綜合醫院',
  '柳營奇美醫院',
  '高雄榮民總醫院',
  '嘉義長庚紀念醫院',
];

test('收案表單院別下拉列出 6 家（含三家新醫院）', async ({ page }) => {
  test.info().annotations.push({ type: '操作步驟', description: '登入 → 前往 /Browser → 點「新增受測者資料」開對話框 → 展開院別下拉 → 確認列出全部 6 家醫院（含高榮、嘉長、柳營奇美）' });
  await page.goto('/Browser');
  await waitForBlazor(page);

  await page.locator('button.btn-add').click();

  const dialog = page.locator('.e-dialog', { hasText: '新增病患資料' });
  await expect(dialog).toBeVisible();

  // 開啟院別下拉（對話框內第一個 e-input-group）
  await dialog.locator('.e-input-group').first().click();

  // Syncfusion 選項 append 到 body，li 具 role="option"
  const options = page.getByRole('option');
  await expect(options).toHaveCount(ALL_HOSPITALS.length);
  for (const name of ALL_HOSPITALS) {
    await expect(page.getByRole('option', { name, exact: true })).toBeVisible();
  }
});
