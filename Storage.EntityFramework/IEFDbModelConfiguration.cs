using System;
using System.Data.Entity;

namespace NuClear.Storage.EntityFramework
{
    public interface IEFDbModelConfiguration
    {
        string ContainerName { get; }
        Type EntityType { get; }
        void Apply(DbModelBuilder builder);
    }
}