using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Common.Crosscutting.ProcessingResults;

namespace DoubleGis.Erm.BLCore.API.Releasing.Releases
{
    public sealed class ReleaseRevertingResult
    {
        public static ReleaseRevertingResult Error(string msg)
        {
            return new ReleaseRevertingResult
                {
                    Succeded = false,
                    ProcessingMessages = new[] { new ProcessingResultsMessage { Type = ProcessingResultsMessageType.Error, Text = msg } }
                };
        }

        public static ReleaseRevertingResult Succeeded
        {
            get
            {
                return new ReleaseRevertingResult { Succeded = true };
            }
        }

        public ReleaseRevertingResult()
        {
            ProcessingMessages = Enumerable.Empty<ProcessingResultsMessage>();
        }

        public bool Succeded { get; set; }
        public IEnumerable<ProcessingResultsMessage> ProcessingMessages { get; set; }
    }
}