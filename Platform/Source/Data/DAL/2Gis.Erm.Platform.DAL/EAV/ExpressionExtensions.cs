using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.Platform.DAL.EAV
{
    internal static class ExpressionExtensions
    {
        public static Expression Unwrap(this Expression expression)
        {
            var unwrapVisitor = new UnwrapVisitor();
            return unwrapVisitor.Visit(expression);
        }

        public static void Check(this Expression expression)
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

            var returnType = GetSingleItemType(expression.Type);
            if (typeof(IPartable).IsAssignableFrom(returnType))
            {
                throw new ArgumentException(string.Format("IPartable entity {0} should not be selected", returnType.Name));
            }
        }

        private static Type GetSingleItemType(Type type)
        {
            var genericCollectionTypes = new[] { typeof(IEnumerable<>), typeof(IQueryable<>) };
            var collectionType = type.GetInterfaces().FirstOrDefault(i => i.IsGenericType &&
                                                                          genericCollectionTypes.Contains(i.GetGenericTypeDefinition()));

            return collectionType != null ? collectionType.GetGenericArguments().Single() : type;
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
        private static readonly IEnumerable<MethodInfo> QueryableSelectMethods =
            typeof(Queryable).GetMethods(BindingFlags.Static | BindingFlags.Public)
                             .Where(m => m.Name == "Select" || m.Name == "SelectMany");

        private static readonly IEnumerable<MethodInfo> EnumerableSelectMethods =
            typeof(Enumerable).GetMethods(BindingFlags.Static | BindingFlags.Public)
                              .Where(m => m.Name == "Select" || m.Name == "SelectMany");

        private static readonly IEnumerable<MethodInfo> QueryableJoinMethods =
            typeof(Queryable).GetMethods(BindingFlags.Static | BindingFlags.Public)
                             .Where(m => m.Name == "Join" || m.Name == "GroupJoin");

        private static readonly IEnumerable<MethodInfo> EnumerableJoinMethods =
            typeof(Enumerable).GetMethods(BindingFlags.Static | BindingFlags.Public)
                              .Where(m => m.Name == "Join" || m.Name == "GroupJoin");

        private static readonly IEnumerable<MethodInfo> QueryableCountMethods =
            typeof(Queryable).GetMethods(BindingFlags.Static | BindingFlags.Public)
                             .Where(m => m.Name == "Any" || m.Name == "Count");

        private static readonly IEnumerable<MethodInfo> EnumerableCountMethods =
            typeof(Enumerable).GetMethods(BindingFlags.Static | BindingFlags.Public)
                              .Where(m => m.Name == "Any" || m.Name == "Count");

        private static readonly MethodInfo[] SelectMethods = QueryableSelectMethods.Concat(EnumerableSelectMethods).ToArray();
        private static readonly MethodInfo[] JoinMethods = QueryableJoinMethods.Concat(EnumerableJoinMethods).ToArray();
        private static readonly MethodInfo[] AnyMethods = QueryableCountMethods.Concat(EnumerableCountMethods).ToArray();

        private readonly List<Expression> _fields = new List<Expression>();

        public IList<Expression> Fields
        {
            get { return _fields; }
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (IsSelectMethod(node.Method))
            {
                var lambdaExpression = GetLambdaArgument(node.Arguments[1]);
                ProcessSelectBody(lambdaExpression);
                return node;
            }

            if (IsJoinMethod(node.Method))
            {
                var lambdaExpression = GetLambdaArgument(node.Arguments[4]);
                ProcessSelectBody(lambdaExpression);
                return node;
            }

            if (IsAnyMethod(node.Method))
            {
                // Потому что метод возвращает bool, а не сущность
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

        private static bool IsSelectMethod(MethodInfo method)
        {
            return method.IsGenericMethod && SelectMethods.Contains(method.GetGenericMethodDefinition());
        }

        private static bool IsJoinMethod(MethodInfo method)
        {
            return method.IsGenericMethod && JoinMethods.Contains(method.GetGenericMethodDefinition());
        }

        private static bool IsAnyMethod(MethodInfo method)
        {
            return method.IsGenericMethod && AnyMethods.Contains(method.GetGenericMethodDefinition());
        }
        
        private void ProcessSelectBody(LambdaExpression lambdaExpression)
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