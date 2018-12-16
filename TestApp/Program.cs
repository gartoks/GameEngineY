using System.Diagnostics;
using GameApp.Application;
using GameEngine.Logging;
using GameEngine.Utility;
using GameEngine.Utility.Extensions;

namespace TestApp {
    public class Program {
        public static void Main(string[] args) {
            ApplicationInitializationSettings initializationSettings = new ApplicationInitializationSettings("modID_testGame", "TestGame", new Version("", 0, 1), "en-US");

            Application application = new Application(initializationSettings);
            Log.OnLog += (text, type, color) => Debug.WriteLine($"{(type == LogType.Message ? "" : "[" + type.ToString().PadBoth(7) + "]")}" + $"{text}");
            application.Launch();

        }
    }
}