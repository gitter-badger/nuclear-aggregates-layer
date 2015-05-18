using System;
using System.Linq.Expressions;

using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Model.Metadata.Entities.CommonFeatures;

using NuClear.Metamodeling.Elements;
using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.BLCore.UI.Metadata.Config.DataLists
{
    public class DataFieldMetadataBuilder : MetadataElementBuilder<DataFieldMetadataBuilder, DataFieldMetadata>
    {
        public DataFieldMetadataBuilder Property(string propertyName, Type propertyType, string expression)
        {
            WithFeatures(new DataFieldExpressionFeature(propertyName, propertyType, expression));
            return this;
        }

        public DataFieldMetadataBuilder Property<TContainer>(Expression<Func<TContainer, object>> dtoPropertyExpression, string expression)
        {
            var propertyName = StaticReflection.GetMemberName(dtoPropertyExpression);
            var propertyType = StaticReflection.GetMemberType(dtoPropertyExpression);

            return Property(propertyName, propertyType, expression);
        }

        public DataFieldMetadataBuilder Property<TListDto, TEntity>(Expression<Func<TListDto, object>> dtoPropertyExpression, Expression<Func<TEntity, object>> selectExpression)
            where TListDto : IOperationSpecificEntityDto where TEntity : IEntityKey
        {
            var propertyName = StaticReflection.GetMemberName(dtoPropertyExpression);
            var propertyType = StaticReflection.GetMemberType(dtoPropertyExpression);

            return Property(propertyName, propertyType, selectExpression.AsDynamicQueryableExpression());
        }


        public DataFieldMetadataBuilder MainAttribute()
        {
            WithFeatures(new MainAttributeFeature());
            return this;
        }

        public DataFieldMetadataBuilder DefaultSortField()
        {
            WithFeatures(new DefaultFilterFeature());
            return this;
        }
        public DataFieldMetadataBuilder Hidden()
        {
            WithFeatures(new HiddenFeature());
            return this;
        }

        public DataFieldMetadataBuilder Filtered()
        {
            WithFeatures(new FilteredDataFieldFeature());
            return this;
        }

        public DataFieldMetadataBuilder DisableSorting()
        {
            WithFeatures(new DisableSortingDataFieldFeature());
            return this;
        }

        public DataFieldMetadataBuilder LocalizedName(Expression<Func<string>> resourceKeyExpression)
        {
            WithFeatures(DisplayNameLocalizedFeature.Create(resourceKeyExpression));
            return this;
        }

        public DataFieldMetadataBuilder ReferenceTo(string referencedPropertyName, IEntityType entityName)
        {
            WithFeatures(new ReferenceDataFieldFeature(referencedPropertyName, entityName));
            return this;
        }

        public DataFieldMetadataBuilder ReferenceTo<TContainer>(Expression<Func<TContainer, object>> referencedIdExpression, IEntityType entityName)
        {
            return ReferenceTo(StaticReflection.GetMemberName(referencedIdExpression), entityName);
        }

        protected override DataFieldMetadata Create()
        {
            return new DataFieldMetadata(Features);
        }

        public DataFieldMetadataBuilder PropertyReference<TListDto, TEntity, TTargetEntity>(Expression<Func<TListDto, object>> dtoNameExpression, Expression<Func<TEntity, object>> nameExpression, Expression<Func<TListDto, object>> dtoIdExpression, Expression<Func<TEntity, object>> idExpression) where TListDto : IOperationSpecificEntityDto where TEntity : IEntityKey
        {
            // Add an extra id field to reference to
            var dataField = DataFieldMetadata.Config
                .Property(dtoIdExpression, idExpression)
                .Hidden();

            WithFeatures(new ExtraDataFieldsFeature(dataField));

            var referencedPropertyName = StaticReflection.GetMemberName(dtoIdExpression);

            return Property(dtoNameExpression, nameExpression).ReferenceTo(referencedPropertyName, typeof(TTargetEntity).AsEntityName());
        }
    }
}