using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

using Effort;
using Effort.Provider;

using FluentAssertions;

using Machine.Specifications;

using NuClear.Model.Common.Entities.Aspects;
using NuClear.Storage.Core;
using NuClear.Storage.EntityFramework;
using NuClear.Storage.UseCases;

namespace Storage.EntityFramework.Tests
{
    public class EFDomainContextSpecs
    {
        static IModifiableDomainContext _domainContext;
        
        [Tags("Storage")]
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

        private abstract class EFDomainContextMockContext
        {
            Establish context = () =>
            {
                EffortProviderConfiguration.RegisterProvider();

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

        private class Entity : IEntity, IEntityKey
        {
            public virtual long Id { get; set; }
        }
    }
}