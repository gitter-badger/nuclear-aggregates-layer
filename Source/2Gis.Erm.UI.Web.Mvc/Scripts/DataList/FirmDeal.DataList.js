window.InitPage = function() {
    Ext.apply(this, {
        AppendFirms: function() {
            var options = {
                UrlParameters: {
                    HonourDataListFields: true,
                    NameLocaleResourceId: 'DListActiveFirmsToAppend'
                }
            }

            this.Append(options);
        }
    });
}; 