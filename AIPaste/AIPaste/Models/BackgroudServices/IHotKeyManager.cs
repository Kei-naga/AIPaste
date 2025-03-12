using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AIPaste.Models.KeyModels;

namespace AIPaste.Models.BackgroudServices
{
    internal interface IHotKeyManager
    {
        KeyPattern KeyPattern { get; set; }
        bool RegisterHotKey(KeyPattern keyPattern);
    }
}
