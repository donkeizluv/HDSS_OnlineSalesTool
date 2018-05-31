import store from './store'
//import AssignerView from './Component/AssignerView.vue'
import CaseView from './Component/CaseView.vue'
//import AdminView from './Component/AdminView.vue'
import adminRoutes from './Component/adminRoutes'
//import TestView from './Component/TestView.vue'
import { Permission } from './AppConst'
import permissionDict from './permissionDict';

function checkPermission(to, from, next) {
    next(store.getters.can(to.name));
}

const routes = [
    //Default
    {
        path: '/',
        redirect: '/Home'
    },
    {
        path: '/Home',
        name: Permission.CaseListing,
        component: CaseView,
        display: 'Trang chính',
        navbar: true, //Renders on nav bar if true
        // beforeEnter: checkPermission
    },
    {
        path: '/Assign',
        name: Permission.ScheduleAssigner,
        component: () => import(/* webpackChunkName: "assignerview" */'./Component/AssignerView.vue'),
        display: 'Lịch trực',
        navbar: true,
        beforeEnter: checkPermission
    },
    {
        path: '/Manage',
        name: Permission.Management,
        component: () => import(/* webpackChunkName: "adminview" */'./Component/AdminView.vue'),
        display: 'Quản lý',
        navbar: true,
        beforeEnter: checkPermission,
        children: [
           ...adminRoutes
        ]
    }
    //{
    //    path: '/Test',
    //    name: 'Test',
    //    component: () => import(/* webpackChunkName: "testview" */'./Component/TestView.vue'),
    //    display: 'Test',
    //    navbar: true
    //}]
    ]
module.exports = routes;