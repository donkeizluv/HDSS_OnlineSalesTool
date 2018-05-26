<template id="adminview">
    <div>
        <div class="row">
            <div class="col-lg-12 mx-auto">
                <ul class="nav nav-tabs">
                    <li class="nav-item" v-for="route in routes">
                        <router-link v-if="can(route.permission)"
                                     v-bind:class="[isActiveRoute(route.name)? 'active' : '', 'nav-link']"
                                     v-bind:to="{ name: route.name }">{{route.display}}</router-link>
                        <span v-else class="text-secondary nav-link">{{route.display}}</span>
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
    //https://github.com/cristijora/vue-tabs/issues/44
    import adminRoutes from './adminRoutes'

    export default {
        name: 'admin-view',
        template: '#adminview',
        activated: function() {
            //Select a default view when no current child view is specified
            if (this.routes.some(r => r.name == this.currentRouteName)) return;
            let r = this.routes.find(r => this.can(r.permission));
            if (!r) return;
            this.$router.push({ name: r.name });
        },
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
            can: function (p) {
                if (!p) return true;
                return this.$store.getters.can(p);
            },
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