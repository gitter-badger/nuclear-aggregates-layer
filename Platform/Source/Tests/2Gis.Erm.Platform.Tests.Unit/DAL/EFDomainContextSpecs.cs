using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

using DoubleGis.Erm.Platform.Core.UseCases.Context;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.EntityFramework;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

using Effort;

using FluentAssertions;

using Machine.Specifications;

using It = Machine.Specifications.It;

namespace DoubleGis.Erm.Platform.Tests.Unit.DAL
{
    public class EFDomainContextSpecs
    {
        const string DefaultContextName = "Erm";

        static IModifiableDomainContext _domainContext;


        public class Entity : IEntity, IEntityKey
        {
            public virtual long Id { get; set; }
        }

        [Tags("DAL")]
        [Subject(typeof(EFDomainContext))]
        public abstract class EFDomainContextMockContext
        {
            Establish context = () =>
            {
                Effort.Provider.EffortProviderConfiguration.RegisterProvider();

                var builder = new DbModelBuilder();
                builder.Entity<Entity>().ToTable("E").HasKey(x => x.Id).Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

                var connection = DbConnectionFactory.CreateTransient();
                var dbContext = new DbContext(connection, builder.Build(connection).Compile(), true);

                dbContext.Configuration.ValidateOnSaveEnabled = true;
                dbContext.Configuration.UseDatabaseNullSemantics = true;
                dbContext.Configuration.LazyLoadingEnabled = false;
                dbContext.Configuration.ProxyCreationEnabled = false;
                dbContext.Configuration.AutoDetectChangesEnabled = false;

                DbContext = dbContext;
            };

            protected static DbContext DbContext { get; private set; }
        }

        [Tags("DAL")]
        [Subject(typeof(EFDomainContext))]
        class When_Disposing_with_NullPendingChangesHandlingStrategy : EFDomainContextMockContext
        {
            static Exception _exception;

            Establish context = () =>
                {
                    _domainContext = new EFDomainContext(new ProcessingContext(),
                                                         DbContext,
                                                         new NullPendingChangesHandlingStrategy());
                    _domainContext.Add(new Entity() { Id = 100 });
                };

            Because of = () => _exception = Catch.Exception(() => _domainContext.Dispose());

            It exception_should_not_be_thrown = () => _exception.Should().Be(null);
        }

        [Tags("DAL")]
        [Subject(typeof(EFDomainContext))]
        class When_Disposing_with_ForcePendingChangesHandlingStrategy : EFDomainContextMockContext
        {
            static Exception _exception;

            Establish context = () =>
            {
                _domainContext = new EFDomainContext(new ProcessingContext(),
                                                     DbContext,
                                                     new ForcePendingChangesHandlingStrategy());
                _domainContext.Add(new Entity() { Id = 100 });
            };

            Because of = () => _exception = Catch.Exception(() => _domainContext.Dispose());

            It exception_of_type_PendingChangesNotHandledException_should_be_thrown = () => _exception.Should().BeOfType<PendingChangesNotHandledException>();
        }
    }
}