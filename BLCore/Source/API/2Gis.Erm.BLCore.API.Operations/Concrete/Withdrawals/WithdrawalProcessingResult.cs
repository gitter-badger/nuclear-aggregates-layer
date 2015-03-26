using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Common.Crosscutting.ProcessingResults;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Withdrawals
{
    public sealed class WithdrawalProcessingResult
    {
        public static WithdrawalProcessingResult Errors(params string[] msg)
        {
            return new WithdrawalProcessingResult
                       {
                           Succeded = false,
                           ProcessingMessages = msg.Select(x => new ProcessingResultsMessage
                                                                    {
                                                                        Type = ProcessingResultsMessageType.Error,
                                                                        Text = x
                                                                    }).ToArray()
                       };
        }

        public static WithdrawalProcessingResult Succeeded
        {
            get
            {
                return new WithdrawalProcessingResult { Succeded = true };
            }
        }

        public WithdrawalProcessingResult()
        {
            ProcessingMessages = Enumerable.Empty<ProcessingResultsMessage>();
        }

        public bool Succeded { get; set; }
        public IEnumerable<ProcessingResultsMessage> ProcessingMessages { get; set; }
    }
}