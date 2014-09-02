window.InitPage = function () {
    window.Card.on("beforebuild", function (cardObject) {
        this.Publish = function () {
            if (Ext.getDom("IsPublished").checked) {
                Ext.Msg.alert('', Ext.LocalizedResources.PriceIsAlreadyPublished);
                return;
            }
            var params = "dialogWidth:" + 500 + "px; dialogHeight:" + 150 + "px; status:yes; scroll:no;resizable:no;";
            var url = '/Price/Publish';
            var arguments = {
                priceId: Ext.getDom("Id").value,
                organizationUnitId: Ext.getDom("OrganizationUnitId").value,
                beginDate: Ext.util.Format.reformatDateFromUserLocaleToInvariant(Ext.getDom("BeginDate").value),
                publishDate: Ext.util.Format.reformatDateFromUserLocaleToInvariant(Ext.getDom("PublishDate").value)
            };

            this.Items.Toolbar.disable();
            window.showModalDialog(url, arguments, params);
            this.refresh(true);
        };

        this.Unpublish = function () {
            if (!Ext.getDom("IsPublished").checked) {
                Ext.Msg.alert('', Ext.LocalizedResources.CantUnpublishPriceWhenUnpublished);
                return;
            }
            var params = "dialogWidth:" + 500 + "px; dialogHeight:" + 150 + "px; status:yes; scroll:no;resizable:no;";
            var url = '/Price/Unpublish';
            var arguments = {
                priceId: Ext.getDom("Id").value
            };

            this.Items.Toolbar.disable();
            window.showModalDialog(url, arguments, params);
            window.Card.isDirty = false;
            this.refresh(true);
        };

        this.Copy = function () {
            var params = "dialogWidth:" + 500 + "px; dialogHeight:" + 250 + "px; status:yes; scroll:no;resizable:no;";
            var url = '/Price/Copy';
            var arguments = {
                priceId: Ext.getDom("Id").value
            };

            var nextAction = window.showModalDialog(url, arguments, params);

            if (nextAction == "Close") {
                window.opener.Entity.refresh();
                window.close();
            }

            if (nextAction == "Reload") {
                this.refresh(true);
            }
        };
    });
};