using System;
using System.Diagnostics.CodeAnalysis;

using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using NuClear.Model.Common.Entities.Aspects;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

using Newtonsoft.Json;

// ReSharper disable CheckNamespace
namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels
// ReSharper restore CheckNamespace
{
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    public abstract partial class EntityViewModelBase : IConfigurableViewModel
    {
        private EntityViewConfig _viewConfig;

        public EntityViewConfig ViewConfig
        {
            get { return _viewConfig ?? (_viewConfig = new EntityViewConfig()); }
            set { _viewConfig = value; }
        }
        
        public virtual string EntityStatus
        {
            get
            {
                if (IsNew)
                {
                    return BLResources.Create;
                }

                if (IsDeletable && IsDeleted)
                {
                    return BLResources.Deleted;
                }

                if (IsDeactivatable && !IsActive)
                {
                    return BLResources.Inactive;
                }

                return BLResources.Active;
            }
        }

        public abstract bool IsNew { get; }
        public abstract LookupField CreatedBy { get; set; }
        public abstract LookupField ModifiedBy { get; set; }
        public abstract DateTime CreatedOn { get; set; }
        public abstract DateTime? ModifiedOn { get;  set; }
        public abstract byte[] Timestamp { get; set; }
        public abstract bool IsDeleted { get; set; }
        public abstract bool IsActive { get; set; }
        public abstract LookupField Owner { get;  set; }
        public abstract long OwnerCode { get; set; }
        public abstract long OldOwnerCode { get; set; }
        public abstract bool IsAuditable { get; }
        public abstract bool IsDeletable { get; }
        public abstract bool IsCurated { get; }
        public abstract bool IsDeactivatable { get; }

        /// <summary>
        /// ƒанное свойство должно быть вбито сюда до тех пор, пока не будет сделана нормальна€ безопасность. 
        /// —уть его в том, что если возвращаетс€ true, то лукап куратора будет выводитьс€ на UI и будет возможность помен€ть значени€ пол€ "куратор" в модели. 
        /// Ёто дает хоть какую-то защиту
        /// </summary>
        [JsonIgnore]
        public virtual bool IsSecurityRoot
        {
            get { return false; }
        }

        public abstract void LoadDomainEntityDto(IDomainEntityDto domainEntityDto);
        public abstract IDomainEntityDto TransformToDomainEntityDto();

        public void SetEntityStateToken()
        {
            EntityStateToken = string.Empty;
            if (Timestamp != null)
            {
                EntityStateToken = Convert.ToBase64String(Timestamp);
            }
        }

        public void GetEntityStateToken()
        {
            if (!string.IsNullOrWhiteSpace(EntityStateToken))
            {
                Timestamp = Convert.FromBase64String(EntityStateToken);
            }
        }
    }

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    public abstract class EntityViewModelBase<T> : EntityViewModelBase where T : IEntityKey
    {
        private LookupField _createdBy;
        private LookupField _modifiedBy;
        private LookupField _owner;
        private DateTime _createdOn;
        private DateTime? _modifiedOn;
        private bool _isDeleted;
        private bool _isActive;
        private long _ownerCode;
        private long _oldOwnerCode;


        public override bool IsNew
        {
            get
            {
                return Timestamp == null;
            }
        }
        
        public override LookupField CreatedBy
        {
            get
            {
                if (IsAuditable)
                {
                    return _createdBy;
                }

                return null;
            }

            set
            {
                if (IsAuditable)
                {
                    _createdBy = value;
                }
            }
        }

        public override LookupField ModifiedBy
        {
            get
            {
                if (IsAuditable)
                {
                    return _modifiedBy;
                }

                return null;
            }

            set
            {
                if (IsAuditable)
                {
                    _modifiedBy = value;
                }
            }
        }

        public override DateTime CreatedOn
        {
            get
            {
                return IsAuditable ? _createdOn : DateTime.UtcNow;
            }

            set
            {
                if (IsAuditable)
                {
                    _createdOn = value;
                }
            }
        }

        public override DateTime? ModifiedOn
        {
            get
            {
                return IsAuditable ? _modifiedOn : DateTime.UtcNow;
            }

            set
            {
                if (IsAuditable)
                {
                    _modifiedOn = value;
                }
            }
        }

        public override bool IsDeleted
        {
            get
            {
                return IsDeletable && _isDeleted;
            }

            set
            {
                if (IsDeletable)
                {
                    _isDeleted = value;
                }
            }
        }

        [YesNoRadio]
        public override bool IsActive
        {
            get
            {
                return !IsDeactivatable || _isActive;
            }

            set
            {
                if (IsDeactivatable)
                {
                    _isActive = value;
                }
            }
        }

        public override LookupField Owner
        {
            get
            {
                if (IsCurated)
                {
                    return _owner;
                }

                return null;
            }

            set
            {
                if (IsCurated /*&& IsSecurityRoot*/)
                {
                    _owner = value;
                }
                else
                {
                    // FIXME {y.baranihin, 15.01.2013}: нужно более €вно выделить кейс использовани€ IsSecurityRoot, т.к. текуща€ имплементаци€ некорректна
                    throw new NotSupportedException("¬ данной сущности нельз€ указывать куратора.");
                }
            }
        }

        public override long OwnerCode
        {
            get
            {
                if (IsCurated)
                {
                    return _ownerCode;
                }

                return 0;
            }

            set
            {
                if (IsCurated && IsSecurityRoot)
                {
                    _ownerCode = value;
                }
                else
                {
                    throw new NotSupportedException("¬ данной сущности нельз€ указывать куратора.");
                }
            }
        }

        public override long OldOwnerCode
        {
            get
            {
                if (IsCurated)
                {
                    return _oldOwnerCode;
                }

                return 0;
            }

            set
            {
                if (IsCurated && IsSecurityRoot)
                {
                    _oldOwnerCode = value;
                }
                else
                {
                    throw new NotSupportedException("¬ данной сущности нельз€ указывать куратора.");
                }
            }
        }

        [JsonIgnore]
        public override bool IsAuditable
        {
            get { return typeof(IAuditableEntity).IsAssignableFrom(typeof(T)); }
        }

        [JsonIgnore]
        public override bool IsDeletable
        {
            get { return typeof(IDeletableEntity).IsAssignableFrom(typeof(T)); }
        }

        [JsonIgnore]
        public override bool IsCurated
        {
            get { return typeof(ICuratedEntity).IsAssignableFrom(typeof(T)); }
        }

        [JsonIgnore]
        public override bool IsDeactivatable
        {
            get { return typeof(IDeactivatableEntity).IsAssignableFrom(typeof(T)); }
        }
        
        [JsonIgnore]
        public bool IsStateTracking
        {
            get { return typeof(IStateTrackingEntity).IsAssignableFrom(typeof(T)); }
        }
    }
}