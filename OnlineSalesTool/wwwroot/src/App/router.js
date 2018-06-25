﻿import Vue from 'vue'
import VueRouter from 'vue-router'
import store from './store'
import routes from './routes'

Vue.use(VueRouter)

export default new VueRouter({
    mode: 'history',
    base: '/App',
    routes: routes
});