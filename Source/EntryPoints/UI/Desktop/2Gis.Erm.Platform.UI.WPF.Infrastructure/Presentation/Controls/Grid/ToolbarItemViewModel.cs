using DoubleGis.Erm.BL.API.Common.Metadata.Old.Dto;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Presentation.Controls.Grid
{
    public sealed class ToolbarItemViewModel
    {
        private readonly ToolbarElementStructure _toolbarJson;

        private ToolbarItemViewModel(ToolbarElementStructure toolbarJson)
        {
            _toolbarJson = toolbarJson;
        }

        public int? SecurityPrivelege
        {
            get { return _toolbarJson.SecurityPrivelege; }
            set { _toolbarJson.SecurityPrivelege = value; }
        }

        public string NameLocaleResourceId
        {
            get { return _toolbarJson.NameLocaleResourceId; }
            set { _toolbarJson.NameLocaleResourceId = value; }
        }

        public bool LockOnInactive
        {
            get { return _toolbarJson.LockOnInactive; }
            set { _toolbarJson.LockOnInactive = value; }
        }

        public bool LockOnNew
        {
            get { return _toolbarJson.LockOnNew; }
            set { _toolbarJson.LockOnNew = value; }
        }

        public string Name
        {
            get { return _toolbarJson.Name; }
            set { _toolbarJson.Name = value; }
        }

        public string ParentName
        {
            get { return _toolbarJson.ParentName; }
            set { _toolbarJson.ParentName = value; }
        }

        public string HideInCardRelatedGrid
        {
            get { return _toolbarJson.HideInCardRelatedGrid; }
            set { _toolbarJson.HideInCardRelatedGrid = value; }
        }

        public string ControlType
        {
            get { return _toolbarJson.ControlType; }
            set { _toolbarJson.ControlType = value; }
        }

        public string Action
        {
            get { return _toolbarJson.Action; }
            set { _toolbarJson.Action = value; }
        }

        public string Icon
        {
            get { return _toolbarJson.Icon; }
            set { _toolbarJson.Icon = value; }
        }

        public bool Disabled
        {
            get { return _toolbarJson.Disabled; }
            set { _toolbarJson.Disabled = value; }
        }

        public string LocalizedName
        {
            get { return _toolbarJson.LocalizedName; }
            set { _toolbarJson.LocalizedName = value; }
        }

        public static ToolbarItemViewModel FromToolbarJson(ToolbarElementStructure toolbarJson)
        {
            return new ToolbarItemViewModel(toolbarJson);
        }
    }
}