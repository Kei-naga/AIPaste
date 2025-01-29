using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace AIPaste.Models.Settings
{
    internal struct KeySettings(string KeyPattern)
    {
        public string KeyPattern = KeyPattern;

        public static KeySettings GetDefaultSettings()
        {
            return new KeySettings(
                KeyPattern: "Ctrl+Shift+V"
            );
        }

        public override string ToString()
        {
            return KeyPattern;
        }
    }
}
