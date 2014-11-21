
window.InitPage = function () {
    if (this.isSearchForm)
        return;

    this.on("beforebuild", function () {
        Ext.apply(this, {
            
            Qualify: function () {
                this.ShowDialogWindowForOneOrMoreEntities("/GroupOperation/Qualify/" + this.EntityName, "dialogWidth:650px; dialogHeight:300px; status:yes; scroll:no; resizable:no; ");
            },
            
            Disqualify: function () {
                this.ShowDialogWindowForOneOrMoreEntities("/GroupOperation/Disqualify/" + this.EntityName, "dialogWidth:650px; dialogHeight:230px; status:yes; scroll:no; resizable:no; ");
            },
            Assign: function () {
                this.ShowDialogWindowForOneOrMoreEntities("/GroupOperation/Assign/" + this.EntityName, "dialogWidth:450px; dialogHeight:320px; status:yes; scroll:no;resizable:no;");
            },
            
            ChangeTerritory: function () {
                this.ShowDialogWindowForOneOrMoreEntities("/GroupOperation/ChangeTerritory/" + this.EntityName, "dialogWidth:450px; dialogHeight:230px; status:yes; scroll:no; resizable:no; ");
            },

            Merge: function () {

                var params = "dialogWidth:" + 800 + "px; dialogHeight:" + 600 + "px; status:yes; scroll:yes;resizable:yes;";
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

                // FIXME {y.baranihin, 16.06.2014}: Давай все же будем использовать ООП. Пусть будет некий объект в этом scope, в не глобальная функция
                var url = BuildMergeClientsUrl(selectedItems[0].data.Id, selectedItems.length < 2 ? '' : selectedItems[1].data.Id);
                window.showModalDialog(url, null, params);
                this.refresh();
            }
        });
    });
};
