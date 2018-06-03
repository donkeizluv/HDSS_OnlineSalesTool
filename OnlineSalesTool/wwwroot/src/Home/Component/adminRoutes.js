import store from '../store'
import { CHECK_AUTH, LOGOUT } from '../actions'
import {
    Permission
} from '../AppConst'

async function checkPermission(to, from, next) {
    if(!store.getters.isAuthChecked)
        await store.dispatch(CHECK_AUTH);
    next(store.getters.can(to.name));
}

const childRoutes = [
    {
        path: '',
        name: Permission.SystemInfo,
        display: 'Thông tin',
        navbar: true,
        component: () =>
            import ( /* webpackChunkName: "posmanager" */ './SystemInfoView.vue'),
    },
    {
        path: 'POS',
        name: Permission.PosManager, //Also acts as nav guard check
        display: 'POS',
        navbar: true,
        component: () =>
            import ( /* webpackChunkName: "posmanager" */ './PosManagerView.vue'),
        beforeEnter: checkPermission
    },
    {
        path: 'User',
        name: Permission.UserManager,
        display: 'Người dùng',
        navbar: true,
        component: () =>
            import ( /* webpackChunkName: "usermanager" */ './UserManagerView.vue'),
        beforeEnter: checkPermission
    }
];

module.exports = childRoutes;