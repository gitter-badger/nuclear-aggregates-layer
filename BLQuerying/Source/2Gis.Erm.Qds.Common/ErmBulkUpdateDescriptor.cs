using Nest;

namespace DoubleGis.Erm.Qds.Common
{
    internal sealed class ErmBulkUpdateDescriptor<TDocument, TPartialDocument> : BulkUpdateDescriptor<TDocument, TPartialDocument>
        where TDocument : class
        where TPartialDocument : class
    {
        public UpdateType UpdateType { private get; set; }

        protected override object GetBulkOperationBody()
        {
            var self = (IBulkUpdateOperation<TDocument, TPartialDocument>)this;
            return new ErmBulkOperationBody { Doc = self.Doc, UpdateType = UpdateType };
        }
    }
}