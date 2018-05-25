<template id="adminview">
    <div>
        <div class="row">
            <div class="col-lg-12 mx-auto">
                <ul class="nav nav-tabs">
                    <li class="nav-item" v-for="route in routes" >
                        <router-link v-bind:class="[isActiveRoute(route.name)? 'active' : '', 'nav-link']" 
                                     v-bind:to="route.path">{{route.display}}</router-link>
                    </li>

                    <!--<li class="nav-item">
                        <router-link v-bind:class="[isActiveRoute('POS')? 'active' : '', 'nav-link']" to="/Manage/POS">POS</router-link>
                    </li>
                    <li v-bind:class="[isActiveRoute('User')? 'active' : '', 'nav-item']">
                        <router-link class="nav-link" to="/Manage/User">Người dùng</router-link>
                    </li>-->
                </ul>
            </div>
        </div>
        <div class="row">
            <div class="col-lg-12 mx-auto">
                <router-view class="top-margin"
                            v-on:showsuccess="ShowSuccessToast"
                            v-on:showinfo="ShowInfoToast"
                            v-on:showerror="ShowBlockingDialog"
                            v-on:showdialog="ShowDialog"></router-view>
            </div>
        </div>
    </div>
</template>
<script>
    //https://github.com/cristijora/vue-tabs/issues/44
    import adminRoutes from './adminRoutes'

    export default {
        name: 'admin-view',
        template: '#adminview',
        
        computed: {
            currentRouteName: function () {
                return this.$route.name;
            },
            routes: function () {
                return adminRoutes;
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
            },
            //Bubbling up
            ShowSuccessToast(mess) {
                this.$emit('showsuccess', mess);
            },
            ShowInfoToast(mess) {
                this.$emit('showinfo',mess);
            },
            ShowBlockingDialog(mess) {
                this.$emit('showerror', mess);
            },
            ShowDialog(mess, t) {
                this.$emit('showdialog', mess, t);
            }

        }

    }
</script>
<style scoped>
    .top-margin {
        margin-top: 1rem
    }
</style>