
using Newtonsoft.Json;
using NLog.Fluent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utilities
{
    public static class AppLogger
    {
        private static NLog.Logger IncidentLogger => NLog.LogManager.GetLogger("IncidentLog");
        private static NLog.Logger DevLogger => NLog.LogManager.GetLogger("DevLog");
        public static NLog.Logger OperationLogger => NLog.LogManager.GetLogger("OperationLog");
        

        public static class OperationLog
        {
            public static string OperationName => "operationName";
            public static string OperationData => "operationData";
        }

        public static void LogOperationException(string operationName, string errDescription, dynamic operationData, Exception ex)
        {
            OperationLogger.Error()
                .Exception(ex)
                .Message(errDescription)
                .Property(OperationLog.OperationName, operationName)
                .Property(
                        OperationLog.OperationData,
                        JsonConvert.SerializeObject(new { OperationData = operationData }, Formatting.Indented))
                .Write();
        }

        public static void LogEvent(string executingOperation, string eventDescription, dynamic eventData)
        {
            IncidentLogger.Info()
                .Message(eventDescription)
                .Property(OperationLog.OperationName, executingOperation)
                .Property(
                    OperationLog.OperationData,
                    JsonConvert.SerializeObject(new { eventData }, Formatting.Indented))
                .Write();
        }
        public static void LogDevNotes(string executingOperation, string eventDescription, dynamic eventData)
        {
            DevLogger.Debug()
                .Message(eventDescription)
                .Property(OperationLog.OperationName, executingOperation)
                .Property(
                    OperationLog.OperationData,
                    JsonConvert.SerializeObject(new { eventData }, Formatting.Indented))
                .Write();
        }

    }
}
