using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIPaste.Models.ClipboardOperate
{
    internal interface IClipboardOperator
    {
        Task<string> GetTextAsync();
        static abstract void SetText(string text);
        static abstract void RegisterContentChangedHandler(EventHandler<object> onContentChanged);
        static abstract void UnregisterContentChangedHandler(EventHandler<object> onContentChanged);
    }
}
