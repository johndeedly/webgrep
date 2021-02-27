using System;
using System.Collections.Generic;
using System.Linq;
using PlaywrightSharp;

namespace webgrep
{
    public interface IBrowserInstance
    {
        List<string> ErrorLog { get; }
        Exception LastError { get; }

        bool ClickOn(string selector, int? timeout = 5000);
        bool ElementIsAttached(string selector, int? timeout = 5000);
        string GetFormattedText(string selector, int? timeout = 5000);
        List<string> GetFormattedTextAll(string selector, int? timeout = 5000);
        string GetText(string selector, int? timeout = 5000);
        List<string> GetTextAll(string selector, int? timeout = 5000);
        bool KeyboardPressKey(string key);
        bool KeyboardTypeText(string text);
        bool NavigateTo(string url, int? timeout = 5000);
        bool TakeScreenshot(string filePath, int? timeout = 5000);
        bool WaitDomContentLoaded(int? timeout = 5000);
    }

    public abstract class BrowserInstance : IDisposable, IBrowserInstance
    {
        protected IPlaywright playwright;
        protected IBrowser browser;
        protected IPage page;
        public List<string> ErrorLog { get; private set; }
        public Exception LastError { get; protected set; }

        public BrowserInstance()
        {
            ErrorLog = new List<string>();
        }

        public bool NavigateTo(string url, int? timeout = 5000)
        {
            try
            {
                page.GoToAsync(url, LifecycleEvent.DOMContentLoaded, timeout: timeout).GetAwaiter().GetResult();
                return true;
            }
            catch (Exception ex)
            {
                ErrorLog.Add(ex.Message);
                LastError = ex;
                return false;
            }
        }

        public bool WaitDomContentLoaded(int? timeout = 5000)
        {
            try
            {
                page.WaitForNavigationAsync(LifecycleEvent.DOMContentLoaded, timeout: timeout).GetAwaiter().GetResult();
                return true;
            }
            catch (Exception ex)
            {
                ErrorLog.Add(ex.Message);
                LastError = ex;
                return false;
            }
        }

        public bool ClickOn(string selector, int? timeout = 5000)
        {
            try
            {
                page.ClickAsync(selector, timeout: timeout).GetAwaiter().GetResult();
                return true;
            }
            catch (TimeoutException ex)
            {
                ErrorLog.Add(ex.Message);
                LastError = ex;
                for (int i = 0; i < page.Frames.Length; i++)
                {
                    try
                    {
                        page.Frames[i].ClickAsync(selector, timeout: timeout).GetAwaiter().GetResult();
                        return true;
                    }
                    catch (TimeoutException ex2)
                    {
                        ErrorLog.Add(ex2.Message);
                        LastError = ex2;
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Add(ex.Message);
                LastError = ex;
            }
            return false;
        }

        public bool KeyboardTypeText(string text)
        {
            try
            {
                page.Keyboard.TypeAsync(text).GetAwaiter().GetResult();
                return true;
            }
            catch (Exception ex)
            {
                ErrorLog.Add(ex.Message);
                LastError = ex;
                return false;
            }
        }

        public bool KeyboardPressKey(string key)
        {
            try
            {
                page.Keyboard.PressAsync(key).GetAwaiter().GetResult();
                return true;
            }
            catch (Exception ex)
            {
                ErrorLog.Add(ex.Message);
                LastError = ex;
                return false;
            }
        }

        public bool TakeScreenshot(string filePath, int? timeout = 5000)
        {
            if (WaitDomContentLoaded(timeout))
            {
                try
                {
                    page.ScreenshotAsync(filePath, true).GetAwaiter().GetResult();
                    return true;
                }
                catch (Exception ex)
                {
                    ErrorLog.Add(ex.Message);
                    LastError = ex;
                }
            }
            return false;
        }

        public bool ElementIsAttached(string selector, int? timeout = 5000)
        {
            try
            {
                page.WaitForSelectorAsync(selector, WaitForState.Attached, timeout: timeout).GetAwaiter().GetResult();
                return true;
            }
            catch (Exception ex)
            {
                ErrorLog.Add(ex.Message);
                LastError = ex;
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
            catch (Exception ex)
            {
                ErrorLog.Add(ex.Message);
                LastError = ex;
                return string.Empty;
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
            catch (Exception ex)
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
            catch (Exception ex)
            {
                ErrorLog.Add(ex.Message);
                LastError = ex;
                return string.Empty;
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
            catch (Exception ex)
            {
                ErrorLog.Add(ex.Message);
                LastError = ex;
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