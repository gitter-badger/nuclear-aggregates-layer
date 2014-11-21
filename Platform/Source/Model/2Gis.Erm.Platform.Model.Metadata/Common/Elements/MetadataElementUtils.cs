using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features;

namespace DoubleGis.Erm.Platform.Model.Metadata.Common.Elements
{
    public static class MetadataElementUtils
    {
        public static bool TryGetElementById<TElement>(this IEnumerable<IMetadataElement> checkingElements, Uri targetId, out TElement resultElement)
            where TElement : class, IMetadataElement
        {
            return checkingElements.TryGetElement(element => element.Identity.Id == targetId, out resultElement);
        }

        public static bool TryGetElementById<TElement>(this IMetadataElement checkingElement, Uri targetId, out TElement resultElement)
            where TElement : class, IMetadataElement
        {
            return checkingElement.TryGetElement(element => element.Identity.Id == targetId, out resultElement);
        }

        public static bool TryGetElement<TElement>(this IEnumerable<IMetadataElement> checkingElements, Predicate<IMetadataElement> predicate, out TElement resultElement)
            where TElement : class, IMetadataElement
        {
            resultElement = default(TElement);

            foreach (var checkingElement in checkingElements)
            {
                if (checkingElement.TryGetElement(predicate, out resultElement))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool TryGetElement<TElement>(this IMetadataElement checkingElement, Predicate<IMetadataElement> predicate, out TElement resultElement)
            where TElement : class, IMetadataElement
        {
            resultElement = default(TElement);

            if (predicate(checkingElement))
            {
                resultElement = (TElement)checkingElement;
                return true;
            }

            if (checkingElement.Elements != null)
            {
                foreach (var child in checkingElement.Elements)
                {
                    if (child.TryGetElement(predicate, out resultElement))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static TMetadataElement Element<TMetadataElement>(this IMetadataElement element)
            where TMetadataElement : class, IMetadataElement
        {
            return element.Elements.OfType<TMetadataElement>().Single();
        }

        public static bool Uses<TMetadataFeature>(this IMetadataElement element)
            where TMetadataFeature : class, IMetadataFeature
        {
            return element.Features.OfType<TMetadataFeature>().Any();
        }

        public static TMetadataFeature Feature<TMetadataFeature>(this IMetadataElement element)
            where TMetadataFeature : class, IMetadataFeature
        {
            return element.Features.OfType<TMetadataFeature>().Single();
        }

        public static IEnumerable<TMetadataElement> Elements<TMetadataElement>(this IMetadataElement element)
            where TMetadataElement : class, IMetadataElement
        {
            return element.Elements.OfType<TMetadataElement>();
        }

        public static IEnumerable<TMetadataFeature> Features<TMetadataFeature>(this IMetadataElement element)
            where TMetadataFeature : class, IMetadataFeature
        {
            return element.Features.OfType<TMetadataFeature>();
        }

        public static bool ContainsElement(this IMetadataElement checkingElement, Uri targetId)
        {
            IMetadataElement element;
            return checkingElement.TryGetElementById(targetId, out element);
        }

        public static bool ContainsElement(this IEnumerable<IMetadataElement> checkingElements, Uri targetId)
        {
            IMetadataElement element;
            return checkingElements.TryGetElementById(targetId, out element);
        }

        /// <summary>
        /// Выполнить какое-то действие над иерархией элементов конфигурации (подходящие элементы подбираются, используя предикат)
        /// Поддерживает greedy/nongreedy режимы ("жадный/нежадный")
        /// </summary>
        public static bool Do(
            this IEnumerable<IMetadataElement> elements,
            Predicate<IMetadataElement> checker,
            bool isGreedy,
            Action<IMetadataElement> applyMethod)
        {
            foreach (var element in elements)
            {
                if (element == null)
                {
                    continue;
                }

                if (checker(element))
                {
                    applyMethod(element);
                    if (!isGreedy)
                    {
                        return true;
                    }
                }

                if (element.Elements == null)
                {
                    continue;
                }

                if (element.Elements.Do(checker, isGreedy, applyMethod))
                {
                    if (!isGreedy)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static void ProcessAll(this IEnumerable<IMetadataElement> elements, Action<IMetadataElement> applyMethod)
        {
            foreach (var element in elements)
            {
                if (element == null)
                {
                    continue;
                }

                applyMethod(element);

                if (element.Elements == null)
                {
                    continue;
                }

                element.Elements.ProcessAll(applyMethod);
            }
        }
    }
}
