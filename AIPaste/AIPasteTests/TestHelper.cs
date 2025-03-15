using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using HuggingfaceHub;
using Microsoft.Testing.Platform.Logging;

namespace AIPasteTests
{
    public static class TestHelper
    {
        /// <summary>
        /// default .env file path. Create .env file in the root of the test project if it does not exist.
        /// </summary>
        private static readonly string DEFAULT_ENV_FILENAME = Path.GetFullPath
                (
                    Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty, @"..\..\..\.env")
                );

        /// <summary>
        /// load enviroment variables from .env file
        /// </summary>
        /// <param name="path"></param>
        public static void Load(string? path = null)
        {
            DotNetEnv.Env.Load(path ?? DEFAULT_ENV_FILENAME);
        }

        public static string GetString([CallerMemberName] string key = "") => DotNetEnv.Env.GetString(key);
        public static int GetInt([CallerMemberName] string key = "") => DotNetEnv.Env.GetInt(key);
        public static bool GetBool([CallerMemberName] string key = "") => DotNetEnv.Env.GetBool(key);
        public static double GetDouble([CallerMemberName] string key = "") => DotNetEnv.Env.GetDouble(key);

        public static string GEMINI_API_KEY => GetString();
    }
}
