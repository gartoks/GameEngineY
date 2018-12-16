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

        public Color MessageColor { get; set; }
        public Color WarningColor { get; set; }
        public Color ErrorColor { get; set; }
        public Color DebugColor { get; set; }


        public event LogEventHandler OnLog;

        internal Log() {
            Log.Instance = this;

            this.MessageColor = AppConstants.Defaults.LOG_DEFAULT_COLOR_MESSAGE;
            this.WarningColor = AppConstants.Defaults.LOG_DEFAULT_COLOR_WARNING;
            this.ErrorColor = AppConstants.Defaults.LOG_DEFAULT_COLOR_ERROR;
            this.DebugColor = AppConstants.Defaults.LOG_DEFAULT_COLOR_DEBUG;
        }

        public void WriteLine(string text, LogType messageType = LogType.Message) {
            WriteLine(null, text, messageType);
        }

        public void WriteLine(Exception exception) {
            WriteLine($"EXCEPTION: {exception.Message}", LogType.Error);
        }

        public void WriteLine(ModBase mod, string text, LogType messageType = LogType.Message) {
            text = $"{TimeTag()}{ModTag(mod)}: {text}";
            OnLog?.Invoke(text, messageType, LogTypeToColor(messageType));
        }

        public void WriteLine(ModBase mod, Exception exception) {
            WriteLine(mod, $"EXCEPTION: {exception.Message}", LogType.Error);
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
                case LogType.Debug:
                    return DebugColor;
                default:
                    throw new ArgumentOutOfRangeException(nameof(logType), logType, null);
            }
        }
    }
}