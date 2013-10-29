using System;
using System.Collections.Generic;

namespace DoubleGis.Erm.Platform.Model.Metadata.Common
{
    public static class ConfigElementUtils
    {
        public static bool TryGetElementById<TElement>(this IEnumerable<IConfigElement> checkingElements, int targetId, out TElement resultElement)
            where TElement : class, IConfigElement
        {
            return checkingElements.TryGetElement(element => element.ElementIdentity.Id == targetId, out resultElement);
        }

        public static bool TryGetElementById<TElement>(this IConfigElement checkingElement, int targetId, out TElement resultElement)
            where TElement : class, IConfigElement
        {
            return checkingElement.TryGetElement(element => element.ElementIdentity.Id == targetId, out resultElement);
        }

        public static bool TryGetElement<TElement>(this IEnumerable<IConfigElement> checkingElements, Predicate<IConfigElement> predicate, out TElement resultElement)
            where TElement : class, IConfigElement
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

        public static bool TryGetElement<TElement>(this IConfigElement checkingElement, Predicate<IConfigElement> predicate, out TElement resultElement)
            where TElement : class, IConfigElement
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

        public static bool ContainsElement(this IConfigElement checkingElement, int targetId)
        {
            IConfigElement element;
            return checkingElement.TryGetElementById(targetId, out element);
        }

        public static bool ContainsElement(this IEnumerable<IConfigElement> checkingElements, int targetId)
        {
            IConfigElement element;
            return checkingElements.TryGetElementById(targetId, out element);
        }

        /// <summary>
        /// Выполнить какое-то действие над иерархией элементов конфигурации (подходящие элементы подбираются, используя предикат)
        /// Поддерживает greedy/nongreedy режимы ("жадный/нежадный")
        /// </summary>
        public static bool Do(
            this IEnumerable<IConfigElement> elements,
            Predicate<IConfigElement> checker,
            bool isGreedy,
            Action<IConfigElement> applyMethod)
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

        public static void ProcessAll(this IEnumerable<IConfigElement> elements, Action<IConfigElement> applyMethod)
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
