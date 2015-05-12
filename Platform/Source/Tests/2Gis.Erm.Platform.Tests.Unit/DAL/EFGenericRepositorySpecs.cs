using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

using AutoMapper;

using DoubleGis.Erm.Platform.Tests.Unit.DAL.Infrastructure.Fakes;

using Effort;
using Effort.Provider;

using FluentAssertions;

using Machine.Specifications;

using Moq;

using NuClear.Model.Common.Entities.Aspects;
using NuClear.Model.Common.Entities.Aspects.Integration;
using NuClear.Storage;
using NuClear.Storage.Core;
using NuClear.Storage.EntityFramework;
using NuClear.Storage.UseCases;

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

        public class ReplicableEntity : IEntity, IEntityKey, IReplicableEntity
        {
            public virtual long Id { get; set; }
            public virtual Guid ReplicationCode { get; set; }
        }

        public class Entity : IEntity, IEntityKey
        {
            public virtual long Id { get; set; }
        }

        [Tags("DAL")]
        [Subject(typeof(EFGenericRepository<>))]
        public abstract class StubEFGenericRepositoryContext<TEntity> where TEntity : class, IEntityKey, IEntity, new()
        {
            Establish context = () =>
            {
                EffortProviderConfiguration.RegisterProvider();

                Entity = new TEntity() { Id = 1 };

                var builder = new DbModelBuilder();
                builder.Entity<AuditableEntity>().ToTable("AE").HasKey(x => x.Id).Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
                builder.Entity<DeletableEntity>().ToTable("DE").HasKey(x => x.Id).Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
                builder.Entity<ReplicableEntity>().ToTable("RE").HasKey(x => x.Id).Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

                var connection = DbConnectionFactory.CreateTransient();
                var dbContext = new DbContext(connection, builder.Build(connection).Compile(), true);

                var domainContext = new EFDomainContext(Mock.Of<IProcessingContext>(),
                                                        dbContext,
                                                        Mock.Of<IPendingChangesHandlingStrategy>());
                dbContext.Configuration.ValidateOnSaveEnabled = true;
                dbContext.Configuration.UseDatabaseNullSemantics = true;
                dbContext.Configuration.LazyLoadingEnabled = false;
                dbContext.Configuration.ProxyCreationEnabled = false;
                dbContext.Configuration.AutoDetectChangesEnabled = false;

                var modifiableDomainContextProviderMock = new Mock<IModifiableDomainContextProvider>();
                modifiableDomainContextProviderMock.Setup(p => p.Get<TEntity>()).Returns(domainContext);

                GenericRepository = new EFGenericRepository<TEntity>(new StubUserContext(),
                                                                     modifiableDomainContextProviderMock.Object,
                                                                     new NullPersistenceChangesRegistryProvider(),
                                                                     Mock.Of<IMappingEngine>());
            };

            protected static TEntity Entity { get; private set; }
            protected static IRepository<TEntity> GenericRepository { get; private set; }
        }

        class When_call_Delete_for_IDeletable_entity : StubEFGenericRepositoryContext<DeletableEntity>
        {
            Because of = () => GenericRepository.Delete(Entity);

            It property_IsDeleted_should_be_set_to_true = () => Entity.IsDeleted = true;
        }

        class When_call_Add_to_create_an_IAuditableEntity_entity_without_identity : StubEFGenericRepositoryContext<AuditableEntity>
        {
            static Exception _exception;

            Establish context = () => Entity.Id = 0;

            Because of = () => _exception = Catch.Exception(() => GenericRepository.Add(Entity));

            It exception_of_type_InvalidOperationException_should_be_thrown = () => _exception.Should().BeOfType<InvalidOperationException>();
        }

        class When_call_Add_to_create_an_IAuditableEntity_entity : StubEFGenericRepositoryContext<AuditableEntity>
        {
            Because of = () => GenericRepository.Add(Entity);

            It property_CreatedBy_should_be_set = () => Entity.CreatedBy.Should().Be(StubUserContext.FakeCurrentUserCode);
            It property_CreatedOn_should_be_set = () => Entity.CreatedOn.Should().BeWithin(TimeSpan.FromMinutes(5)).After(DateTime.UtcNow);
            It property_ModifiedBy_should_be_set = () => Entity.ModifiedBy.Should().Be(StubUserContext.FakeCurrentUserCode);
            It property_ModifiedOn_should_be_set = () => Entity.ModifiedOn.Should().BeWithin(TimeSpan.FromMinutes(5)).After(DateTime.UtcNow);
        }

        class When_call_Update_to_update_an_IAuditableEntity_entity : StubEFGenericRepositoryContext<AuditableEntity>
        {
            Establish context = () =>
                                    {
                                        Entity.CreatedBy = 1234;
                                        Entity.CreatedOn = new DateTime(2000, 01, 01);
                                    };

            Because of = () => GenericRepository.Update(Entity);

            It property_CreatedBy_should_not_be_set = () => Entity.CreatedBy.Should().Be(1234);
            It property_CreatedOn_should_not_be_set = () => Entity.CreatedOn.Should().Be(new DateTime(2000, 01, 01));
            It property_ModifiedBy_should_be_set = () => Entity.ModifiedBy.Should().Be(StubUserContext.FakeCurrentUserCode);
            It property_ModifiedOn_should_be_set = () => Entity.ModifiedOn.Should().BeWithin(TimeSpan.FromMinutes(5)).After(DateTime.UtcNow);
        }

        class When_call_Add_to_create_an_IReplicableEntity_entity : StubEFGenericRepositoryContext<ReplicableEntity>
        {
            Because of = () => GenericRepository.Add(Entity);

            It property_ReplicationCode_should_be_set = () => Entity.ReplicationCode.Should().NotBe(Guid.Empty);
        }

        class When_call_Update_to_update_an_IReplicableEntity_entity : StubEFGenericRepositoryContext<ReplicableEntity>
        {
            Establish context = () =>
                                    {
                                        Entity.ReplicationCode = new Guid("FE2DCB8F-4D29-4164-B826-C2A94A2A1AAB");
                                    };

            Because of = () => GenericRepository.Update(Entity);

            It property_ReplicationCode_should_not_be_set = () => Entity.ReplicationCode.Should().Be(new Guid("FE2DCB8F-4D29-4164-B826-C2A94A2A1AAB")); 
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