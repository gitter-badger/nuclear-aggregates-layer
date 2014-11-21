using DoubleGis.Erm.BLCore.API.Common.Metadata.Old.Dto;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Presentation.Controls.Grid
{
    public sealed class GridColumnViewModel
    {
        private readonly DataListColumnStructure _dataViewFieldJson;

        private GridColumnViewModel(DataListColumnStructure dataViewFieldJson)
        {
            _dataViewFieldJson = dataViewFieldJson;
        }

        public string ReferenceEntityName
        {
            set { _dataViewFieldJson.ReferenceEntityName = value; }
        }

        public string ReferenceTo
        {
            get { return _dataViewFieldJson.ReferenceTo; }
            set { _dataViewFieldJson.ReferenceTo = value; }
        }

        public string ReferenceFieldName
        {
            set { _dataViewFieldJson.ReferenceFieldName = value; }
        }

        public string ReferenceKeyField
        {
            get { return _dataViewFieldJson.ReferenceKeyField; }
            set { _dataViewFieldJson.ReferenceKeyField = value; }
        }

        public string NameLocaleResourceId
        {
            get { return _dataViewFieldJson.NameLocaleResourceId; }
            set { _dataViewFieldJson.NameLocaleResourceId = value; }
        }

        public string LocalizedName
        {
            get { return _dataViewFieldJson.LocalizedName; }
            set { _dataViewFieldJson.LocalizedName = value; }
        }

        public bool Hidden
        {
            get { return _dataViewFieldJson.Hidden; }
            set { _dataViewFieldJson.Hidden = value; }
        }

        public string Name
        {
            get { return _dataViewFieldJson.Name; }
            set { _dataViewFieldJson.Name = value; }
        }

        public short Width
        {
            get { return _dataViewFieldJson.Width; }
            set { _dataViewFieldJson.Width = value; }
        }

        public string FieldType
        {
            get { return _dataViewFieldJson.FieldType; }
            set { _dataViewFieldJson.FieldType = value; }
        }

        public string Type
        {
            get { return _dataViewFieldJson.Type; }
            set { _dataViewFieldJson.Type = value; }
        }

        public bool Sortable
        {
            get { return _dataViewFieldJson.Sortable; }
            set { _dataViewFieldJson.Sortable = value; }
        }

        public static GridColumnViewModel FromDataViewFieldJson(DataListColumnStructure dataViewFieldJson)
        {
            return new GridColumnViewModel(dataViewFieldJson);
        }
    }
}