window.InitPage = function() {
    Ext.apply(this, {
        AppendFirms: function() {

            var urlComponents = location.pathname.split('/');
            var parentEntityId = urlComponents[5];

            var options = {
                UrlParameters: {
                    HonourDataListFields: true,
                    NameLocaleResourceId: 'DListActiveFirmsToAppend',
                    extendedInfo: "appendToDealId=" + parentEntityId
                }
            }

            this.Append(options);
        }
    });
}; 