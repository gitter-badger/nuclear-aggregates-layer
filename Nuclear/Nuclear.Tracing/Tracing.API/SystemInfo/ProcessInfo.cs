using System;
using System.Diagnostics;
using System.Text;

namespace DoubleGis.Erm.Platform.Common.Logging.SystemInfo
{
    /// <summary>
    /// Класс сбора информации о данном процессе
    /// </summary>
    public static class ProcessInfo
    {
        /// <summary>
        /// Возвращает описание данного процесса - головной модуль процесса + модули загруженные в процесс
        /// </summary>
        public static String CallingProcessDescription
        {
            get 
            {
                var sb = new StringBuilder();

                try
                {
                    var callingProcess = Process.GetCurrentProcess();
                    
                    var mainModule = callingProcess.MainModule;
                    var mainWindowTitle = callingProcess.MainWindowHandle != IntPtr.Zero
                                              ? callingProcess.MainWindowTitle
                                              : null;

                    sb.AppendLine("Process: " + mainModule.ModuleName + "\n\tlocation: " + mainModule.FileName + (mainWindowTitle != null?("\n\ttitle:" + mainWindowTitle):""));
                    var modules = callingProcess.Modules;
                    if (modules.Count > 0)
                    {
                        sb.AppendLine("Process loaded modules:");
                        foreach (ProcessModule module in callingProcess.Modules)
                        {
                            var name = mainModule.ModuleName;
                            var file = mainModule.FileName;
                            var version = module.FileVersionInfo;

                            sb.AppendLine("Module: " +
                                          (version != null ? version.ToString() : (name + "\n\tlocation: " + file)));
                        }
                    }
                }
                catch (Exception)
                {
                    sb.AppendLine("Can't get info about calling process");
                }

                return sb.ToString();
            }
        }
    }
}
