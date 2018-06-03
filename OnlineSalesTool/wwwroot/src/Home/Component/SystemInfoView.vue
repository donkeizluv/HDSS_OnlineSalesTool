<template>
    <div>
        <h4>General info goes here</h4>
    </div>
</template>
<script>
import API from "../API";
//Permission
import { Permission } from "../AppConst";
import axios from "axios";

export default {
    name: "systemInfoView",
    mounted: function() {
        this.init();
    },
    computed: {

    },
    data: function() {
        return{

        };     
    },
    methods: {
        init: function() {

        }
    }
};
</script>
<style scoped>

</style>