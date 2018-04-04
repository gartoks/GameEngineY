using GameEngine.Modding;
using GameEngine.Utility;

namespace GameEngine.Logging {
    public delegate void LogEventHandler(string text, LogType logType, Color color);

    public interface ILog {

        event LogEventHandler OnLog;

        void WriteLine(string text, LogType messageType = LogType.Message);

        void WriteLine(ModBase mod, string text, LogType messageType = LogType.Message);

        Color MessageColor { get; set; }

        Color WarningColor { get; set; }

        Color ErrorColor { get; set; }
    }
}