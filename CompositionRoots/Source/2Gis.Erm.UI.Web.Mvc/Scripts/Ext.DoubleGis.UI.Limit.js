window.InitPage = function () {
    window.Card.on("beforebuild", function (card) {

        this.CheckDirty = function() {
            if (this.isDirty) {
                Ext.Msg.alert('', Ext.LocalizedResources.CardIsDirtyAlert);
                return false;
            }
            return true;
        };

        this.ShowErrorAndRestoreCard = function (xhr) {
            Ext.MessageBox.show({
                title: '',
                msg: xhr.responseText,
                buttons: Ext.MessageBox.OK,
                width: 300,
                icon: Ext.MessageBox.ERROR
            });

            card.Mask.hide();
            card.recalcToolbarButtonsAvailability();
        };

        this.IncreaseLimit = function() {

            this.Items.Toolbar.disable();

            card.Mask.show();

            var limitId = Ext.getDom("Id").value;

            Ext.Ajax.request({
                timeout: 1200000,
                method: 'GET',
                url: '/Limit/IncreaseLimit',
                params: { limitId: limitId },
                limitId: limitId,
                scope: this,
                success: this.ProcessIncreaseLimitResponse,
                failure: this.ShowErrorAndRestoreCard
            });
        };

        this.ProcessIncreaseLimitResponse = function(xhr, options) {
            var limitIncreasingInfo = Ext.decode(xhr.responseText);

            if (!limitIncreasingInfo.IsLimitIncreasingRequired) {
                Ext.MessageBox.alert('', Ext.LocalizedResources.LimitIncreasingIsNotRequired);

                card.Mask.hide();
                this.recalcToolbarButtonsAvailability();
                return;
            }

            var amountToIncreaseLocalized = Ext.util.Format.exNumber(limitIncreasingInfo.AmountToIncrease, Ext.CultureInfo.NumberFormatInfo, false);

            Ext.MessageBox.show({
                title: Ext.LocalizedResources.Alert,
                msg: String.format(Ext.LocalizedResources.LimitWillBeIncreased, amountToIncreaseLocalized),
                width: 300,
                buttons: window.Ext.MessageBox.ContinueCANCEL,
                fn: function(buttonId) {
                    if (buttonId == 'Continue') {
                        Ext.Ajax.request({
                            timeout: 1200000,
                            method: 'POST',
                            url: '/Limit/IncreaseLimit',
                            params: { limitId: options.limitId, amountToIncrease: amountToIncreaseLocalized },
                            success: function() {
                                card.refresh(true);
                            },
                            failure: options.scope.ShowErrorAndRestoreCard
                        });
                    }
                },
                icon: window.Ext.MessageBox.QUESTION
            });
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
    
};
