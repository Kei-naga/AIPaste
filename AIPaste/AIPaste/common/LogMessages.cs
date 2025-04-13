using System.Collections.Generic;

namespace AIPaste.common
{
    internal class LogMessages
    {
        public Dictionary<string, string> _InfoMessages = new()
        {
            { "START_APP", "Start AIPaste!" },
            { "START_DEBUG", "Start on debug mode" },
            { "SHUTDOWN_APP", "Shutdown AIPaste!" },
            { "CLOSE_WINDOW", "Close Window" },
            { "SHOW_WINDOW", "Show Window" },
            { "HIDE_WINDOW", "Hide Window" },
            { "OPEN_SETTINGS_PAGE", "Open Settings Page" },
            { "SAVE_SETTINGS", "Saving settings" },
            { "SUCCESS_SAVING", "Saving settings is success!" },
            { "ACTIVATE_AUTOSTART", "Auto start activateing..." },
            { "DEACTIVATE_AUTOSTART", "Auto start deactivating..." },
            { "REGISTER_HOTKEY", "Register hotkey" },
            { "UNREGISTER_HOTKEY", "Unregister hotkey" },
            { "LOAD_SETTINGS", "Loading settings" },
            { "RESET_SETTINGS", "Resetting settings" },
        };

        public Dictionary<string, string> _warnMessages = new()
        {
            { "INVALID_LLM_SETINGS", "Invalid llm settings!" },
            { "FAILED_GENERATE_TEXT", "Failed to generate text" },
            { "FAILED_TO_SET_CLIPBOARD_TEXT", "Failed to set text to clipboard" },
            { "FAILED_TO_GET_CLIPBOARD_TEXT", "Failed to get text from clipboard" },
        };

        public Dictionary<string, string> _errorMessages = new()
        {
            { "FAILED_NAVIGATION", "Failed to load Page" },
            { "FAILED_SAVING", "Failed to save settings" },
            { "FAILED_TO_APPLY_HOTKEY", "Failed to apply hotkey settings" },
            { "FAILED_TO_APPLY_AUTOSTART", "Failed to apply auto start settings" },
            { "FAILING_BACK_SETTINGS", "Failing back because saving is failed" },
            { "FAILED_REGISTER_HOTKEY", "Failed to register hotkey" },
            { "FAILED_TO_LOAD_SETTINGS", "Failed to load settings" }
        };
    }
}
