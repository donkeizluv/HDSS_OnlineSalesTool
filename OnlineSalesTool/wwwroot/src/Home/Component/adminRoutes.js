import store from '../store'
import { Permission } from '../AppConst'
//import PosManagerView from './PosManagerView.vue'
//import UserManagerView from './UserManagerView.vue'

//All possible route
const childRoutes = [
    //Triggers warning but this is a redirect so that wont be a problem
    //Moved this into component for dynamic permission checking
    //Fuckkkkkkkk moved this into component never worksssss, no idea why
    //Its plausible that routes get init/render so soon that permission in store havent initialized yet
    //So when routes get rendered, permission checking in component failed to find correct permitted routes 
    //-> no default route / redirect set on routes init
    {
        path: '',
        name: 'default',
        navbar: false,
        redirect: { name: 'POS' }
    },
    {
        path: 'POS',
        //alias: '', //Clean and simple but doesnt play nicely with active-class prop
        name: 'POS',
        display: 'POS',
        navbar: true,
        component: () => import(/* webpackChunkName: "posmanager" */'./PosManagerView.vue'),
        //component: PosManagerView,
        permission: Permission.CanSeePosManager
    },
    {
        path: 'User',
        name: 'User',
        display: 'Người dùng',
        navbar: true,
        component: () => import(/* webpackChunkName: "usermanager" */'./UserManagerView.vue'),
        //component: UserManagerView,
        permission: Permission.CanSeeUserManager
    }
];

module.exports = childRoutes;