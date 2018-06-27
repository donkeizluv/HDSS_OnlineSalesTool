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
                    <table class="table table-hover text-center">
                        <thead>
                            <tr class="th-text-center th-no-top-border">
                                <th>
                                    <button class="btn btn-link" @click="orderByClicked('Name')">
                                        <span v-html="headerOrderState('Name')"></span>Tên Kh.
                                    </button>
                                </th>
                                <th>
                                    <button class="btn btn-link" @click="orderByClicked('Phone')">
                                        <span v-html="headerOrderState('Phone')"></span>SĐT
                                    </button>
                                </th>
                                <th>
                                    <button class="btn btn-link" @click="orderByClicked('Address')">
                                        <span v-html="headerOrderState('Address')"></span>Địa chỉ
                                    </button>
                                </th>
                                <th>
                                    <button class="btn btn-link text-dark">Pos</button>
                                </th>
                                <th>
                                    <button class="btn btn-link text-dark">Ngày</button>
                                </th>
                                <th>
                                    <button class="btn btn-link text-dark">Số hd.</button>
                                </th>
                                <th>
                                    <button class="btn btn-link text-dark">Tình trạng</button>
                                </th>
                                <th>
                                    <button class="btn btn-link text-dark">Số bill</button>
                                </th>
                                <th>
                                    <button class="btn btn-link text-dark">Chi tiết vay</button>
                                </th>
                            </tr>
                        </thead>
                    <tbody class="td-item-middle">
                            <template v-if="hasItems">
                                <template v-for="item in items">
                                    <tr class="fixed-height" :key="item.OrderId">
                                        <!--Name-->
                                        <td class="text-center">
                                            <div class="width-8 mx-auto">{{item.Name}}</div>
                                        </td>
                                        <!-- Phone -->
                                        <td class="text-center">
                                            <div class="width-8 mx-auto">{{item.Phone}}</div>
                                        </td>
                                        <!-- Address -->
                                        <td class="text-center">
                                            <div class="width-8 mx-auto">{{item.Address}}</div>
                                        </td>
                                        <!-- PosCode -->
                                        <td class="text-center">
                                            <div class="width-8 mx-auto">{{item.PosCode}}</div>
                                        </td>
                                        <!-- Received -->
                                        <td class="text-center">
                                            <div class="width-8 mx-auto">{{formatDatetime(item.Received)}}</div>
                                        </td>
                                        <!-- Indus contract -->
                                        <td class="text-center">
                                            <div class="width-8 mx-auto">{{item.Induscontract}}</div>
                                        </td>
                                        <!-- Stage -->
                                        <td class="text-center">
                                            <div class="width-8 mx-auto">{{item.Stage}}</div>
                                        </td>
                                        <!-- Order number -->
                                        <td class="text-center">
                                            <div class="width-8 mx-auto">{{item.OrderNumber}}</div>
                                        </td>
                                        <td>
                                            <button @click="toggleLoanDetail(item.OrderId)"
                                                    class="btn btn-sm btn-link">
                                                <span class="fas" 
                                                    :class="{'fa-minus': isShowingLoanDetail(item.OrderId),
                                                    'fa-plus': !isShowingLoanDetail(item.OrderId)}">
                                                </span>
                                            </button>
                                        </td>
                                    </tr>
                                    <tr :key="item.OrderId + 1" v-show="isShowingLoanDetail(item.OrderId)">
                                        <loan-detail :colspan=9 :detail="item"/>
                                    </tr>
                                </template>
                            </template>
                            <template v-else>
                                <tr>
                                    <td class="text-center font-weight-bold" colspan="9">
                                        <span>Chưa có dữ liệu :(</span>
                                    </td>
                                </tr>
                            </template>
                        </tbody>
                        <tfoot>
                            <tr class="td-no-top-border">
                                <td colspan=9>
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
import API from "../../API";
//Permission
import { Permission } from "../../AppConst";
import searchBar from "../Shared/SearchBar.vue";
import loanDetail from "./LoanDetail.vue";
import pagenav from "vuejs-paginate";
import axios from "axios";
import listingMix from "../../Mixins/listingViewMixin";
import format from "date-fns/format";
export default {
    name: "CaseView",
    mixins: [listingMix],
    mounted: function() {
        this.init();
    },
    components: {
        "search-bar": searchBar,
        "page-nav": pagenav,
        "loan-detail": loanDetail
    },
    computed: {},
    data() {
        return {
            showLoanDetail: [],
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
                    name: "Sản phẩm",
                    value: "Product"
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
        loadVM: async function() {
            try {
                let params = { ...this.getQuery() };
                //console.log(params);
                let { data } = await axios.get(API.CaseVM, {
                    params
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
        formatDatetime(d) {
            if (!d) return "";
            return format(d, "DD-MM-YYYY");
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
                this.hideLoanDetail(id);
                this.showLoanDetail.push(id);
            } else {
                //hides
                this.hideLoanDetail(id);
            }
        },
        hideLoanDetail(id) {
            //hide event details
            let index = this.showLoanDetail.indexOf(id);
            if (index != -1) {
                this.showLoanDetail.splice(index, 1);
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
</style>