using System;

using NuClear.Storage.UseCases;

namespace DoubleGis.Erm.Platform.API.Core.UseCases
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public sealed class UseCaseAttribute : Attribute
    {
        public UseCaseAttribute()
        {
            Duration = UseCaseDuration.None;
        } 
        
        /// <summary>
        /// Задает явно длительность операции в попугаях, по умолчанию имеет значение = UseCaseDuration.Normal
        /// </summary>
        public UseCaseDuration Duration { get; set; }
    }
}
