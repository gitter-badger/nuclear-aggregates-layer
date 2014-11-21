window.InitPage = function ()
{
    if (this.isSearchForm)
        return;

    this.on("beforebuild", function() {
        Ext.apply(this, {
            SetStatus: function(status) {
                var params = "dialogWidth:" + 500 + "px; dialogHeight:" + 150 + "px; status:yes; scroll:no;resizable:no;";
                var url = '/Limit/SetStatus';
                var selectedItems = this.Items.Grid.getSelectionModel().selections.items;

                if (selectedItems.length == 0) {
                    alert(Ext.LocalizedResources.IsNecessaryChooseLimitForStatusChange);
                    return;
                }

                if (selectedItems.length > 1) {
                    alert(Ext.LocalizedResources.LimitStatusChangeIsNotSupportedAsTheGroupOperation);
                    return;
                }

                var arguments = {
                    limitId: selectedItems[0].data.Id,
                    status: status
                };

                window.showModalDialog(url, arguments, params);
                this.refresh();
            },

            OpenLimit: function() {
                this.SetStatus('Opened');
            },

            ApproveLimit: function() {
                this.SetStatus('Approved');
            },

            RejectLimit: function() {
                this.SetStatus('Rejected');
            }
        });
    });
}