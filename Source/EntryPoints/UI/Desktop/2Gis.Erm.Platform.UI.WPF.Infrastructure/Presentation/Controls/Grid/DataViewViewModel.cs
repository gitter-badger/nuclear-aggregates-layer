using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Common.Metadata.Old.Dto;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Presentation.Controls.Grid
{
    public sealed class DataViewViewModel
    {
        private readonly DataListStructure _dataViewJson;

        private DataViewViewModel(DataListStructure dataViewJson)
        {
            _dataViewJson = dataViewJson;
        }

        public IEnumerable<GridColumnViewModel> Columns { get; set; }
        public IEnumerable<ToolbarItemViewModel> ToolbarItems { get; set; }

        public string NameLocaleResourceId
        {
            get { return _dataViewJson.NameLocaleResourceId; }
            set { _dataViewJson.NameLocaleResourceId = value; }
        }

        public string Title
        {
            get { return _dataViewJson.Title; }
            set { _dataViewJson.Title = value; }
        }

        public string TitleLocaleResourceId
        {
            get { return _dataViewJson.TitleLocaleResourceId; }
            set { _dataViewJson.TitleLocaleResourceId = value; }
        }

        public string Name
        {
            get { return _dataViewJson.Name; }
            set { _dataViewJson.Name = value; }
        }

        public string Icon
        {
            get { return _dataViewJson.Icon; }
            set { _dataViewJson.Icon = value; }
        }

        public string MainAttribute
        {
            get { return _dataViewJson.MainAttribute; }
            set { _dataViewJson.MainAttribute = value; }
        }

        public short RowsPerPage
        {
            get { return _dataViewJson.RowsPerPage; }
            set { _dataViewJson.RowsPerPage = value; }
        }

        public bool AllowMultiple
        {
            get { return _dataViewJson.AllowMultiple; }
            set { _dataViewJson.AllowMultiple = value; }
        }

        public string ControllerAction
        {
            get { return _dataViewJson.ControllerAction; }
            set { _dataViewJson.ControllerAction = value; }
        }

        public bool ReadOnly
        {
            get { return _dataViewJson.ReadOnly; }
            set { _dataViewJson.ReadOnly = value; }
        }

        public string LocalizedName
        {
            get { return _dataViewJson.LocalizedName; }
            set { _dataViewJson.LocalizedName = value; }
        }

        public string DefaultSortField
        {
            get { return _dataViewJson.DefaultSortField; }
            set { _dataViewJson.DefaultSortField = value; }
        }

        public byte DefaultSortDirection
        {
            get { return _dataViewJson.DefaultSortDirection; }
            set { _dataViewJson.DefaultSortDirection = value; }
        }

        public bool DisableEdit
        {
            get { return _dataViewJson.DisableEdit; }
            set { _dataViewJson.DisableEdit = value; }
        }

        public string DefaultFilter
        {
            get { return _dataViewJson.DefaultFilter; }
            set { _dataViewJson.DefaultFilter = value; }
        }

        public string ExtendedInfo
        {
            get { return _dataViewJson.ExtendedInfo; }
            set { _dataViewJson.ExtendedInfo = value; }
        }

        public static DataViewViewModel FromDataViewJson(DataListStructure dataViewJson)
        {
            return new DataViewViewModel(dataViewJson)
            {
                Columns = dataViewJson.Fields.Select(GridColumnViewModel.FromDataViewFieldJson).ToArray(),
                ToolbarItems = dataViewJson.ToolbarItems.Select(ToolbarItemViewModel.FromToolbarJson).ToArray()
            };
        }
    }
}