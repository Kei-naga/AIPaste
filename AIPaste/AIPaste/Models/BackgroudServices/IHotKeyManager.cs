using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AIPaste.Models.DataModels;

namespace AIPaste.Models.BackgroudServices
{
    public interface IHotKeyManager
    {
        KeyPattern KeyPattern { get; set; }
        bool RegisterHotKey(KeyPattern keyPattern, IHotkeyControler? hotkeyControler = null);
        void UnRegisterHotKey();
    }
}
