window.InitPage = function () {
    Ext.apply(this, {
        AppendClients: function () {
            var options = {
                UrlParameters: {
                    HonourDataListFields: true,
                    NameLocaleResourceId: 'DListActiveClientsToAppend',
                    extendedInfo: 'AvailableForLinking=true;ExcludeChildClients=true;'
                }
            }

            this.Append(options);
        }
    });
};