using System;
using System.Collections.Generic;
using System.Linq;
using PlaywrightSharp;

namespace webgrep
{
    public sealed class FirefoxInstance : IDisposable
    {
        IPlaywright playwright;
        IBrowser browser;
        IPage page;
        
        public FirefoxInstance()
        {
            playwright = Playwright.CreateAsync().GetAwaiter().GetResult();
            browser = playwright.Firefox.LaunchAsync().GetAwaiter().GetResult();
            page = browser.NewPageAsync().GetAwaiter().GetResult();
        }

        public void NavigateTo(string url, int? timeout = 5000)
        {
            page.GoToAsync(url, LifecycleEvent.Networkidle, timeout: timeout).Wait();
        }

        public void ClickOn(string selector, int? timeout = 5000)
        {
            page.ClickAsync(selector, timeout: timeout).Wait();
        }

        public void KeyboardTypeText(string text)
        {
            page.Keyboard.TypeAsync(text).Wait();
        }

        public void KeyboardPressKey(string key)
        {
            page.Keyboard.PressAsync(key).Wait();
        }

        public void TakeScreenshot(string filePath)
        {
            page.ScreenshotAsync(filePath, true).Wait();
        }

        public bool ElementIsAttached(string selector, int? timeout = 5000)
        {
            try
            {
                var elem = page.WaitForSelectorAsync(selector, WaitForState.Attached, timeout: timeout).GetAwaiter().GetResult();
                return elem != null;
            }
            catch (TimeoutException)
            {
                return false;
            }
        }

        public string GetText(string selector, int? timeout = 5000)
        {
            try
            {
                string text = page.GetTextContentAsync(selector, timeout).GetAwaiter().GetResult();
                return text.Trim();
            }
            catch (TimeoutException ex)
            {
                return ex.Message;
            }
        }

        public List<string> GetTextAll(string selector, int? timeout = 5000)
        {
            List<string> list = new List<string>();
            try
            {
                List<IElementHandle> elements = page.QuerySelectorAllAsync(selector).GetAwaiter().GetResult().ToList();
                foreach (var elem in elements)
                {
                    string text = elem.GetTextContentAsync(timeout).GetAwaiter().GetResult();
                    list.Add(text.Trim());
                }
            }
            catch (TimeoutException ex)
            {
                list.Add(ex.Message);
            }
            return list;
        }

        ~FirefoxInstance()
        {
            Dispose();
        }

        public void Dispose()
        {
            page = null;
            browser?.CloseAsync();
            browser = null;
            playwright?.Dispose();
            playwright = null;
        }
    }
}