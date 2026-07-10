import { test, expect } from './fixtures';
import { waitForBlazor, selectHospital } from './helpers';

/**
 * 建立測試病患，驗證受試者編號（SubjectNo）產號正確：
 *  - 高雄榮民總醫院 → KSVGH####
 *  - 嘉義長庚紀念醫院 → CYCGMH####
 *  - 柳營奇美醫院 → CHIMEIH####（與奇美共用 prefix），且「醫院名稱」欄
 *    正確顯示「柳營奇美醫院」（讀 Patient.醫院，非以 prefix 反查）。
 *
 * 注意：本測試會實際寫入 BackendDB.db 與 SubjectNoGenerator.json，
 * 請透過 scripts/Run-E2E.ps1 執行（自動備份/還原），避免污染開發資料。
 */
type PatientCase = { hospital: string; prefix: string };

const CASES: PatientCase[] = [
  { hospital: '高雄榮民總醫院', prefix: 'KSVGH' },
  { hospital: '嘉義長庚紀念醫院', prefix: 'CYCGMH' },
  { hospital: '柳營奇美醫院', prefix: 'CHIMEIH' },
];

test.describe('建立測試病患驗證 SubjectNo 產號', () => {
  for (const c of CASES) {
    test(`建立「${c.hospital}」→ 病人編號前綴 ${c.prefix}`, async ({ page }) => {
      test.info().annotations.push({ type: '操作步驟', description: `登入 → 前往 /Browser → 開新增病患對話框 → 選院別「${c.hospital}」→ 收案日期預設今日 → 按確認送出 → 以院名搜尋 → 驗證病人編號符合前綴 ${c.prefix}、醫院名稱正確` });
      await page.goto('/Browser');
      await waitForBlazor(page);

      // 1. 開新增病患對話框
      await page.locator('button.btn-add').click();
      const dialog = page.locator('.e-dialog', { hasText: '新增病患資料' });
      await expect(dialog).toBeVisible();

      // 2. 選院別（收案日期預設今日）
      await selectHospital(page, dialog, c.hospital);

      // 3. 確認送出
      await dialog.getByRole('button', { name: '確認' }).click();

      // 4. 成功訊息 → 按確定關閉
      await expect(page.getByText('受測者資料已成功新增。')).toBeVisible();
      await page.getByRole('button', { name: '確定' }).click();

      // 5. 展開篩選並搜尋。注意：此頁關鍵字實際比對「醫院/癌別」欄位（非 SubjectNo），
      //    故以醫院名稱搜尋；並用 blur() 觸發 @bind 的 change 事件提交 Keyword。
      await page.locator('button.btn-filter').click();
      const search = page.locator('input[placeholder="搜尋受測者編號"]');
      await search.fill(c.hospital);
      await search.blur();
      await page.locator('span.search-icon').click();

      // 6. 驗證表格出現一列，其病人編號符合 prefix + 4 碼、且醫院名稱正確。
      //    AntDesign 表格列具 role="row"，可及名稱為各儲存格文字串接。
      const row = page.getByRole('row', { name: new RegExp(`${c.prefix}\\d{4}`) }).first();
      await expect(row).toBeVisible();
      await expect(row).toContainText(c.hospital);
    });
  }
});
