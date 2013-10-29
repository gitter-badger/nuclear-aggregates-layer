using System;
using System.Linq.Expressions;

namespace DoubleGis.Erm.Platform.Common.Utils.Data
{
    public static class ExpressionHelper
    {
        public static Expression<Func<T, bool>> CombineExpressionsByAnd<T>(Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
        {
            return CombineExpressions(expr1, expr2, OperationCombineType.And);
        }

        public static Expression<Func<T, bool>> CombineExpressionsByOr<T>(Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
        {
            return CombineExpressions(expr1, expr2, OperationCombineType.Or);
        }

        private static Expression<Func<T, bool>> CombineExpressions<T>(Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2, OperationCombineType combineType)
        {
            var param = Expression.Parameter(typeof(T), "x");

            var visitor = new ReplacementVisitor(expr1.Parameters[0], param);
            var newExpr1 = visitor.Visit(expr1.Body);

            visitor = new ReplacementVisitor(expr2.Parameters[0], param);
            var newExpr2 = visitor.Visit(expr2.Body);


            if(combineType == OperationCombineType.And)
                return Expression.Lambda<Func<T, bool>>(Expression.AndAlso(newExpr1, newExpr2), param);

            if (combineType == OperationCombineType.Or)
                return Expression.Lambda<Func<T, bool>>(Expression.OrElse(newExpr1, newExpr2), param);

            throw new NotImplementedException(combineType.ToString());
        }

        public enum OperationCombineType
        {
            And,
            Or
        }
        
        private class ReplacementVisitor : ExpressionVisitor
        {
            private readonly Expression _oldExpr;
            private readonly Expression _newExpr;
            public ReplacementVisitor(Expression oldExpr, Expression newExpr)
            {
                _oldExpr = oldExpr;
                _newExpr = newExpr;
            }

            public override Expression Visit(Expression node)
            {
                if (node == _oldExpr)
                    return _newExpr;
                return base.Visit(node);
            }
        }
    }
}
