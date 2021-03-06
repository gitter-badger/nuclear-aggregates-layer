﻿using System;
using System.Linq.Expressions;

namespace NuClear.Storage.Specifications
{
    [Obsolete("Use MapSpecification instead")]
    public class SelectSpecification<TInput, TOutput>
    {
        private readonly Expression<Func<TInput, TOutput>> _selector;

        public SelectSpecification(Expression<Func<TInput, TOutput>> selector)
        {
            _selector = selector;
        }

        internal Expression<Func<TInput, TOutput>> Selector
        {
            get { return _selector; }
        }

        public static implicit operator SelectSpecification<TInput, TOutput>(Expression<Func<TInput, TOutput>> selector)
        {
            return new SelectSpecification<TInput, TOutput>(selector);
        }
    }
}
