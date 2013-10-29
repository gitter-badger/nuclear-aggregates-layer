using System;
using System.Collections.Generic;

namespace DoubleGis.Erm.Platform.Web.Mvc.ResourceHandling
{
    public struct ResourceAssemblyInfo : IEquatable<ResourceAssemblyInfo>
    {
        public ResourceAssemblyInfo(string assemblyName, string singleResXFile)
            : this(assemblyName, new string[] { singleResXFile })
        {
        }

        public ResourceAssemblyInfo(string assemblyName, IEnumerable<string> resXFiles) : this()
        {
            AssemblyName = assemblyName;
            ResXFiles = resXFiles;
        }

        public string AssemblyName { get; set; }

        public IEnumerable<string> ResXFiles { get; set; }

        #region Equals implementation

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (obj.GetType() != typeof (ResourceAssemblyInfo)) return false;
            return Equals((ResourceAssemblyInfo) obj);
        }

        public bool Equals(ResourceAssemblyInfo other)
        {
            return Equals(other.ResXFiles, ResXFiles) && Equals(other.AssemblyName, AssemblyName);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = (ResXFiles != null ? ResXFiles.GetHashCode() : 0);
                result = (result*397) ^ (AssemblyName != null ? AssemblyName.GetHashCode() : 0);
                return result;
            }
        }

        public static bool operator ==(ResourceAssemblyInfo left, ResourceAssemblyInfo right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ResourceAssemblyInfo left, ResourceAssemblyInfo right)
        {
            return !left.Equals(right);
        }

        #endregion
    }
}