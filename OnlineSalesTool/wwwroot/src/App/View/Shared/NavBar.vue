<template id="nav-bar">
    <b-navbar toggleable="md" type="dark" variant="dark">
        <b-navbar-toggle target="nav_collapse"></b-navbar-toggle>
        <b-navbar-brand>
            <router-link class="navbar-brand" to="/"><span class="env">{{env}}</span> {{appName}}</router-link>
        </b-navbar-brand>
        <b-collapse is-nav id="nav_collapse">
            <b-navbar-nav>
                <template v-for="route in routes" v-if="route.navbar">
                    <router-link v-if="allow(route)"
                            active-class="active font-italic"
                            :key="route.name" 
                            class="nav-link"
                            :to="{ name: route.name }">{{route.display}}</router-link>
                    <span v-else class="text-secondary nav-link" :key="route.name" >{{route.display}}</span>
                </template>
            </b-navbar-nav>
            <!-- Right aligned nav items -->
            <b-navbar-nav class="ml-auto">
                <b-nav-item right>
                    <ul v-if="isAuthenticated" class="navbar-nav ml-auto">
                        <li class="nav-item">
                            <router-link class="nav-link" to="Info">
                                <span>
                                    Xin chào! {{identity}}
                                </span>
                            </router-link>
                        </li>
                        <!--Dispatch log out action-->
                        <li class="nav-item"><a class="nav-link" @click="logout">Thoát</a></li>
                    </ul>
                </b-nav-item>
            </b-navbar-nav>
        </b-collapse>
    </b-navbar>
</template>
<script>
import routes from "../../routes";
import { LOGOUT } from "../../actions";

export default {
    name: "NavBar",
    template: "#nav-bar",
    props: {
        appName: {
            type: String,
            default: "VueJS SPA"
        },
        env: {
            type: String,
            default: "DEV"
        }
    },
    computed: {
        isAuthenticated() {
            return this.$store.getters.isAuthenticated;
        },
        identity() {
            return this.$store.getters.identity;
        },
        currentRouteName() {
            return this.$route.name;
        }
    },
    data() {
        return {
            routes
        };
    },
    methods: {
        allow(route) {
            return this.$store.getters.can(route.name);
        },
        logout() {
            this.$store.dispatch(LOGOUT);
        },
        isActiveRoute(name) {
            return this.currentRouteName === name;
        }
    }
};
</script>