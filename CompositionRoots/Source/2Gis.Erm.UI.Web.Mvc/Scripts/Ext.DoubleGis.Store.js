Ext.namespace("Ext.DoubleGis");
Ext.DoubleGis.Store = Ext.extend(Ext.data.Store, {
    constructor: function (config) {
        Ext.DoubleGis.Store.superclass.constructor.call(this, config);
    },
    load: function (options) {
        options = Ext.apply({}, options);
        this.storeOptions(options);
        if (this.remoteSort) {
            options.params = Ext.apply({}, options.params);

            function convertSortAndDirForSending(sort, dir) {
                return (sort + " " + dir);
            }
            if (this.sortInfo) {
                options.params.sort = convertSortAndDirForSending(this.sortInfo.field, this.sortInfo.direction);
            } else if (this.baseParams.sort.constructor === Array && this.baseParams.dir.constructor === Array) {
                var sortArray = [];
                for (var i = 0; i < this.baseParams.sort.length; i++) {
                    sortArray.push(convertSortAndDirForSending(this.baseParams.sort[i], this.baseParams.dir[i]));
                }
                options.params.sort = sortArray;
            } else {
                options.params.sort = convertSortAndDirForSending(this.baseParams.sort, this.baseParams.dir);
            }
        }
        try {
            return this.execute('read', null, options); // <-- null represents rs.  No rs for load actions.
        } catch (e) {
            this.handleException(e);
            return false;
        }
    }
});