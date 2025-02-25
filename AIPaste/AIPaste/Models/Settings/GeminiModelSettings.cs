using System.Globalization;

namespace AIPaste.Models.Settings
{
    class GeminiModelSettings(string apiKey, string modelName = "gemini-1.5-flash", string location = "") : ILLMModelSettings
    {
        public string ApiKey { get; set; } = apiKey;
        public string ModelName { get; set; } = modelName;
        public string Location { get; set; } = string.IsNullOrEmpty(location) ? CultureInfo.CurrentCulture.TwoLetterISOLanguageName : location;

        public static ILLMModelSettings GetDefaultSettings()
        {
            return new GeminiModelSettings("", "gemini-1.5-flash", CultureInfo.CurrentCulture.TwoLetterISOLanguageName);
        }

        public bool Equals(ILLMModelSettings otherSettings)
        {
            if (otherSettings is not GeminiModelSettings)
            {
                return false;
            }
            var geminiOtherSetitngs = (GeminiModelSettings)otherSettings;
            return ApiKey == geminiOtherSetitngs.ApiKey &&
                ModelName == geminiOtherSetitngs.ModelName &&
                Location == geminiOtherSetitngs.Location;
        }

        public override string ToString()
        {
            return $"ApiKey: {ApiKey}, Model Name: {ModelName}, Location: {Location}";
        }
    }
}
