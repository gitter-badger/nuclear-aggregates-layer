using System;

using Nest;

namespace DoubleGis.Erm.Qds.Common
{
    public interface IElasticManagementApi
    {
        void Map<T>(Func<PutMappingDescriptor<T>, PutMappingDescriptor<T>> mappingSelector) where T : class;
        void DeleteIndex<T>() where T : class;
        void CreateIndex<T>(Func<CreateIndexDescriptor, CreateIndexDescriptor> createIndexSelector) where T : class;
        void AddAlias<T>(string alias) where T : class;
        IndexSettings GetIndexSettings(Type indexType);
        void UpdateIndexSettings(Type indexType, Func<UpdateSettingsDescriptor, UpdateSettingsDescriptor> updateSettingsSelector, bool optimize = false);
        void DeleteMapping<T>() where T : class;

        bool TypeExists<T>() where T : class;
    }
}