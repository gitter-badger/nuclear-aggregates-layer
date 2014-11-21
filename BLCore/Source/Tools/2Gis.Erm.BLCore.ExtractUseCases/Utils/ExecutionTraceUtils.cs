using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace DoubleGis.Erm.BLCore.ExtractUseCases.Utils
{
    public static class ExecutionTraceUtils
    {
        private static readonly IDictionary<string, string> LogFullPathsMap = new Dictionary<string, string>();

        public static void Log(string logPath, string logName, string logEntry)
        {
            string fullPath;
            if (!LogFullPathsMap.TryGetValue(logName, out fullPath))
            {
                fullPath = Path.Combine(logPath, logName + ".log");
                LogFullPathsMap.Add(logName, fullPath);

                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                }
            }
            
            using (var wr = new StreamWriter(fullPath, true))
            {
                wr.WriteLine(logEntry);
            }
        }

        public static void ThrowExceptionIfTrue<T>(this T value, Predicate<T> predicate)
        {
            if (predicate(value))
            {
                throw new Exception("Specified predicate is true");
            }   
        }

        public static void DebuggerBreakIfTrue<T>(this T value, Predicate<T> predicate)
        {
            if (predicate(value))
            {
                Debugger.Break();
            }
        }

        //ExecutionTraceUtils.Log("d:\\", GetType().Name, checkingType.Name);
        //checkingType.Name.DebuggerBreakIfTrue(s => s.Contains("HierarchyElement"));
    }
}
