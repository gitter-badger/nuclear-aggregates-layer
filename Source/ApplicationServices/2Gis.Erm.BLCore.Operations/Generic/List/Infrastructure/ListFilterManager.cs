using System;
using System.Linq.Expressions;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.Platform.Model.Entities;

namespace DoubleGis.Erm.BLCore.Operations.Generic.List.Infrastructure
{
    public enum ExtendedPropertyUnionType
    {
        And,
        Or
    }

    public sealed class ListFilterManager
    {
        private readonly SearchListModel _searchListModel;

        public ListFilterManager(SearchListModel searchListModel)
        {
            _searchListModel = searchListModel;
        }

        public EntityName ParentEntityName
        {
            get
            {
                if (string.IsNullOrEmpty(_searchListModel.PType))
                {
                    return EntityName.None;
                }

                EntityName parentEntityName;
                if (!Enum.TryParse(_searchListModel.PType, true, out parentEntityName))
                {
                    throw new ArgumentException(string.Format("Cannot parse parent entity type {0}", _searchListModel.PType), "searchListModel");
                }

                return parentEntityName;
            }
        }

        public long ParentEntityId
        {
            get { return _searchListModel.PId; }
        }

        public string UserInputFilter
        {
            get { return _searchListModel.FilterInput; }
        }

        public Expression<Func<TEntity, bool>> CreateForExtendedProperty<TEntity, TParam>(string parameterName, Func<TParam, Expression<Func<TEntity, bool>>> action)
        {
            if (_searchListModel.HasExtendedProperty(parameterName))
            {
                var param = _searchListModel.GetExtendedProperty<TParam>(parameterName);
                return !param.Equals(default(TParam)) ? action(param) : null;
            }

            return null;
        }

        public Expression<Func<TEntity, bool>> CreateForExtendedProperty<TEntity, TParam1, TParam2>(
            string parameterName1,
            string parameterName2,
            Func<TParam1, TParam2, Expression<Func<TEntity, bool>>> action)
        {
            return CreateForExtendedProperty(parameterName1, parameterName2, ExtendedPropertyUnionType.And, action);
        }

        public Expression<Func<TEntity, bool>> CreateForExtendedProperty<TEntity, TParam1, TParam2>(
            string parameterName1, 
            string parameterName2,
            ExtendedPropertyUnionType extendedPropertyUnionType,
            Func<TParam1, TParam2, Expression<Func<TEntity, bool>>> action)
        {
            if (_searchListModel.HasExtendedProperty(parameterName1) && _searchListModel.HasExtendedProperty(parameterName2))
            {
                var param1 = _searchListModel.GetExtendedProperty<TParam1>(parameterName1);
                var param2 = _searchListModel.GetExtendedProperty<TParam2>(parameterName2);
                if (extendedPropertyUnionType == ExtendedPropertyUnionType.And)
                {
                    return !param1.Equals(default(TParam1)) && !param2.Equals(default(TParam2)) ? action(param1, param2) : null;
                }

                return !param1.Equals(default(TParam1)) || !param2.Equals(default(TParam2)) ? action(param1, param2) : null;
            }

            return null;
        }

        public Expression<Func<TEntity, bool>> CreateForExtendedProperty<TEntity, TParam1, TParam2, TParam3>(
            string parameterName1, 
            string parameterName2, 
            string parameterName3, 
            Func<TParam1, TParam2, TParam3, Expression<Func<TEntity, bool>>> action)
        {
            if (_searchListModel.HasExtendedProperty(parameterName1) && _searchListModel.HasExtendedProperty(parameterName2) && _searchListModel.HasExtendedProperty(parameterName3))
            {
                var param1 = _searchListModel.GetExtendedProperty<TParam1>(parameterName1);
                var param2 = _searchListModel.GetExtendedProperty<TParam2>(parameterName2);
                var param3 = _searchListModel.GetExtendedProperty<TParam3>(parameterName3);
                return !param1.Equals(default(TParam1)) && !param2.Equals(default(TParam2)) && !param3.Equals(default(TParam3)) ? action(param1, param2, param3) : null;
            }

            return null;
        }
        
        public Expression<Func<TEntity, bool>> CreateForExtendedProperty<TEntity, TParam>(
            string parameterName,
            Func<TParam, Expression<Func<TEntity, bool>>> action,
            Expression<Func<TEntity, bool>> defaultExpression)
        {
            if (_searchListModel.HasExtendedProperty(parameterName))
            {
                var param = _searchListModel.GetExtendedProperty<TParam>(parameterName);
                return !param.Equals(default(TParam)) ? action(param) : defaultExpression;
            }

            return null;
        }

        public Expression<Func<TEntity, bool>> CreateByParentEntity<TEntity>(EntityName parentEntityName, Func<Expression<Func<TEntity, bool>>> action)
        {
            if (_searchListModel.PId != 0 && _searchListModel.PType == parentEntityName.ToString())
            {
                return action();
            }

            return null;
        }

        public bool HasExtendedProperty(string name)
        {
            return _searchListModel.HasExtendedProperty(name);
        }

        public T GetExtendedProperty<T>(string name)
        {
            return _searchListModel.GetExtendedProperty<T>(name);
        }
    }
}