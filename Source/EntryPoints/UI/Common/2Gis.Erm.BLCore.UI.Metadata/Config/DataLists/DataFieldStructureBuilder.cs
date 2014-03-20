using System;
using System.Linq.Expressions;

using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Metadata.Common;
using DoubleGis.Erm.Platform.Model.Metadata.Entities.CommonFeatures;

namespace DoubleGis.Erm.BLCore.UI.Metadata.Config.DataLists
{
    public class DataFieldStructureBuilder : ConfigElementBuilder<DataFieldStructureBuilder, DataFieldStructure>
    {
        public DataFieldStructureBuilder Property(string propertyName, Type propertyType, string expression)
        {
            Features.Add(new DataFieldExpressionFeature(propertyName, propertyType, expression));
            return this;
        }

        public DataFieldStructureBuilder Property<TContainer>(Expression<Func<TContainer, object>> dtoPropertyExpression, string expression)
        {
            var propertyName = StaticReflection.GetMemberName(dtoPropertyExpression);
            var propertyType = StaticReflection.GetMemberType(dtoPropertyExpression);

            return Property(propertyName, propertyType, expression);
        }

        public DataFieldStructureBuilder Property<TListDto, TEntity>(Expression<Func<TListDto, object>> dtoPropertyExpression, Expression<Func<TEntity, object>> selectExpression)
            where TListDto : IOperationSpecificEntityDto where TEntity : IEntityKey
        {
            var propertyName = StaticReflection.GetMemberName(dtoPropertyExpression);
            var propertyType = StaticReflection.GetMemberType(dtoPropertyExpression);

            return Property(propertyName, propertyType, selectExpression.AsDynamicQueryableExpression());
        }


        public DataFieldStructureBuilder MainAttribute()
        {
            Features.Add(new MainAttributeFeature());
            return this;
        }

        public DataFieldStructureBuilder DefaultSortField()
        {
            Features.Add(new DefaultFilterFeature());
            return this;
        }
        public DataFieldStructureBuilder Hidden()
        {
            Features.Add(new HiddenFeature());
            return this;
        }

        public DataFieldStructureBuilder Filtered()
        {
            Features.Add(new FilteredDataFieldFeature());
            return this;
        }

        public DataFieldStructureBuilder DisableSorting()
        {
            Features.Add(new DisableSortingDataFieldFeature());
            return this;
        }

        public DataFieldStructureBuilder LocalizedName(Expression<Func<string>> resourceKeyExpression)
        {
            Features.Add(DisplayNameLocalizedFeature.Create(resourceKeyExpression));
            return this;
        }

        public DataFieldStructureBuilder ReferenceTo(string referencedPropertyName, EntityName entityName)
        {
            Features.Add(new ReferenceDataFieldFeature(referencedPropertyName, entityName));
            return this;
        }

        public DataFieldStructureBuilder ReferenceTo<TContainer>(Expression<Func<TContainer, object>> referencedIdExpression, EntityName entityName)
        {
            return ReferenceTo(StaticReflection.GetMemberName(referencedIdExpression), entityName);
        }

        protected override DataFieldStructure Create()
        {
            return new DataFieldStructure(Features);
        }

        public DataFieldStructureBuilder PropertyReference<TListDto, TEntity, TTargetEntity>(Expression<Func<TListDto, object>> dtoNameExpression, Expression<Func<TEntity, object>> nameExpression, Expression<Func<TListDto, object>> dtoIdExpression, Expression<Func<TEntity, object>> idExpression) where TListDto : IOperationSpecificEntityDto where TEntity : IEntityKey
        {
            // Add an extra id field to reference to
            var dataField = DataFieldStructure.Config
                .Property(dtoIdExpression, idExpression)
                .Hidden();

            Features.Add(new ExtraDataFieldsFeature(dataField));

            var referencedPropertyName = StaticReflection.GetMemberName(dtoIdExpression);

            return Property(dtoNameExpression, nameExpression).ReferenceTo(referencedPropertyName, typeof(TTargetEntity).AsEntityName());
        }
    }
}