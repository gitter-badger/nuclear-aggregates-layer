using System;
using System.Data.Entity;

namespace DoubleGis.Erm.Platform.Model.EntityFramework
{
    public interface IEfDbModelConfiguration
    {
        string ContainerName { get; }
        Type EntityType { get; }
        void Apply(DbModelBuilder builder);
    }
}