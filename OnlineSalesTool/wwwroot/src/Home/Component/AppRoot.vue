<template>
    <div>
        <nav-bar :app-name="'Online Sales Tool'" :env="'DEV'"></nav-bar>
        <div v-bind:class="{'greybg':!IsAuthenticated }">
            <div class="container-fluid">
                <div class="row">
                    <div class="col">
                        <router-view></router-view>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>
<script>
    import Vue from 'vue'
    import navbar from './NavBar.vue'
    import axios from 'axios'
    import { RELOAD_TOKEN, LOGOUT } from '../actions'

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
            axios.interceptors.response.use( (response) => {
                return response;
            }, (error) => {
                if (error.response.status == 401) {
                    this.$store.dispatch(LOGOUT);
                }
                return Promise.reject(error);
            });
            //Init store if authed
            if (this.$store.getters.IsAuthenticated) {
                try {
                    await this.$store.dispatch(RELOAD_TOKEN);
                } catch (e) {
                    //Reload fail then logout
                    await this.$store.dispatch(LOGOUT);
                }
            }
        },
        data() {
            return {
            }
        }
    }
</script>
<style scoped>
    .greybg {
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