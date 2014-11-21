Ext.ns("Ext.ux");


Ext.ux.AsyncOperationClientManager = Ext.extend(Ext.util.Observable,
function ()
{
    var //Private members
        self,
        initialConfig,
        uiConfig,
        operationId,
        timeout = 1200000,
        progressWindow;

    var //Private methods
        poll = function ()
        {
            Ext.Ajax.request({
                method: "POST",
                timeout: timeout,
                url: "/AsyncOperation/GetState",
                params: { operationId: operationId },
                success: onPollSucceeded,
                failure: onPollFailed
            });
        },
        finish = function ()
        {
            if (progressWindow)
                progressWindow.close();
            self.fireEvent("completed");
        },
        onBeginOperationSuccess = function (xhr)
        {
            var response = Ext.decode(xhr.responseText);
            operationId = response.OperationId;
            timeout = response.Timeout * 2;

            progressWindow = new Ext.ux.DetailedProgressWindow({ renderTarget: uiConfig.renderTarget });
            progressWindow.show(uiConfig.operationName, response.Steps);
            poll();
        },
        onBeginOperationFailure = function ()
        {
            Ext.Msg.show({
                title: Ext.LocalizedResources.Error,
                msg: Ext.LocalizedResources.OrderValidationFailed,
                buttons: Ext.Msg.OK,
                icon: Ext.MessageBox.ERROR,
                fn: finish
            });
        },
        onPollSucceeded = function (xhr)
        {
            var response = Ext.decode(xhr.responseText);
            switch (response.Status)
            {
                case "Running":
                    progressWindow.setProgress(response.Data);
                    break;
                case "Completed":
                    self.fireEvent("immediateResultAvailable", response.Data);
                    finish();
                    return;
                case "DefferedResultCompleted":
                    self.fireEvent("deferredResultAvailable", operationId);
                    finish();
                    return;
                case "Failed":
                    Ext.Msg.show({
                        title: Ext.LocalizedResources.Error,
                        msg: Ext.LocalizedResources.OrderValidationFailed,
                        buttons: Ext.Msg.OK,
                        icon: Ext.MessageBox.ERROR,
                        fn: finish
                    });
                    return;
                case "NotFound":
                    Ext.Msg.show({
                        title: Ext.LocalizedResources.Error,
                        msg: Ext.LocalizedResources.OrderValidationFailed,
                        buttons: Ext.Msg.OK,
                        icon: Ext.MessageBox.ERROR,
                        fn: finish
                    });
                    return;
                case "Timeout":
                    //Just continue polling
                    break;
            }
            setTimeout(poll, 0);
        },
        onPollFailed = function ()
        {
            Ext.Msg.show({
                title: Ext.LocalizedResources.Error,
                msg: Ext.LocalizedResources.OrderValidationFailed,
                buttons: Ext.Msg.OK,
                icon: Ext.MessageBox.ERROR,
                fn: finish
            });
        };
    return {
        constructor: function (initialConfigParameter, uiConfigParameter)
        {
            self = this;
            initialConfig = Ext.apply({}, initialConfigParameter);
            uiConfig = Ext.apply({}, uiConfigParameter);
            this.addEvents("deferredResultAvailable");
            this.addEvents("immediateResultAvailable");
            this.addEvents("completed");
        },

        beginOperation: function ()
        {
            Ext.Ajax.request({
                method: "POST",
                url: initialConfig.url,
                timeout: timeout,
                params: initialConfig.params,
                success: onBeginOperationSuccess,
                failure: onBeginOperationFailure
            });
        }
    };
}
());
        