using System;
using System.Collections.Generic;
using System.Linq;
using PlaywrightSharp;

namespace webgrep
{
    public interface IFrameWrapper
    {
        bool ClickOn(string selector, int? timeout = 5000);
        bool ElementIsAttached(string selector, int? timeout = 5000);
        string GetFormattedText(string selector, int? timeout = 5000);
        List<string> GetFormattedTextAll(string selector, int? timeout = 5000);
        string GetText(string selector, int? timeout = 5000);
        List<string> GetTextAll(string selector, int? timeout = 5000);
        bool WaitDomContentLoaded(int? timeout = 5000);
    }

    public class FrameWrapper : IFrameWrapper
    {
        IFrame frame;
        IBrowserInstance instance;
        
        public FrameWrapper(IFrame frame, IBrowserInstance instance)
        {
            this.frame = frame;
            this.instance = instance;
        }

        public bool WaitDomContentLoaded(int? timeout = 5000)
        {
            try
            {
                frame.WaitForNavigationAsync(LifecycleEvent.DOMContentLoaded, timeout: timeout).GetAwaiter().GetResult();
                return true;
            }
            catch (Exception ex)
            {
                instance.ErrorLog.Add(ex.Message);
                instance.LastError = ex;
                return false;
            }
        }

        public bool ClickOn(string selector, int? timeout = 5000)
        {
            try
            {
                frame.ClickAsync(selector, timeout: timeout).GetAwaiter().GetResult();
                return true;
            }
            catch (Exception ex)
            {
                instance.ErrorLog.Add(ex.Message);
                instance.LastError = ex;
                return false;
            }
        }
        
        public bool ElementIsAttached(string selector, int? timeout = 5000)
        {
            try
            {
                frame.WaitForSelectorAsync(selector, WaitForState.Attached, timeout: timeout).GetAwaiter().GetResult();
                return true;
            }
            catch (Exception ex)
            {
                instance.ErrorLog.Add(ex.Message);
                instance.LastError = ex;
                return false;
            }
        }

        public string GetText(string selector, int? timeout = 5000)
        {
            try
            {
                string text = frame.GetTextContentAsync(selector, timeout).GetAwaiter().GetResult();
                return text.Trim();
            }
            catch (Exception ex)
            {
                instance.ErrorLog.Add(ex.Message);
                instance.LastError = ex;
                return string.Empty;
            }
        }

        public List<string> GetTextAll(string selector, int? timeout = 5000)
        {
            List<string> list = new List<string>();
            try
            {
                List<IElementHandle> elements = frame.QuerySelectorAllAsync(selector).GetAwaiter().GetResult().ToList();
                foreach (var elem in elements)
                {
                    string text = elem.GetTextContentAsync(timeout).GetAwaiter().GetResult();
                    list.Add(text.Trim());
                }
            }
            catch (Exception ex)
            {
                instance.ErrorLog.Add(ex.Message);
                instance.LastError = ex;
            }
            return list;
        }

        public string GetFormattedText(string selector, int? timeout = 5000)
        {
            try
            {
                string text = frame.GetInnerTextAsync(selector, timeout).GetAwaiter().GetResult();
                return text.Trim();
            }
            catch (Exception ex)
            {
                instance.ErrorLog.Add(ex.Message);
                instance.LastError = ex;
                return string.Empty;
            }
        }

        public List<string> GetFormattedTextAll(string selector, int? timeout = 5000)
        {
            List<string> list = new List<string>();
            try
            {
                List<IElementHandle> elements = frame.QuerySelectorAllAsync(selector).GetAwaiter().GetResult().ToList();
                foreach (var elem in elements)
                {
                    string text = elem.GetInnerTextAsync(timeout).GetAwaiter().GetResult();
                    list.Add(text.Trim());
                }
            }
            catch (Exception ex)
            {
                instance.ErrorLog.Add(ex.Message);
                instance.LastError = ex;
            }
            return list;
        }
    }
}