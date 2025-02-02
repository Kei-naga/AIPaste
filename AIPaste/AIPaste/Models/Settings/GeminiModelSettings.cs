using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Content;

namespace AIPaste.Models.Settings
{
    class GeminiModelSettings : ILLMModelSettings
    {
        public string ApiKey { get; } = "";
        public string ModelName { get; } = "";
        public string Location { get; } = "";

        public static ILLMModelSettings GetDefaultSettings()
        {
            return new GeminiModelSettings();
        }

        public bool Equals(ILLMModelSettings otherSettings)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return "";
        }
    }
}
