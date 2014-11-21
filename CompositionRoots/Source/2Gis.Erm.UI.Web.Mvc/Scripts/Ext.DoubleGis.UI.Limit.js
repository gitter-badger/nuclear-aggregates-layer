window.InitPage = function () {
    window.Card.on("beforebuild", function (card) {

        this.CheckDirty = function() {
            if (this.isDirty) {
                Ext.Msg.alert('', Ext.LocalizedResources.CardIsDirtyAlert);
                return false;
            }
            return true;
        };
        
        this.SetStatus = function (status) {
            var params = "dialogWidth:" + 500 + "px; dialogHeight:" + 150 + "px; status:yes; scroll:no;resizable:no;";
            var url = '/Limit/SetStatus';
            var arguments = {
                limitId: Ext.getDom("Id").value,
                status: status
            };

            this.Items.Toolbar.disable();
            window.showModalDialog(url, arguments, params);

            card.refresh(true);
        };

        this.OpenLimit = function () { if (!this.CheckDirty()) return; this.SetStatus('Opened'); };

        this.ApproveLimit = function () { if (!this.CheckDirty()) return; this.SetStatus('Approved'); };

        this.RejectLimit = function () { if (!this.CheckDirty()) return; this.SetStatus('Rejected'); };

        this.RecalculateLimit = function () {
            if (!this.CheckDirty()) return;
            var url = '/Limit/Recalculate';
            url = window.Ext.urlAppend(url, window.Ext.urlEncode({ id: Ext.getDom("Id").value }));
            
            window.Ext.Ajax.request({
                url: url,
                method: 'POST',
                success: function () { this.refresh(); },
                failure: function (xhr) { this.AddNotification(xhr.responseText || xhr.statusText, "CriticalError", "ServerError"); },
                scope: this
            });
        };
    });
    
    window.Card.on("afterbuild", function () {
        if (window.Ext.getDom("ViewConfig_Id").value && window.Ext.getDom("ViewConfig_Id").value != "0") {
            this.Items.TabPanel.add(
                {
                    xtype: "actionshistorytab",
                    pCardInfo:
                    {
                        pTypeName: this.Settings.EntityName,
                        pId: window.Ext.getDom("ViewConfig_Id").value
                    }
                });
        }
    });
};
