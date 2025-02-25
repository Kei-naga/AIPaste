using System;

namespace AIPaste.Models.Settings
{
    interface ILLMModelSettings
    {
        public static ILLMModelSettings GetDefaultSettings()
        {
            // Provide a default implementation or throw a NotImplementedException
            throw new NotImplementedException();
        }
        public bool Equals(ILLMModelSettings otherSettings);
        public string ToString();
    }
}
