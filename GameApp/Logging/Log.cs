using System;
using GameApp.Application;
using GameEngine.Logging;
using GameEngine.Modding;
using GameEngine.Utility;

namespace GameApp.Logging {
    internal class Log : ILog {

        private static Log instance;
        public static Log Instance {
            get => Log.instance;
            private set { if (Log.instance != null) throw new InvalidOperationException("Only one instance per manager type permitted."); else instance = value; }
        }

        private Color messageColor;
        private Color warningColor;
        private Color errorColor;

        public event LogEventHandler OnLog;

        internal Log() {
            Log.Instance = this;

            this.messageColor = AppConstants.Defaults.LOG_DEFAULT_COLOR_MESSAGE;
            this.warningColor = AppConstants.Defaults.LOG_DEFAULT_COLOR_WARNING;
            this.errorColor = AppConstants.Defaults.LOG_DEFAULT_COLOR_ERROR;
        }

        public void WriteLine(string text, LogType messageType = LogType.Message) {
            WriteLine(null, text, messageType);
        }

        public void WriteLine(ModBase mod, string text, LogType messageType = LogType.Message) {
            text = $"{TimeTag()}{ModTag(mod)}: {text}";
            OnLog?.Invoke(text, messageType, LogTypeToColor(messageType));
        }

        public Color MessageColor {
            get => messageColor;
            set => messageColor = value;
        }

        public Color WarningColor {
            get => warningColor;
            set => warningColor = value;
        }

        public Color ErrorColor {
            get => errorColor;
            set => errorColor = value;
        }

        private static string TimeTag() {
            DateTime timestamp = DateTime.Now;
            return $"[{timestamp.Hour.ToString().PadLeft(2, '0')}:{timestamp.Minute.ToString().PadLeft(2, '0')}:{timestamp.Second.ToString().PadLeft(2, '0')}]";
        }

        private static string ModTag(ModBase mod) {
            if (mod == null)
                return "";

            return $"[{mod.ModID}]";
        }

        private Color LogTypeToColor(LogType logType) {
            switch (logType) {
                case LogType.Message:
                    return MessageColor;
                case LogType.Warning:
                    return WarningColor;
                case LogType.Error:
                    return ErrorColor;
                default:
                    throw new ArgumentOutOfRangeException(nameof(logType), logType, null);
            }
        }
    }
}