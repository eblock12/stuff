using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Tank.Common
{
    /// <summary>
    /// Provides logging facilities for the game engine.
    /// </summary>
    public class Logger : Singleton<Logger>, ITask
    {
        private TextWriter infoWriter = Console.Out;
        private TextWriter errorWriter = Console.Error;

        #region ITask Members

        public void OnRun()
        {
        }

        public void OnStart()
        {
        }

        public void OnStop()
        {
        }

        #endregion

        /// <summary>
        /// Outputs informational text to the log.
        /// </summary>
        /// <param name="source">The name of log message source.</param>
        /// <param name="text">The text of the log message.</param>
        public void LogInformation(string source, string text)
        {
            infoWriter.WriteLine(String.Format(Resources.LogInformationFormat, source, text));
        }

        /// <summary>
        /// Outputs information text to the log using the specified string format and arguments.
        /// </summary>
        /// <param name="source">The name of log message source.</param>
        /// <param name="textFormat">The string format of the log message.</param>
        /// <param name="args"></param>
        public void LogInformation(string source, string textFormat, params object[] args)
        {
            LogInformation(source, String.Format(textFormat, args));
        }

        /// <summary>
        /// Outputs information about an unhandled exception to the log.
        /// </summary>
        /// <param name="ex">The unhandled exception</param>
        public void LogUnhandledException(Exception ex)
        {
            errorWriter.WriteLine(Resources.UnhandledExceptionMessage1);
            errorWriter.WriteLine(Resources.UnhandledExceptionMessage2);
            errorWriter.WriteLine();
            errorWriter.WriteLine(ex);
        }

        /// <summary>
        /// Outputs an error and its exception to the log
        /// </summary>
        /// <param name="error">The error message</param>
        /// <param name="ex">The exception associated with this error</param>
        public void LogError(String error, Exception ex)
        {
            errorWriter.WriteLine(error);
            errorWriter.WriteLine();
            errorWriter.WriteLine(ex);
        }
    }
}
