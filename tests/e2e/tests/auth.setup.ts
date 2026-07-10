import { test as setup, expect } from '@playwright/test';

const AUTH_FILE = '.auth/state.json';

/**
 * 登入一次並把 Cookie 存進 storageState，供其餘測試共用，
 * 避免每支測試重複登入。
 *
 * 登入頁為靜態 SSR 表單 POST（HttpContext.SignInAsync），
 * 成功後導向 "/"，經 SplashView 約 0.5 秒自動轉址到 /Dashboard。
 */
setup('登入並儲存登入狀態', async ({ page }) => {
  await page.goto('/Auths/Login');

  await page.locator('input[placeholder="請輸入帳號"]').fill('support');
  await page.locator('input[placeholder="請輸入密碼"]').fill('support');
  await page.getByRole('button', { name: '登入' }).click();

  // 登入成功會經 "/" splash 自動轉址到 /Dashboard
  await page.waitForURL('**/Dashboard', { timeout: 30_000 });
  await expect(page).toHaveURL(/\/Dashboard/);

  await page.context().storageState({ path: AUTH_FILE });
});
