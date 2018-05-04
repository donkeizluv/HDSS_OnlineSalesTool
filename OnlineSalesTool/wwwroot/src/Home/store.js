import Vue from 'vue'
import Vuex from 'vuex'
import axios from 'axios'
import jwt from 'jwt-decode'

import common from '../Home/Common'

import appConst from './AppConst'
import API from './API'
import router from './router'


import { LOGIN, LOGOUT, RELOAD_TOKEN, SET_LOADING, CHECK_TOKEN_EXPIRE } from './actions'
import { IDENTITY, EXPIRE, TOKEN, LOADING } from './mutations'

Vue.use(Vuex)

export default new Vuex.Store({
    state: {
        //Auth
        Token: localStorage.getItem(appConst.TokenStorage) || '',
        Identity: localStorage.getItem(appConst.IdentityStorage) || '',
        Expire: localStorage.getItem(appConst.ExpireStorage) || 0,
        //Token: null,
        //Identity: null,
        //Expire: 0,

        //Loading
        Loading: false

    },
    getters: {
        AuthToken: state => state.Token,
        IsAuthenticated: state => !!state.Token,
        Identity: state => state.Identity,
        Loading: state => state.Loading
    },
    mutations: {
        [TOKEN](state, value) {
            this.state.Token = value;
        },
        [EXPIRE](state, value) {
            this.state.Expire = value;
        },
        [IDENTITY] (state, value) {
            this.state.Identity = value;
        },
        [LOADING](state, value) {
            this.state.Loading = value;
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
                    //Decode jwt
                    var decode = jwt(token);
                    //Store token data
                    localStorage.setItem(appConst.TokenStorage, token);
                    localStorage.setItem(appConst.IdentityStorage, cred.username);
                    localStorage.setItem(appConst.ExpireStorage, response.data.expires_in);
                    //Load states
                    await dispatch(RELOAD_TOKEN);
                    //Go
                    router.push('Home');
                    await dispatch(SET_LOADING, false);
                } catch (e) {
                    //console.log(e);
                    await dispatch(SET_LOADING, false);
                    localStorage.removeItem(appConst.TokenStorage);
                    throw e;
                }
        },
        //Call this to init app using stored token
        [RELOAD_TOKEN]: async ({ commit, dispatch }) => {
                //Get token from store
                var token = localStorage.getItem(appConst.TokenStorage);
                var identity = localStorage.getItem(appConst.IdentityStorage);
                var exp = localStorage.getItem(appConst.ExpireStorage);
                //Check
                if (!token || !identity || exp < 1)
                    throw new Error('Fail to load token from storage');
                var decode = jwt(token);
                //Init state
                commit(TOKEN, token);
                commit(EXPIRE, exp);
                commit(IDENTITY, decode.sub);
                //Other claim and values
                //commit(LAYER, decode.LayerName);
                //commit(LAYERRANK, decode.LayerRank);
                //Set token to axios
                axios.defaults.headers.common['Authorization'] = 'Bearer ' + token;
        },
        [LOGOUT]: async ({ commit, dispatch }) => {
                //Clear all state and storage
                commit(TOKEN, '');
                commit(EXPIRE, 0);
                commit(IDENTITY,'');

                localStorage.removeItem(appConst.TokenStorage);
                localStorage.removeItem(appConst.Identity);
                localStorage.removeItem(appConst.ExpireStorage);
                router.push('Login');
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