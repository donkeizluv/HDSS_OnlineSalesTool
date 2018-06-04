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
                                            v-bind:class="[{'border border-danger': !item.PosName},
                                                {'border border-success': item.PosName}]"
                                           v-model.trim="item.PosName"
                                           v-bind:maxlength="maxFieldLength.posName" />
                                </td>
                                <td class="text-center" v-else>
                                    <div class="width-8 mx-auto">{{item.PosName}}</div>
                                </td>
                                <!--PosCode-->
                                <td class="text-center" v-if="isEditMode(item.PosId)">
                                    <input type="text"
                                            class="form-control form-control-sm width-8 mx-auto"
                                                v-bind:class="[{'border border-danger': !item.codeChecked},
                                                {'border border-success': item.codeChecked}]"
                                            v-bind:value="item.PosCode.toUpperCase()" 
                                            v-on:input="item.PosCode = $event.target.value.toUpperCase().trim()"
                                            v-on:keyup="checkCode(item.PosId)"
                                            v-bind:maxlength="maxFieldLength.posCode" />
                                </td>
                                <td class="text-center" v-else>
                                    <div class="width-8 mx-auto">{{item.PosCode}}</div>
                                </td>
                                <!--Address-->
                                <td class="text-center" v-if="isEditMode(item.PosId)">
                                    <input type="text"
                                           class="form-control form-control-sm width-8 mx-auto"
                                            v-bind:class="[{'border border-danger': !item.Address},
                                                {'border border-success': item.Address}]"
                                           v-model.trim="item.Address"
                                           v-bind:maxlength="maxFieldLength.address" />
                                </td>
                                <td class="text-center text-truncate" v-else>
                                    <div class="width-8 mx-auto">{{item.Address}}</div>
                                </td>
                                <!--Phone-->
                                <td class="text-center" v-if="isEditMode(item.PosId)">
                                    <input type="text"
                                            class="form-control form-control-sm width-8 mx-auto"
                                            v-bind:class="[{'border border-danger': !item.Phone},
                                                {'border border-success': item.Phone}]"
                                            v-model.trim="item.Phone"
                                            v-bind:maxlength="maxFieldLength.phone" />
                                </td>
                                <td class="text-center" v-else>
                                    <div class="width-8 mx-auto">{{item.Phone}}</div>
                                </td>
                                <!-- Shifts -->
                                <td v-if="isEditMode(item.PosId)" class="width-16">
                                    <span class="d-inline-flex">
                                        <v-select
                                            class="width-16"
                                            v-bind:filterable=false
                                            v-bind:searchable=false
                                            multiple
                                            v-bind:options="shifts"
                                            v-model="item.Shifts"
                                            label="Name">
                                            <template slot="option" slot-scope="option">
                                                {{option.Name}} {{option.ExtName}}
                                            </template>
                                        </v-select>
                                        <light v-bind:state="item.Shifts.length > 0"/>
                                    </span> 
                                </td>
                                <td v-else class="text-center">
                                    <template v-for="s in item.Shifts" >
                                         <popper :options="{placement: 'top'}" v-bind:key="s.Name">
                                            <div class="popper">
                                                {{s.ExtName}}
                                            </div>
                                            <span class="badge badge-info" slot="reference">
                                                {{s.Name}}
                                            </span>
                                        </popper>
                                        &nbsp;
                                    </template>
                                </td>
                                <!--BDS-->
                                <td v-if="isEditMode(item.PosId)" class="width-14">
                                    <span class="d-inline-flex">
                                        <d-select v-model="item.BDS"
                                                class="width-14"
                                                label="DisplayName"
                                                v-bind:api="searchSuggestAPI"></d-select>
                                        <light v-bind:state="item.BDS? true : false"/>
                                    </span>
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
                                                v-bind:class="[{'border border-danger': !newItem.PosName},
                                                    {'border border-success': newItem.PosName}]"
                                                class="form-control form-control-sm width-8 mx-auto"
                                                v-model.trim="newItem.PosName"
                                                v-bind:maxlength="maxFieldLength.posName" />
                                    </div>
                                </td>
                                <!-- Pos code -->
                                <td class="text-center">
                                    <div>
                                        <input type="text"
                                                class="form-control form-control-sm width-8 mx-auto"
                                                v-bind:class="[{'border border-danger': !newItem.codeChecked},
                                                                {'border border-success': newItem.codeChecked}]"
                                                v-bind:value="newItem.PosCode.toUpperCase()" 
                                                v-on:input="newItem.PosCode = $event.target.value.toUpperCase().trim()"
                                                v-on:keyup="checkCode(-1)"
                                                v-bind:maxlength="maxFieldLength.posCode" />
                                    </div>
                                </td>
                                <!-- Address -->
                                <td class="text-center">
                                    <div>
                                        <input type="text"
                                                v-bind:class="[{'border border-danger': !newItem.Address},
                                                    {'border border-success': newItem.Address}]"
                                                class="form-control form-control-sm width-8 mx-auto"
                                                v-model.trim="newItem.Address"
                                                v-bind:maxlength="maxFieldLength.address" />
                                    </div>
                                </td>
                                <!-- Phone -->
                                <td class="text-center">
                                    <div>
                                        <input type="text"
                                                v-bind:class="[{'border border-danger': !newItem.Phone},
                                                    {'border border-success': newItem.Phone}]"
                                                class="form-control form-control-sm width-8 mx-auto"
                                                v-model.trim="newItem.Phone"
                                                v-bind:maxlength="maxFieldLength.phone"/>
                                    </div>
                                </td>
                                <td class="width-16">
                                    <span class="d-inline-flex">
                                        <v-select
                                            class="width-16"
                                            v-bind:filterable=false
                                            v-bind:searchable=false
                                            multiple 
                                            :options="shifts"
                                            v-model="newItem.Shifts"
                                            label="Name">
                                            <template slot="option" slot-scope="option">
                                                {{option.Name}} {{option.ExtName}}
                                            </template>
                                        </v-select>
                                        <light v-bind:state="newItem.Shifts.length > 0"/>
                                    </span>
                                </td>
                                <!--Manager-->
                                <td class="width-14">
                                    <span class="d-inline-flex">
                                        <d-select class="width-14"
                                                v-model="newItem.BDS"
                                                label="DisplayName"
                                                v-bind:api="searchSuggestAPI"></d-select>
                                        <light v-bind:state="newItem.BDS? true : false"/>
                                    </span>
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
                                            :container-class="'pagination no-bottom-margin justify-content-center'">
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
import light from "./ValidLight.vue";
import DynamicSelect from "./DynamicSelect.vue";
import vSelect from "vue-select";
import Popper from 'vue-popperjs';
import pagenav from "vuejs-paginate";
import axios from "axios";
import listingMix from "./Shared/listingViewMixins";
import debounce from 'lodash.debounce'

export default {
    name: "posManagerView",
    template: "posmanager",
    mixins: [listingMix],
    components: {
        "search-bar": SearchBar,
        "page-nav": pagenav,
        "d-select": DynamicSelect,
        "v-select": vSelect,
        "popper": Popper,
        "light": light
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
        // isNewItemPosCodeValid: function(){
        //     return this.newItem.codeChecked;
        // }
    },
    data: function() {
        return {
            orderBy: "PosName",
            newItem: {
                PosCode: '',
                PosName: null,
                Address: null,
                Phone: null,
                BDS: null,
                Shifts: [],
                codeChecked: false //Check PosCode unique
            },
            errors: [],
            shifts: [],
            maxFieldLength: {
                posName: 50,
                posCode: 8, //POS12345
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
                //Attach prop
                data.Items.forEach(element => {
                    element.codeChecked = true;
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
            if (!item.codeChecked ||
                !item.PosCode ||
                !item.PosName ||
                !item.Address ||
                !item.BDS ||
                !item.Phone ||
                item.Shifts.length < 1
            )
                return false;
            return true;
        },
        createItem: async function() {
            if (!this.isNewItemValid || !this.canCreate) return;
            try {
                let newItem = this.clone(this.newItem);
                let { data } = await axios.post(API.CreatePos, newItem);
                //Set newly created id to item
                newItem.PosId = data;
                this.$emit("showsuccess", "Tạo POS mới thành công!");
                this.items.push(newItem);
            } catch (e) {
                // throw e;
                this.$emit("showinfo", "Có lỗi trong quá trình tạo mới.");
            } finally {
                //Clear this anyways
                this.clearNewItem();
            }
        },
        updateItem: async function(id) {
            if (!this.canUpdateItem(id) || !this.canUpdate) return;
            try {
                let index = this.findItemIndex(id);
                let clone = this.clone(this.items[index]);
                // console.log(clone);
                await axios.post(API.UpdatePos, clone);
                //Replace with clone on update success
                this.$set(this.items, index, clone);
                //Exit edit mode
                clone.editMode = false;
                this.$emit("showsuccess", "Chỉnh sửa POS thành công!");
            } catch (e) {
                // throw e;
                this.$emit("showinfo", "Có lỗi trong quá trình chỉnh sửa.");
            }
        },
        canUpdateItem: function(id) {
            if(!this.canUpdate) return;
            let index = this.findItemIndex(id);
            //Must be in Edit mode to save
            if (!this.items[index].editMode) return false;
            //Values check
            return this.checkItem(this.items[index]);
        },
        clearNewItem: function() {
            this.newItem = {
                PosCode: '',
                PosName: null,
                Address: null,
                Phone: null,
                BDS: null,
                Shifts: [],
                codeChecked: false
            };
        },
        findItemIndex: function(id) {
            let index = this.items.findIndex(x => x.PosId == id);
            if (index == -1) throw "Cant find POS of id: " + id;
            return index;
        },
        isPosCodeValid: function(id){
            let index = this.findItemIndex(id);
            return this.items[index].codeChecked;
        },
        checkCode: function(id){
            this.checkCodeDebounce(id, this);
        },
        checkCodeDebounce: debounce(async (id, vm) => {
            let index = -1;
            let item = null;
            //New item
            if(id == -1){ 
                item = vm.newItem;
            }else{
                index = vm.findItemIndex(id);
                item = vm.items[index];
            }
            let code = item.PosCode || '';
            if(!code){
                vm.$set(item, 'codeChecked', false);
                // item.codeChecked  = false;
            }else{
                let { data } = await axios.get(API.CheckCode.replace('{code}', code));
                //-1 means not exist
                vm.$set(item, 'codeChecked', data == -1);
                // item.codeChecked = data == -1;
            }
        }, 300)
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
/* vuejs popper */
.popper {
  width: auto;
  background-color: #fafafa;
  color: #212121;
  text-align: center;
  padding: 4px;
  display: inline-block;
  border-radius: 3px;
  position: absolute;
  font-size: 1rem;
  font-weight: normal;
  border: 1px #969696 solid;
  z-index: 200000;
}
.popper[x-placement^="top"] {
  margin-bottom: 5px;
}
</style>