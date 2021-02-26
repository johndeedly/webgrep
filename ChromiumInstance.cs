using PlaywrightSharp;

namespace webgrep
{
    public sealed class ChromiumInstance : BrowserInstance
    {
        public ChromiumInstance()
        {
            playwright = Playwright.CreateAsync().GetAwaiter().GetResult();
            browser = playwright.Chromium.LaunchAsync().GetAwaiter().GetResult();
            page = browser.NewPageAsync().GetAwaiter().GetResult();
        }
    }
}