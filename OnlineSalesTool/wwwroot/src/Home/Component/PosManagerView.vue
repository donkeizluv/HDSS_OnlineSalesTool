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
                                    <div class="btn btn-link text-dark">Ca trực</div>
                                </th>
                                <th>
                                    <button class="btn btn-link" v-on:click="orderByClicked('Manager')">
                                        <span v-html="headerOrderState('Manager')"></span>BDS
                                    </button>
                                </th>
                                <th>
                                    <div class="btn btn-link text-dark">Chỉnh sửa</div>
                                </th>
                            </tr>
                        </thead>
                        <tbody class="td-item-middle">
                            <template v-if="hasItems">
                                <tr class="fixed-height" v-for="item in items" v-bind:key="item.PosId">
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
                                <td class="text-center text-truncate" v-else>
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
                                <!-- Shifts -->
                                <td v-if="isEditMode(item.PosId)" class="width-16">
                                   <div>
                                        <v-select
                                            multiple :options="shifts"
                                            v-model="item.Shifts"
                                            label="Name">
                                            <template slot="option" slot-scope="option">
                                                {{option.Name}} {{option.ExtName}}
                                            </template>
                                        </v-select>
                                    </div> 
                                </td>
                                <td v-else class="text-center">
                                    <template v-for="s in item.Shifts" >
                                        <span v-bind:key="s.Name" class="badge badge-success">
                                            {{s.Name}}
                                        </span>
                                        &nbsp;
                                    </template>
                                </td>
                                <!--BDS-->
                                <td v-if="isEditMode(item.PosId)">
                                    <div class="width-14 mx-auto">
                                        <d-select v-model="item.BDS"
                                                label="DisplayName"
                                                v-bind:api="searchSuggestAPI"></d-select>
                                    </div>
                                </td>
                                <td class="text-center" v-else>
                                    <div class="width-14 mx-auto">{{item.BDS? item.BDS.DisplayName : 'N/A'}}</div>
                                </td>
                                <!--CRUD-->
                                    <td class="text-center">
                                        <div class="d-inline">
                                            <button v-if="isEditMode(item.PosId)"
                                                    class="btn btn-sm btn-outline-warning"
                                                    v-on:click="exitEditMode(item.PosId)">
                                                <span class="fas fa-times"></span>
                                            </button>
                                            <!--Enter edit-->
                                            <button v-else
                                                    v-bind:disabled="!canUpdate"
                                                    class="btn btn-sm btn-outline-primary"
                                                    v-on:click="enterEditMode(item.PosId)">
                                                <span class="fas fa-pencil-alt"></span>
                                            </button>
                                            <!--Save changes-->
                                            <button class="btn btn-sm ml-2"
                                                    v-bind:class="{'btn-outline-success': canUpdateItem(item.PosId),
                                                        'btn-outline-secondary': !canUpdateItem(item.PosId)}"
                                                    v-bind:disabled="!canUpdateItem(item.PosId)"
                                                    v-on:click="updateItem(item.PosId)">
                                                <span class="fas fa-save"></span>
                                            </button>
                                        </div>
                                    </td>
                            </tr>
                            </template>
                            <template v-else>
                                <tr>
                                    <td class="text-center font-weight-bold" colspan="7">
                                        <span>Chưa có dữ liệu :(</span>
                                    </td>
                                </tr>
                            </template>
                        </tbody>
                        <!-- New item -->
                        <tfoot v-show="canCreate">
                            <tr>
                                <!-- Pos name -->
                                <td class="text-center">
                                    <div>
                                        <input type="text"
                                                class="form-control form-control-sm width-8 mx-auto"
                                                v-model="newItem.PosName"
                                                v-bind:maxlength="maxFieldLength.posName" />
                                    </div>
                                </td>
                                <!-- Pos code -->
                                <td class="text-center">
                                    <div>
                                        <input type="text"
                                                class="form-control form-control-sm width-8 mx-auto"
                                                v-model="newItem.PosCode"
                                                v-bind:maxlength="maxFieldLength.posCode" />
                                    </div>
                                </td>
                                <!-- Address -->
                                <td class="text-center">
                                    <div>
                                        <input type="text"
                                                class="form-control form-control-sm width-8 mx-auto"
                                                v-model="newItem.Address"
                                                v-bind:maxlength="maxFieldLength.address" />
                                    </div>
                                </td>
                                <!-- Phone -->
                                <td class="text-center">
                                    <div>
                                        <input type="text"
                                                class="form-control form-control-sm width-8 mx-auto"
                                                v-model="newItem.Phone"
                                                v-bind:maxlength="maxFieldLength.phone"/>
                                    </div>
                                </td>
                                <td class="width-16">
                                    <div>
                                        <v-select
                                            multiple 
                                            :options="shifts"
                                            v-model="newItem.Shifts"
                                            label="Name">
                                            <template slot="option" slot-scope="option">
                                                {{option.Name}} {{option.ExtName}}
                                            </template>
                                        </v-select>
                                    </div>
                                </td>
                                <!--Manager-->
                                <td>
                                    <div class="width-14 mx-auto">
                                        <d-select v-model="newItem.BDS"
                                                label="DisplayName"
                                                v-bind:api="searchSuggestAPI"></d-select>
                                    </div>
                                </td>
                                <!--New item actions-->
                                <td class="text-center">
                                    <div class="d-inline">
                                        <!--Clear-->
                                        <button class="btn btn-sm btn-outline-warning"
                                                v-on:click="clearNewItem">
                                            <span class="fas fa-times"></span>
                                        </button>
                                        <!--Create-->
                                        <button class="btn btn-sm ml-2"
                                                v-bind:class="{'btn-outline-success': isNewItemValid,
                                                        'btn-outline-secondary': !isNewItemValid}"
                                                v-bind:disabled="!isNewItemValid"
                                                v-on:click="createItem">
                                            <span class="fas fa-plus"></span>
                                        </button>
                                    </div>
                                </td>
                            <tr>
                            <tr class="td-no-top-border">
                                <td colspan=7 class="lastrow-padding">
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
                                </td>
                            </tr>
                        </tfoot>
                    </table>
                    
                </div>
            </div>
        </div>
    </div>
</template>
<script>
import API from "../API";
//Permission
import { Permission } from "../AppConst";
//Components
import SearchBar from "./SearchBar.vue";
import DynamicSelect from "./DynamicSelect.vue";
import vSelect from "vue-select";
// import multiselect from "vue-multiselect";

import pagenav from "vuejs-paginate";
import axios from "axios";
import listingMix from "./Shared/listingViewMixins";

export default {
    name: "posManagerView",
    template: "posmanager",
    mixins: [listingMix],
    components: {
        "search-bar": SearchBar,
        "page-nav": pagenav,
        "d-select": DynamicSelect,
        "v-select": vSelect
    },
    mounted: function() {
        this.init();
    },
    computed: {
        searchSuggestAPI: function() {
            return API.UserSearchSuggest.replace("{role}", "BDS");
        },
        //Permission
        canUpdate: function() {
            return this.$store.getters.can(Permission.UpdatePOS);
        },
        canCreate: function() {
            return this.$store.getters.can(Permission.CreatePOS);
        },
        //CRUD
        isNewItemValid: function() {
            if (!this.canCreate) return false;
            return this.checkItem(this.newItem);
        }
    },
    data: function() {
        return {
            orderBy: "PosName",
            newItem: {},
            shifts: [],
            maxFieldLength: {
                posName: 50,
                posCode: 20,
                address: 100,
                phone: 20
            },
            searchFilters: [
                { name: "Tên POS", value: "PosName" },
                { name: "Pos code", value: "PosCode" },
                { name: "SĐT", value: "Phone" },
                { name: "BDS", value: "Manager" }
            ]
            // colors:[
            //     'primary',
            //     'secondary',
            //     'success',
            //     'danger',
            //     'warning',
            //     'info',
            //     'dark'
            // ],
            // randomPills: []
        };
    },
    methods: {
        init: function() {
            this.loadVM();
        },
        loadVM: async function() {
            try {
                let params = { ...this.getQuery() };
                //console.log(params);
                //PosListingVM
                let { data } = await axios.get(API.PosVM, {
                    params
                });
                this.items = data.Items;
                this.shifts = data.Shifts;
                this.refreshCopy(); //Refresh clones
                this.updatePagination(data.TotalPages, data.TotalRows);
            } catch (e) {
                this.$emit(
                    "showerror",
                    "Tải dữ liệu thất bại, vui lòng liên hệ IT."
                );
            }
        },
        //New item check
        checkItem: function(item) {
            if (!item) return false;
            if (
                !item.PosCode ||
                !item.PosName ||
                !item.Address ||
                !item.BDS ||
                !item.Phone
            )
                return false;
            return true;
        },
        //Call API
        createItem: async function() {
            if (!this.isNewItemValid) return;
            try {
                let clone = this.clone(this.newItem);
                await axios.post(API.CreatePOS, clone);
                //Clear new item
                this.$emit("showsuccess", "Tạo người dùng mới thành công!");
                
            } catch (e) {
                // throw e;
                this.$emit("showinfo", "Có lỗi trong quá trình tạo mới.");
            } finally {
                //Clear this anyways
                this.clearNewItem();
            }
        },
        //Call API
        updateItem: async function(id) {
            if (!this.canUpdateItem(id)) return;
            try {
                let index = this.findItemIndex(id);
                let clone = this.clone(this.items[index]);
                // console.log(clone);
                await axios.post(API.UpdatePos, clone);
                //Replace with clone on update success
                this.$set(this.items, index, clone);
                //Exit edit mode
                clone.editMode = false;
                this.$emit("showsuccess", "Chỉnh sửa người dùng thành công!");
            } catch (e) {
                // throw e;
                this.$emit("showinfo", "Có lỗi trong quá trình chỉnh sửa.");
            }
        },
        canUpdateItem: function(id) {
            let index = this.findItemIndex(id);
            //Must be in Edit mode to save
            if (!this.items[index].editMode) return false;
            //Values check
            return this.checkItem(this.items[index]);
        },
        clearNewItem: function() {
            this.newItem = {
                PosCode: null,
                PosName: null,
                Address: null,
                Phone: null,
                BDS: null
            };
        },
        findItemIndex: function(id) {
            let index = this.items.findIndex(x => x.PosId == id);
            if (index == -1) throw "Cant find POS of id: " + id;
            return index;
        },
        // composeShiftNames: function(list){
        //     if(!list) return '';
        //     return list.reduce((acc, i) =>
        //         acc = acc + ' ' + i.Name, '');
        // },
        //  //Fun
        // randomPillColor: function(){
        //     if(this.randomPills.length < 1){
        //         //Refill
        //         for(let i=0; i < 10; i++){
        //             let r = this.colors[Math.floor(Math.random() * (this.colors.length - 1))];
        //             this.randomPills.push('badge badge-' + r);
        //         }
        //     }
        //     return this.randomPills.splice(0, 1)[0];
        // }
    }
};
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
.td-no-top-border td {
    border-top: none !important;
}

.th-text-center th {
    text-align: center;
}
.width-16 {
    width: 16rem;
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
/* Workaround for overflow y of datatable */
.lastrow-padding{
    height: 150px;
}
</style>