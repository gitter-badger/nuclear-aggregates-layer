using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;

namespace DoubleGis.Erm.Platform.API.Core.Metadata.Security
{
    public sealed class FunctionalAccessRequirement : IAccessRequirement
    {
        private readonly FunctionalPrivilegeName _requirement;

        public FunctionalAccessRequirement(FunctionalPrivilegeName requirement)
        {
            _requirement = requirement;
        }

        public FunctionalPrivilegeName AccessType 
        {
            get { return _requirement; }
        }

        public bool Equals(IAccessRequirement obj)
        {
            var other = obj as FunctionalAccessRequirement;
            return other != null && Equals(other);
        }

        public override bool Equals(object obj)
        {
            var other = obj as FunctionalAccessRequirement;
            return other != null && Equals(other);
        }

        public override int GetHashCode()
        {
            return (int)_requirement;
        }

        private bool Equals(FunctionalAccessRequirement other)
        {
            return _requirement == other._requirement;
        }
    }
}
