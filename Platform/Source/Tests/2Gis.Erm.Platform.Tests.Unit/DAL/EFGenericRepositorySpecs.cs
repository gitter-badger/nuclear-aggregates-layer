using System;
using System.Linq;

using DoubleGis.Erm.Platform.API.Core.UseCases.Context;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.EntityFramework;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Metadata.Replication.Metadata;
using DoubleGis.Erm.Platform.Tests.Unit.DAL.Infrastructure.Fakes;
using DoubleGis.Erm.Platform.Tests.Unit.DAL.Infrastructure.Fakes.Repositories;

using FluentAssertions;

using Machine.Specifications;

using Moq;

using Nuclear.Tracing.API;

using It = Machine.Specifications.It;

namespace DoubleGis.Erm.Platform.Tests.Unit.DAL
{
    public class EFGenericRepositorySpecs
    {
        public class AuditableEntity : IEntity, IEntityKey, IAuditableEntity
        {
            public virtual long Id { get; set; }
            public virtual long CreatedBy { get; set; }
            public virtual DateTime CreatedOn { get; set; }
            public virtual long? ModifiedBy { get; set; }
            public virtual DateTime? ModifiedOn { get; set; }
        }

        public class DeletableEntity : IEntity, IEntityKey, IDeletableEntity
        {
            public virtual long Id { get; set; }
            public virtual bool IsDeleted { get; set; }
        }

        public abstract class Entity : IEntity
        {
        }

        [Tags("DAL")]
        [Subject(typeof(EFGenericRepository<>))]
        public abstract class StubEFGenericRepositoryContext<TEntity> where TEntity : class, IEntity
        {
            Establish context = () =>
            {
                EntityMock = new Mock<TEntity>();

                var objectContext = new Mock<IDbContext>();
                objectContext.Setup(x => x.Set<TEntity>()).Returns(new StubObjectSet<TEntity>());

                var domainContext = new EFDomainContext(Mock.Of<IProcessingContext>(),
                                                        "Erm",
                                                        objectContext.Object,
                                                        Mock.Of<IPendingChangesHandlingStrategy>(),
                                                        new MsCrmReplicationMetadataProvider(EntityNameUtils.AsyncReplicated2MsCrmEntities, EntityNameUtils.AllReplicated2MsCrmEntities.Except(EntityNameUtils.AsyncReplicated2MsCrmEntities)),
                                                        Mock.Of<ICommonLog>());

                var modifiableDomainContextProviderMock = new Mock<IModifiableDomainContextProvider>();
                modifiableDomainContextProviderMock.Setup(p => p.Get<TEntity>()).Returns(domainContext);

                GenericRepository = new EFGenericRepositoryWrapper<TEntity>(new EFGenericRepository<TEntity>(new StubUserContext(),
                                                                                                             modifiableDomainContextProviderMock.Object,
                                                                                                             new NullPersistenceChangesRegistryProvider()));
            };

            protected static Mock<TEntity> EntityMock { get; private set; }
            protected static IRepository<TEntity> GenericRepository { get; private set; }
        }

        class When_call_Delete_for_IDeletable_entity : StubEFGenericRepositoryContext<DeletableEntity>
        {
            Establish context = () => EntityMock.Setup(x => x.IsDeleted).Verifiable();

            Because of = () => GenericRepository.Delete(EntityMock.Object);

            It property_IsDeleted_should_be_set_to_true_only_once = () => EntityMock.VerifySet(x => x.IsDeleted = true, Times.Once);
        }

        class When_call_Add_to_create_an_IAuditableEntity_entity_without_identity : StubEFGenericRepositoryContext<AuditableEntity>
        {
            static Exception _exception;

            Establish context = () => EntityMock.Setup(x => x.Id).Returns(0);

            Because of = () => _exception = Catch.Exception(() => GenericRepository.Add(EntityMock.Object));

            It exception_of_type_InvalidOperationException_should_be_thrown = () => _exception.Should().BeOfType<InvalidOperationException>();
        }

        class When_call_Add_to_create_an_IAuditableEntity_entity : StubEFGenericRepositoryContext<AuditableEntity>
        {
            Establish context = () =>
                {
                    EntityMock.Setup(x => x.Id).Returns(1);
                    EntityMock.Setup(x => x.CreatedBy).Verifiable();
                    EntityMock.Setup(x => x.CreatedOn).Verifiable();
                    EntityMock.Setup(x => x.ModifiedBy).Verifiable();
                    EntityMock.Setup(x => x.ModifiedOn).Verifiable();
                };

            Because of = () => GenericRepository.Add(EntityMock.Object);

            It property_CreatedBy_should_be_set = () => EntityMock.VerifySet(x => x.CreatedBy = Moq.It.IsAny<long>(), Times.Once);
            It property_CreatedOn_should_be_set = () => EntityMock.VerifySet(x => x.CreatedOn = Moq.It.IsAny<DateTime>(), Times.Once);
            It property_ModifiedBy_should_be_set = () => EntityMock.VerifySet(x => x.ModifiedBy = Moq.It.IsAny<long>(), Times.Once);
            It property_ModifiedOn_should_be_set = () => EntityMock.VerifySet(x => x.ModifiedOn = Moq.It.IsAny<DateTime>(), Times.Once);
        }

        class When_call_Update_to_update_an_IAuditableEntity_entity : StubEFGenericRepositoryContext<AuditableEntity>
        {
            Establish context = () =>
            {
                EntityMock.Setup(x => x.CreatedBy).Verifiable();
                EntityMock.Setup(x => x.CreatedOn).Verifiable();
                EntityMock.Setup(x => x.ModifiedBy).Verifiable();
                EntityMock.Setup(x => x.ModifiedOn).Verifiable();
            };

            Because of = () => GenericRepository.Update(EntityMock.Object);

            It property_CreatedBy_should_not_be_set = () => EntityMock.VerifySet(x => x.CreatedBy = Moq.It.IsAny<long>(), Times.Never);
            It property_CreatedOn_should_not_be_set = () => EntityMock.VerifySet(x => x.CreatedOn = Moq.It.IsAny<DateTime>(), Times.Never);
            It property_ModifiedBy_should_be_set = () => EntityMock.VerifySet(x => x.ModifiedBy = Moq.It.IsAny<long>(), Times.Once);
            It property_ModifiedOn_should_be_set = () => EntityMock.VerifySet(x => x.ModifiedOn = Moq.It.IsAny<DateTime>(), Times.Once);
        }

        class When_call_Add_with_null_as_entity : StubEFGenericRepositoryContext<Entity>
        {
            static Exception _exception;

            Because of = () => _exception = Catch.Exception(() => GenericRepository.Add(null));

            It exception_of_type_ArgumentNullException_should_be_thrown = () => _exception.Should().BeOfType<ArgumentNullException>();
        }

        class When_call_Update_with_null_as_entity : StubEFGenericRepositoryContext<Entity>
        {
            static Exception _exception;

            Because of = () => _exception = Catch.Exception(() => GenericRepository.Update(null));

            It exception_of_type_ArgumentNullException_should_be_thrown = () => _exception.Should().BeOfType<ArgumentNullException>();
        }

        class When_call_Delete_with_null_as_entity : StubEFGenericRepositoryContext<Entity>
        {
            static Exception _exception;

            Because of = () => _exception = Catch.Exception(() => GenericRepository.Delete(null));

            It exception_of_type_ArgumentNullException_should_be_thrown = () => _exception.Should().BeOfType<ArgumentNullException>();
        }
    }
}