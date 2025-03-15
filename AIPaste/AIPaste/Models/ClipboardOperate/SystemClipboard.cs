using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;

namespace AIPaste.Models.ClipboardOperate
{
    internal class SystemClipboard : IClipboardAccess
    {
        public DataPackageView GetContent() => Clipboard.GetContent();

        public void SetContent(DataPackage dataPackage) => Clipboard.SetContent(dataPackage);

        public event EventHandler<object> ContentChanged {
            add => Clipboard.ContentChanged += value;
            remove => Clipboard.ContentChanged -= value; 
        }
    }
}
