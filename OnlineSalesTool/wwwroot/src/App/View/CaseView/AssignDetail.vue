<template>
     <td class="borderless" :colspan="colspan">
        <b-card class="text-center mb-2 ml-5">
            <template v-if="hasData">
                <b-row class="justify-content-end">
                    <b-col>
                        <span class="font-weight-bold">Họ tên:</span>
                        <span>{{detail.DisplayName}}</span>
                    </b-col>
                    <b-col>
                        <span class="font-weight-bold">Tên đăng nhập:</span>
                        <span>{{detail.Username}}</span>
                    </b-col>
                    <b-col>
                        <span class="font-weight-bold">SĐT:</span>
                        <span>{{detail.Phone}}</span>
                    </b-col>
                </b-row>
            </template>
            <template v-else>
                <b-row class="justify-content-end">
                    <b-col>
                        <span class="font-italic">Chưa được phân bổ</span>
                    </b-col>
                </b-row>
            </template>
        </b-card>
     </td>
</template>
<script>
import commonMix from "../../Mixins/commonMixin";
export default {
    name: "AssignDetail",
    props: {
        detail: {
            type: Object,
        },
        colspan: {
            type: Number,
            default: 12
        }
    },
    computed:{
        hasData(){
            return !!this.detail;
        }
    }
};
</script>
<style scoped>
.borderless {
    border: none;
}
</style>