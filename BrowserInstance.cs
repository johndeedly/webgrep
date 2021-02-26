using System;
using System.Collections.Generic;
using System.Linq;
using PlaywrightSharp;

namespace webgrep
{
    public interface IBrowserInstance
    {
        void ClickOn(string selector, int? timeout = 5000);
        bool ElementIsAttached(string selector, int? timeout = 5000);
        string GetFormattedText(string selector, int? timeout = 5000);
        List<string> GetFormattedTextAll(string selector, int? timeout = 5000);
        string GetText(string selector, int? timeout = 5000);
        List<string> GetTextAll(string selector, int? timeout = 5000);
        void KeyboardPressKey(string key);
        void KeyboardTypeText(string text);
        void NavigateTo(string url, int? timeout = 5000);
        void TakeScreenshot(string filePath);
    }

    public abstract class BrowserInstance : IBrowserInstance, IDisposable
    {
        protected IPlaywright playwright;
        protected IBrowser browser;
        protected IPage page;

        public BrowserInstance()
        { }

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

        public string GetFormattedText(string selector, int? timeout = 5000)
        {
            try
            {
                string text = page.GetInnerTextAsync(selector, timeout).GetAwaiter().GetResult();
                return text.Trim();
            }
            catch (TimeoutException ex)
            {
                return ex.Message;
            }
        }

        public List<string> GetFormattedTextAll(string selector, int? timeout = 5000)
        {
            List<string> list = new List<string>();
            try
            {
                List<IElementHandle> elements = page.QuerySelectorAllAsync(selector).GetAwaiter().GetResult().ToList();
                foreach (var elem in elements)
                {
                    string text = elem.GetInnerTextAsync(timeout).GetAwaiter().GetResult();
                    list.Add(text.Trim());
                }
            }
            catch (TimeoutException ex)
            {
                list.Add(ex.Message);
            }
            return list;
        }

        ~BrowserInstance()
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