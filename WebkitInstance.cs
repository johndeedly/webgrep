using PlaywrightSharp;

namespace webgrep
{
    public sealed class WebkitInstance : BrowserInstance
    {
        public WebkitInstance()
        {
            playwright = Playwright.CreateAsync().GetAwaiter().GetResult();
            browser = playwright.Webkit.LaunchAsync().GetAwaiter().GetResult();
            page = browser.NewPageAsync().GetAwaiter().GetResult();
        }
    }
}