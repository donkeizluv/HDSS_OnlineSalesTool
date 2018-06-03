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
                    <table class="table table-hover">
                        <thead>
                            <tr class="th-text-center th-no-top-border">
                                <th>
                                    <button class="btn btn-link" v-on:click="orderByClicked('Username')">
                                        <span v-html="headerOrderState('Username')"></span>Username
                                    </button>
                                </th>
                                <th>
                                    <button class="btn btn-link" v-on:click="orderByClicked('Name')">
                                        <span v-html="headerOrderState('Name')"></span>Họ tên
                                    </button>
                                </th>
                                <th>
                                    <button class="btn btn-link" v-on:click="orderByClicked('Role')">
                                        <span v-html="headerOrderState('Role')"></span>Loại
                                    </button>
                                </th>
                                <th>
                                    <button class="btn btn-link" v-on:click="orderByClicked('Active')">
                                        <span v-html="headerOrderState('Active')"></span>Kích hoạt
                                    </button>
                                </th>
                                <th>
                                    <button class="btn btn-link text-dark">HR</button>
                                </th>
                                <th>
                                    <button class="btn btn-link text-dark">SĐT 1</button>
                                </th>
                                <th>
                                    <button class="btn btn-link text-dark">SĐT 2</button>
                                </th>
                                <th>
                                    <button class="btn btn-link" v-on:click="orderByClicked('Manager')">
                                        <span v-html="headerOrderState('Manager')"></span>Quản lý
                                    </button>
                                </th>
                                <th>
                                    <div class="btn btn-link text-dark">Chỉnh sửa</div>
                                </th>
                            </tr>
                        </thead>
                        <tbody class="td-item-middle">
                            <template v-if="hasItems">
                                <tr class="fixed-height" v-for="item in items" v-bind:key="item.UserId">
                                    <!--Username-->
                                    <td class="text-center" v-if="isEditMode(item.UserId)">
                                        <input type="text"
                                               class="form-control form-control-sm width-8 mx-auto"
                                               v-model="item.Username"
                                               v-bind:maxlength="maxFieldLength.username" />
                                    </td>
                                    <td class="text-center" v-else>
                                        <div class="width-8 mx-auto">{{item.Username}}</div>
                                    </td>
                                    <!--Name-->
                                    <td class="text-center" v-if="isEditMode(item.UserId)">
                                        <input type="text"
                                               class="form-control form-control-sm width-10 mx-auto"
                                               v-model="item.Name"
                                               v-bind:maxlength="maxFieldLength.name" />
                                    </td>
                                    <td class="text-center" v-else>
                                        <div class="width-10 mx-auto">{{item.Name}}</div>
                                    </td>
                                    <!--Role-->
                                    <td class="text-center">
                                        <div class="width-3 mx-auto">
                                            {{item.Role}}
                                        </div>
                                    </td>
                                    <!--Active-->
                                    <td class="text-center" v-if="isEditMode(item.UserId)">
                                        <input class="width-3 mx-auto" type="checkbox" v-model="item.Active">
                                    </td>
                                    <td class="text-center" v-else>
                                        <div class="width-3 mx-auto">
                                            {{item.Active? 'Có' : 'Không'}}
                                        </div>
                                    </td>
                                    <!--HR-->
                                    <td class="text-center" v-if="isEditMode(item.UserId)">
                                        <input type="text"
                                               class="form-control form-control-sm width-5"
                                               v-model="item.HR"
                                               v-bind:maxlength="maxFieldLength.hr" />
                                    </td>
                                    <td class="text-center" v-else>
                                        <div class="width-5 mx-auto">
                                            {{item.HR}}
                                        </div>
                                    </td>
                                    <!--Phone-->
                                    <td class="text-center" v-if="isEditMode(item.UserId)">
                                        <input type="text"
                                               class="form-control form-control-sm width-8 mx-auto"
                                               v-model="item.Phone"
                                               v-bind:maxlength="maxFieldLength.phone" />
                                    </td>
                                    <td class="text-center" v-else>
                                        <div class="width-8 mx-auto">{{item.Phone}}</div>
                                    </td>
                                    <!--Phone2-->
                                    <td class="text-center" v-if="isEditMode(item.UserId)">
                                        <input type="text"
                                               class="form-control form-control-sm width-8 mx-auto"
                                               v-model="item.Phone2"
                                               v-bind:maxlength="maxFieldLength.phone" />
                                    </td>
                                    <td class="text-center" v-else>
                                        <div class="width-8 mx-auto">{{item.Phone2}}</div>
                                    </td>
                                    <!--Manager-->
                                    <td v-if="isEditMode(item.UserId)">
                                        <div class="width-14 mx-auto">
                                            <d-select v-bind:disabled="!canEditManager(item.UserId)"
                                                      v-model="item.Manager"
                                                      label="DisplayName"
                                                      v-bind:api="searchSuggestAPI"></d-select>
                                        </div>
                                    </td>
                                    <td class="text-center" v-else>
                                        <div class="width-14 mx-auto">{{item.Manager? item.Manager.DisplayName : 'N/A'}}</div>
                                    </td>
                                    <!--CRUD-->
                                    <td class="text-center">
                                        <div class="d-inline">
                                            <button v-if="isEditMode(item.UserId)"
                                                    class="btn btn-sm btn-outline-warning"
                                                    v-on:click="exitEditMode(item.UserId)">
                                                <span class="fas fa-times"></span>
                                            </button>
                                            <!--Enter edit-->
                                            <button v-else
                                                    v-bind:disabled="!canUpdate"
                                                    class="btn btn-sm btn-outline-primary"
                                                    v-on:click="enterEditMode(item.UserId)">
                                                <span class="fas fa-pencil-alt"></span>
                                            </button>
                                            <!--Save changes-->
                                            <button class="btn btn-sm ml-2"
                                                    v-bind:class="{'btn-outline-success': canUpdateItem(item.UserId),
                                                        'btn-outline-secondary': !canUpdateItem(item.UserId)}"
                                                    v-bind:disabled="!canUpdateItem(item.UserId)"
                                                    v-on:click="updateItem(item.UserId)">
                                                <span class="fas fa-save"></span>
                                            </button>
                                        </div>
                                    </td>
                                </tr>
                            </template>
                            <template v-else>
                                <tr>
                                    <td class="text-center font-weight-bold" colspan="9">
                                        <span>Chưa có dữ liệu :(</span>
                                    </td>
                                </tr>
                            </template>
                        </tbody>
                        <!--New item-->
                        <tfoot v-show="canCreate">
                            <tr>
                                <!--Username-->
                                <td class="text-center">
                                    <div>
                                        <input type="text"
                                            class="form-control form-control-sm width-8 mx-auto"
                                            v-model="newItem.Username"
                                            v-bind:maxlength="maxFieldLength.username" />
                                    </div>
                                </td>
                                <!--Name-->
                                <td class="text-center">
                                    <div>
                                        <input type="text"
                                            class="form-control form-control-sm width-10 mx-auto"
                                            v-model="newItem.Name"
                                            v-bind:maxlength="maxFieldLength.name" />
                                    </div>
                                </td>
                                <!--Role-->
                                <td>
                                    <div class="width-6 mx-auto">
                                        <v-select v-model="newItem.Role"
                                                v-bind:options="roles"></v-select>
                                    </div>
                                </td>
                                <!--Active-->
                                <td class="text-center">
                                    <!--New user must be activated-->
                                    <div>
                                        <input class="width-3 mx-auto" type="checkbox" disabled checked>
                                    </div>
                                </td>
                                <!--HR-->
                                <td class="text-center">
                                    <div>
                                        <input type="text"
                                            class="form-control form-control-sm width-5 mx-auto"
                                            v-model="newItem.HR"
                                            v-bind:maxlength="maxFieldLength.hr" />
                                    </div>
                                </td>
                                <!--Phone-->
                                <td class="text-center">
                                    <div>
                                        <input type="text"
                                            class="form-control form-control-sm width-5 mx-auto"
                                            v-model="newItem.Phone"
                                            v-bind:maxlength="maxFieldLength.phone" />
                                    </div>
                                </td>
                                <!--Phone-->
                                <td class="text-center">
                                    <div>
                                        <input type="text"
                                            class="form-control form-control-sm width-5 mx-auto"
                                            v-model="newItem.Phone2"
                                            v-bind:maxlength="maxFieldLength.phone" />
                                    </div>
                                </td>
                                <!--Manager-->
                                <td>
                                    <div class="width-14 mx-auto">
                                        <d-select v-model="newItem.Manager"
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
                            </tr>
                            <tr class="td-no-top-border">
                                <td colspan=9 class="lastrow-padding">
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
import vSelect from "vue-select";
import DynamicSelect from "./DynamicSelect.vue";
import pagenav from "vuejs-paginate";
import axios from "axios";
import listingMix from "./Shared/listingViewMixins";
export default {
    name: "userManagerView",
    template: "usermanager",
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
        //Permission
        canUpdate: function() {
            return this.$store.getters.can(Permission.UpdateUser);
        },
        canCreate: function(){
            return this.$store.getters.can(Permission.CreateUser);
        },
        //API
        searchSuggestAPI: function() {
            return API.UserSearchSuggest.replace("{role}", "BDS");
        },
        //CRUD
        isNewItemValid: function() {
            if(!this.canCreate) return false;
            //use checkItem bc both basically have same validation
            return this.checkItem(this.newItem);
            //return false;
        }
    },
    data: function() {
        return {
            orderBy: "Username",

            //New item
            newItem: {},

            //Validate model field's length
            maxFieldLength: {
                name: 50,
                username: 50,
                hr: 20,
                email: 60,
                phone: 20
            },
            //User role dict
            roles: ["CA", "BDS"],
            searchFilters: [
                { name: "Username", value: "Username" },
                { name: "Họ tên", value: "Name" },
                { name: "SĐT", value: "Phone" },
                { name: "Manager", value: "Manager" }
            ]
        };
    },
    methods: {
        init: function() {
            this.clearNewItem();
            this.loadVM();
        },
        //Overrides mixins
        loadVM: async function() {
            try {
                let params = { ...this.getQuery() };
                //console.log(params);
                let { data } = await axios.get(API.UserVM, {
                    params
                });
                this.items = data.Items;
                this.refreshCopy(); //Refresh clones
                this.updatePagination(data.TotalPages, data.TotalRows);
            } catch (e) {
                this.$emit(
                    "showerror",
                    "Tải dữ liệu thất bại, vui lòng liên hệ IT."
                );
            }
        },
        findItemIndex: function(id) {
            let index = this.items.findIndex(x => x.UserId == id);
            if (index == -1) throw "Cant find items of id: " + id;
            return index;
        },
        //End overrides
        //CRUD
        //control states
        canUpdateItem: function(id) {
            let index = this.findItemIndex(id);
            //Must be in Edit mode to save
            if (!this.items[index].editMode) return false;
            //Values check
            return this.checkItem(this.items[index]);
        },
        canEditManager: function(id) {
            let index = this.findItemIndex(id);
            //Only CA can have manager
            return this.items[index].Role === "CA";
        },
        //Call API
        createItem: async function() {
            if (!this.isNewItemValid) return;
            try {
                let newUser = this.clone(this.newItem);
                // console.log(newUser);
                await axios.post(API.CreateUser, newUser);
                //Clear new item
                this.$emit("showsuccess", "Tạo người dùng mới thành công!");
            } catch (e) {
                throw e;
                this.$emit("showinfo", "Có lỗi trong quá trình tạo mới.");
            } finally {
                //Clear this anyways
                this.clearNewItem();
            }
        },
        updateItem: async function(id) {
            if (!this.canUpdateItem(id)) return;
            try {
                let index = this.findItemIndex(id);
                let clone = this.clone(this.items[index]);
                // console.log(clone);
                await axios.post(API.UpdateUser, clone);
                //Replace with clone on update success
                this.$set(this.items, index, clone);
                //Exit edit mode
                clone.editMode = false;
                this.$emit("showsuccess", "Chỉnh sửa người dùng thành công!");
            } catch (e) {
                throw e;
                this.$emit("showinfo", "Có lỗi trong quá trình chỉnh sửa.");
            }
        },
        //New item check
        checkItem: function(item) {
            if (!item) return false;
            if (!item.Username || !item.Name || !item.HR)
                return false;
            //CA must have manager
            if (item.Role == "CA") {
                if (!item.Manager) return false;
            } else {
                //Non-CA must not have manager
                if (item.Manager) return false;
            }
            return true;
        },
        //Init & clear item
        clearNewItem: function() {
            this.newItem = {
                Username: null,
                Name: null,
                Role: null,
                Active: true,
                HR: null,
                Phone: null,
                Phone2: null,
                Manager: null
            };
        }
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