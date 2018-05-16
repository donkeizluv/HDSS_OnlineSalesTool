import Vue from 'vue'
import Vuex from 'vuex'
import axios from 'axios'
import jwt from 'jwt-decode'

import common from '../Home/Common'

import { AppFunction, ConstStorage } from './AppConst'
import API from './API'
import router from './router'


import { LOGIN, LOGOUT, RELOAD_TOKEN, SET_LOADING, CHECK_TOKEN_EXPIRE, CLEAR_LOCALSTORE } from './actions'
import { IDENTITY, EXPIRE, TOKEN, LOADING, ABILITY, ROLE } from './mutations'

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
        //Token: null,
        //Identity: null,
        //Expire: 0,

        //Loading
        Loading: false

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
                var role = state.RoleFuncDict.find(r => r.Role == state.Role)
                if (!role) return false;
                return role.Can.some(c => c == can);
            }
        },
        //App wide loading
        Loading: state => state.isLoading
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

        //App wide loading
        [LOADING](state, value) {
            this.state.isLoading = value;
        }
        
    },
    actions: {
        [LOGIN]: async ({ commit, dispatch }, cred) => {
                await dispatch(SET_LOADING, true);
                try {
                    var form = new FormData();
                    form.append('username', cred.username);
                    form.append('pwd', cred.pwd);
                    var response = await axios.post(API.Login, form);
                    var token = response.data.auth_token;
                    //console.log(response);
                    //Store token & not in token info
                    localStorage.setItem(ConstStorage.TokenStorage, token);
                    localStorage.setItem(ConstStorage.IdentityStorage, cred.username);
                    localStorage.setItem(ConstStorage.ExpireStorage, response.data.expires_in);
                    //init states
                    await dispatch(RELOAD_TOKEN);
                    //Go
                    router.push('Home');
                    await dispatch(SET_LOADING, false);
                } catch (e) {
                    //console.log(e);
                    await dispatch(SET_LOADING, false);
                    await dispatch(CLEAR_LOCALSTORE);
                    throw e;
                }
        },
        //Call this to init app using stored token
        [RELOAD_TOKEN]: async ({ commit, dispatch }) => {
                //Get token from store
                var token = localStorage.getItem(ConstStorage.TokenStorage);
                var identity = localStorage.getItem(ConstStorage.IdentityStorage);
                var exp = localStorage.getItem(ConstStorage.ExpireStorage);

                //Check
                if (token == undefined || identity == undefined || exp < 1)
                    throw new Error('Fail to load token from storage');
                var decode = jwt(token);
                //Check decode
                if (decode.Role == undefined) throw new 'Missing properties in token';
                var ability = [];
                if (decode.hasOwnProperty('Ability'))
                    ability = decode.Ability;
                //init states
                commit(TOKEN, token);
                commit(EXPIRE, exp);
                commit(IDENTITY, decode.sub);
                commit(ROLE, decode.Role);
                commit(ABILITY, ability);
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
                await dispatch(CLEAR_LOCALSTORE);
                router.push('Login');
        },
        [CLEAR_LOCALSTORE]: async () => {
            localStorage.removeItem(ConstStorage.TokenStorage);
            localStorage.removeItem(ConstStorage.Identity);
            localStorage.removeItem(ConstStorage.ExpireStorage);
            localStorage.removeItem(ConstStorage.AbilityStoreage);
            localStorage.removeItem(ConstStorage.RoleStoreage);
        },
        [SET_LOADING]: async ({ commit, dispatch }, value) => {
            commit(LOADING, value);
        },
        [CHECK_TOKEN_EXPIRE]: async ({ state }) => {
            try {
                await axios({
                    url: API.Ping,
                    headers: { 'Authorization': `Bearer ${state.Token}` }
                });
                //console.log('Token ok');
            } catch (e) {
                //console.log(e);
                //console.log('Token invalid');
                throw e;
            }
            
        }
    }
})