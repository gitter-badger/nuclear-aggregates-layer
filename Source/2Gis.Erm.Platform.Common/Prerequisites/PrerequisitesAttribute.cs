using System;

namespace DoubleGis.Erm.Platform.Common.Prerequisites
{
    // FIXME {i.maslennikov, 19.03.2014}: в процессе работы над MMR, уточнить необходимость поддержки такого понятния как prerequisites 
    // COMMENT {i.maslennikov, 21.04.2014}: Согласен. Возможно ли выделить аспект зависимостей?

    /// <summary>
    /// Если к какому то классу применен данный аттрибут, то это говорит о том, что данный класс имеет зависимость какого-то характера от других типов
    /// Одно из типовых применений - сортировка множеств типов (возможно, экземпляров этих типов и т.п.)
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public sealed class PrerequisitesAttribute : Attribute
    {
        private readonly Type[] _prerequisites;

        public PrerequisitesAttribute(params Type[] prerequisites)
        {
            _prerequisites = prerequisites ?? new Type[0];
        }

        public Type[] Prerequisites
        {
            get { return _prerequisites; }
        }
    }
}
