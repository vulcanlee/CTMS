import { test, expect } from './fixtures';

/**
 * 收案進度統計頁 /EnrollmentProgress 已改用 HospitalRegistry.PrefixOwners
 * 與 NormalizeToOwnerShortName，醫院欄位與儀表板同一套歸戶邏輯：
 *   - 表頭為 5 家 prefix 擁有者（成大 / 奇美 / 郭綜合 / 高榮 / 嘉長），順序同 registry。
 *   - 柳營奇美不單獨成欄，其病例併入奇美。
 * 表頭文字為「短名 + 醫院」（EnrollmentProgressView.razor 既有樣板）。
 * 內容在 prerender（OnInitializedAsync 設 isLoaded=true）即產出，不需等 circuit。
 */
test.describe('收案進度 /EnrollmentProgress（registry 化，5 家）', () => {
  test('表頭為成大/奇美/郭綜合/高榮/嘉長五家', async ({ page }) => {
    test.info().annotations.push({ type: '操作步驟', description: '前往 /EnrollmentProgress → 確認表頭為 5 家（成大、奇美、郭綜合、高榮、嘉長），順序同 HospitalRegistry' });
    await page.goto('/EnrollmentProgress');
    await expect(page.locator('.enrollment-table')).toBeVisible();

    const headers = page.locator('th.hospital-header');
    await expect(headers).toHaveText([
      '成大醫院', '奇美醫院', '郭綜合醫院', '高榮醫院', '嘉長醫院',
    ]);
  });

  test('柳營奇美不單獨成欄（併入奇美）', async ({ page }) => {
    test.info().annotations.push({ type: '操作步驟', description: '前往 /EnrollmentProgress → 確認沒有「柳營」欄位，柳營奇美病例歸戶至奇美' });
    await page.goto('/EnrollmentProgress');
    await expect(page.locator('.enrollment-table')).toBeVisible();

    await expect(page.locator('th.hospital-header', { hasText: '柳營' })).toHaveCount(0);
  });
});
