using System;

namespace AIPaste.Models.DTO
{
    public interface ILlmModelSettings
    {
        public static ILlmModelSettings GetDefaultSettings()
        {
            // Provide a default implementation or throw a NotImplementedException
            throw new NotImplementedException();
        }
        public bool Equals(ILlmModelSettings otherSettings);
        public string ToString();
        public uint MaxContextSize { get; set; }
    }
}
