<template id="usermanager">
    <div>
        <!--Search bar-->
        <div class="row">
            <div class="col-sm-8 mx-auto">
                <search-bar v-bind:items="searchFilters"
                            v-on:submit="submitSearch"></search-bar>
            </div>
        </div>
        <!--Listing-->
        <div class="row">
            <div class="col-12">
                <div class="table-responsive">
                    <table class="table table-hover no-top-border">
                        <thead>
                            <tr>
                                <th>
                                    <button class="btn btn-link" v-on:click="orderByClicked('PosName')">
                                        <span v-html="headerOrderState('PosName')"></span>Username
                                    </button>
                                </th>
                                <th>
                                    <button class="btn btn-link" v-on:click="orderByClicked('PosCode')">
                                        <span v-html="headerOrderState('PosCode')"></span>Họ tên
                                    </button>
                                </th>
                                <th>
                                    <button class="btn btn-link" v-on:click="orderByClicked('PosCode')">
                                        <span v-html="headerOrderState('PosCode')"></span>Loại
                                    </button>
                                </th>
                                <th>
                                    <span class="text-primary">HR</span>
                                </th>
                                <th>
                                    <span class="text-primary">SĐT1</span>
                                </th>
                                <th>
                                    <span class="text-primary">SĐT2</span>
                                </th>
                                <th>
                                    <button class="btn btn-link" v-on:click="orderByClicked('BDS')">
                                        <span v-html="headerOrderState('BDS')"></span>Quản lý
                                    </button>
                                </th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr v-for="item in items" v-bind:key="item.PosId">
                                <td>
                                    <span>{{item.Username}}</span>
                                </td>
                                <td>
                                    <span>{{item.Role}}</span>
                                </td>
                                <td>
                                    <span>{{item.HR}}</span>
                                </td>
                                <td>
                                    <span>{{item.Phone}}</span>
                                </td>
                                <td>
                                    <span>{{item.Phone2}}</span>
                                </td>
                                <td>
                                    <span>{{item.BDS.DisplayName}}</span>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                    <page-nav :page-count="totalPages"
                              :click-handler="pageNavClicked"
                              :page-range="2"
                              :prev-text="'Trước'"
                              :force-page="onPage - 1"
                              :next-text="'Sau'"
                              :page-class="'page-item'"
                              :page-link-class="'page-link'"
                              :prev-class="'page-item'"
                              :prev-link-class="'page-link'"
                              :next-class="'page-item'"
                              :next-link-class="'page-link'"
                              :container-class="'pagination pagination-sm no-bottom-margin justify-content-center'">
                    </page-nav>
                </div>
            </div>
        </div>
    </div>
</template>
<script>
    import API from '../API'
    //Actions
    import { LOGOUT, CHECK_TOKEN_EXPIRE } from '../actions'
    //Mutation
    import { VM_USER } from '../mutations'
    //Components
    import SearchBar from './SearchBar.vue'
    import pagenav from 'vuejs-paginate'
    import axios from 'axios'

    export default {
        name: 'userManagerView',
        template: 'usermanager',
        components: {
            'search-bar': SearchBar,
            'page-nav': pagenav
        },
        beforeRouteEnter(to, from, next) {
            //console.log('enter pos man');
            next(async me => {
                if (!me.vm)
                    await me.init();
            })
        },
        computed: {
            //VM
            vm: function () {
                return this.$store.getters.vm_user;
            }
        },
        data: function () {
            return {
                items: [],
                onPage: 1,
                itemPerPage: 10,
                filterBy: '',
                filterString: '',
                orderBy: 'UserName',
                orderAsc: true,
                items: [],
                totalRows: 0,
                totalPages: 0,

                searchFilters: [
                    { name: 'Tên POS', value: 'PosName' },
                    { name: 'Pos code', value: 'PosCode' },
                    { name: 'SĐT', value: 'Phone' },
                    { name: 'BDS', value: 'BDS' }
                ]
            }
        },
        methods: { 
            init: async function () {
                await this.$store.dispatch(CHECK_TOKEN_EXPIRE);
                await this.loadVM();
            },
            loadVM: async function () {
                try {
                    let params = { ...this.getQuery() };
                    //console.log(params);
                    let { data } = await axios.get(API.UserVM, {
                        params
                    });
                    this.$store.commit(VM_USER, data);
                    this.items = data.Items;
                    this.updatePagination(data.TotalPages, data.TotalRows);

                } catch (e) {
                    this.$emit('showerror', 'Tải dữ liệu thất bại, vui lòng liên hệ IT.');
                }
            },
            updatePagination: function (totalPages, totalRows) {
                this.totalPages = totalPages;
                this.totalRows = totalRows;
            },
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
                this.loadItems();
            },
            //order methods
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
        }
    }
</script>
<style scoped>
    
    .no-top-border th{
        border-top: none!important;
    }
</style>