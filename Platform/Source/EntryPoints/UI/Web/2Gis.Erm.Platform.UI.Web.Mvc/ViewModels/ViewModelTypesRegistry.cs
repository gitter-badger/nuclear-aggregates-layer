using System;
using System.Collections.Generic;

namespace DoubleGis.Erm.Platform.UI.Web.Mvc.ViewModels
{
    public class ViewModelTypesRegistry : IViewModelTypesRegistry
    {
        private readonly IEnumerable<Type> _declaredViewModelTypes;

        public ViewModelTypesRegistry(IEnumerable<Type> declaredViewModelTypes)
        {
            _declaredViewModelTypes = declaredViewModelTypes;
        }

        public IEnumerable<Type> DeclaredViewModelTypes
        {
            get { return _declaredViewModelTypes; }
        }
    }
}