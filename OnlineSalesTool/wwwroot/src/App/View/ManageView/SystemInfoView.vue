<template>
    <div>
        <h4>General info goes here</h4>
    </div>
</template>
<script>
import API from "../../API";
//Permission
import { Permission } from "../../AppConst";
import axios from "axios";

export default {
    name: "systemInfoView",
    mounted() {
        this.init();
    },
    computed: {

    },
    data() {
        return{

        };     
    },
    methods: {
        init() {

        }
    }
};
</script>
<style scoped>

</style>