import { defineConfig, devices } from '@playwright/test';

/**
 * CTMS 新增三家醫院需求 E2E 驗證設定。
 *
 * - App 由 scripts/Run-E2E.ps1 負責啟停（非 Playwright webServer），
 *   以確保「先關 App 再還原資料檔」的順序，避免 SQLite 檔案鎖。
 * - headed 模式 + slowMo 讓瀏覽器操作過程可親眼看到。
 * - 失敗時保留 trace 與影片，方便回放除錯。
 */
export default defineConfig({
  testDir: './tests',
  // Blazor Server 走 SignalR，互動與轉址需要較寬鬆的等待時間
  timeout: 90_000,
  expect: { timeout: 15_000 },
  // 建立病患會寫入共用資料檔，測試間不可平行，避免計數/DB 競態
  fullyParallel: false,
  workers: 1,
  retries: 0,
  reporter: [['html', { open: 'never' }], ['list'], ['./video-collector.ts']],
  use: {
    baseURL: process.env.CTMS_BASE_URL ?? 'http://localhost:5272',
    // 預設 headed（讓操作可見）；設 CTMS_HEADLESS=1 可切無頭（開發迭代較快）
    headless: process.env.CTMS_HEADLESS === '1',
    // 放慢每個操作，讓過程看得清楚
    launchOptions: { slowMo: 400 },
    trace: 'retain-on-failure',
    // 全程錄影（含成功案例）；每支測試結束後由 fixtures.ts 另存到 videos/ 資料夾
    video: 'on',
    screenshot: 'only-on-failure',
    locale: 'zh-TW',
    ignoreHTTPSErrors: true,
  },
  projects: [
    // 先登入一次並存下 storageState
    { name: 'setup', testMatch: /auth\.setup\.ts/ },
    {
      name: 'chromium',
      use: {
        ...devices['Desktop Chrome'],
        storageState: '.auth/state.json',
      },
      dependencies: ['setup'],
    },
  ],
});
