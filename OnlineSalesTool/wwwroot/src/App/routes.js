import store from "./store";
import { CHECK_AUTH } from './actions'
import adminRoutes from "./View/ManageView/adminRoutes";
import { Permission } from "./AppConst";

async function checkPermission(to, from, next) {
    if(!store.getters.isAuthChecked)
        await store.dispatch(CHECK_AUTH);
    next(store.getters.can(to.name));
}

const routes = [
    //Default
    {
        path: "/",
        redirect: "/Home"
    },
    {
        path: "/Home",
        name: Permission.CaseListing,
        component: () =>
            import(/* webpackChunkName: "caseview" */ "./View/CaseView/CaseView.vue"),
        display: "Trang chính",
        navbar: true, //Renders on nav bar if true
        beforeEnter: checkPermission
    },
    {
        path: "/Assign",
        name: Permission.ScheduleAssigner,
        component: () =>
            import(/* webpackChunkName: "assignerview" */ "./View/ScheduleView/ScheduleView.vue"),
        display: "Lịch trực",
        navbar: true,
        beforeEnter: checkPermission
    },
    {
        path: "/Manage",
        name: Permission.Management,
        component: () =>
            import(/* webpackChunkName: "adminview" */ "./View/ManageView/ManageView.vue"),
        display: "Quản lý",
        navbar: true,
        beforeEnter: checkPermission,
        children: [...adminRoutes],
        redirect: {
            name: Permission.SystemInfo //Default
        }
    }
    //{
    //    path: '/Test',
    //    name: 'Test',
    //    component: () => import(/* webpackChunkName: "testview" */'./Component/TestView.vue'),
    //    display: 'Test',
    //    navbar: true
    //}]
];
module.exports = routes;
