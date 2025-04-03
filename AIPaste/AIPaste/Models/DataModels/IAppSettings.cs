using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIPaste.Models.DataModels
{
    public interface IAppSettings
    {
        bool AutoStart { get; set; }
        ModelType ModelType { get; set; }
        IKeySettings KeySettings { get; set; }
        ILLMModelSettings[] ModelSettingsList { get; set; }
        string ToString();
        bool Equals(IAppSettings otherSettings);
    }
}
