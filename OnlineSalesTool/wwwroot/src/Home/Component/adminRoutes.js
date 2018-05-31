import store from '../store'
import { Permission } from '../AppConst'

function checkPermission(to, from, next) {
    next(store.getters.can(to.name));
}

const childRoutes = [
    //Triggers warning but this is a redirect so that wont be a problem
    //Moved this into component for dynamic permission checking
    //Fuckkkkkkkk moved this into component never worksssss, no idea why
    //Its plausible that routes get init/render so soon that permission in store havent initialized yet
    //So when routes get rendered, permission checking in component failed to find correct permitted routes 
    //-> no default route / redirect set on routes init
    //Solution:
    //Best way to workaround this is probly create a default landing view for all roles
    {
        path: '',
        name: 'default',
        navbar: false,
        redirect: { name: Permission.PosManager }
    },
    {
        path: 'POS',
        name: Permission.PosManager, //Also acts as nav guard check
        display: 'POS',
        navbar: true,
        component: () => import(/* webpackChunkName: "posmanager" */'./PosManagerView.vue'),
        beforeEnter: checkPermission
    },
    {
        path: 'User',
        name: Permission.UserManager,
        display: 'Người dùng',
        navbar: true,
        component: () => import(/* webpackChunkName: "usermanager" */'./UserManagerView.vue'),
        beforeEnter: checkPermission
    }
];

module.exports = childRoutes;