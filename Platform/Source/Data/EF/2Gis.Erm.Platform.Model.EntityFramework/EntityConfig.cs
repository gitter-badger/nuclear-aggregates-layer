using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;

using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.Platform.Model.EntityFramework
{
    public abstract class EntityConfig<TEntity, TEntityContainer> : EntityTypeConfiguration<TEntity>,
                                                                    IEfDbModelConfiguration
        where TEntity : class, IEntity
        where TEntityContainer : class, IEntityContainer, new()
    {
        private static readonly Lazy<TEntityContainer> LazyContainer = new Lazy<TEntityContainer>(() => new TEntityContainer());

        void IEfDbModelConfiguration.Apply(DbModelBuilder builder)
        {
            builder.Configurations.Add(this);
        }

        string IEfDbModelConfiguration.ContainerName
        {
            get { return LazyContainer.Value.Name; }
        }

        Type IEfDbModelConfiguration.EntityType
        {
            get { return typeof(TEntity); }
        }
    }
}