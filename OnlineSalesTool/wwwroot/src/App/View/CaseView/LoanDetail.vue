<template>
     <td class="borderless" :colspan="colspan">
        <b-card class="text-center mb-2 ml-5">
            <b-row class="justify-content-end">
                <b-col>
                    <span class="font-weight-bold">Tên sản phẩm:</span>
                    <span>{{detail.Product}}</span>
                </b-col>
                <b-col>
                    <span class="font-weight-bold">Giá:</span>
                    <span>{{thousandSep(detail.Amount)}}</span>
                </b-col>
                <b-col>
                    <span class="font-weight-bold">Trả trước:</span>
                    <span>{{thousandSep(detail.Paid)}}</span>
                </b-col>
                <b-col>
                    <span class="font-weight-bold">Còn lại:</span>
                    <span>{{thousandSep(detail.LoanAmount)}}</span>
                </b-col>
                <b-col>
                    <span class="font-weight-bold">Kỳ hạn:</span>
                    <span>{{detail.Term}}</span>
                </b-col>
            </b-row>
        </b-card>
     </td>
</template>
<script>
import commonMix from "../../Mixins/commonMixin";
export default {
    name: "LoanDetail",
    mixins: [commonMix],
    props: {
        detail: {
            type: Object,
            required: true
        },
        colspan: {
            type: Number,
            default: 8
        }
    },
    data() {
        return {};
    },
    methods: {}
};
</script>
<style scoped>
.borderless {
    border: none;
}
</style>