using System;
using System.Collections.Generic;
using System.Linq;
using PlaywrightSharp;

namespace webgrep
{
    public interface IBrowserInstance : IFrameWrapper, IDisposable
    {
        List<string> ErrorLog { get; }
        Exception LastError { get; set; }

        bool KeyboardPressKey(string key);
        bool KeyboardTypeText(string text);
        bool NavigateTo(string url, int? timeout = 5000);
        bool TakeScreenshot(string filePath, int? timeout = 5000);
    }

    public abstract class BrowserInstance : IBrowserInstance
    {
        protected IPlaywright playwright;
        protected IBrowser browser;
        protected IPage page;
        public List<string> ErrorLog { get; private set; }
        public Exception LastError { get; set; }
        public FrameWrapper[] Frames
        {
            get
            {
                return page.Frames.Select(x => new FrameWrapper(x, this)).ToArray();
            }
        }

        public BrowserInstance()
        {
            ErrorLog = new List<string>();
        }

        public bool NavigateTo(string url, int? timeout = 5000)
        {
            try
            {
                try
                {
                    page.GoToAsync(url, LifecycleEvent.Networkidle, timeout: timeout).GetAwaiter().GetResult();
                }
                catch (TimeoutException ex)
                {
                    ErrorLog.Add(ex.Message);
                    LastError = ex;
                    page.WaitForNavigationAsync(LifecycleEvent.DOMContentLoaded, timeout: timeout).GetAwaiter().GetResult();
                }
                return true;
            }
            catch (Exception ex)
            {
                ErrorLog.Add(ex.Message);
                LastError = ex;
                return false;
            }
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

        public bool WaitDomContentLoaded(int? timeout = 5000)
        {
            return new FrameWrapper(page.MainFrame, this).WaitDomContentLoaded(timeout);
        }

        public bool ClickOn(string selector, int? timeout = 5000)
        {
            return new FrameWrapper(page.MainFrame, this).ClickOn(selector, timeout);
        }

        public bool ElementIsAttached(string selector, int? timeout = 5000)
        {
            return new FrameWrapper(page.MainFrame, this).ElementIsAttached(selector, timeout);
        }

        public string GetText(string selector, int? timeout = 5000)
        {
            return new FrameWrapper(page.MainFrame, this).GetText(selector, timeout);
        }

        public List<string> GetTextAll(string selector, int? timeout = 5000)
        {
            return new FrameWrapper(page.MainFrame, this).GetTextAll(selector, timeout);
        }

        public string GetFormattedText(string selector, int? timeout = 5000)
        {
            return new FrameWrapper(page.MainFrame, this).GetFormattedText(selector, timeout);
        }

        public List<string> GetFormattedTextAll(string selector, int? timeout = 5000)
        {
            return new FrameWrapper(page.MainFrame, this).GetFormattedTextAll(selector, timeout);
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