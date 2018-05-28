<template id="adminview">
    <div>
        <div class="row">
            <div class="col-lg-12 mx-auto">
                <ul class="nav nav-tabs">
                    <li class="nav-item" v-for="route in computedRoutes">
                        <template v-if="route.navbar">
                            <router-link class="nav-link"
                                         active-class="active"
                                         v-if="can(route.permission)"
                                         v-bind:to="{ name: route.name }"
                                         exact>{{route.display}}</router-link>
                            <!--Show only name if not permitted-->
                            <span v-else class="text-secondary nav-link">{{route.display}}</span>
                        </template>
                    </li>
                </ul>
            </div>
        </div>
        <div class="row">
            <div class="col-lg-12 mx-auto">
                <keep-alive>
                    <router-view class="top-margin"
                                 v-on:showsuccess="ShowSuccessToast"
                                 v-on:showinfo="ShowInfoToast"
                                 v-on:showerror="ShowBlockingDialog"
                                 v-on:showdialog="ShowDialog"></router-view>
                </keep-alive>
            </div>
        </div>
    </div>
</template>
<script>
    import adminRoutes from './adminRoutes'

    export default {
        name: 'admin-view',
        template: '#adminview',
        computed: {
            computedRoutes: function () {
                //Cant get this working ever, no idea why
                //Allow routes based on this user's role
                //let routes = [...adminRoutes];
                //let allowRoute = routes.find(c => this.can(c.permission) && c.name != 'default');
                //console.log(allowRoute);
                //if (allowRoute) {
                    //Default route is first allow route
                    //No fucking idea why this doesnt work
                    //return [{
                    //    path: '',
                    //    name: 'default',
                    //    navbar: false,
                    //    redirect: { name: allowRoute.name }
                    //}, ...routes];
                    //Doesnt work
                    //let defaultRoute = routes.find(r => r.name == 'default');
                    //defaultRoute.redirect = { name: allowRoute.name };
                //}
                //return routes;
                return adminRoutes;
            }
        },
        //data: function () {
        //    return {
        //        //routes: []
        //    }
        //},
        methods: {
            can: function (p) {
                if (!p) return true;
                return this.$store.getters.can(p);
            },
            isActiveRoute: function (name) {
                //console.log(this.currentRouteName);
                return this.currentRouteName === name;
            },
            //Select 1st allow view
            //setDefaultView() {

            //},
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