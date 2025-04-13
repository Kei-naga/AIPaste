using System;
using System.Threading.Tasks;
using AIPaste.common;
using Windows.ApplicationModel.DataTransfer;

namespace AIPaste.Models.ClipboardOperate
{
    public class ClipboardOperator : IClipboardOperator
    {
        private IMyLogger _logger;
        private IClipboardAccess _clipboard;

        public ClipboardOperator(IClipboardAccess? clipboard = null, IMyLogger? logger = null)
        {
            _logger = logger ?? MyLogger.GetInstance();
            _clipboard = clipboard ?? new SystemClipboard();
            _logger.Trace("ClipboardOperator created");
        }

        /// <summary>
        /// get text from clipboard asynchronously
        /// </summary>
        public async Task<string> GetTextAsync()
        {
            try
            {
                var content = _clipboard.GetContent();
                if (content.Contains(StandardDataFormats.Text))
                {
                    var text = await content.GetTextAsync();
                    _logger.Debug($"get clipboard text: {text}");
                    return text;
                }
                else
                {
                    return "";
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to retrieve text from the clipboard.", ex);
            }
        }

        /// <summary>
        /// set text from clipboard asynchronously
        /// </summary>
        public void SetText(string text)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(text))
                {
                    text = "";
                }

                var dataPackage = new DataPackage();
                dataPackage.SetText(text);
                _clipboard.SetContent(dataPackage);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to set text to the clipboard.", ex);
            }
        }

        /// <summary>
        /// register clipboard content changed handler
        /// </summary>
        /// <param name="onContentChanged">A delegate that will be called on change</param>
        public void RegisterContentChangedHandler(EventHandler<object> onContentChanged)
        {
            _clipboard.ContentChanged += onContentChanged;
        }

        /// <summary>
        /// unregister clipboard content changed handler
        /// </summary>
        /// <param name="onContentChanged">a delegate  </param>
        public void UnregisterContentChangedHandler(EventHandler<object> onContentChanged)
        {
            _clipboard.ContentChanged -= onContentChanged;
        }
    }
}