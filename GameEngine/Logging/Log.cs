using GameEngine.Modding;
using GameEngine.Utility;

namespace GameEngine.Logging {
    public static class Log {
        public static event LogEventHandler OnLog;

        static Log() {  // TODO make sure this isnt called before the ModBase is initialized
            ModBase.Log.OnLog += (text, logType, color) => {
                OnLog?.Invoke(text, logType, color);
            };
        }

        public static void WriteLine(string text, LogType messageType = LogType.Message) => ModBase.Log.WriteLine(ModBase.Instance, text, messageType);

        public static Color MessageColor {
            get => ModBase.Log.MessageColor;
            set => ModBase.Log.MessageColor = value;
        }

        public static Color WarningColor {
            get => ModBase.Log.WarningColor;
            set => ModBase.Log.WarningColor = value;
        }

        public static Color ErrorColor {
            get => ModBase.Log.ErrorColor;
            set => ModBase.Log.ErrorColor = value;
        }
    }
}