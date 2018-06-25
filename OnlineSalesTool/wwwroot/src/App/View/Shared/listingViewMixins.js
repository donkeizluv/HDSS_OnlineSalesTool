//Shared composition for listing view
export default {
    computed: {
        hasItems: function () {
            return this.items.length > 0;
        }
    },
    data: function () {
        return {
            items: [],
            items_copy: [], //To revert cancel update
            onPage: 1,
            
            filterBy: '',
            filterString: '',
            orderBy: '', //Overide this in components
            orderAsc: true,

            //Only change these through update paging
            itemPerPage: 10, //setting
            totalRows: 0,
            totalPages: 0,
        };
    },
    methods: {
        //Ults
        resetData: function(){
            this.items = [];
            this.items_copy = [];
            this.onPage =  1;
            this.filterBy = '',
            this.filterString = '';
        },
        //Edit mode methods
        //Requires items, items_copy to function properly
        enterEditMode: function (id) {
            let index = this.findItemIndex(id);
            if (index == -1) {
                return;
            }
            this.$set(this.items[index], 'editMode', true)
        },
        isEditMode: function (id) {
            let index = this.findItemIndex(id);
            return !!this.items[index].editMode;
        },
        findItemIndex: function (id) {
            throw 'Overide this in components';
        },
        exitEditMode: function (id) {
            let index = this.findItemIndex(id);
            this.revertItem(index);
        },
        revertItem: function(index){
            let revert = JSON.parse(JSON.stringify(this.items_copy[index]));
            this.$set(this.items, index, revert);
        },
        refreshCopy: function () {
            this.items_copy = this.items.map(i => this.clone(i));
        },
        clone: function (o) {
            return JSON.parse(JSON.stringify(o));
        },
        //End edit mode
        
        updatePagination: function (totalPages, totalRows) {
            this.totalPages = totalPages;
            this.totalRows = totalRows;
        },
        //Order, nav
        getQuery: function () {
            return {
                count: this.itemPerPage,
                page: this.onPage,
                type: this.filterBy,
                contain: this.filterString,
                order: this.orderBy,
                asc: this.orderAsc,
            }
        },
        submitSearch: function (model) {
            this.resetData();
            this.filterBy = model.filter;
            this.filterString = model.text;
            this.loadVM();
        },
        orderByClicked: function (orderBy) {
            //Flip order by
            if (this.orderBy === orderBy) {
                this.orderAsc = !this.orderAsc;
            }
            else {
                //Order this column
                this.orderBy = orderBy;
                this.orderAsc = true;
            }
            this.loadVM();
        },
        pageNavClicked: function (page) {
            this.onPage = page;
            this.loadVM();
        },
        orderByClicked: function (orderBy) {
            //Flip asc
            if (this.orderBy === orderBy) {
                this.orderAsc = !this.orderAsc;
            }
            else {
                //Order this column
                this.orderBy = orderBy;
                this.orderAsc = true;
            }
            this.loadVM();
        },
        headerOrderState: function (orderBy) {
            //console.log(orderBy);
            if (orderBy === this.orderBy) {
                if (this.orderAsc)
                    return '&utrif;';
                return '&dtrif;';
            }
            return '';
        },
        loadVM: function () {
            throw 'Overide this in components';
        }
    }
}