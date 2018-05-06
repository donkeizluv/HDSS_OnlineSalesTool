<template>
    <div>
        <v-dialog :clickToClose=false />
        <nav-bar :app-name="'Online Sales Tool'" :env="'DEV'"></nav-bar>
        <div v-bind:class="{'bg-grey': !IsAuthenticated }">
            <div class="container-fluid">
                <keep-alive>
                    <router-view v-on:showsuccess="ShowSuccessToast"
                                 v-on:showinfo="ShowInfoToast"
                                 v-on:showerror="ShowBlockingDialog"
                                 v-on:showdialog="ShowDialog"></router-view>
                </keep-alive>
            </div>
        </div>
    </div>
</template>
<script>
    import Vue from 'vue'
    import navbar from './NavBar.vue'
    import axios from 'axios'
    import { RELOAD_TOKEN, LOGOUT } from '../actions'
    import API from '../API'

    export default {
        components: {
            'nav-bar': navbar
        },
        computed: {
            IsAuthenticated: function () {
                return this.$store.getters.IsAuthenticated;
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
            //Reload state if has token
            if (this.$store.getters.IsAuthenticated) {
                try {
                    await this.$store.dispatch(RELOAD_TOKEN);
                } catch (e) {
                    //Reload fail then logout
                    await this.$store.dispatch(LOGOUT);
                }
            }
        },
        data: function () {
            return {
                blackBg: false
            }
        },
        methods: {
            ShowSuccessToast(mess) {
                //This has shitty support for specific icon & multiple style class
                this.$toasted.success(mess, {
                    icon: 'fa-check-circle',
                    className: 'toast-font-size'
                });
            },
            ShowInfoToast(mess) {
                this.$toasted.info(mess, {
                    icon: 'fa-exclamation-circle',
                    className: 'toast-font-size'
                });
            },
            ShowBlockingDialog(mess) {
                this.blackBg = true;
                this.$modal.show('dialog', {
                    title: 'Lỗi :(',
                    text: mess,
                    buttons: []
                });
            },
            ShowDialog(mess, t) {
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
</style>