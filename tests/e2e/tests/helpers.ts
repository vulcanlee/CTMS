import { Page, Locator, expect } from '@playwright/test';

/**
 * Blazor Server 走 SignalR circuit，互動事件（按鈕、下拉）要等 circuit
 * 連線後才會回應。這裡等 window.Blazor 就緒再加一段緩衝時間。
 */
export async function waitForBlazor(page: Page): Promise<void> {
  await page
    .waitForFunction(() => (window as unknown as { Blazor?: unknown }).Blazor !== undefined, null, {
      timeout: 30_000,
    })
    .catch(() => {
      /* 若腳本尚未載入，靠下方緩衝與後續斷言的自動重試補償 */
    });
  // circuit 連線為非同步，給予緩衝時間確保互動事件能被接收
  await page.waitForTimeout(2500);
}

/**
 * Syncfusion SfDropDownList 不是原生 <select>：它渲染成 input + 一個 append 到
 * <body> 的 popup 清單。操作方式為「點開輸入框 → 點 popup 內的選項」。
 *
 * @param page      Playwright page
 * @param container 下拉所在的容器（例如新增病患對話框 .e-dialog）
 * @param optionText 要選的選項文字（精確比對，避免「奇美醫院」誤中「柳營奇美醫院」）
 */
export async function selectHospital(page: Page, container: Locator, optionText: string): Promise<void> {
  // 對話框內第一個 e-input-group 即院別下拉（第二個是收案日期）
  await container.locator('.e-input-group').first().click();
  await page.getByRole('option', { name: optionText, exact: true }).click();
}
