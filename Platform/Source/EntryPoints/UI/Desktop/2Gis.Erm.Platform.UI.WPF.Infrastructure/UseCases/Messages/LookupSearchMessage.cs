using System;

using DoubleGis.Erm.Platform.Model.Metadata.Entities.PropertyFeatures;
using DoubleGis.Platform.UI.WPF.Infrastructure.Messaging;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases.Messages
{
    public class LookupSearchMessage : MessageBase<FreeProcessingModel>
    {
        public LookupSearchMessage(string searchText, LookupPropertyFeature property, Guid sourceId)
            : base(null)
        {
            SearchText = searchText;
            Property = property;
            SourceId = sourceId;
        }

        public string SearchText { get; set; }
        public LookupPropertyFeature Property { get; set; }
        public Guid SourceId { get; set; }
    }
}