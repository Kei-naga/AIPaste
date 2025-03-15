using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;

namespace AIPaste.Models.ClipboardOperate
{
    public interface IClipboardAccess
    {
        DataPackageView GetContent();
        void SetContent(DataPackage dataPackage);
        event EventHandler<object> ContentChanged;
    }
}
