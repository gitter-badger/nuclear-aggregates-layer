using Microsoft.Crm.SdkTypeProxy;

namespace DoubleGis.Erm.BLCore.Common.Infrastructure.MsCRM
{
    public sealed class CrmEntityName
    {
        private readonly EntityName _standartName;
        private readonly string _customName;

        public CrmEntityName(EntityName standartName)
        {
            _standartName = standartName;
        }
        
        internal CrmEntityName(string customName)
        {
            _customName = customName;
        }
        
        public bool IsCustom 
        {
            get { return _customName != null; }
        }
        
        public static implicit operator CrmEntityName(EntityName standartEntity)
        {
            return new CrmEntityName(standartEntity);
        }

        public string GetName()
        {
            return _customName ?? _standartName.ToString();
        }

        public override string ToString()
        {
            return _customName ?? _standartName.ToString();
        }
    }
}
