<template id="posmanager">
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
                    <table class="table table-hover">
                        <thead>
                            <tr class="th-text-center th-no-top-border">
                                <th>
                                    <button class="btn btn-link" v-on:click="orderByClicked('PosName')">
                                        <span v-html="headerOrderState('PosName')"></span>Tên POS
                                    </button>
                                </th>
                                <th>
                                    <button class="btn btn-link" v-on:click="orderByClicked('PosCode')">
                                        <span v-html="headerOrderState('PosCode')"></span>Pos Code
                                    </button>
                                </th>
                                <th>
                                    <button class="btn btn-link text-dark">Địa chỉ</button>
                                </th>
                                <th>
                                    <button class="btn btn-link text-dark">Phone</button>
                                </th>
                                <th>
                                    <button class="btn btn-link" v-on:click="orderByClicked('Manager')">
                                        <span v-html="headerOrderState('Manager')"></span>Quản lý
                                    </button>
                                </th>
                            </tr>
                        </thead>
                        <tbody class="td-item-middle">
                            <tr v-for="item in items" v-bind:key="item.PosId">
                                <!--PosName-->
                                <td class="text-center" v-if="isEditMode(item.PosId)">
                                    <input type="text"
                                           class="form-control form-control-sm width-8 mx-auto"
                                           v-model="item.PosName"
                                           v-bind:maxlength="maxFieldLength.posName" />
                                </td>
                                <td class="text-center" v-else>
                                    <div class="width-8 mx-auto">{{item.PosName}}</div>
                                </td>
                                <!--PosCode-->
                                <td class="text-center" v-if="isEditMode(item.PosId)">
                                    <input type="text"
                                           class="form-control form-control-sm width-8 mx-auto"
                                           v-model="item.PosCode"
                                           v-bind:maxlength="maxFieldLength.posCode" />
                                </td>
                                <td class="text-center" v-else>
                                    <div class="width-8 mx-auto">{{item.PosCode}}</div>
                                </td>
                                <!--Address-->
                                <td class="text-center" v-if="isEditMode(item.PosId)">
                                    <input type="text"
                                           class="form-control form-control-sm width-8 mx-auto"
                                           v-model="item.Address"
                                           v-bind:maxlength="maxFieldLength.address" />
                                </td>
                                <td class="text-center" v-else>
                                    <div class="width-8 mx-auto">{{item.Address}}</div>
                                </td>
                                <!--Phone-->
                                <td class="text-center" v-if="isEditMode(item.PosId)">
                                    <input type="text"
                                           class="form-control form-control-sm width-8 mx-auto"
                                           v-model="item.Phone"
                                           v-bind:maxlength="maxFieldLength.phone" />
                                </td>
                                <td class="text-center" v-else>
                                    <div class="width-8 mx-auto">{{item.Phone}}</div>
                                </td>
                                <!--BDS-->
                                <!--Manager-->
                                <td v-if="isEditMode(item.PosId)">
                                    <div class="width-14 mx-auto">
                                        <d-select v-bind:disabled="!canEditManager(item.UserId)"
                                                  v-model="item.BDS"
                                                  v-bind:api="searchSuggestAPI"></d-select>
                                    </div>
                                </td>
                                <td class="text-center" v-else>
                                    <div class="width-14 mx-auto">{{item.BDS? item.BDS.DisplayName : 'N/A'}}</div>
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
    //Components
    import SearchBar from './SearchBar.vue'
    import pagenav from 'vuejs-paginate'
    import axios from 'axios'
    import listingMix from './Shared/listingViewMixins'

    export default {
        name: 'posManagerView',
        template: 'posmanager',
        mixins: [listingMix],
        components: {
            'search-bar': SearchBar,
            'page-nav': pagenav
        },
        mounted: function () {
            this.init();
        },
        data: function () {
            return {
                //items: [],
                //items_copy: [], //To revert cancel update
                //onPage: 1,
                //itemPerPage: 10,
                //filterBy: '',
                //filterString: '',
                orderBy: 'PosName',
                //orderAsc: true,
                //totalRows: 0,
                //totalPages: 0,

                searchFilters: [
                    { name: 'Tên POS', value: 'PosName' },
                    { name: 'Pos code', value: 'PosCode' },
                    { name: 'SĐT', value: 'Phone' },
                    { name: 'BDS', value: 'Manager' }
                ]
            }
        },
        methods: { 
            init: function () {
                this.loadVM();
            },
            loadVM: async function () {
                try {
                    let params = { ...this.getQuery() };
                    //console.log(params);
                    let { data } = await axios.get(API.PosVM, {
                        params
                    });
                    this.items = data.Items;
                    this.updatePagination(data.TotalPages, data.TotalRows);

                } catch (e) {
                    this.$emit('showerror', 'Tải dữ liệu thất bại, vui lòng liên hệ IT.');
                }
            },
            findItemIndex: function (id) {
                let index = this.items.findIndex(x => x.PosId == id);
                if (index == -1) throw 'Cant find POS of id: ' + id;
                return index;
            }
        }
    }
</script>
<style scoped>
    .td-text-center tr td {
        text-align: center;
    }

    .td-item-middle tr td {
        vertical-align: middle;
    }

    .th-no-top-border th {
        border-top: none !important;
    }

    .th-text-center th {
        text-align: center;
    }

    .width-14 {
        width: 14rem;
    }

    .width-10 {
        width: 10rem;
    }

    .width-8 {
        width: 8rem;
    }

    .width-6 {
        width: 6rem;
    }

    .width-5 {
        width: 5rem;
    }

    .width-3 {
        width: 3rem;
    }

    .fixed-height {
        height: 61px;
    }
</style>