using System;
using QLog.Models;
using QLog.Helpers;
using QLog.Areas.Base;
using QLog.Areas.Default;
using QLog.Components;

namespace QLog
{
    public class CustomLogInfo
    {
        public string Message { get; set; }
        public string DocumentId { get; set; }
        public string DocumentName { get; set; }
        public string User { get; set; }
    }

    public partial class Logger
    {
        /// <summary>
        /// Writes a log at the QTrace area with a specified message
        /// </summary>
        /// <param name="msg"></param>
        public static void LogTrace(CustomLogInfo domainLogInfo)
        {
            DoLog<QTrace>(domainLogInfo, null);
        }

        /// <summary>
        /// Writes a log at the QTrace area with a specified message (applying String.Format() first)
        /// </summary>
        /// <param name="domainLogInfo"></param>
        /// <param name="args"></param>
        public static void LogTrace(CustomLogInfo domainLogInfo, params object[] args)
        {
            DoLog<QTrace>(domainLogInfo, null, args);
        }

        /// <summary>
        /// Writes a log at the QTrace area with a specified message and exception
        /// </summary>
        /// <param name="domainLogInfo"></param>
        /// <param name="e"></param>
        public static void LogTrace(CustomLogInfo domainLogInfo, Exception e)
        {
            DoLog<QTrace>(domainLogInfo, e);
        }

        /// <summary>
        /// Writes a log at the QDebug area with a specified message
        /// </summary>
        /// <param name="domainLogInfo"></param>
        public static void LogDebug(CustomLogInfo domainLogInfo)
        {
            DoLog<QDebug>(domainLogInfo, null);
        }

        /// <summary>
        /// Writes a log at the QDebug area with a specified message (applying String.Format() first)
        /// </summary>
        /// <param name="domainLogInfo"></param>
        /// <param name="args"></param>
        public static void LogDebug(CustomLogInfo domainLogInfo, params object[] args)
        {
            DoLog<QDebug>(domainLogInfo, null, args);
        }

        /// <summary>
        /// Writes a log at the QDebug area with a specified message and exception
        /// </summary>
        /// <param name="domainLogInfo"></param>
        /// <param name="e"></param>
        public static void LogDebug(CustomLogInfo domainLogInfo, Exception e)
        {
            DoLog<QDebug>(domainLogInfo, e);
        }

        /// <summary>
        /// Writes a log at the QInfo area with a specified message
        /// </summary>
        /// <param name="domainLogInfo"></param>
        public static void LogInfo(CustomLogInfo domainLogInfo)
        {
            DoLog<QInfo>(domainLogInfo, null);
        }

        /// <summary>
        /// Writes a log at the QInfo area with a specified message (applying String.Format() first)
        /// </summary>
        /// <param name="domainLogInfo"></param>
        /// <param name="args"></param>
        public static void LogInfo(CustomLogInfo domainLogInfo, params object[] args)
        {
            DoLog<QInfo>(domainLogInfo, null, args);
        }

        /// <summary>
        /// Writes a log at the QInfo area with a specified message and exception
        /// </summary>
        /// <param name="domainLogInfo"></param>
        /// <param name="e"></param>
        public static void LogInfo(CustomLogInfo domainLogInfo, Exception e)
        {
            DoLog<QInfo>(domainLogInfo, e);
        }

        /// <summary>
        /// Writes a log at the QWarn area with a specified message
        /// </summary>
        /// <param name="domainLogInfo"></param>
        public static void LogWarn(CustomLogInfo domainLogInfo)
        {
            DoLog<QWarn>(domainLogInfo, null);
        }

        /// <summary>
        /// Writes a log at the QWarn area with a specified message (applying String.Format() first)
        /// </summary>
        /// <param name="domainLogInfo"></param>
        /// <param name="args"></param>
        public static void LogWarn(CustomLogInfo domainLogInfo, params object[] args)
        {
            DoLog<QWarn>(domainLogInfo, null, args);
        }

        /// <summary>
        /// Writes a log at the QWarn area with a specified message and exception
        /// </summary>
        /// <param name="domainLogInfo"></param>
        /// <param name="e"></param>
        public static void LogWarn(CustomLogInfo domainLogInfo, Exception e)
        {
            DoLog<QWarn>(domainLogInfo, e);
        }

        /// <summary>
        /// Writes a log at the QError area with a specified message
        /// </summary>
        /// <param name="domainLogInfo"></param>
        public static void LogError(CustomLogInfo domainLogInfo)
        {
            DoLog<QError>(domainLogInfo, null);
        }

        /// <summary>
        /// Writes a log at the QError area with a specified message (applying String.Format() first)
        /// </summary>
        /// <param name="domainLogInfo"></param>
        /// <param name="args"></param>
        public static void LogError(CustomLogInfo domainLogInfo, params object[] args)
        {
            DoLog<QError>(domainLogInfo, null, args);
        }

        /// <summary>
        /// Writes a log at the QError area with a specified message and exception
        /// </summary>
        /// <param name="domainLogInfo"></param>
        /// <param name="e"></param>
        public static void LogError(CustomLogInfo domainLogInfo, Exception e)
        {
            DoLog<QError>(domainLogInfo, e);
        }

        /// <summary>
        /// Writes a log at the QCritical area with a specified message
        /// </summary>
        /// <param name="domainLogInfo"></param>
        public static void LogCritical(CustomLogInfo domainLogInfo)
        {
            DoLog<QCritical>(domainLogInfo, null);
        }

        /// <summary>
        /// Writes a log at the QCritical area with a specified message (applying String.Format() first)
        /// </summary>
        /// <param name="domainLogInfo"></param>
        /// <param name="args"></param>
        public static void LogCritical(CustomLogInfo domainLogInfo, params object[] args)
        {
            DoLog<QCritical>(domainLogInfo, null, args);
        }

        /// <summary>
        /// Writes a log at the QCritical area with a specified message and exception
        /// </summary>
        /// <param name="domainLogInfo"></param>
        /// <param name="e"></param>
        public static void LogCritical(CustomLogInfo domainLogInfo, Exception e)
        {
            DoLog<QCritical>(domainLogInfo, e);
        }

        /// <summary>
        /// Writes a log at the specified QArea with a specified message
        /// </summary>
        /// <typeparam name="Area"></typeparam>
        /// <param name="domainLogInfo"></param>
        public static void Log<Area>(CustomLogInfo domainLogInfo) where Area : QArea
        {
            DoLog<Area>(domainLogInfo, null);
        }

        /// <summary>
        /// Writes a log at the specified QArea with a specified message (applying String.Format() first)
        /// </summary>
        /// <typeparam name="Area"></typeparam>
        /// <param name="domainLogInfo"></param>
        /// <param name="args"></param>
        public static void Log<Area>(CustomLogInfo domainLogInfo, params object[] args) where Area : QArea
        {
            DoLog<Area>(domainLogInfo, null, args);
        }

        /// <summary>
        /// Writes a log at the specified QArea with a specified message and exception
        /// </summary>
        /// <typeparam name="Area"></typeparam>
        /// <param name="domainLogInfo"></param>
        /// <param name="e"></param>
        public static void Log<Area>(CustomLogInfo domainLogInfo, Exception e) where Area : QArea
        {
            DoLog<Area>(domainLogInfo, e);
        }

        /// <summary>
        /// Creates and saves a log message at the specified QArea. Depending on the settings the message is being saved to buffer 
        /// that will be flushed asynchronously later, or directly to the database via IRepository.
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="exception"></param>
        /// <param name="args"></param>
        private static void DoLog<Area>(CustomLogInfo domainLogInfo, Exception exception, params object[] args) where Area : QArea
        {
            var msg = domainLogInfo.Message;
            try
            {
                if (ComponentsService.Config.IsValidLogArea(typeof(Area)))
                {
                    msg = MessageHelper.GetMessage(msg, exception, args);
                    Tuple<string, string> ClassMethod = GetCallingClassAndMethod();
                    QLogEntry log = ComponentsService.Environment.GetLog(typeof(Area), msg, ClassMethod.Item1, ClassMethod.Item2);
                    SetCustomLogInfo(log, domainLogInfo);
                    if (ComponentsService.Config.IsAsyncLogEnabled(typeof(Area)))
                    {
                        bool isBufferFull = false;
                        ComponentsService.Buffer.Enqueue(log, out isBufferFull);
                        if (isBufferFull)
                            Flush();
                    }
                    else
                        ComponentsService.Repository.Save(log);
                }
            }
            catch (Exception e)
            {
                ComponentsService.SilentModeHandle(e);
            }
        }

        private static void SetCustomLogInfo(QLogEntry log, CustomLogInfo domainLogInfo)
        {
            log.DocumentId = domainLogInfo.DocumentId;
            log.DocumentName = domainLogInfo.DocumentName;
            log.User = domainLogInfo.User;
        }

    }
}
