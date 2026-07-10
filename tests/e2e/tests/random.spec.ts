import { test, expect } from './fixtures';
import { waitForBlazor } from './helpers';

/**
 * 驗證 /Random 隨機表頁為每個 PrefixOwner 產生 4 個分頁（EC/OC × Early/Advance）。
 * 高榮、嘉長各應有 4 個分頁，共 8 個；點入其一確認 RandomView 能正常渲染
 * （佐證高榮/嘉長隨機表已載入、無伺服器錯誤）。
 */
const NEW_HOSPITAL_TABS = [
  '高榮 EC Early',
  '高榮 OC Early',
  '高榮 EC Advance',
  '高榮 OC Advance',
  '嘉長 EC Early',
  '嘉長 OC Early',
  '嘉長 EC Advance',
  '嘉長 OC Advance',
];

test.describe('隨機表 /Random 出現高榮、嘉長分頁', () => {
  test('高榮、嘉長各 4 個分頁皆存在（共 8 個）', async ({ page }) => {
    test.info().annotations.push({ type: '操作步驟', description: '前往 /Random → 確認高榮、嘉長各有 EC/OC × Early/Advance 共 8 個分頁皆存在' });
    await page.goto('/Random');
    // MudTabs 的分頁標題在 prerender HTML 就存在（AlwaysShowScrollButtons 全渲染）
    for (const label of NEW_HOSPITAL_TABS) {
      await expect(page.locator('.mud-tab', { hasText: label })).toBeAttached();
    }
  });

  test('點入高榮分頁可正常渲染（無 Blazor 錯誤）', async ({ page }) => {
    test.info().annotations.push({ type: '操作步驟', description: '前往 /Random → 點入「高榮 EC Early」分頁 → 確認該分頁作用中且頁面無 Blazor 錯誤' });
    await page.goto('/Random');
    await waitForBlazor(page);

    const tab = page.locator('.mud-tab', { hasText: '高榮 EC Early' }).first();
    await tab.click();
    await expect(tab).toHaveClass(/mud-tab-active/);
    // Blazor 未處理例外會顯示 #blazor-error-ui；確認它維持隱藏 = RandomView 正常渲染
    await expect(page.locator('#blazor-error-ui')).toBeHidden();
  });
});

/**
 * 內容斷言（防「空表假陽性」）。
 * RandomView 僅在 Items.Count > 0 才渲染 <table class="sample-table">；因此「分頁 tab 存在」
 * 不等於「有隨機分派資料」。這裡點入各分頁後，斷言表格實際載入預期列數（Early=80、Advance=320）
 * 且首列編號可見，確保高榮/嘉長隨機表確有內容——空表會讓本測試失敗。
 */
test.describe('隨機表 /Random 新醫院分頁「有內容」（防空表假陽性）', () => {
  const CONTENT_CASES = [
    { tab: '高榮 EC Early', firstId: 'Early-01', rows: 80 },
    { tab: '高榮 EC Advance', firstId: 'Advance-001', rows: 320 },
    { tab: '嘉長 EC Early', firstId: 'Early-01', rows: 80 },
    { tab: '嘉長 EC Advance', firstId: 'Advance-001', rows: 320 },
  ];
  for (const c of CONTENT_CASES) {
    test(`分頁「${c.tab}」載入 ${c.rows} 列隨機分派資料`, async ({ page }) => {
      test.info().annotations.push({ type: '操作步驟', description: `前往 /Random → 點入「${c.tab}」→ 確認表格實際載入 ${c.rows} 列隨機分派（非空表）且首列 ${c.firstId} 可見` });
      await page.goto('/Random');
      await waitForBlazor(page);

      const tab = page.locator('.mud-tab', { hasText: c.tab }).first();
      await tab.click();
      await expect(tab).toHaveClass(/mud-tab-active/);

      const rows = page.locator('table.sample-table tbody tr');
      await expect(rows.first()).toBeVisible();
      await expect(rows).toHaveCount(c.rows);
      await expect(page.getByText(c.firstId, { exact: true })).toBeVisible();
    });
  }
});
