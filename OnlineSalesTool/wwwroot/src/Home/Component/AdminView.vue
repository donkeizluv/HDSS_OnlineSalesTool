<template id="adminview">
    <div class="row">
        <div class="col-lg-12 mx-auto">
            <ul class="nav nav-tabs">
                <li class="nav-item">
                    <router-link v-bind:class="[isActiveRoute('POS')? 'active' : '', 'nav-link']" to="/Manage/POS">POS</router-link>
                </li>
                <li v-bind:class="[isActiveRoute('User')? 'active' : '', 'nav-item']">
                    <router-link class="nav-link" to="/Manage/User">Người dùng</router-link>
                </li>
            </ul>
            <keep-alive>
                <router-view></router-view>
            </keep-alive>
        </div>
    </div>
</template>
<script>
    //https://github.com/cristijora/vue-tabs/issues/44

    export default {
        name: 'admin-view',
        template: '#adminview',
        
        computed: {
            currentRouteName: function () {
                return this.$route.name;
            }
        },
        data: function () {
            return {

            }
        },
        methods: {
            isActiveRoute: function (name) {
                //console.log(this.currentRouteName);
                return this.currentRouteName === name;
            }
        }

    }
</script>
<style scoped>

</style>