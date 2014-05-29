using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.Platform.DAL.EAV
{
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    internal sealed class WrappedQueryProvider : IQueryProvider
    {
        private readonly IQueryProvider _provider;
        private readonly UnwrapVisitor _unwrapVisitor;

        public WrappedQueryProvider(IQueryProvider provider)
        {
            _provider = provider;
            _unwrapVisitor = new UnwrapVisitor();
        }

        public IQueryable CreateQuery(Expression expression)
        {
            var queryable = _provider.CreateQuery(expression);
            var provider = new WrappedQueryProvider(queryable.Provider);

            if (!queryable.GetType().IsGenericType)
            {
                return new WrappedQuery(queryable, provider);
            }

            var genericType = typeof(WrappedQuery<>).MakeGenericType(queryable.GetType().GetGenericArguments());
            return (IQueryable)Activator.CreateInstance(genericType, new object[] { queryable, provider });
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            var queryable = _provider.CreateQuery<TElement>(expression);
            var provider = new WrappedQueryProvider(queryable.Provider);
            return new WrappedQuery<TElement>(queryable, provider);
        }

        public object Execute(Expression expression)
        {
            Check(expression);
            expression = _unwrapVisitor.Visit(expression);
            return _provider.Execute(expression);
        }

        public TResult Execute<TResult>(Expression expression)
        {
            Check(expression);
            expression = _unwrapVisitor.Visit(expression);
            return _provider.Execute<TResult>(expression);
        }

        private static void Check(Expression expression)
        {
            var isPartsPropertyUsedVisitor = new IsPartsPropertyUsedVisitor();
            isPartsPropertyUsedVisitor.Visit(expression);
            if (isPartsPropertyUsedVisitor.ArePartsAccessed)
            {
                throw new ArgumentException("Parts property cannot be used in LINQ To SQL Query: " + isPartsPropertyUsedVisitor.FieldName);
            }

            var selectExpressionVisitor = new SelectExpressionVisitor();
            selectExpressionVisitor.Visit(expression);
            var forbiddenField = selectExpressionVisitor.Fields
                                                        .FirstOrDefault(x => typeof(IPartable).IsAssignableFrom(x.Type) ||
                                                                             typeof(IEnumerable<IPartable>).IsAssignableFrom(x.Type));
            if (forbiddenField != null)
            {
                throw new ArgumentException("Entity of IPartable type cannot be selected without its parts: " + forbiddenField);
            }
        }
    }

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    internal class UnwrapVisitor : ExpressionVisitor
    {
        protected override Expression VisitMember(MemberExpression node)
        {
            if (node.Type.IsGenericType && typeof(IQueryable).IsAssignableFrom(node.Type.GetGenericTypeDefinition()))
            {
                var queryableVariable = Expression.Lambda<Func<IQueryable>>(node).Compile()();
                var wrappedQuery = queryableVariable as WrappedQuery;
                if (wrappedQuery != null)
                {
                    return Expression.Constant(wrappedQuery.Unwrap());
                }
            }

            return base.VisitMember(node);
        }
    }

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    internal class IsPartsPropertyUsedVisitor : ExpressionVisitor
    {
        public bool ArePartsAccessed { get; private set; }
        public string FieldName { get; private set; }

        protected override Expression VisitMember(MemberExpression node)
        {
            if (typeof(IEnumerable<IEntityPart>).IsAssignableFrom(node.Type))
            {
                ArePartsAccessed = true;
                FieldName = node.ToString();
            }

            return base.VisitMember(node);
        }
    }

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    internal class SelectExpressionVisitor : ExpressionVisitor
    {
        private static readonly MethodInfo[] QueryableMarkerMethods =
            typeof(Queryable).GetMethods(BindingFlags.Static | BindingFlags.Public)
                             .Where(m => m.Name == "Select" || m.Name == "SelectMany" || m.Name == "Any")
                             .ToArray();

        private static readonly MethodInfo[] EnumerableMarkerMethods =
            typeof(Enumerable).GetMethods(BindingFlags.Static | BindingFlags.Public)
                              .Where(m => m.Name == "Select" || m.Name == "SelectMany" || m.Name == "Any")
                              .ToArray();

        private static readonly MethodInfo[] MarkerMethods = QueryableMarkerMethods.Concat(EnumerableMarkerMethods).ToArray();

        private readonly List<Expression> _fields = new List<Expression>();

        public IList<Expression> Fields 
        {
            get { return _fields; }
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Method.IsGenericMethod && MarkerMethods.Contains(node.Method.GetGenericMethodDefinition()))
            {
                var lambdaExpression = node.Arguments.Select(GetLambdaArgument).SingleOrDefault(expression => expression != null);
                if (lambdaExpression != null)
                {
                    switch (lambdaExpression.Body.NodeType)
                    {
                        case ExpressionType.New:
                        case ExpressionType.MemberInit:
                            Visit(lambdaExpression.Body);
                            break;
                        default:
                            ProcessPossibleFields(new[] { lambdaExpression.Body });
                            break;
                    }
                }

                return node;
            }

            return base.VisitMethodCall(node);
        }

        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            Visit(node.Body);
            return node;
        }

        protected override Expression VisitNew(NewExpression node)
        {
            ProcessPossibleFields(node.Arguments);
            return node;
        }

        protected override Expression VisitMemberInit(MemberInitExpression node)
        {
            var memberAssignments = node.Bindings.Cast<MemberAssignment>().Select(assignment => assignment.Expression).ToArray();
            ProcessPossibleFields(memberAssignments);
            return node;
        }

        private static bool TryCastAndExecute<TExpression>(Expression expression, Action<TExpression> action) 
            where TExpression : class
        {
            var casted = expression as TExpression;
            if (casted == null)
            {
                return false;
            }

            action.Invoke(casted);
            return true;
        }

        private static LambdaExpression GetLambdaArgument(Expression expression)
        {
            var ue = expression as UnaryExpression;
            if (ue != null && ue.NodeType == ExpressionType.Quote)
            {
                return ue.Operand as LambdaExpression;
            }

            return expression as LambdaExpression;
        }

        private void ProcessPossibleFields(IEnumerable<Expression> fields)
        {
            foreach (var field in fields)
            {
                var result = TryCastAndExecute<MemberExpression>(field, _fields.Add)
                             || TryCastAndExecute<BinaryExpression>(field, _fields.Add)
                             || TryCastAndExecute<UnaryExpression>(field, _fields.Add)
                             || TryCastAndExecute<ParameterExpression>(field, _fields.Add)
                             || TryCastAndExecute<ConditionalExpression>(field, _fields.Add)
                             || TryCastAndExecute<ConstantExpression>(field, _fields.Add)
                             || TryCastAndExecute<MethodCallExpression>(field, f => VisitMethodCall(f))
                             || TryCastAndExecute<NewExpression>(field, f => Visit(f))
                             || TryCastAndExecute<MemberInitExpression>(field, f => Visit(f));

                if (!result)
                {
                    var message = string.Format("Unknown expression {0} of type {1}", field, field.GetType().FullName);
                    throw new InvalidOperationException(message);
                }
            }
        }
    }
}