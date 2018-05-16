<template id="nav-bar">
    <nav class="navbar navbar-expand-md bg-dark navbar-dark">
        <router-link class="navbar-brand" to="/"><span class="env">{{env}}</span> {{appName}}</router-link>
        <!-- Toggler/collapsibe Button -->
        <button class="navbar-toggler" type="button" data-toggle="collapse" data-target="#collapsibleNavbar">
            <span class="navbar-toggler-icon"></span>
        </button>
        <div class="collapse navbar-collapse" id="collapsibleNavbar">
            <ul class="navbar-nav">
                <li class="nav-item" v-for="route in routes">
                    <router-link v-show="route.navbar" class="nav-link" v-bind:to="route.path">{{route.display}}</router-link>
                </li>
            </ul>
            <!--Account-->
            <ul v-if="isAuthenticated" class="navbar-nav ml-auto">
                <li class="nav-item">
                    <router-link class="nav-link" to="Info">
                        <span>
                            Xin chào!
                            {{identity}}
                        </span>
                    </router-link>
                </li>
                <!--Dispatch log out action-->
                <li class="nav-item"><a class="nav-link" v-on:click="logout">Logout</a></li>
            </ul>
            <ul v-else class="navbar-nav ml-auto">
                <li class="nav-item">
                    <router-link class="nav-link" to="Login">Login</router-link>
                </li>
            </ul>

        </div>
    </nav>
</template>
<script>
    import routes from '../routes'
    import { LOGOUT } from '../actions'
    export default {
        name: 'NavBar',
        template: '#nav-bar',
        props: {
            appName: {
                type: String,
                default: 'VueJS SPA'
            },
            env: {
                type: String,
                default: 'DEV'
            }
        },
        computed: {
            isAuthenticated: function(){
                return this.$store.getters.IsAuthenticated;
            },
            identity: function () {
                return this.$store.getters.Identity;
            }
        },
        data: function () {
            return {
                routes
            }
        },
        methods: {
            logout: function () {
                this.$store.dispatch(LOGOUT);
            }
        }
    }
</script>
<style>

</style>