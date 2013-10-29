using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using Microsoft.Practices.Unity;

namespace DoubleGis.Erm.Platform.DI.Common.Extensions
{
    public class QueryableContainerExtension : UnityContainerExtension
    {
        private List<RegisterEventArgs> _registrations;
        private List<RegisterInstanceEventArgs> _instanceRegistrations;
        
        public IList<RegisterEventArgs> Registrations
        {
            get { return new ReadOnlyCollection<RegisterEventArgs>(_registrations); }
        }

        public IList<RegisterInstanceEventArgs> InstanceRegistrations
        {
            get { return new ReadOnlyCollection<RegisterInstanceEventArgs>(_instanceRegistrations); }
        }

        public IList<RegisterEventArgs> GetByScope(string name)
        {
            return _registrations.Where(x => x.Name == name).ToList();
        }

        public IList<string> GetScopes()
        {
            return _registrations.Select(x => x.Name).Distinct().ToList();
        }

        public bool IsTypeRegistered<TFrom, TTo>()
        {
            return _registrations.Exists(e => e.TypeFrom == typeof(TFrom) && e.TypeTo == typeof(TTo));
        }

        public bool IsTypeRegistered<TFrom>()
        {
            return _registrations.Exists(e => e.TypeFrom == typeof(TFrom));
        }
        
        protected override void Initialize()
        {
            _registrations = new List<RegisterEventArgs>();
            _instanceRegistrations = new List<RegisterInstanceEventArgs>();
            Context.Registering += (s, e) => _registrations.Add(e);
            Context.RegisteringInstance += (s, e) => _instanceRegistrations.Add(e);
        }
    }
}
