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
        IKeyPattern KeyPattern { get; set; }
        bool RegisterHotKey(IKeyPattern keyPattern);
        void UnRegisterHotKey();
    }
}
