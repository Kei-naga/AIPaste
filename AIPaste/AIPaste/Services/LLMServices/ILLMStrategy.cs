using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIPaste.Services.LLMServices
{
    /// <summary>
    /// This class is a strategy pattern for LLM
    /// </summary>
    internal interface ILLMStrategy
    {
        /// <summary>
        /// Create a model prompt
        /// </summary>
        /// <returns>(string Model Request, string Model Answer)</returns>
        (string, string) CreateModelPrompt();
        string CreateOptimizedReq(string targetText, string input);
        string GetSystemPrompt();
    }
}
