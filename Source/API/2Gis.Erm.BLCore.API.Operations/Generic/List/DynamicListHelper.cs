using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Expressions;
using System.Reflection;

using DoubleGis.Erm.Platform.Common.Utils.Data;

using DynamicExpression = System.Linq.Dynamic.DynamicExpression;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.List
{
    public static class DynamicListHelper
    {
        public static IEnumerable<DynamicListRow> ToDynamicList(this IQueryable sourceQuery, IEnumerable<QueryField> fields)
        {
            Type anonymousObjectType;
            var anonymousObjects = (sourceQuery.SelectFields(fields, out anonymousObjectType) as IQueryable<object>).ToList();
            var dynamicList = anonymousObjects.SelectDynamicRows(fields, anonymousObjectType);
            return dynamicList;
        }

        /// <summary>
        /// Строит Select в анонимный объект. Запросы годны для использования на стороне БД (в запросах Linq to Entities).
        /// </summary>
        private static IQueryable SelectFields(this IQueryable source, IEnumerable<QueryField> fields, out Type returnElementType)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (fields == null)
            {
                throw new ArgumentNullException("fields");
            }

            var lambda = BuildFieldsToAnonymousTypeSelectionLambda(source.ElementType, fields);
            returnElementType = lambda.Body.Type;

            return source.Provider.CreateQuery(Expression.Call(typeof(Queryable), "Select", new[] { source.ElementType, returnElementType }, source.Expression, Expression.Quote(lambda)));
        }

        /// <summary>
        /// Строит Select для выборки данных в DynamicListRow. Выполняется на серверной стороне.
        /// </summary>
        private static IEnumerable<DynamicListRow> SelectDynamicRows(this IEnumerable source, IEnumerable<QueryField> fields, Type sourceType)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (fields == null)
            {
                throw new ArgumentNullException("fields");
            }

            var lambda = BuildFieldsSelectionLambda(sourceType, fields);

            var parameter = Expression.Parameter(typeof(IEnumerable));
            var castResult = Expression.Call(typeof(Enumerable), "Cast", new[] { sourceType }, parameter);
            var expressionCall = Expression.Call(typeof(Enumerable), "Select", new[] { sourceType, typeof(DynamicListRow) }, castResult, lambda);
            return Expression.Lambda<Func<IEnumerable, IEnumerable<DynamicListRow>>>(expressionCall, parameter).Compile()(source).ToArray();
        }

        private static LambdaExpression BuildFieldsSelectionLambda(Type inputType, IEnumerable<QueryField> fields)
        {
            ConstructorInfo propertyConstructorInfo = typeof(DynamicPropertyValue).GetConstructor(Type.EmptyTypes);
            var rowInitExpressions = new List<MemberInitExpression>();
            var createRowParameterExpression = Expression.Parameter(inputType, string.Empty);

            var nameProperty = GetMemberInfo<DynamicPropertyValue>(x => x.Name);
            var valueProperty = GetMemberInfo<DynamicPropertyValue>(x => x.Value);

            foreach (var fieldInfo in fields)
            {
                //// Выражение вида new DynamicPropertyValue() {Name = "Name", Value = x.SomeProperty}
                var propCreateExpression =
                    Expression.MemberInit(
                    Expression.New(propertyConstructorInfo),
                    Expression.Bind(nameProperty, Expression.Constant(fieldInfo.Name)),
                    Expression.Bind(valueProperty, Expression.Convert(Expression.MakeMemberAccess(createRowParameterExpression, inputType.GetProperty(fieldInfo.Name)), typeof(object))));

                rowInitExpressions.Add(propCreateExpression);
            }

            ConstructorInfo rowConstructorInfo = typeof(DynamicListRow).GetConstructor(Type.EmptyTypes);
            var valuesProperty = GetMemberInfo<DynamicListRow>(x => x.Values);

            // Выражение вида x => new DynamicListRow() { Values =new[] {new DynamicPropertyValue("Name", x.SomeProperty1), new DynamicPropertyValue("Name", x.SomeProperty2)}}
            NewArrayExpression rowsCreateCtorArgsExpression = Expression.NewArrayInit(typeof(DynamicPropertyValue), rowInitExpressions);
            var rowCreateExpression = Expression.MemberInit(Expression.New(rowConstructorInfo), Expression.Bind(valuesProperty, rowsCreateCtorArgsExpression));
            var createRowLambda = Expression.Lambda(rowCreateExpression, createRowParameterExpression);

            return createRowLambda;
        }

        private static LambdaExpression BuildFieldsToAnonymousTypeSelectionLambda(Type inputType, IEnumerable<QueryField> fields)
        {
            var parameterExpession = Expression.Parameter(inputType, string.Empty);
            var properties = new Dictionary<string, Tuple<DynamicProperty, LambdaExpression>>();
            foreach (var fieldInfo in fields)
            {
                // Парсим доступ к свойству вида "it.Prop1.Prop2.Prop3"
                var propertyExpression = DynamicExpression.ParseLambda(new[] { parameterExpession }, null, fieldInfo.Expression, null);
                properties[fieldInfo.Name] = Tuple.Create(new DynamicProperty(fieldInfo.Name, propertyExpression.ReturnType), propertyExpression);
            }

            var anonymousType = DynamicExpression.CreateClass(properties.Select(x => x.Value.Item1));
            var propertiesInitializers = properties.Select(x => Expression.Bind(anonymousType.GetProperty(x.Key), x.Value.Item2.Body));

            var defaultCtor = anonymousType.GetConstructor(Type.EmptyTypes);
            var objectCreateExpression = Expression.MemberInit(Expression.New(defaultCtor), propertiesInitializers);

            // Выражение вида x => new { Property1 = x.SomeProperty1, ..., Property2 = x.SomeProperty2)}}
            var objectCreateLambda = Expression.Lambda(objectCreateExpression, parameterExpession);
            return objectCreateLambda;
        }

        private static MemberInfo GetMemberInfo<T>(Expression<Func<T, object>> propertyAccessor)
        {
            return ((MemberExpression)propertyAccessor.Body).Member;
        }
    }
}
