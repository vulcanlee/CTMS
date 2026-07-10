import type { Reporter, TestCase, TestResult } from '@playwright/test/reporter';
import fs from 'node:fs';
import path from 'node:path';

/**
 * 自訂報告器：把每支測試的操作影片複製到專屬 videos/ 資料夾，並以測試名命名。
 *
 * 在 onTestEnd 執行——此時測試已結束、page/context 已關閉、影片檔已寫入，
 * 直接複製檔案即可，不會像在 fixture teardown 呼叫 video.saveAs() 那樣死結。
 */
const VIDEO_DIR = 'videos';

function safeName(name: string): string {
  return name.replace(/[^\p{L}\p{N}]+/gu, '_').replace(/^_+|_+$/g, '').slice(0, 80);
}

export default class VideoCollector implements Reporter {
  onTestEnd(test: TestCase, result: TestResult): void {
    const video = result.attachments.find(a => a.name === 'video' && a.path);
    if (!video?.path) return;
    try {
      fs.mkdirSync(VIDEO_DIR, { recursive: true });
      fs.copyFileSync(video.path, path.join(VIDEO_DIR, `${safeName(test.title)}.webm`));
    } catch {
      // 影片複製失敗不影響測試結果
    }
  }
}
