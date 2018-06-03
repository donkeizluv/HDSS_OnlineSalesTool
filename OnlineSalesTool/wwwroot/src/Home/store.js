import Vue from "vue";
import Vuex from "vuex";
import axios from "axios";
import jwt from "jwt-decode";

import common from "../Home/Common";

import { ConstStorage } from "./AppConst";
import permissionDict from "./permissionDict";

import API from "./API";

import {
    CHECK_AUTH,
    LOGIN,
    LOGOUT,
    RELOAD_TOKEN,
    CLEAR_LOCALSTORE
} from "./actions";
import {
    AUTHENTICATED,
    AUTH_CHECKED,
    IDENTITY,
    EXPIRE,
    TOKEN,
    LOADING,
    ABILITY,
    ROLE
} from "./mutations";

Vue.use(Vuex);

export default new Vuex.Store({
    state: {
        //Auth
        authChecked: false, //Auth check on app boot
        authenticated: false,
        token: localStorage.getItem(ConstStorage.TokenStorage) || null,
        identity: localStorage.getItem(ConstStorage.IdentityStorage) || null,
        expire: localStorage.getItem(ConstStorage.ExpireStorage) || 0,
        ability: [],
        role: null,
        //Roles & functionality dict
        permissionDict,
        //VM
        vm_assigner: null,
        vm_posman: null,
        vm_user: null,
        //Loading
        isLoading: false
    },
    getters: {
        //Auth
        isAuthenticated: state => state.authenticated,
        isAuthChecked: state => state.authChecked,
        identity: state => state.identity,
        //Ability: state => state.Ability,
        //HasAbility(state) {
        //    //NYI
        //    return name => state.Ability.some(i => {
        //        return i == name;
        //    });
        //},
        can(state) {
            return permission => {
                if (!permission) return false;
                let dict = state.permissionDict.filter(
                    d => d.role == state.role || d.role == ""
                );
                if (dict.length < 1) return false;
                return dict.some(d => d.can.some(p => p == permission));
            };
        },
        //App wide loading
        isLoading: state => state.isLoading
    },
    mutations: {
        //Auth
        [AUTHENTICATED](state, value) {
            this.state.authenticated = value;
        },
        [AUTH_CHECKED](state, value) {
            this.state.authChecked = value;
        },
        [TOKEN](state, value) {
            this.state.token = value;
        },
        [EXPIRE](state, value) {
            this.state.expire = value;
        },
        [IDENTITY](state, value) {
            this.state.identity = value;
        },
        [ABILITY](state, value) {
            this.state.ability = value;
        },
        [ROLE](state, value) {
            this.state.role = value;
        },
        //App wide loading
        [LOADING](state, value) {
            this.state.isLoading = value;
        }
    },
    actions: {
        [CHECK_AUTH]: async ({ commit, dispatch, state }) => {
            if (!state.token) {
                //If no token then no need to check
                commit(AUTH_CHECKED, true);
                commit(AUTHENTICATED, false);
                return;
            }
            //Start checking
            commit(AUTH_CHECKED, false);
            try {
                await axios({
                    url: API.Ping,
                    headers: { Authorization: `Bearer ${state.token}` }
                });
                //Reload auth states
                await dispatch(RELOAD_TOKEN);
            } catch (e) {
                await dispatch(LOGOUT);
            } finally {
                //Check done
                commit(AUTH_CHECKED, true);
            }
        },
        [LOGIN]: async ({ commit, dispatch }, cred) => {
            commit(LOADING, true);
            try {
                let { username, pwd } = cred;
                let form = new FormData();
                form.append("username", username);
                form.append("pwd", pwd);
                let { data } = await axios.post(API.Login, form);
                let token = data.auth_token;
                //Store token & not in token info
                localStorage.setItem(ConstStorage.TokenStorage, token);
                localStorage.setItem(ConstStorage.IdentityStorage, username);
                localStorage.setItem(
                    ConstStorage.ExpireStorage,
                    data.expires_in
                );
                //init states
                await dispatch(RELOAD_TOKEN);
                commit(LOADING, false);
            } catch (e) {
                //console.log(e);
                commit(LOADING, false);
                await dispatch(CLEAR_LOCALSTORE);
                throw e;
            }
        },
        //Call this to init app using stored token
        [RELOAD_TOKEN]: async ({ commit, dispatch }) => {
            //Get token from store
            let token = localStorage.getItem(ConstStorage.TokenStorage);
            let identity = localStorage.getItem(ConstStorage.IdentityStorage);
            let exp = localStorage.getItem(ConstStorage.ExpireStorage);

            //Check
            if (token == undefined || identity == undefined || exp < 1)
                throw new Error("Fail to load token from storage");
            let { Role, Ability, sub } = jwt(token);
            if (!Role) throw new "Missing properties in token"();
            //init states
            commit(TOKEN, token);
            commit(EXPIRE, exp);
            commit(IDENTITY, sub);
            commit(ROLE, Role);
            commit(ABILITY, Ability || []);
            //Set token to axios
            axios.defaults.headers.common["Authorization"] = "Bearer " + token;
            commit(AUTHENTICATED, true);
        },
        [LOGOUT]: async ({ commit, dispatch }) => {
            //Clear axios
            delete axios.defaults.headers.common["Authorization"];
            //Clear all state and storage
            commit(TOKEN, null);
            commit(EXPIRE, 0);
            commit(IDENTITY, null);
            commit(ROLE, null);
            commit(ABILITY, []);
            await dispatch(CLEAR_LOCALSTORE);
            commit(AUTHENTICATED, false);
        },
        [CLEAR_LOCALSTORE]: async () => {
            localStorage.removeItem(ConstStorage.TokenStorage);
            localStorage.removeItem(ConstStorage.IdentityStorage);
            localStorage.removeItem(ConstStorage.ExpireStorage);
            localStorage.removeItem(ConstStorage.AbilityStoreage);
            localStorage.removeItem(ConstStorage.RoleStoreage);
        }
    }
});
