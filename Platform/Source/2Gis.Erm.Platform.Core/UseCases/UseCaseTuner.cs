using System;

using DoubleGis.Erm.Platform.API.Core.UseCases;

namespace DoubleGis.Erm.Platform.Core.UseCases
{
    public sealed class UseCaseTuner : IUseCaseTuner
    {
        private readonly IProcessingContext _processingContext;

        public UseCaseTuner(IProcessingContext processingContext)
        {
            _processingContext = processingContext;
        }

        /// <summary>
        /// Указать ожидаемую длительность выполняющегося usecase.
        /// Значение длительности считывается из атрибута UseCase, которым должен быть помечен тип, который хочет указать длительность usecase
        /// </summary>
        void IUseCaseTuner.AlterDuration<THost>()
        {   
            var hostingType = typeof(THost);
            AlterDuration(hostingType);
        }

        private void AlterDuration(Type hostingType)
        {
            #region Замечания
            // Пока не поддерживаем возможность явно указывать duration в качестве аргумента AlterDuration или  выввод duration из метаданных чего либо, кроме host типа выполняющего usecase
            // Причина - наличие жесткого constraint - чтобы использовать AlterDuration, надо прописывать duration через атрибут UseCase - обеспечит нормальные метаданные usecase (пусть пока и в виде атрибутов в коде)
            // Замечание_1 - если есть тип в котором например, есть два метода, с разной ожидаемой продолжительностью uscase - 
            // сейчас нет возможности отдельно управлять duration per method, т.е. более детально чем для всего типа сразу => указывать в атрибуте uscase нужно максимальную ожиджаемую длительность.
            // Появление такого case также может говорить о нарушении SRP - кода в одном типе смешаны настолько разнородные usecase.
            // Замечание_2 - также пока не стали реализовывать вариант, duration выводится по иерархическому принципу, из атрибута UseCase, которым бы можно было пометить метод, свойство, или весь тип
            // причины:
            //     - если использовать только reflection - то не учитываются case когда метаданные исполняемого кода отличаются от compiletime (например, из-за dynamic proxy, interseption и т.п.)
            //     - если использовать [CallerMemberName] - то возможна неоднозначность (если несколько members помечено атрибутом UseCase) => неусточивость к изенениям (добавили метод с таким же именем и пипец)
            //     - комбинация вышеуказанных вариантов намного лучше, но все равно не дает полной гарантии
            //    Вывод - гемор превосходит профит.
            #endregion

            UseCaseDuration specifiedDuration;
            if (!hostingType.TryGetUseCaseDuration(out specifiedDuration))
            {
                throw new InvalidOperationException("Use case duration can't be inferred from type. Attribute " + typeof(UseCaseAttribute) + " have to be specified explicitly for type " + hostingType);
            }

            _processingContext.EnsureDurationConsidered(specifiedDuration);
        }
    }
}
