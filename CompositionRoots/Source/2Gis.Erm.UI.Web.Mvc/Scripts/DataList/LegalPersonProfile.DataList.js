window.InitPage = function () {
    if (this.isSearchForm)
        return;

    this.on("beforebuild", function () {
        Ext.apply(this, {
            MakeMain: function () {
                if (this.Items.Grid.getSelectionModel().selections.items.length != 1) {
                    window.Ext.MessageBox.show({
                        title: '',
                        msg: Ext.LocalizedResources.MustSelectOnlyOneObject,
                        width: 300,
                        buttons: window.Ext.MessageBox.OK,
                        icon: window.Ext.MessageBox.ERROR
                    });

                    return;
                }

                if (this.Items.Grid.getSelectionModel().selections.items[0].data.IsMainProfile) {
                    window.Ext.MessageBox.show({
                        title: '',
                        msg: Ext.LocalizedResources.LegalPersonProfileIsAlreadyMain,
                        width: 300,
                        buttons: window.Ext.MessageBox.OK,
                        icon: window.Ext.MessageBox.ERROR
                    });

                    return;
                }

                Ext.Ajax.request(
                    {
                        timeout: 1200000,
                        url: "/LegalPersonProfile/MakeProfileMain",
                        method: "POST",
                        params: { profileId: this.Items.Grid.getSelectionModel().selections.items[0].data.Id },
                        success: function () {
                            location.reload();
                        }
                    });
            }
        });
    });
}
