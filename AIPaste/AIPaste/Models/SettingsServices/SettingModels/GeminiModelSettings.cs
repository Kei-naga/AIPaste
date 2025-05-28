using System.Globalization;

namespace AIPaste.Models.SettingsServices.SettingModels
{
    public class GeminiModelSettings(string apiKey, string modelName = "gemini-2.0-flash", string location = "", uint maxContextSize = 1048576) : ILlmModelSettings
    {
        public string ApiKey { get; set; } = apiKey;
        public string ModelName { get; set; } = modelName;
        public string Location { get; set; } = string.IsNullOrEmpty(location) ? CultureInfo.CurrentCulture.TwoLetterISOLanguageName : location;
        public uint MaxContextSize { get; set; } = maxContextSize;

        public static ILlmModelSettings GetDefaultSettings()
        {
            return new GeminiModelSettings("", "gemini-2.0-flash", CultureInfo.CurrentCulture.TwoLetterISOLanguageName);
        }

        public bool Equals(ILlmModelSettings otherSettings)
        {
            if (otherSettings is not GeminiModelSettings)
            {
                return false;
            }
            var geminiOtherSetitngs = (GeminiModelSettings)otherSettings;
            return ApiKey == geminiOtherSetitngs.ApiKey &&
                ModelName == geminiOtherSetitngs.ModelName &&
                Location == geminiOtherSetitngs.Location &&
                MaxContextSize == geminiOtherSetitngs.MaxContextSize;
        }

        public override string ToString()
        {
            return $"ApiKey: {ApiKey}, Model Name: {ModelName}, Location: {Location}, MaxContextSize: {MaxContextSize}";
        }
    }
}
