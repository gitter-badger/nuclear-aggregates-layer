using System.IO;

using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.Old;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.LocalMessages
{
    public sealed class CreateLocalMessageRequest : EditRequest<LocalMessage>
    {
        public int IntegrationType { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public Stream Content { get; set; }
    }
}