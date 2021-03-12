using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PlaywrightSharp;

namespace webgrep
{
    public interface IFrameWrapper
    {
        List<string> ErrorLog { get; }
        Exception LastError { get; set; }

        IEnumerable<IElementWrapper> AttachedElements(string selector, int? timeout = 5000);
        bool ClickOn(string selector, int? timeout = 5000);
        bool ElementIsAttached(string selector, int? timeout = 5000);
        string GetFormattedText(string selector, int? timeout = 5000);
        string GetText(string selector, int? timeout = 5000);
        bool WaitDomContentLoaded(int? timeout = 5000);
    }

    public class FrameWrapper : IFrameWrapper
    {
        IFrame frame;
        IBrowserInstance instance;

        public List<string> ErrorLog => instance.ErrorLog;

        public Exception LastError
        {
            get => instance.LastError;
            set => instance.LastError = value;
        }

        public FrameWrapper(IFrame frame, IBrowserInstance instance)
        {
            this.frame = frame;
            this.instance = instance;
        }

        public IEnumerable<IElementWrapper> AttachedElements(string selector, int? timeout = 5000)
        {
            try
            {
                IEnumerable<IElementHandle> elements = Task.Run(() =>
                    frame.QuerySelectorAllAsync(selector))
                    .GetAwaiter().GetResult();
                return elements.Select(x => new ElementWrapper(x, this));
            }
            catch (Exception ex)
            {
                ErrorLog.Add(ex.Message);
                LastError = ex;
                return Enumerable.Empty<IElementWrapper>();
            }
        }

        public bool ClickOn(string selector, int? timeout = 5000)
        {
            try
            {
                Task.Run(() => frame.ClickAsync(selector, timeout: timeout))
                    .GetAwaiter().GetResult();
                return true;
            }
            catch (Exception ex)
            {
                ErrorLog.Add(ex.Message);
                LastError = ex;
                return false;
            }
        }

        public bool ElementIsAttached(string selector, int? timeout = 5000)
        {
            try
            {
                Task.Run(() =>
                    frame.WaitForSelectorAsync(selector, WaitForState.Attached, timeout: timeout))
                    .GetAwaiter().GetResult();
                return true;
            }
            catch (Exception ex)
            {
                ErrorLog.Add(ex.Message);
                LastError = ex;
                return false;
            }
        }

        public string GetFormattedText(string selector, int? timeout = 5000)
        {
            try
            {
                string text = Task.Run(() =>
                    frame.GetInnerTextAsync(selector, timeout))
                    .GetAwaiter().GetResult();
                return text.Trim();
            }
            catch (Exception ex)
            {
                ErrorLog.Add(ex.Message);
                LastError = ex;
                return string.Empty;
            }
        }

        public string GetText(string selector, int? timeout = 5000)
        {
            try
            {
                string text = Task.Run(() =>
                    frame.GetTextContentAsync(selector, timeout))
                    .GetAwaiter().GetResult();
                return text.Trim();
            }
            catch (Exception ex)
            {
                ErrorLog.Add(ex.Message);
                LastError = ex;
                return string.Empty;
            }
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
    }
}