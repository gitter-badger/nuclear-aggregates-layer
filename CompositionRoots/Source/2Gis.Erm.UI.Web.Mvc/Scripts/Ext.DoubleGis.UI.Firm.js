window.InitPage = function ()
{
    window.Card.on("beforebuild", function ()
    {
        this.ChangeOwner = function ()
        {
            var params = "dialogWidth:450px; dialogHeight:300px; status:yes; scroll:no;resizable:no;";
            var sUrl = "/GroupOperation/Assign/Firm";
            var result = window.showModalDialog(sUrl, [Ext.getDom("Id").value], params);
            if (result === true)
            {
                this.refresh(true);
            }
        };

        this.ChangeTerritory = function ()
        {
            var params = "dialogWidth:450px; dialogHeight:200px; status:yes; scroll:no; resizable:no; ";
            var sUrl = "/GroupOperation/ChangeTerritory/Firm";
            var result = window.showModalDialog(sUrl, [Ext.getDom("Id").value], params);
            if (result === true)
            {
                this.refresh(true);
            }
        };

        this.ChangeFirmClient = function ()
        {
            var params = "dialogWidth:500px; dialogHeight:210px; status:yes; scroll:no; resizable:no; ";
            var url = '/GroupOperation/ChangeClient/Firm';

            this.Items.Toolbar.disable();
            window.showModalDialog(url, [Ext.getDom("Id").value], params);
            window.location.reload();
        };

        this.Qualify = function ()
        {
            var params = "dialogHeight:400px; dialogWidth:650px; status:yes; scroll:no; resizable:no; ";
            var sUrl = "/GroupOperation/Qualify/Firm";
            var result = window.showModalDialog(sUrl, [Ext.getDom("Id").value], params);
            if (result === true) {
                this.refresh(true);
            }
        };

        this.AssignWhiteListedAd = function ()
        {
            var self = this;

            var url = "/Grid/Search/Advertisement/Firm/" + Ext.getDom('Id').value + "?extendedInfo=" + encodeURIComponent("firmId=" + Ext.getDom('Id').value + "&isAllowedToWhiteList=true");
            var result = window.showModalDialog(url, null, 'status:no; resizable:yes; dialogWidth:900px; dialogHeight:500px; resizable: yes; scroll: no; location:yes;');
            if (result && result.items.length && result.items.length == 1)
            {
                window.Ext.Ajax.request({
                    method: 'POST',
                    url: '/Advertisement/SelectWhiteListedAd/',
                    params: { advertisementId: result.items[0].id, firmId: Ext.getDom('Id').value },
                    timeout: 1200000,
                    success: function (xhr, options)
                    {
                        var response = window.Ext.decode(xhr.responseText);
                        if (!response)
                        {
                            window.Ext.Msg.show({
                                title: Ext.LocalizedResources.SelectToWhiteListMatherial,
                                msg: Ext.LocalizedResources.SelectToWhiteListMatherialFailed,
                                buttons: window.Ext.Msg.OK,
                                icon: window.Ext.MessageBox.INFO
                            });
                        }
                        else
                            self.refresh(true);
                    },
                    failure: function (xhr, options)
                    {
                        window.Ext.Msg.show({
                            title: Ext.LocalizedResources.Error,
                            msg: Ext.LocalizedResources.SelectToWhiteListMatherialFailed,
                            buttons: window.Ext.Msg.OK,
                            icon: window.Ext.MessageBox.ERROR
                        });
                    }
                });
            }
        };
    });
    
    });
};
