import store from './store'
//import AssignerView from './Component/AssignerView.vue'
import CaseView from './Component/CaseView.vue'
//import AdminView from './Component/AdminView.vue'
import adminRoutes from './Component/adminRoutes'
//import TestView from './Component/TestView.vue'

function requireAuth(to, from, next) {
    if (!store.getters.isAuthenticated) {
        next(false);
    } else {
        next();
    }
}

function requireNoAuth(to, from, next) {
    if (store.getters.isAuthenticated) {
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
    {
        path: '/Home',
        name: 'Home',
        component: CaseView,
        display: 'Trang chính',
        navbar: true, //Renders on nav bar if true
        //beforeEnter: requireAuth
    },
    {
        path: '/Assign',
        name: 'Assign',
        component: () => import(/* webpackChunkName: "assignerview" */'./Component/AssignerView.vue'),
        display: 'Lịch trực',
        navbar: true,
        //beforeEnter: requireAuth
    },
    {
        path: '/Manage',
        name: 'Manage',
        component: () => import(/* webpackChunkName: "adminview" */'./Component/AdminView.vue'),
        display: 'Quản lý',
        navbar: true,
        //beforeEnter: requireAuth,
        //Nested routes
        children: [
           ...adminRoutes
        ]
    },
    {
        path: '/Test',
        name: 'Test',
        component: () => import(/* webpackChunkName: "testview" */'./Component/TestView.vue'),
        display: 'Test',
        navbar: true
    }]
module.exports = routes;