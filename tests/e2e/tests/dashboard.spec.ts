import { test, expect } from './fixtures';

/**
 * 驗證 /Dashboard「合作醫院」呈現 5 家 PrefixOwner，含新增的高榮、嘉長；
 * 柳營奇美因 IsPrefixOwner=false，併入奇美、不單獨計數。
 *
 * 註：醫院長條圖是 <canvas>（Chart.js 繪製），高榮/嘉長不是 DOM 文字，
 * 故改以「合作醫院」統計卡的數字與醫院清單文字（GetHospitalList 以「、」join
 * 短名）作為可靠的 DOM 斷言點。儀表板內容在 prerender HTML 即產出，不需等 circuit。
 */
test.describe('儀表板 /Dashboard 顯示三家新醫院', () => {
  test('合作醫院數為 5 且醫院清單含高榮、嘉長', async ({ page }) => {
    test.info().annotations.push({ type: '操作步驟', description: '前往 /Dashboard → 檢查「合作醫院」統計卡數字為 5、清單含高榮與嘉長（既有三家仍在）' });
    await page.goto('/Dashboard');

    const card = page.locator('.stat-card', { hasText: '合作醫院' });
    await expect(card).toBeVisible();
    await expect(card.locator('.stat-value')).toHaveText('5');

    const subtitle = card.locator('.stat-subtitle');
    // 新增三家中的兩家 PrefixOwner
    await expect(subtitle).toContainText('高榮');
    await expect(subtitle).toContainText('嘉長');
    // 既有三家仍在
    await expect(subtitle).toContainText('成大');
    await expect(subtitle).toContainText('奇美');
    await expect(subtitle).toContainText('郭綜合');
  });
});
