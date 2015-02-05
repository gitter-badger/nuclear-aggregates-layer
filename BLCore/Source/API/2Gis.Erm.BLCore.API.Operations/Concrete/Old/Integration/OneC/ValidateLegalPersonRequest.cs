using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Integration.OneC
{
    public sealed class ValidateLegalPersonRequestItem
    {
        [Obsolete]
        public LegalPerson Entity { get; set; }

        public string SyncCode1C { get; set; }
        public long LegalPersonId { get; set; }
    }

    public sealed class ValidateLegalPersonsFor1CRequest : Request
    {
        public IEnumerable<ValidateLegalPersonRequestItem> Entities { get; set; }
    }

    public sealed class ErrorDto
    {
        public long LegalPersonId { get; set; }
        public string SyncCode1C { get; set; }
        public bool IsBlockingError { get; set; }

        public string ErrorMessage { get; set; }
    }

    public sealed class ValidateLegalPersonsResponse : Response
    {
        public IReadOnlyList<ErrorDto> BlockingErrors { get; set; }
        public IReadOnlyList<ErrorDto> NonBlockingErrors { get; set; }
    }
}