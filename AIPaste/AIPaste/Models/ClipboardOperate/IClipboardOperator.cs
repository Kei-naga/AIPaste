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
        void SetText(string text);
        void RegisterContentChangedHandler(EventHandler<object> onContentChanged);
        void UnregisterContentChangedHandler(EventHandler<object> onContentChanged);
    }
}
