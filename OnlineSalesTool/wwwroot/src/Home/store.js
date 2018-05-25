import Vue from 'vue'
import Vuex from 'vuex'
import axios from 'axios'
import jwt from 'jwt-decode'

import common from '../Home/Common'

import { AppFunction, ConstStorage } from './AppConst'
import API from './API'
import router from './router'


import { LOGIN, LOGOUT, RELOAD_TOKEN, CHECK_TOKEN_EXPIRE, CLEAR_LOCALSTORE, CLEAR_VM } from './actions'
import { IDENTITY, EXPIRE, TOKEN, LOADING, ABILITY, ROLE, VM_ASSIGNER, VM_POSMAN, VM_USER } from './mutations'
//import * as mutationType from './mutations'

Vue.use(Vuex)

export default new Vuex.Store({
    state: {
        //Auth
        Token: localStorage.getItem(ConstStorage.TokenStorage) || null,
        Identity: localStorage.getItem(ConstStorage.IdentityStorage) || null,
        Expire: localStorage.getItem(ConstStorage.ExpireStorage) || 0,
        Ability: [],
        Role: null,
        //Roles & functionality dict
        RoleFuncDict: [
            { Role: 'BDS', Can: [AppFunction.CreateShiftSchedule] },
            { Role: 'ADMIN', Can: [AppFunction.CreateShiftSchedule, AppFunction.EditShiftSchedule] }
        ],
        //VM
        vm_assigner: null,
        vm_posman: null,
        vm_user: null,
        //Loading
        isLoading: false

    },
    getters: {
        //Auth
        AuthToken: state => state.Token,
        IsAuthenticated: state => !!state.Token,
        Identity: state => state.Identity,
        Role: state => state.Role,
        Ability: state => state.Ability,
        HasAbility(state) {
            //NYI
            return name => state.Ability.some(i => {
                return i == name;
            });
        },
        Can(state) {
            return can => {
                //Get role dict of current user
                let role = state.RoleFuncDict.find(r => r.Role == state.Role)
                if (!role) return false;
                return role.Can.some(c => c == can);
            }
        },
        //VM
        vm_assigner: state => state.vm_assigner,
        vm_posman: state => state.vm_posman,
        vm_user: state => state.vm_user,
        //App wide loading
        isLoading: state => state.isLoading
    },
    mutations: {
        //Auth
        [TOKEN](state, value) {
            this.state.Token = value;
        },
        [EXPIRE](state, value) {
            this.state.Expire = value;
        },
        [IDENTITY] (state, value) {
            this.state.Identity = value;
        },
        [ABILITY](state, value) {
            this.state.Ability = value;
        },
        [ROLE](state, value) {
            this.state.Role = value;
        },
        //VM
        [VM_ASSIGNER](state, vm) {
            this.state.vm_assigner = vm;
        },
        [VM_POSMAN](state, vm) {
            this.state.vm_posman = vm;
        },
        [VM_USER](state, vm) {
            this.state.vm_user = vm;
        },
        //App wide loading
        [LOADING](state, value) {
            this.state.isLoading = value;
        }
        
    },
    actions: {
        [LOGIN]: async ({ commit, dispatch }, cred) => {
                commit(LOADING, true);
                try {
                    let { username, pwd } = cred;
                    let form = new FormData();
                    form.append('username', username);
                    form.append('pwd', pwd);
                    let { data } = await axios.post(API.Login, form);
                    let token = data.auth_token;
                    //Store token & not in token info
                    localStorage.setItem(ConstStorage.TokenStorage, token);
                    localStorage.setItem(ConstStorage.IdentityStorage, username);
                    localStorage.setItem(ConstStorage.ExpireStorage, data.expires_in);
                    //init states
                    await dispatch(RELOAD_TOKEN);
                    //Go
                    router.push('/Home');
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
                    throw new Error('Fail to load token from storage');
                let { Role, Ability, sub } = jwt(token);
                if (!Role) throw new 'Missing properties in token';
                //init states
                commit(TOKEN, token);
                commit(EXPIRE, exp);
                commit(IDENTITY, sub);
                commit(ROLE, Role);
                commit(ABILITY, Ability || []);
                //Set token to axios
                axios.defaults.headers.common['Authorization'] = 'Bearer ' + token;
        },
        [LOGOUT]: async ({ commit, dispatch }) => {
                //Clear all state and storage
                commit(TOKEN, null);
                commit(EXPIRE, 0);
                commit(IDENTITY, null);
                commit(ROLE, null);
                commit(ABILITY, []);
                await dispatch(CLEAR_VM);
                await dispatch(CLEAR_LOCALSTORE);
                router.push('/Login');
        },
        [CLEAR_VM]: async ({ commit }) => {
            commit(VM_ASSIGNER, null);
            commit(VM_POSMAN, null);
        },
        [CLEAR_LOCALSTORE]: async () => {
            localStorage.removeItem(ConstStorage.TokenStorage);
            localStorage.removeItem(ConstStorage.Identity);
            localStorage.removeItem(ConstStorage.ExpireStorage);
            localStorage.removeItem(ConstStorage.AbilityStoreage);
            localStorage.removeItem(ConstStorage.RoleStoreage);
        },
        //[SET_LOADING]: async ({ commit }, value) => {
        //    commit(LOADING, value);
        //},
        [CHECK_TOKEN_EXPIRE]: async ({ state, dispatch }) => {
            try {
                await axios({
                    url: API.Ping,
                    headers: { 'Authorization': `Bearer ${state.Token}` }
                });
                //console.log('Token ok');
            } catch (e) {
                await dispatch(LOGOUT);
            }
            
        }
    }
})