﻿import store from './store'

import AssignerView from './Component/AssignerView.vue'
import CaseView from './Component/CaseView.vue'
import AdminView from './Component/AdminView.vue'
import LoginView from './Component/LoginView.vue'



function requireAuth(to, from, next) {
    if (!store.getters.IsAuthenticated) {
        next({
            path: '/Login',
            query: { redirect: to.path }
        });
    } else {
        next();
    }
}

function requireNoAuth(to, from, next) {
    if (store.getters.IsAuthenticated) {
        //next(false);
        next('Home');
    } else {
        next();
    }
}

const routes = [
    //Default
    {
        path: '/',
        redirect: '/Home'
    },
    //Auth
    {
        path: '/Home',
        name: 'Home',
        component: CaseView,
        display: 'Trang chính',
        navbar: true,
        beforeEnter: requireAuth
    },
    {
        path: '/Assign',
        name: 'Assign',
        component: AssignerView,
        display: 'Ca trực',
        navbar: true,
        beforeEnter: requireAuth
    },
    {
        path: '/Manage',
        name: 'Manage',
        component: AdminView,
        display: 'Quản lý',
        navbar: true,
        beforeEnter: requireAuth
    },
    //No auth
    {
        path: '/Login',
        name: 'Login',
        component: LoginView,
        display: 'Đăng nhập',
        navbar: false,
        beforeEnter: requireNoAuth
    }]
module.exports = routes;