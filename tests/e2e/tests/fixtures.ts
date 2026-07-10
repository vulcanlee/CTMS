import { test as base, expect, Page, TestInfo } from '@playwright/test';
import fs from 'node:fs';
import path from 'node:path';

/**
 * 自訂 fixture：為每支測試自動擷取「操作前 / 操作後」全螢幕截圖。
 *  - 操作前（before）：第一次 page.goto 完成、頁面載入後立即擷取。
 *  - 操作後（after） ：測試主體結束、瀏覽器關閉前擷取最終畫面。
 *
 * 截圖會同時：
 *  1. 存成檔案到 tests/e2e/screenshots/<測試名>__before|after.png
 *  2. attach 進 Playwright HTML 報告（可在報告內逐測試檢視前後對照）
 */
const DIR = 'screenshots';

function safeName(name: string): string {
  return name.replace(/[^\p{L}\p{N}]+/gu, '_').replace(/^_+|_+$/g, '').slice(0, 80);
}

async function snap(page: Page, testInfo: TestInfo, phase: 'before' | 'after'): Promise<void> {
  try {
    if (page.isClosed()) return;
    const buf = await page.screenshot({ fullPage: false });
    fs.mkdirSync(DIR, { recursive: true });
    fs.writeFileSync(path.join(DIR, `${safeName(testInfo.title)}__${phase}.png`), buf);
    const label = phase === 'before' ? '操作前' : '操作後';
    await testInfo.attach(label, { body: buf, contentType: 'image/png' });
  } catch {
    // 截圖失敗不影響測試判定，忽略即可
  }
}

export const test = base.extend({
  page: async ({ page }, use, testInfo) => {
    let firstNav = true;
    const originalGoto = page.goto.bind(page);
    // 包裝 goto：首次導覽完成後擷取「操作前」畫面
    page.goto = (async (url: string, opts?: Parameters<Page['goto']>[1]) => {
      const res = await originalGoto(url, opts);
      if (firstNav) {
        firstNav = false;
        await snap(page, testInfo, 'before');
      }
      return res;
    }) as Page['goto'];

    await use(page);

    // 測試結束、關閉前擷取「操作後」畫面。
    // 註：操作影片改由 video-collector 報告器在測試結束後複製到 videos/，
    // 不可在此呼叫 video.saveAs()——它會等待 page 關閉，而 page 要等本
    // teardown 結束才關閉，將造成死結、逐測試逾時。
    await snap(page, testInfo, 'after');
  },
});

export { expect };
