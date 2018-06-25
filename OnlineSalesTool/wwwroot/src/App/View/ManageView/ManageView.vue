<template>
    <div>
        <div class="row">
            <div class="col-lg-12 mx-auto">
                <ul class="nav nav-tabs">
                    <li class="nav-item" v-for="route in computedRoutes" v-bind:key="route.name">
                        <template v-if="route.navbar">
                            <router-link class="nav-link"
                                         active-class="active"
                                         v-if="allow(route)"
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
                <div v-if="isCurrentRouteAllowed">
                    <keep-alive>
                        <router-view class="top-margin"
                                    v-on:showsuccess="showSuccessToast"
                                    v-on:showinfo="showInfoToast"
                                    v-on:showerror="showBlockingDialog"
                                    v-on:showdialog="showDialog"></router-view>
                    </keep-alive>
                </div>
            </div>
        </div>
    </div>
</template>
<script>
import adminRoutes from "./adminRoutes";

export default {
    name: "ManageView",
    computed: {
        computedRoutes(){
            return adminRoutes;
        },
        currentRoute(){
            return this.$route;
        },
        isCurrentRouteAllowed(){
            return this.allow(this.currentRoute);
        }
    },
    //data() {
    //    return {
    //        //routes: []
    //    }
    //},
    methods: {
        allow(route) {
            return this.$store.getters.can(route.name);
        },
        //Bubbling up
        showSuccessToast(mess) {
            this.$emit("showsuccess", mess);
        },
        showInfoToast(mess) {
            this.$emit("showinfo", mess);
        },
        showBlockingDialog(mess) {
            this.$emit("showerror", mess);
        },
        showDialog(mess, t) {
            this.$emit("showdialog", mess, t);
        }
    }
};
</script>
<style scoped>
.top-margin {
    margin-top: 1rem;
}
</style>