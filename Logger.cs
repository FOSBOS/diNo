using System;
using System.Configuration;
using System.IO;

namespace diNo
{
  public enum LogLevel
  {
    Debug,
    Info,
    Warn,
    Error,
    Fatal
  }

  /// <summary>
  /// Zentrale Logger-Klasse für die gesamte Anwendung. Schreibt alle Meldungen
  /// thread-sicher in eine gemeinsame Log-Datei im Downloads-Verzeichnis des Benutzers.
  /// API ist absichtlich an log4net.ILog angelehnt, damit bestehende Aufrufe (log.Debug(...),
  /// log.Error(msg, ex), ...) unverändert bleiben können.
  /// Das Mindest-Log-Level wird über den appSettings-Schlüssel "LogLevel" in der app.config
  /// gesteuert (Debug/Info/Warn/Error/Fatal); fehlt er oder ist er ungültig, gilt Info.
  /// </summary>
  public sealed class Logger
  {
    public static readonly Logger Instance = new Logger();

    private readonly object syncRoot = new object();
    private readonly string logFilePath;
    private readonly LogLevel minLevel;

    private Logger()
    {
      string downloads = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
      Directory.CreateDirectory(downloads);
      logFilePath = Path.Combine(downloads, "diNo_log.txt");
      minLevel = ErmittleMinLevel();
    }

    private static LogLevel ErmittleMinLevel()
    {
      string konfiguriert = ConfigurationManager.AppSettings["LogLevel"];
      if (!string.IsNullOrWhiteSpace(konfiguriert) && Enum.TryParse(konfiguriert, true, out LogLevel level))
        return level;
      return LogLevel.Info;
    }

    public void Debug(object message) => Write(LogLevel.Debug, message, null);
    public void Debug(object message, Exception exception) => Write(LogLevel.Debug, message, exception);
    public void Info(object message) => Write(LogLevel.Info, message, null);
    public void Info(object message, Exception exception) => Write(LogLevel.Info, message, exception);
    public void Warn(object message) => Write(LogLevel.Warn, message, null);
    public void Warn(object message, Exception exception) => Write(LogLevel.Warn, message, exception);
    public void Error(object message) => Write(LogLevel.Error, message, null);
    public void Error(object message, Exception exception) => Write(LogLevel.Error, message, exception);
    public void Fatal(object message) => Write(LogLevel.Fatal, message, null);
    public void Fatal(object message, Exception exception) => Write(LogLevel.Fatal, message, exception);

    public void DebugFormat(string format, params object[] args) => Write(LogLevel.Debug, string.Format(format, args), null);
    public void InfoFormat(string format, params object[] args) => Write(LogLevel.Info, string.Format(format, args), null);
    public void WarnFormat(string format, params object[] args) => Write(LogLevel.Warn, string.Format(format, args), null);
    public void ErrorFormat(string format, params object[] args) => Write(LogLevel.Error, string.Format(format, args), null);
    public void FatalFormat(string format, params object[] args) => Write(LogLevel.Fatal, string.Format(format, args), null);

    private void Write(LogLevel level, object message, Exception exception)
    {
      if (level < minLevel)
        return;

      string line = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{level.ToString().ToUpperInvariant()}] {message}";
      if (exception != null)
        line += Environment.NewLine + exception;

      lock (syncRoot)
      {
        try
        {
          File.AppendAllText(logFilePath, line + Environment.NewLine);
        }
        catch
        {
          // Logging darf die Anwendung nicht zum Absturz bringen.
        }
      }
    }
  }
}
