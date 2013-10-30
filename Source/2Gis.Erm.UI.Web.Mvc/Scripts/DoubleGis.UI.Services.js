Ext.ns("Ext.DoubleGis.UI");
Ext.DoubleGis.UI.Services = Ext.extend(Ext.util.Observable, {
    constructor: function (settings) {
        this.settings = settings;
        this.control = Ext.get(this.settings.renderTo);
        this.mask = new Ext.LoadMask(Ext.get(settings.maskPanel));
        this.makeRequest(this.settings.url, this.renderData);
    },

    renderData: function (data) {
        this.control.clean();
        
        Ext.each(data, function (service) {
            var blockName = 'block_' + service.Name;
            var block = Ext.get(blockName) || this.createBlock(blockName, service);
            var blockRecord = block.createChild({ tag: 'div' });
            var imageContainer = blockRecord.createChild({ tag: 'div', style: 'float: left' });
            if (service.Available) {
                var message = String.format('OK: {0}, v{1}.{2}.{3}.{4}',
                    service.Address, 
                    service.Version['_Major'],
                    service.Version['_Minor'],
                    service.Version['_Build'],
                    service.Version['_Revision']);

                if (service.BusinessLogicAdaptation) {
                    message += ', ' + service.BusinessLogicAdaptation;
                }
                
                imageContainer.createChild({ tag: 'img', src: this.settings.successImage });
                blockRecord.createChild({ tag: 'div', html: message });
            } else {
                imageContainer.createChild({ tag: 'img', src: this.settings.errorImage });
                blockRecord.createChild({ tag: 'div', html: 'NA' });
            }
            
        }, this);
    },
    
    createBlock: function (blockName, service) {
        var block = this.control.createChild({ tag: 'div', id: blockName, 'class': 'row-wrapper', style: 'margin: 5px' });
        var blockHeader = block.createChild({ tag: 'div', html: service.Name });
        return block;
    },

    makeRequest: function (url, successCallback) {
        this.mask.show();
        Ext.Ajax.request({
            method: 'GET',
            url: url,
            scope: this,
            success: function (xhr) {
                this.mask.hide();
                var response = Ext.decode(xhr.responseText);
                successCallback.call(this, response);
            },
            failure: function () {
                this.mask.hide();
                Ext.Msg.show({
                    title: Ext.LocalizedResources.Error,
                    msg: "Сервис метаданных не ответил, но, в целом, всё хорошо",
                    buttons: Ext.Msg.OK,
                    icon: Ext.MessageBox.ERROR
                });
            }
        });
    }
});