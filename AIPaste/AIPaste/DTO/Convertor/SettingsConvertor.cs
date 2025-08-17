using AIPaste.DTO.SettingsDTO;
using AIPaste.Models.SettingsServices.SettingModels;
using System.Collections.Generic;
using System;

namespace AIPaste.DTO.Convertor
{
    public class SettingsConvertor : ISettingsConvertor
    {
        public IAppSettings ConvertToAppSettings(AppSettingsDTO appSettingsDto)
        {
            if (appSettingsDto == null)
            {
                throw new ArgumentNullException(nameof(appSettingsDto), "AppSettingsDTO cannot be null");
            }
            var keySettings = ConvertToKeySettings(appSettingsDto.KeySettings);
            var modelSettingsList = new List<ILlmModelSettings>();
            if (appSettingsDto.LocalModelSettings != null)
            {
                modelSettingsList.Add(ConvertToLlmLocalModelSettings(appSettingsDto.LocalModelSettings));
            }
            if (appSettingsDto.GeminiSettings != null)
            {
                modelSettingsList.Add(ConvertToGeminiModelSettings(appSettingsDto.GeminiSettings));
            }
            var activeLlmModels = ConvertToActiveLlmModels(appSettingsDto.ActiveLlmModels);
            return new AppSettings(
                autoStartSetting: appSettingsDto.AutoStart,
                keySettings: keySettings,
                modelSettingsList: [.. modelSettingsList],
                activeLlmModels: activeLlmModels
            );
        }

        private KeySettings ConvertToKeySettings(KeySettingsDTO keySettingsDto)
        {
            if (keySettingsDto == null)
            {
                throw new ArgumentNullException(nameof(keySettingsDto), "KeySettingsDTO cannot be null");
            }
            return new KeySettings(
                IsHotkeyEnabled: keySettingsDto.IsHotkeyEnabled,
                KeyPattern: new KeyPattern(
                    keySettingsDto.KeyPattern.Modifiers,
                    keySettingsDto.KeyPattern.Key
                )
            );
        }

        private LlmLocalModelSettings ConvertToLlmLocalModelSettings(LocalModelSettingsDTO localModelSettingsDto)
        {
            if (localModelSettingsDto == null)
            {
                throw new ArgumentNullException(nameof(localModelSettingsDto), "LocalModelSettingsDTO cannot be null");
            }
            return new LlmLocalModelSettings(
                localModelSettingsDto.ModelPath,
                localModelSettingsDto.GpuEnabled,
                localModelSettingsDto.GpuLayerCount,
                localModelSettingsDto.MaxContextSize,
                localModelSettingsDto.MaxTokens
            );
        }

        private GeminiModelSettings ConvertToGeminiModelSettings(GeminiSettingsDTO geminiSettingsDto)
        {
            if (geminiSettingsDto == null)
            {
                throw new ArgumentNullException(nameof(geminiSettingsDto), "GeminiSettingsDTO cannot be null");
            }
            return new GeminiModelSettings(
                geminiSettingsDto.ApiKey,
                geminiSettingsDto.ModelName,
                location: geminiSettingsDto.Location,
                maxContextSize: geminiSettingsDto.MaxContextSize
            );
        }

        private ActiveLlmModels ConvertToActiveLlmModels(EnabledModelDTO enabledModelDto)
        {
            if (enabledModelDto == null)
            {
                throw new ArgumentNullException(nameof(enabledModelDto), "EnabledModelDTO cannot be null");
            }
            return new ActiveLlmModels(
                isLocalLlmActive: enabledModelDto.IsLocalLlmActive,
                isGeminiActive: enabledModelDto.IsGeminiActive
            );
        }
    }
    public interface ISettingsConvertor
    {
        public IAppSettings ConvertToAppSettings(AppSettingsDTO appSettingsDto);
    }
}
