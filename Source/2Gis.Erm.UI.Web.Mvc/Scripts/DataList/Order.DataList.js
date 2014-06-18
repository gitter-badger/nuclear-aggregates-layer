SharedOrderDataListOperations = {
    CustomizeToolbar: function () {

        if (this.ParentType) {
            var tbar = this.Items.Grid.getTopToolbar();
            Ext.each(tbar.items.items, function (item) {
                if (item.id == 'DeleteConfirmed') {
                    item.hide();
                }
            });
            tbar.doLayout();
        }
    },
    CheckOrdersReadinessForRelease: function () {
        this.ShowDialogWindow('/Order/CheckOrdersReadinessForReleaseDialog', 'dialogHeight:310px; dialogWidth:530px; status:yes; scroll:no; resizable:no; ', false);
    },
    MakeRegionalAdsDocs: function () {
        this.ShowDialogWindow('/OrderDialogs/MakeRegionalAdsDocs', 'dialogHeight:280px; dialogWidth:530px; status:yes; scroll:no; resizable:no; ', false);
    },
    CloseWithDenial: function () {

        if (!this.EnsureOneSelected()) {
            return;
        }

        var selectedItems = this.GetSelectedItems();
        this.ShowDialogWindow('/Order/CloseWithDenial?id=' + selectedItems[0], 'dialogHeight:280px; dialogWidth:530px; status:yes; scroll:no; resizable:no; ', true);
    },
    Assign: function () {
        this.ShowDialogWindowForOneOrMoreEntities("/GroupOperation/Assign/" + this.EntityName, "dialogHeight:300px; dialogWidth:650px; status:yes; scroll:no; resizable:no; ");
    }
}
