
window.InitPage = function () {
    if (this.isSearchForm)
        return;

    this.on("beforebuild", function () {

        // Удаляем кнопку "Создать" для общего грида
        if (this.ParentType == null) {
            this.currentSettings.ToolbarItems.removeFirstWhere(function(item) { return item.Name === "Create"; });
        }
        
        Ext.apply(this, {
            Merge: function () {
                var params = "dialogWidth:" + 700 + "px; dialogHeight:" + 350 + "px; status:yes; scroll:yes;resizable:yes;";
                var url = '/Russia/LegalPerson/Merge?masterId={0}&subordinateId={1}';
                var selectedItems = this.Items.Grid.getSelectionModel().selections.items;

                if (!selectedItems.length || selectedItems.length == 0 || selectedItems.length > 2) {
                    Ext.MessageBox.show({
                        title: '',
                        msg: Ext.LocalizedResources.NeedToSelectOneOrTwoItems,
                        buttons: Ext.MessageBox.OK,
                        icon: Ext.MessageBox.ERROR
                    });
                    return;
                } 
                window.showModalDialog(String.format(url, selectedItems[0].data.Id, selectedItems.length < 2 ? '' : selectedItems[1].data.Id), null, params);
                this.refresh();
            },
            Assign: function () {
                this.ShowDialogWindowForOneOrMoreEntities("/GroupOperation/Assign/" + this.EntityName, "dialogHeight:300px; dialogWidth:650px; status:yes; scroll:no; resizable:no; ");
            }
        });
    });
};
