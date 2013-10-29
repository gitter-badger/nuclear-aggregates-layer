using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity;

namespace DoubleGis.Erm.Platform.Model.Metadata.Common.Features.Operations
{
    public sealed class EntitySpecificOperationFeature<TOperationIdentity> : IBoundOperationFeature<TOperationIdentity>, IEntitySpecificOperationFeature
        where TOperationIdentity : OperationIdentityBase<TOperationIdentity>, new()
    {
        public static EntitySpecificOperationFeature<TOperationIdentity> Instance
        {
            get
            {
                return new EntitySpecificOperationFeature<TOperationIdentity>();
            }
        }

        public IOperationIdentity Identity
        {
            get
            {
                return new TOperationIdentity();
            }
        }

        public EntitySet Entity { get; set; }
    }
}