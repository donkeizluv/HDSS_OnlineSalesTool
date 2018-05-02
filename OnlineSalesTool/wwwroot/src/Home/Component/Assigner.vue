<!--Assigner view-->
<template id="assigner">
    <div class="d-flex justify-content-around flex-wrap">
        <!--Use slot-->
        <template v-for="item in days">
            <shift-detail v-bind:users="users" v-bind:readonly="readonly" v-bind:day.sync="item" />
        </template>
    </div>
</template>
<script>
    import shiftdetail from './ShiftDetail.vue'
    export default {
        name: 'assigner',
        template: '#assigner',
        components: {
            'shift-detail': shiftdetail
        },
        mounted: function () {

        },
        props: {
            days: {
                type: Array
            },
            //shifts: {
            //    type: Array
            //},
            //Users to assign to shifts
            users: {
                type: Array
            },
            readonly: {
                type: Boolean,
                default: false
            }
        },
        data: function () {
            return {
                
            };
        },
        computed: {
            DisplayText: function () {
                return 
            }
        },
        methods: {

        }
    }
</script>
<style scoped>
   
</style>