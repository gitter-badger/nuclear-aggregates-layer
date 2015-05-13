using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

using DoubleGis.Erm.BLCore.ExtractUseCases.Processors;
using DoubleGis.Erm.BLCore.ExtractUseCases.Processors.ExtractUseCases;
using DoubleGis.Erm.BLCore.ExtractUseCases.Processors.FindAllEntities;
using DoubleGis.Erm.BLCore.ExtractUseCases.Processors.FindAllHandlers;
using DoubleGis.Erm.BLCore.ExtractUseCases.Processors.FindAllRequests;
using DoubleGis.Erm.BLCore.ExtractUseCases.Processors.FindAllSubRequestCalls;
using DoubleGis.Erm.BLCore.ExtractUseCases.Processors.FindEditableEntityControllers;
using DoubleGis.Erm.BLCore.ExtractUseCases.Processors.FindPublicServiceUseCases;
using DoubleGis.Erm.BLCore.ExtractUseCases.Processors.GenerateUseCases;
using DoubleGis.Erm.BLCore.ExtractUseCases.Utils;

using Microsoft.Win32;

using NuClear.Storage.UseCases;

using Roslyn.Services;

namespace DoubleGis.Erm.BLCore.ExtractUseCases
{
    internal class Program
    {
        private static string ErmSolutionFullPath
        {
            get
            {
                string ermWorkspaceBasePath = null;
                var localPath = Path.GetDirectoryName(Assembly.GetCallingAssembly().GetModules()[0].FullyQualifiedName);
                if (!string.IsNullOrEmpty(localPath))
                {
                    string ermWorkspaceNameTemplate = Path.DirectorySeparatorChar + "ERM" + Path.DirectorySeparatorChar;
                    var indexOfErmWorkspace = localPath.LastIndexOf(ermWorkspaceNameTemplate, StringComparison.InvariantCultureIgnoreCase);
                    if (indexOfErmWorkspace > 0)
                    {
                        ermWorkspaceBasePath = localPath.Substring(0, indexOfErmWorkspace + ermWorkspaceNameTemplate.Length);
                    }
                }

                var dlg = new OpenFileDialog();
                if (!string.IsNullOrEmpty(ermWorkspaceBasePath))
                {
                    dlg.InitialDirectory = ermWorkspaceBasePath;
                }

                dlg.Filter = "Visual Studio solution file (.sln)|*.sln"; // Filter files by extension

                var result = dlg.ShowDialog();
                if (result.Value)
                {
                    return dlg.FileName;
                }

                return null;
            }
        }

        private static string ResolveSolutionPath()
        {
            var localPath = Path.GetDirectoryName(Assembly.GetCallingAssembly().GetModules()[0].FullyQualifiedName);
            if (localPath == null)
            {
                return null;
            }

            const string ErmSolutionName = "2GIS.ERM.VS2012.sln";
            var parentDir = Directory.GetParent(localPath);
            while (parentDir != null)
            {
                var checkingPath = Path.Combine(parentDir.FullName, ErmSolutionName);
                if (File.Exists(checkingPath))
                {
                    return checkingPath;
                }

                parentDir = Directory.GetParent(parentDir.FullName);
            }

            return null;
        }

        private static void ProcessSourceFiles(IEnumerable<string> args)
        {
            var solutionPath = IsSolutionAutoResolve(args) ? ResolveSolutionPath() : ErmSolutionFullPath;

            if (string.IsNullOrEmpty(solutionPath))
            {
                Console.WriteLine("Can't find Erm solution file");
                return;
            }

            Console.WriteLine("Starting processing Erm solution: " + solutionPath);

            var workspace = Workspace.LoadSolution(solutionPath);
            var solution = workspace.CurrentSolution;
            if (solution != null)
            {
                var watcher = new Stopwatch();
                watcher.Start();
                IProcessingContext context = new ProcessingContext();
                var processors = new IProcessor[]
                {
                    new EntitiesProcessor(context, workspace, solution),
                    new RequestHandlersProcessor(context, workspace, solution),
                    new RequestsProcessor(context, workspace, solution),
                    new EntityControllersProcessor(context, workspace, solution),
                    new ExplicitSubRequestsProcessor(context, workspace, solution),
                    new ResumedUsecasesProcessor(context, workspace, solution),
                    new UseCaseStartEndpointProcessor(context, workspace, solution),
                    new ExtractUseCasesProcessor(context, workspace, solution),
                    new GenerateUseCasesProcessor(context, workspace, solution)
                };

                foreach (var project in solution.Projects.Where(p => !Workarounds.ExcludedProjectNames.Contains(p.Name)))
                {
                    var compilation = project.GetCompilation();
                    var documentsToProcess = project.Documents;
                    foreach (var document in documentsToProcess.Where(d => !Workarounds.ExcludedDocumentNames.Contains(Path.GetFileNameWithoutExtension(d.Name))))
                    {
                        var syntaxTree = document.GetSyntaxTree();
                        var symanticModel = compilation.GetSemanticModel(syntaxTree);

                        foreach (var processor in processors)
                        {
                            if (processor.IsDocumentProcessingRequired(symanticModel, document))
                            {
                                processor.ProcessDocument(symanticModel, document);
                            }
                        }
                    }
                }

                foreach (var processor in processors)
                {
                    processor.FinishProcessing();
                }

                watcher.Stop();
                Console.WriteLine("Execution time ms:" + watcher.ElapsedMilliseconds);
            }
        }

        private static bool IsSolutionAutoResolve(IEnumerable<string> args)
        {
            return args.Any(x => x.LastIndexOf("autosln", StringComparison.OrdinalIgnoreCase) >= 0);
        }

        private static void ThreadLauncher(object state)
        {
            var args = state as string[];
            if (args == null)
            {
                return;
            }

            ProcessSourceFiles(args);
        }

        [STAThread]
        private static void Main(string[] args)
        {
            //var workingThread = new Thread(ThreadLauncher, int.MaxValue);
            //workingThread.Start(args);
            //workingThread.Join();
            ProcessSourceFiles(args);
            Console.WriteLine("Press \"ENTER\" to finish program ...");
            Console.ReadLine();
        }
    }
}
