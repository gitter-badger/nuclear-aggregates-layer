using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

using FluentAssertions;

using Machine.Specifications;

namespace DoubleGis.Erm.Qds.Etl.Tests.Unit.AcceptanceTests
{
    class EntityChangedContext<TEntity, TDoc> : IndexationFacadeContext
        where TEntity : class, IEntityKey, IEntity, new()
        where TDoc : IDoc, new()
    {
        Establish context = () =>
            {
                ChangedEntityId = 42;
                ChangedEntity = new TEntity { Id = ChangedEntityId };
                Target.LogChangesForEntity(ChangedEntity);
            };

        protected static void ContainDocumentOfSpecifiedTypeWithExpectedId()
        {
            DocsStorage.NewPublishedDocs.Should().Contain(d => d.Id == ChangedEntityId.ToString() && d is TDoc);
        }

        protected static long ChangedEntityId;
        protected static TEntity ChangedEntity;
    }
}