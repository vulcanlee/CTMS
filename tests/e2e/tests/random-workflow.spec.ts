import { test, expect } from './fixtures';
import type { Page } from '@playwright/test';
import { waitForBlazor, selectHospital } from './helpers';

/**
 * 隨機表運作規範測試（系統規範，自動＋人工共用同一套）。
 * 設計：docs/superpowers/specs/2026-07-10-隨機表運作規範測試-design.md
 *
 * 每案：建案 → 讀「最後自動編號」計數（驗遞增、取號）→ 目標頁籤尚無此號
 *      → 由個案列「修改」開 BasicClinical 設癌別+分期並儲存 → 目標頁籤出現此號且 Treatment 非空。
 * 案 4/5/6（柳營、奇美、柳營，皆 OC＋Advance）最後於「奇美 OC Advance」驗證三者共存互異、不覆蓋不遺漏。
 *
 * 注意：實際寫入 BackendDB.db / RandomListRuntime.json，請以 scripts/Run-E2E.ps1 執行（自動備份/還原）。
 */

interface Case {
  hospital: string;   // 院別下拉字串（也是 Patient.醫院）
  prefix: string;     // SubjectNo 前綴
  counterKey: string; // 「最後自動編號」計數鍵
  ecoc: 'EC' | 'OC';  // 癌別
  stage: string;      // 癌症分期(FIGO) 值
  tab: string;        // 目標 /Random 頁籤
}

const CASES: Case[] = [
  { hospital: '高雄榮民總醫院', prefix: 'KSVGH', counterKey: 'KSVGH高榮', ecoc: 'OC', stage: 'IB', tab: '高榮 OC Early' },
  { hospital: '嘉義長庚紀念醫院', prefix: 'CYCGMH', counterKey: 'CYCGMH嘉長', ecoc: 'EC', stage: 'IIIA', tab: '嘉長 EC Advance' },
  { hospital: '成大醫院', prefix: 'NCKUH', counterKey: 'NCKUH成大', ecoc: 'EC', stage: 'IB', tab: '成大 EC Early' },
  { hospital: '柳營奇美醫院', prefix: 'CHIMEIH', counterKey: 'CHIMEIH奇美', ecoc: 'OC', stage: 'IIIB', tab: '奇美 OC Advance' },
  { hospital: '奇美醫院', prefix: 'CHIMEIH', counterKey: 'CHIMEIH奇美', ecoc: 'OC', stage: 'IIIB', tab: '奇美 OC Advance' },
  { hospital: '柳營奇美醫院', prefix: 'CHIMEIH', counterKey: 'CHIMEIH奇美', ecoc: 'OC', stage: 'IIIB', tab: '奇美 OC Advance' },
  { hospital: '嘉義長庚紀念醫院', prefix: 'CYCGMH', counterKey: 'CYCGMH嘉長', ecoc: 'EC', stage: 'IB', tab: '嘉長 EC Early' },
  { hospital: '高雄榮民總醫院', prefix: 'KSVGH', counterKey: 'KSVGH高榮', ecoc: 'OC', stage: 'IIIB', tab: '高榮 OC Advance' },
];

function subjectNoOf(prefix: string, counter: number): string {
  return prefix + String(counter).padStart(4, '0');
}

/** 建立一位指定院別的病患。 */
async function createPatient(page: Page, hospital: string): Promise<void> {
  await page.goto('/Browser');
  await waitForBlazor(page);
  await page.locator('button.btn-add').click();
  const dialog = page.locator('.e-dialog', { hasText: '新增病患資料' });
  await expect(dialog).toBeVisible();
  await selectHospital(page, dialog, hospital);
  await dialog.getByRole('button', { name: '確認' }).click();
  await expect(page.getByText('受測者資料已成功新增。')).toBeVisible();
  await page.getByRole('button', { name: '確定' }).click();
}

/** 讀「最後自動編號」某計數鍵目前值（＝最後配發的流水號）。 */
async function readCounter(page: Page, counterKey: string): Promise<number> {
  await page.goto('/Random');
  await waitForBlazor(page);
  const tab = page.locator('.mud-tab', { hasText: '最後自動編號' }).first();
  await tab.click();
  await expect(tab).toHaveClass(/mud-tab-active/);
  const input = page
    .locator('div.m-2', { has: page.getByText(counterKey, { exact: true }) })
    .locator('input')
    .first();
  await expect(input).toBeVisible();
  const raw = await input.inputValue();
  return parseInt(raw.replace(/[^0-9]/g, ''), 10);
}

/** 前往 /Random 並點入指定分頁。 */
async function gotoTab(page: Page, tab: string): Promise<void> {
  await page.goto('/Random');
  await waitForBlazor(page);
  const t = page.locator('.mud-tab', { hasText: tab }).first();
  await t.click();
  await expect(t).toHaveClass(/mud-tab-active/);
}

/** 目標頁籤中 SubjectNo 對應的隨機表列（0 或 1 筆）。 */
function tabRows(page: Page, subjectNo: string) {
  // (?!\d) 避免 KSVGH0007 誤中 KSVGH00070 之類（實際 4 碼補零已足夠，仍加保險）
  return page.locator('table.sample-table tbody tr', { hasText: new RegExp(subjectNo + '(?!\\d)') });
}

/** 展開篩選並以院別搜尋。 */
async function filterByHospital(page: Page, hospital: string): Promise<void> {
  await page.locator('button.btn-filter').click();
  const search = page.locator('input[placeholder="搜尋受測者編號"]');
  await search.fill(hospital);
  await search.blur();
  await page.locator('span.search-icon').click();
  await page.waitForTimeout(500);
}

/** 由個案列「修改」鍵開啟指定 SubjectNo 的 BasicClinical（browse 為插入順序、每頁 5 筆，逐頁尋找）。 */
async function openBasicClinical(page: Page, hospital: string, subjectNo: string): Promise<void> {
  await page.goto('/Browser');
  await waitForBlazor(page);
  await filterByHospital(page, hospital);

  // 篩選條件在 BrowseSearchingService 跨頁保留 PageIndex；上一案若曾翻頁，
  // 本案較少結果會落在超出範圍的頁而顯示空白列。先按「上一頁」回到第 1 頁。
  const prev = page.locator('li.ant-pagination-prev');
  for (let g = 0; g < 40; g++) {
    const cls = (await prev.getAttribute('class')) ?? '';
    if (cls.includes('disabled')) break;
    await prev.click();
    await page.waitForTimeout(400);
  }

  const rowRe = new RegExp(subjectNo + '(?!\\d)');
  for (let i = 0; i < 40; i++) {
    const row = page.getByRole('row', { name: rowRe });
    if ((await row.count()) > 0) {
      await row.first().locator('button.btn-edit').filter({ hasText: '修改' }).click();
      return;
    }
    const next = page.locator('li.ant-pagination-next');
    const cls = (await next.getAttribute('class')) ?? '';
    if (cls.includes('disabled')) break;
    await next.click();
    await page.waitForTimeout(600);
  }
  throw new Error(`個案清單找不到 ${subjectNo} 的列（${hospital}）`);
}

/** 於 BasicClinical 設定癌別＋癌症分期並儲存。 */
async function setClinicalAndSave(page: Page, ecoc: string, stage: string): Promise<void> {
  await page.locator('span.edit-icon').filter({ hasText: '編輯' }).click();

  const ecRow = page.locator('tr', { has: page.getByText('臨床資訊 癌別', { exact: true }) });
  await ecRow.locator('.e-input-group').first().click();
  await page.getByRole('option', { name: ecoc, exact: true }).click();

  const figoRow = page.locator('tr', { has: page.getByText('癌症分期(2023 FIGO)', { exact: true }) });
  await figoRow.locator('.e-input-group').first().click();
  await page.getByRole('option', { name: stage, exact: true }).click();

  await page.locator('button.save-button').click();
  await waitForBlazor(page);
}

test.describe('隨機表運作規範（8 案）', () => {
  test('建案遞增→未上表→填癌別+分期→出現在對應頁籤；柳營/奇美共存', async ({ page }) => {
    test.setTimeout(15 * 60 * 1000);

    const lastCounter: Record<string, number> = {};
    const subjectNos: string[] = [];

    for (let idx = 0; idx < CASES.length; idx++) {
      const c = CASES[idx];
      const label = `案${idx + 1} ${c.hospital}/${c.ecoc}/${c.stage}→${c.tab}`;
      await test.step(label, async () => {
        // 1. 建案
        await createPatient(page, c.hospital);

        // 2. 讀計數、驗遞增、取號
        const counter = await readCounter(page, c.counterKey);
        if (c.counterKey in lastCounter) {
          expect(counter, `${c.counterKey} 流水號應遞增`).toBeGreaterThan(lastCounter[c.counterKey]);
        }
        lastCounter[c.counterKey] = counter;
        const subjectNo = subjectNoOf(c.prefix, counter);
        subjectNos.push(subjectNo);

        // 3. 建案後尚未上表
        await gotoTab(page, c.tab);
        await expect(tabRows(page, subjectNo), `${subjectNo} 建案後不應在「${c.tab}」`).toHaveCount(0);

        // 4. 開 BasicClinical 設癌別+分期並儲存
        await openBasicClinical(page, c.hospital, subjectNo);
        await expect(page.getByText(subjectNo, { exact: false }).first()).toBeVisible();
        await setClinicalAndSave(page, c.ecoc, c.stage);

        // 5. 填後應出現在對應頁籤，且 Treatment 非空
        await gotoTab(page, c.tab);
        const assigned = tabRows(page, subjectNo);
        await expect(assigned, `${subjectNo} 填臨床資訊後應出現在「${c.tab}」`).toHaveCount(1);
        await expect(assigned.first()).toContainText(/Dr|AI/);
      });
    }

    // 共存驗證：案 4/5/6（柳營、奇美、柳營）皆在「奇美 OC Advance」且互異、各佔一列
    await test.step('柳營/奇美於「奇美 OC Advance」共存互異', async () => {
      await gotoTab(page, '奇美 OC Advance');
      for (const i of [3, 4, 5]) {
        await expect(tabRows(page, subjectNos[i]), `共存：${subjectNos[i]} 應各佔一列`).toHaveCount(1);
      }
      const trio = [subjectNos[3], subjectNos[4], subjectNos[5]];
      expect(new Set(trio).size, `三案 SubjectNo 應互異：${trio.join(', ')}`).toBe(3);
    });
  });
});
