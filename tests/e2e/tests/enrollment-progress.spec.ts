import { test, expect } from './fixtures';

/**
 * 【已知缺口】收案進度統計頁 /EnrollmentProgress 目前仍硬編碼只有 3 家
 * （成大 / 郭綜合 / 奇美），EnrollmentProgressService 未改用 HospitalRegistry，
 * 因此高榮、嘉長不會出現在此頁。
 *
 * 本測試斷言「現況」——僅 3 家表頭——以在報告中明確標記此缺口，
 * 供團隊決定是否補修（把 EnrollmentProgressService registry 化）。
 * 內容在 prerender（OnInitializedAsync 設 isLoaded=true）即產出，不需等 circuit。
 */
test.describe('收案進度 /EnrollmentProgress（已知缺口：僅硬編碼 3 家）', () => {
  test('表頭僅成大/郭綜合/奇美，不含高榮、嘉長', async ({ page }) => {
    test.info().annotations.push({ type: '操作步驟', description: '前往 /EnrollmentProgress → 確認表頭僅成大/郭綜合/奇美三家（高榮、嘉長尚未納入，屬已知後續優化項）' });
    await page.goto('/EnrollmentProgress');
    await expect(page.locator('.enrollment-table')).toBeVisible();

    const headers = page.locator('th.hospital-header');
    await expect(headers).toHaveText(['成大醫院', '郭綜合醫院', '奇美醫院']);

    // 明確記錄缺口：新醫院不出現在此頁
    await expect(page.locator('th.hospital-header', { hasText: '高' })).toHaveCount(0);
    await expect(page.locator('th.hospital-header', { hasText: '嘉' })).toHaveCount(0);
  });
});
