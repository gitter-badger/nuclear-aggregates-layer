Ext.grid.CheckColumn = Ext.extend(Object, {
    
    readOnly: false,
    triState: false,
    
    constructor: function (config) {
        Ext.apply(this, config);
        if (!this.id) {
            this.id = Ext.id();
        }
        this.renderer = this.renderer.createDelegate(this);
    },
    
    init: function (grid) {
        this.grid = grid;
        
        this.grid.on('render', function () {
            var view = grid.getView();
            view.mainBody.on('mousedown', this.onMouseDown, this);
        }, this);
    },

    onMouseDown: function (e, t) {

        if (!t.className)
            return;

        if (t.className.indexOf('x-grid3-cc-' + this.id + '-') == -1)
            return;
        
        e.stopEvent();

        if (this.readOnly)
            return;

        var index = this.grid.getView().findRowIndex(t);
        var record = this.grid.store.getAt(index);
        var state = record.data[this.dataIndex];

        var nextState = this.getNextState(state);

        record.set(this.dataIndex, nextState);
    },
    
    getNextState: function (state) {
        var nextState = false;

        if (this.triState) {
            if (state === 'true')
                nextState = 'false';
            else if (state === 'false')
                nextState = '';
            else if (state === '')
                nextState = 'true';
        } else {
            nextState = !state;
        }

        return nextState;
    },

    renderer: function (v, p, record) {
        p.css += ' x-grid3-check-col-td';

        var idClass = 'x-grid3-cc-' + this.id + '-';

        var checkedClass = 'x-grid3-check-col';
        if (v === 'true')
            checkedClass += '-on';
        else if (v === '')
            checkedClass += '-intermediate';
        else if (v === 'false')
            checkedClass += '';
        else if (v)
            checkedClass += '-on';
        else
            checkedClass += '';

        if (this.readOnly)
        {
            var readonlyClass = checkedClass + '-disabled';
            checkedClass += ' ' + readonlyClass;
        }

        return '<div class="' + idClass + ' ' + checkedClass + '">&#160;</div>';
    }
});
