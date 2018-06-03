<template>
    <div>
        <v-dialog :clickToClose=false />
        <nav-bar :app-name="'Online Sales Tool'" :env="'DEV'"></nav-bar>
        <div v-bind:class="{'bg-grey': !isAuthenticated || !isAuthChecked }">
            <!--Only render app when auth is valid & checked-->
            <div v-if="!isAuthenticated || !isAuthChecked">
                <!--Hide login when still checking to avoid ugly flash-->
                <login v-show="isAuthChecked"></login>
            </div>
            <div v-else>
                <div class="container-fluid">
                    <div v-if="isCurrentRouteAllowed">
                    </div>
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
    import Vue from 'vue'
    import navbar from './NavBar.vue'
    import axios from 'axios'
    import { CHECK_AUTH, LOGOUT } from '../actions'
    import API from '../API'
    import LoginView from './LoginView.vue'

    export default {
        components: {
            'nav-bar': navbar,
            'login': LoginView
        },
        computed: {
            isCurrentRouteAllowed: function(){
                return this.allow(this.currentRoute);
            },
            currentRoute: function(){
                return this.$route;
            },
            isAuthenticated: function () {
                return this.$store.getters.isAuthenticated;
            },
            isAuthChecked: function () {
                return this.$store.getters.isAuthChecked;
            }
        },
        created: async function () {
            //To login page incase 401
            axios.interceptors.response.use(r => {
                return r;
            }, async e => {
                if (e.response.status == 401) {
                    await this.$store.dispatch(LOGOUT);
                }
                return Promise.reject(e);
                });
            //Check auth expire/reload auth
            if(!this.$store.getters.isAuthChecked)
                await this.$store.dispatch(CHECK_AUTH);
        },
        data: function () {
            return {
                blackBg: false
            }
        },
        methods: {
            allow: function(route) {
                return this.$store.getters.can(route.name);
            },
            showSuccessToast(mess) {
                //This has shitty support for specific icon & multiple style class
                this.$toasted.success(mess, {
                    icon: 'fa-check-circle',
                    className: 'toast-font-size'
                });
            },
            showInfoToast(mess) {
                this.$toasted.info(mess, {
                    icon: 'fa-exclamation-circle',
                    className: 'toast-font-size'
                });
            },
            showBlockingDialog(mess) {
                this.blackBg = true;
                this.$modal.show('dialog', {
                    title: 'Lỗi :(',
                    text: mess,
                    buttons: []
                });
            },
            showDialog(mess, t) {
                this.$modal.show('dialog', {
                    title: t,
                    text: mess,
                    buttons: [
                        {
                            title: 'OK',
                            handler: () => { this.$modal.hide('dialog') }
                        }]
                });
            }
        }
    }
</script>
<style scoped>
    .bg-grey {
        position: fixed; /* Stay in place */
        z-index: 1; /* Sit on top */
        left: 0;
        top: 0;
        width: 100%; /* Full width */
        height: 100%; /* Full height */
        overflow: auto; /* Enable scroll if needed */
        background-color: rgb(0,0,0); /* Fallback color */
        background-color: rgba(0,0,0,0.4); /* Black w/ opacity */
    }
    .top-margin {
        margin-top: 1rem;
    }
</style>