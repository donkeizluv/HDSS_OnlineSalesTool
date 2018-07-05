<template>
    <div>
        <!--Search bar-->
        <div class="row">
            <div class="col-sm-8 mx-auto">
                <search-bar :disabled="loading"
                            :items="searchProps"
                            @submit="submitSearch"></search-bar>
            </div>
        </div>
        <!--Listing-->
        <div class="row mt-3">
            <div class="col-12">
                <div class="table-responsive">
                    <table class="table table-hover">
                        <thead>
                            <tr class="th-text-center th-no-top-border">
                                <th>
                                    <button class="btn btn-sm btn-link text-dark">Tên Kh.</button>
                                </th>
                                <th>
                                    <button class="btn btn-sm btn-link text-dark">CMND</button>
                                </th>
                                <th>
                                    <button class="btn btn-sm btn-link text-dark">SĐT</button>
                                </th>
                                <th>
                                    <button class="btn btn-sm btn-link text-dark">Địa chỉ</button>
                                </th>
                                <th v-if="canSeePosCode">
                                    <button class="btn btn-sm btn-link text-dark">Pos</button>
                                </th>
                                <th>
                                    <button class="btn btn-sm btn-link" @click="orderByClicked('Received')">
                                        <span v-html="headerOrderState('Received')"></span>Ngày
                                    </button>
                                </th>
                                <th>
                                    <button class="btn btn-sm btn-link text-dark">Số bill</button>
                                </th>
                                <th>
                                    <button class="btn btn-sm btn-link text-dark">Số hd.</button>
                                </th>
                                <th>
                                    <button class="btn btn-sm btn-link" @click="orderByClicked('Stage')">
                                        <span v-html="headerOrderState('Stage')"></span>Tình trạng
                                    </button>
                                </th>
                                <th>
                                    <button class="btn btn-sm btn-link text-dark">Action</button>
                                </th>
                                <th class="pl-0 pr-0" v-if="canSeeAssignDetail">
                                    <button class="btn btn-sm btn-link text-dark">Phân bổ</button>
                                </th>
                                <th class="pl-0 pr-0">
                                    <button class="btn btn-sm btn-link text-dark">Chi tiết vay</button>
                                </th>
                            </tr>
                        </thead>
                        <tbody class="td-item-middle td-padding td-font">
                            <template v-if="hasItems">
                                <template v-for="(item, i) in items">
                                    <tr :key="item.OrerId">
                                        <!--Name-->
                                        <td class="text-center">
                                            <div class="mx-auto"
                                                v-b-popover.hover.top="item.Name">{{item.Name}}</div>
                                        </td>
                                        <td class="text-center">
                                            <div class="mx-auto">{{item.NatId}}</div>
                                        </td>
                                        <!-- Phone -->
                                        <td class="text-center">
                                            <div class="mx-auto">{{item.Phone}}</div>
                                        </td>
                                        <!-- Address -->
                                        <td class="text-center">
                                            <div class="width-8 mx-auto text-truncate"
                                                v-b-popover.hover.top="item.Address">{{item.Address}}</div>
                                        </td>
                                        <!-- PosCode -->
                                        <td v-if="canSeePosCode" class="text-center">
                                            <div class="mx-auto">{{item.PosCode}}</div>
                                        </td>
                                        <!-- Received -->
                                        <td class="text-center">
                                            <div class="mx-auto width-6">{{formatDatetime(item.Received)}}</div>
                                        </td>
                                        <!-- Order number -->
                                        <td class="text-center">
                                            <div class="mx-auto">{{item.OrderNumber}}</div>
                                        </td>
                                        <!-- Indus contract -->
                                        <td class="text-center">
                                            <div class="mx-auto ">{{item.Induscontract}}</div>
                                        </td>
                                        <!-- Stage -->
                                        <td class="text-center">
                                            <div class="mx-auto font-italic">{{item.Stage}}</div>
                                        </td>
                                        <!-- Process action -->
                                        <!-- Action set according to role/stage -->
                                        <!-- Makes this a component -->
                                        <td>
                                            <div class="text-center width-3 mx-auto" v-if="actionCode(item.Stage) == stageActions.NoAction">
                                                <span class="fas fa-ellipsis-h"></span>
                                            </div>
                                            <div class="text-center mx-auto" v-if="actionCode(item.Stage) == stageActions.EnterContractNumber">
                                                <div class="form-inline justify-content-center">
                                                    <div class="form-group width-11">
                                                        <div class="input-group">
                                                            <b-form-input size="sm"
                                                                type="text"
                                                                placeholder="Số hd."
                                                                v-model.trim="item.InduscontractTemp"></b-form-input>
                                                                <!--Submit-->
                                                                <span class="input-group-append">
                                                                <b-button variant="success"
                                                                    size="sm"
                                                                    @click="submitContract(item)"
                                                                    :disabled="!canSubmitContract(item)">
                                                                    OK
                                                                </b-button>
                                                            </span>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="text-center width-8 mx-auto" v-if="actionCode(item.Stage) == stageActions.CustomerConfirm">
                                                    <div class="form-inline justify-content-center">
                                                    <div class="form-group">
                                                        <b-button class="mr-2"
                                                            variant="success"
                                                            size="sm"
                                                            @click="customerConfirm(item, true)">
                                                            Đồng ý
                                                        </b-button>
                                                        <b-button variant="warning"
                                                            size="sm"
                                                            @click="customerConfirm(item, false)">
                                                            Từ chối
                                                        </b-button>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="text-center width-8 mx-auto" v-if="actionCode(item.Stage) == stageActions.DocumentConfirm">
                                                    <div class="form-inline justify-content-center">
                                                    <div class="form-group">
                                                        <b-button class="mr-2"
                                                            variant="success"
                                                            size="sm"
                                                            @click="documentConfirm(item, true)">
                                                            Hợp lệ
                                                        </b-button>
                                                        <b-button variant="warning"
                                                            size="sm"
                                                            @click="documentConfirm(item, false)">
                                                            Nhập lại
                                                        </b-button>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="width-14 mx-auto" v-if="actionCode(item.Stage) == stageActions.AssignCase">
                                                <div class="form-inline justify-content-center">
                                                    <div class="form-group">
                                                        <d-select class="width-14"
                                                            v-model="item.assignTo"
                                                            label="DisplayName"
                                                            :api="assignSuggestApi"></d-select>
                                                        <span class="input-group-append">
                                                            <button class="btn btn-sm btn-link"
                                                                :disabled="!item.assignTo"
                                                                @click="assignCase(item)">
                                                                <span class="fas fa-save"></span>
                                                            </button>
                                                        </span>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="text-center width-3 mx-auto" v-if="actionCode(item.Stage) == stageActions.Rejected">
                                                <span class="fas fa-check"></span>
                                            </div>
                                            <div class="text-center width-3 mx-auto" v-if="actionCode(item.Stage) == stageActions.Completed">
                                                <span class="fas fa-check"></span>
                                            </div>
                                        </td>
                                        <td v-if="canSeeAssignDetail" class="text-center">
                                            <button @click="toggleAssignDetail(item.OrderId)"
                                                    class="btn btn-sm btn-link">
                                                <span class="fas" 
                                                    :class="{'fa-minus': isShowingAssignDetail(item.OrderId),
                                                    'fa-plus': !isShowingAssignDetail(item.OrderId)}">
                                                </span>
                                            </button>
                                        </td>
                                        <td class="text-center">
                                            <button @click="toggleLoanDetail(item.OrderId)"
                                                    class="btn btn-sm btn-link">
                                                <span class="fas" 
                                                    :class="{'fa-minus': isShowingLoanDetail(item.OrderId),
                                                    'fa-plus': !isShowingLoanDetail(item.OrderId)}">
                                                </span>
                                            </button>
                                        </td>
                                    </tr>
                                    <tr :key="'loan' + i">
                                        <loan-detail v-show="isShowingLoanDetail(item.OrderId)"
                                            :colspan="totalColumn"
                                            :detail="item"/>
                                    </tr>
                                    <tr v-if="canSeeAssignDetail" :key="'assign' + i" >
                                        <assign-detail v-show="isShowingAssignDetail(item.OrderId)"
                                            :colspan="totalColumn"
                                            :detail="item.AssignUser"/>
                                    </tr>
                                </template>
                            </template>
                            <template v-else>
                                <tr>
                                    <td class="text-center" :colspan="totalColumn">
                                        <span class="font-italic">Chưa có dữ liệu :(</span>
                                    </td>
                                </tr>
                            </template>
                        </tbody>
                        <tfoot>
                            <tr class="td-no-top-border">
                                <td :colspan="totalColumn" class="lastrow-padding">
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
import API from "../../API";
import { Permission, Roles } from "../../AppConst";
import searchBar from "../Shared/SearchBar.vue";
import loanDetail from "./LoanDetail.vue";
import assignDetail from "./AssignDetail.vue";
import pagenav from "vuejs-paginate";
import DynamicSelect from "../Shared/DynamicSelect.vue";
import axios from "axios";
import listingMix from "../../Mixins/listingViewMixin";
import format from "date-fns/format";
import stageActions from "./stageActions";
//Total number of column
const staticColumnCount = 12;

export default {
    name: "CaseView",
    mixins: [listingMix],
    mounted() {
        this.init();
    },
    components: {
        "search-bar": searchBar,
        "d-select": DynamicSelect,
        "page-nav": pagenav,
        "loan-detail": loanDetail,
        "assign-detail": assignDetail
    },
    computed: {
        stageActions() {
            return stageActions;
        },
        canAssign() {
            return this.$store.getters.can(Permission.AssignCase);
        },
        canConfirmCustomer() {
            return this.$store.getters.can(Permission.CustomerConfirm);
        },
        canInputContract() {
            return this.$store.getters.can(Permission.EnterContractNumber);
        },
        canConfirmDocument() {
            return this.$store.getters.can(Permission.DocumentConfirm);
        },
        canSeeAssignDetail() {
            //Hide this for CA since CA only see cases assigned to him
            return this.$store.getters.role !== Roles.CA;
        },
        canSeePosCode() {
            return this.$store.getters.role !== Roles.CA;
        },
        assignSuggestApi() {
            return API.SuggestAssign;
        },
        //Dynamic colspan based on show/hide tr
        totalColumn() {
            let hide = 0;
            if (!this.canSeeAssignDetail) hide--;
            if (!this.canSeePosCode) hide--;
            return staticColumnCount + hide;
        }
    },
    data() {
        return {
            showLoanDetail: [],
            showAssignDetail: [],
            orderBy: "Received",
            searchProps: [
                {
                    name: "Tên Kh.",
                    value: "Name"
                },
                {
                    name: "SĐT",
                    value: "Phone"
                },
                {
                    name: "CMND",
                    value: "NatId"
                },
                {
                    name: "Số Hd.",
                    value: "Induscontract"
                }
            ],
            loading: false
        };
    },

    methods: {
        init() {
            this.loadVM();
        },
        //Overrides mixins
        async loadVM() {
            try {
                let params = { ...this.getQuery() };
                let { data } = await axios.get(API.CaseVM, {
                    params
                });
                //Attach InduscontractTemp
                data.Items.forEach(i => {
                    i.InduscontractTemp = null;
                });
                this.items = data.Items;
                this.updatePagination(data.TotalPages, data.TotalRows);
            } catch (e) {
                this.$emit(
                    "showerror",
                    "Tải dữ liệu thất bại, vui lòng liên hệ IT."
                );
            }
        },
        //Translates stage name to appropriate action
        actionCode(stage) {
            switch (stage) {
                case "NotAssigned":
                    return stageActions.NoAction;
                    break;
                case "CustomerConfirm":
                    return this.canConfirmCustomer
                        ? stageActions.CustomerConfirm
                        : stageActions.NoAction;
                    break;
                case "EnterContractNumber":
                    return this.canInputContract
                        ? stageActions.EnterContractNumber
                        : stageActions.NoAction;
                    break;
                case "WaitForFinalStatus":
                    return stageActions.NoAction;
                    break;
                case "WaitForOnlineBill":
                    return stageActions.NoAction;
                    break;
                case "WaitForDocument":
                    return this.canConfirmDocument
                        ? stageActions.DocumentConfirm
                        : stageActions.NoAction;
                    break;
                case "Reject":
                    return stageActions.Rejected;
                    break;
                case "CustomerReject":
                    return stageActions.Rejected;
                    break;
                case "NotAssignable":
                    return this.canAssign
                        ? stageActions.AssignCase
                        : stageActions.NoAction;
                    break;
                case "Completed":
                    return stageActions.Completed;
                    break;
                default:
                    console.log(`Invalid stage: ${stage}`);
                    return null;
            }
        },
        canSubmitContract(item) {
            //Must not have contract attached
            if (item.Induscontract) return false;
            if (!item.InduscontractTemp) return false;
            return !/[ !@#$%^&*()_+\-=\[\]{};':"\\|,.<>\/?]/.test(
                item.InduscontractTemp
            );
        },
        async submitContract(item) {
            if (!this.canSubmitContract(item)) return;
            //Submit logic
            try {
                await axios.post(API.CaseUpdateIndusContract, {
                    Id: item.OrderId,
                    Contract: item.InduscontractTemp
                });
                //Refresh
                await this.loadVM();
            } catch (error) {
                console.log(error);
                this.$emit("showinfo", "Cập nhật số hợp đồng thất bại");
            }
        },
        async customerConfirm(item, confirm) {
            //Confirm logic
            try {
                await axios.post(API.CaseCustomerConfirm, {
                    Id: item.OrderId,
                    Confirm: confirm
                });
                //Refresh
                await this.loadVM();
            } catch (error) {
                console.log(error);
                this.$emit("showinfo", "Cập nhật thất bại");
            }
        },
        async documentConfirm(item, confirm) {
            //Confirm logic
            try {
                await axios.post(API.CaseDocumentConfirm, {
                    Id: item.OrderId,
                    Confirm: confirm
                });
                //Refresh
                await this.loadVM();
            } catch (error) {
                console.log(error);
                this.$emit("showinfo", "Cập nhật thất bại");
            }
        },
        async assignCase(item) {
            //Assign logic
            //Confirm logic
            try {
                await axios.post(API.CaseAssign, {
                    Id: item.OrderId,
                    UserId: item.assignTo.UserId
                });
                //Refresh
                await this.loadVM();
            } catch (error) {
                console.log(error);
                this.$emit("showinfo", "Chia case thất bại");
            }
        },
        formatDatetime(d) {
            if (!d) return "";
            return format(d, "DD-MM-YYYY");
        },
        //Assign detail
        isShowingAssignDetail(id) {
            if (!id) return false;
            let index = this.showAssignDetail.indexOf(id);
            if (index != -1) {
                return true;
            }
            return false;
        },
        toggleAssignDetail(id) {
            let index = this.showAssignDetail.indexOf(id);
            if (index == -1) {
                //shows
                this.hideAllDetail(id);
                this.showAssignDetail.push(id);
            } else {
                //hides
                this.hideAllDetail(id);
            }
        },
        // Loan detail
        isShowingLoanDetail(id) {
            if (!id) return false;
            let index = this.showLoanDetail.indexOf(id);
            if (index != -1) {
                return true;
            }
            return false;
        },
        toggleLoanDetail(id) {
            let index = this.showLoanDetail.indexOf(id);
            if (index == -1) {
                //shows
                this.hideAllDetail(id);
                this.showLoanDetail.push(id);
            } else {
                //hides
                this.hideAllDetail(id);
            }
        },
        hideAllDetail(id) {
            //hide specific item detail
            let index = this.showLoanDetail.indexOf(id);
            if (index != -1) {
                this.showLoanDetail.splice(index, 1);
            }
            index = -1; //Reuse
            index = this.showAssignDetail.indexOf(id);
            if (index != -1) {
                this.showAssignDetail.splice(index, 1);
            }
        },
        hideAll() {
            while (this.showLoanDetail.length > 0) {
                this.showLoanDetail.pop();
            }
        }
    }
};
</script>
<style scoped>
.td-no-top-border td {
    border-top: none !important;
}
.th-text-center th {
    text-align: center;
}
.td-item-middle tr td {
    vertical-align: middle;
}
.td-padding tr td {
    padding-left: 5px;
    padding-right: 5px;
}
.td-font tr td{
    font-size: .827rem;
}
.width-14 {
    width: 14rem;
}
.width-11 {
    width: 11rem;
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
.vetical-center {
    vertical-align: middle;
}
/* Workaround for overflow y of datatable */
.lastrow-padding {
    height: 150px;
}
</style>00