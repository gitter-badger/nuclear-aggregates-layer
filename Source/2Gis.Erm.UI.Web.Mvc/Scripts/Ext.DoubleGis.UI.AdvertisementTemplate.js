window.InitPage = function () {
    var cardExtension = {
        Publish: function () {
            Ext.MessageBox.show(this.isDirty ? this.createDialogDirtyFormPublish() : this.createDialogPublish());
        },

        Unpublish: function () {
            Ext.MessageBox.show(this.createDialogUnpublish());
        },

        createDialogDirtyFormPublish: function () {
            return this.createDialogOptions(
                Ext.LocalizedResources.AdvertisementTemplatePublishConfirmationLabel,
                Ext.LocalizedResources.AdvertisementTemplatePublishConfirmationWithDirtyForm,
                this.doPublish);
        },

        createDialogPublish: function () {
            return this.createDialogOptions(
                Ext.LocalizedResources.AdvertisementTemplatePublishConfirmationLabel,
                Ext.LocalizedResources.AdvertisementTemplatePublishConfirmation,
                this.doPublish);
        },

        createDialogUnpublish: function () {
            return this.createDialogOptions(
                Ext.LocalizedResources.UnpublishAdvertisementTemplateLabel,
                Ext.LocalizedResources.UnpublishAdvertisementTemplateConfirmation,
                this.doUnpublish);
        },

        createDialogOptions: function (title, message, callback) {
            return {
                title: title,
                msg: message,
                buttons: Ext.MessageBox.YESNO,
                icon: Ext.MessageBox.QUESTION,
                scope: this,
                fn: function (button) {
                    if (button == 'yes') {
                        callback.apply(this);
                    }
                }
            };
        },

        doPublish: function () {
            this.changeStatus('/AdvertisementTemplate/Publish');
        },

        doUnpublish: function () {
            this.changeStatus('/AdvertisementTemplate/Unpublish');
        },

        changeStatus: function (url) {
            var mask = new window.Ext.LoadMask(document.body);
            mask.show();
            Ext.Ajax.request({
                url: url,
                method: 'POST',
                params: { advertisementTemplateId: Ext.getDom('Id').value },
                scope: this,
                success: function (response, opts) {
                    // Позволяет обновить страницу без дополнительного запроса к пользователю. Запрос уже сделали раньше, если он был нужен.
                    Ext.EventManager.un(window, "beforeunload", this.commitClose); 
                    this.refresh(true);
                },
                failure: function (response, opts) {
                    this.AddNotification(response.responseText, 'CriticalError');
                    this.Items.Toolbar.disable();
                    this.Items.TabPanel.disable();
                    mask.hide();
                }
            });
        }
    };

    Ext.apply(window.Card, cardExtension);
};
