using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PlaywrightSharp;

namespace webgrep
{
    public interface IElementWrapper
    {
        IEnumerable<IElementWrapper> AttachedElements(string selector);
        bool ClickOn(int? timeout = 5000);
        bool ElementIsAttached(string selector, int? timeout = 5000);
        string GetFormattedText(int? timeout = 5000);
        string GetText(int? timeout = 5000);
    }

    public class ElementWrapper : IElementWrapper
    {
        IElementHandle element;
        IFrameWrapper frame;
        
        public ElementWrapper(IElementHandle element, IFrameWrapper frame)
        {
            this.element = element;
            this.frame = frame;
        }

        public IEnumerable<IElementWrapper> AttachedElements(string selector)
        {
            try
            {
                IEnumerable<IElementHandle> elements = Task.Run(() =>
                    element.QuerySelectorAllAsync(selector))
                    .GetAwaiter().GetResult();
                return elements.Select(x => new ElementWrapper(x, frame));
            }
            catch (Exception ex)
            {
                frame.ErrorLog.Add(ex.Message);
                frame.LastError = ex;
                return Enumerable.Empty<IElementWrapper>();
            }
        }

        public bool ClickOn(int? timeout = 5000)
        {
            try
            {
                Task.Run(() => element.ClickAsync(timeout: timeout))
                    .GetAwaiter().GetResult();
                return true;
            }
            catch (Exception ex)
            {
                frame.ErrorLog.Add(ex.Message);
                frame.LastError = ex;
                return false;
            }
        }

        public bool ElementIsAttached(string selector, int? timeout = 5000)
        {
            try
            {
                Task.Run(() =>
                    element.WaitForSelectorAsync(selector, WaitForState.Attached, timeout: timeout))
                    .GetAwaiter().GetResult();
                return true;
            }
            catch (Exception ex)
            {
                frame.ErrorLog.Add(ex.Message);
                frame.LastError = ex;
                return false;
            }
        }

        public string GetFormattedText(int? timeout = 5000)
        {
            try
            {
                string text = Task.Run(() =>
                    element.GetInnerTextAsync(timeout))
                    .GetAwaiter().GetResult();
                return text.Trim();
            }
            catch (Exception ex)
            {
                frame.ErrorLog.Add(ex.Message);
                frame.LastError = ex;
                return string.Empty;
            }
        }

        public string GetText(int? timeout = 5000)
        {
            try
            {
                string text = Task.Run(() =>
                    element.GetTextContentAsync(timeout))
                    .GetAwaiter().GetResult();
                return text.Trim();
            }
            catch (Exception ex)
            {
                frame.ErrorLog.Add(ex.Message);
                frame.LastError = ex;
                return string.Empty;
            }
        }
    }
}