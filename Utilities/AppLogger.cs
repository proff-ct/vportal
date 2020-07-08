
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
    public static NLog.Logger OperationLogger { get { return NLog.LogManager.GetLogger("OperationLog"); } }

    public static class OperationLog
    {
      public static string OperationName { get { return "operationName"; } }
      public static string OperationData { get { return "operationData"; } }
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
  }
}
