using System.Globalization;

namespace AIPaste.Models.SettingsServices.SettingModels
{
    public class GeminiModelSettings(string apiKey, string modelName = "gemini-2.5-flash", string location = "", uint maxContextSize = 1048576) : ILlmModelSettings
    {
        public const string DefaultModelName = "gemini-2.5-flash";

        public string ApiKey { get; set; } = apiKey;
        public string ModelName { get; set; } = NormalizeModelName(modelName);
        public string Location { get; set; } = string.IsNullOrEmpty(location) ? CultureInfo.CurrentCulture.TwoLetterISOLanguageName : location;
        public uint MaxContextSize { get; set; } = maxContextSize;

        public static ILlmModelSettings GetDefaultSettings()
        {
            return new GeminiModelSettings("", DefaultModelName, CultureInfo.CurrentCulture.TwoLetterISOLanguageName);
        }

        public static string NormalizeModelName(string modelName)
        {
            return modelName switch
            {
                "gemini-2.0-flash" => DefaultModelName,
                "gemini-2.0-flash-001" => DefaultModelName,
                _ => string.IsNullOrWhiteSpace(modelName) ? DefaultModelName : modelName,
            };
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
