using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Entities.Aspects;
using NuClear.Storage.EntityFramework;

namespace DoubleGis.Erm.Platform.Model.EntityFramework
{
    public abstract class EntityConfig<TEntity, TEntityContainer> : EntityTypeConfiguration<TEntity>,
                                                                    IEFDbModelConfiguration
        where TEntity : class, IEntity
        where TEntityContainer : class, IEntityContainer, new()
    {
        private static readonly Lazy<TEntityContainer> LazyContainer = new Lazy<TEntityContainer>(() => new TEntityContainer());

        void IEFDbModelConfiguration.Apply(DbModelBuilder builder)
        {
            builder.Configurations.Add(this);
        }

        string IEFDbModelConfiguration.ContainerName
        {
            get { return LazyContainer.Value.Name; }
        }

        Type IEFDbModelConfiguration.EntityType
        {
            get { return typeof(TEntity); }
        }
    }
}