namespace AIPaste.DTO.SettingsDTO
{
    public class GeminiSettingsDTO(string apiKey, string modelName = "gemini-2.0-flash", string location = "", uint maxContextSize = 1048576)
    {
        public string ApiKey { get; } = apiKey;
        public string ModelName { get; } = modelName;
        public string Location { get; } = location;
        public uint MaxContextSize { get; } = maxContextSize;
    }
}
