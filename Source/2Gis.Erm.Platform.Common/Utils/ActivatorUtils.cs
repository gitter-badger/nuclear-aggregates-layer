using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace DoubleGis.Erm.Platform.Common.Utils
{
    public delegate T ObjectActivator<T>(params object[] args);
    public delegate object ObjectActivator(params object[] args);

    public static class ActivatorUtils
    {
        public static ObjectActivator<T> GetActivator<T>(ConstructorInfo ctor)
        {
            return (ObjectActivator<T>)CreateActivator<ObjectActivator<T>>(ctor);
        }

        public static ObjectActivator GetActivator(ConstructorInfo ctor)
        {
            return (ObjectActivator)CreateActivator<ObjectActivator>(ctor);
        }

        public static object New(this Type input, params object[] args)
        {
            var constructor = ResolveConstructor(input, args);
            var activator = GetActivator(constructor);
            return activator(args);
        }

        public static T New<T>(this Type input, params object[] args)
        {
            var constructor = ResolveConstructor(input, args);
            var activator = GetActivator<T>(constructor);
            return activator(args);
        }

        private static ConstructorInfo ResolveConstructor(Type input, params object[] args)
        {
            var types = args != null ? args.Select(p => p.GetType()) : new Type[0];
            return input.GetConstructor(types.ToArray());
        }

        private static Delegate CreateActivator<T>(ConstructorInfo ctor)
        {
            Type type = ctor.DeclaringType;
            ParameterInfo[] paramsInfo = ctor.GetParameters();

            //create a single param of type object[]
            ParameterExpression param = Expression.Parameter(typeof(object[]), "args");

            var argsExp = new Expression[paramsInfo.Length];

            //pick each arg from the params array 
            //and create a typed expression of them
            for (int i = 0; i < paramsInfo.Length; i++)
            {
                Expression index = Expression.Constant(i);
                Type paramType = paramsInfo[i].ParameterType;

                Expression paramAccessorExp = Expression.ArrayIndex(param, index);

                Expression paramCastExp = Expression.Convert(paramAccessorExp, paramType);

                argsExp[i] = paramCastExp;
            }

            //make a NewExpression that calls the
            //ctor with the args we just created
            NewExpression newExp = Expression.New(ctor, argsExp);

            //create a lambda with the New
            //Expression as body and our param object[] as arg
            LambdaExpression lambda = Expression.Lambda(typeof(T), newExp, param);

            //compile it
            var compiled = lambda.Compile();
            return compiled;
        }
    }
}
