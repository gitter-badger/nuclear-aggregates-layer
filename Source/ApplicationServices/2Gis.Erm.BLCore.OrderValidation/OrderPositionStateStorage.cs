using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using DoubleGis.Erm.BLCore.Resources.Server.Properties;

namespace DoubleGis.Erm.BLCore.OrderValidation
{
    [DebuggerDisplay("{PositionName} of {Order.Number}")]
    public class OrderPositionStateStorage
    {
        public const int Missing = -1;

        private readonly HashSet<KeyValuePair<long, long>> _linkingObjectsOriginal;
        private readonly HashSet<KeyValuePair<long, long>> _linkingObjectsCategoryAncestor;
        private readonly HashSet<KeyValuePair<long, long>> _linkingObjectsNulledParameter;
        private readonly HashSet<KeyValuePair<long, long>> _linkingObjectsCategoryAncestorNulledParameter;

        private readonly List<LinkingObjectData> _linkingObjects;
        private Dictionary<long, long> _parentCategories;

        public OrderPositionStateStorage()
        {
            _linkingObjects = new List<LinkingObjectData>();

            _linkingObjectsOriginal = new HashSet<KeyValuePair<long, long>>();
            _linkingObjectsCategoryAncestor = new HashSet<KeyValuePair<long, long>>();
            _linkingObjectsNulledParameter = new HashSet<KeyValuePair<long, long>>();
            _linkingObjectsCategoryAncestorNulledParameter = new HashSet<KeyValuePair<long, long>>();
        }

        public OrderStateStorage Order { get; set; }
        public long OrderPositionId { get; set; }
        public long OrderPositionAdvertisementId { get; set; }
        public long PositionId { get; set; }
        public string PositionName { get; set; }
        public long PriceId { get; set; }
        public int BeginRelaseNumber { get; set; }
        public int EndReleaseNumber { get; set; }
        public PositionType Type { get; set; }
        public OrderPositionStateStorage ParentPositionStateStorage { get; set; }
        
        public IEnumerable<LinkingObjectData> LinkingObjects
        {
            get { return _linkingObjects; }
        }

        public void AddLinkingObject(LinkingObjectData linkingObject)
        {
            _linkingObjects.Add(linkingObject);

            _linkingObjectsOriginal.Add(linkingObject.Key);

            if (linkingObject.CategoryId != Missing && linkingObject.FirmAddressId != Missing)
            {
                _linkingObjectsNulledParameter.Add(LinkingObjectData.CreateKey(linkingObject.CategoryId, Missing));
                _linkingObjectsNulledParameter.Add(LinkingObjectData.CreateKey(Missing, linkingObject.FirmAddressId));
            }
        }

        public void RegisterParentCategories(Dictionary<long, long> parentCategories)
        {
            _parentCategories = parentCategories;

            foreach (var linkingObject in LinkingObjects.Where(item => item.CategoryId != Missing))
            {
                var currentCategoryId = linkingObject.CategoryId;
                while (parentCategories.ContainsKey(currentCategoryId))
                {
                    currentCategoryId = parentCategories[currentCategoryId];

                    _linkingObjectsCategoryAncestor.Add(LinkingObjectData.CreateKey(currentCategoryId, linkingObject.FirmAddressId));

                    if (linkingObject.FirmAddressId != Missing)
                    {
                        _linkingObjectsCategoryAncestorNulledParameter.Add(LinkingObjectData.CreateKey(currentCategoryId, Missing));
                    }
                }
            }
        }

        public bool ContainsLinkingObjectLike(LinkingObjectData linkingObject)
        {
            if (_linkingObjectsOriginal.Contains(linkingObject.Key))
            {
                return true;
            }

            if (linkingObject.CategoryId != Missing && linkingObject.FirmAddressId != Missing)
            {
                var missingFirmAddressKey = LinkingObjectData.CreateKey(linkingObject.CategoryId, Missing);
                if (_linkingObjectsOriginal.Contains(missingFirmAddressKey))
                {
                    return true;
                }

                if (_linkingObjectsCategoryAncestor.Contains(missingFirmAddressKey))
                {
                    return true;
                }

                var missingCategoryKey = LinkingObjectData.CreateKey(Missing, linkingObject.FirmAddressId);
                if (_linkingObjectsOriginal.Contains(missingCategoryKey))
                {
                    return true;
                }
            }

            if (linkingObject.CategoryId != Missing)
            {
                var currentCategoryId = linkingObject.CategoryId;
                while (_parentCategories.ContainsKey(currentCategoryId))
                {
                    currentCategoryId = _parentCategories[currentCategoryId];

                    var key = LinkingObjectData.CreateKey(currentCategoryId, linkingObject.FirmAddressId);
                    if (_linkingObjectsOriginal.Contains(key))
                    {
                        return true;
                    }

                    if (_linkingObjectsNulledParameter.Contains(key))
                    {
                        return true;
                    }

                    if (linkingObject.FirmAddressId != Missing)
                    {
                        var missingFirmAddressKey = LinkingObjectData.CreateKey(currentCategoryId, Missing);
                        if (_linkingObjectsOriginal.Contains(missingFirmAddressKey))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public string GetDescription(bool rich, bool includeOrderDesctiption = false)
        {
            string template;
            string description;
            switch (Type)
            {
                case PositionType.Child:
                    template = rich ? BLResources.RichChildPositionTypeTemplate : BLResources.ChildPositionTypeTemplate;
                    description = string.Format(template, PositionName, ParentPositionStateStorage.PositionName, ParentPositionStateStorage.OrderPositionId);
                    break;
                default:    
                    template = rich ? BLResources.RichDefaultPositionTypeTemplate : BLResources.DefaultPositionTypeTemplate;
                    description = string.Format(template, PositionName, OrderPositionId);
                    break;
            }

            if (includeOrderDesctiption)
            {
                description = string.Format(BLResources.OrderDescriptionTemplate, description, Order.GetDescription(rich));
            }

            return description;
        }

        public struct LinkingObjectData
        {
            public LinkingObjectData(long categoryId, long firmAddressId)
                : this()
            {
                CategoryId = categoryId;
                FirmAddressId = firmAddressId;
                Key = CreateKey(CategoryId, FirmAddressId);
            }

            public long CategoryId { get; private set; }
            public long FirmAddressId { get; private set; }
            public KeyValuePair<long, long> Key { get; private set; } // Must be struct to achieve the highest look up performance when used as a key in a hashtable

            public static KeyValuePair<long, long> CreateKey(long categoryId, long firmAddressId)
            {
                return new KeyValuePair<long, long>(categoryId, firmAddressId);
            }
        }
    }
}