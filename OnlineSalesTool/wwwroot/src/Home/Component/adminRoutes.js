//import PosManagerView from './PosManagerView.vue'
//import UserManagerView from './UserManagerView.vue'
import { Permission } from '../AppConst'

const childRoutes = [
    {
        path: 'POS',
        name: 'POS',
        display: 'POS',
        component: () => import(/* webpackChunkName: "posmanager" */'./PosManagerView.vue'),
        permission: Permission.CanSeePosManager
    },
    {
        path: 'User',
        name: 'User',
        display: 'Người dùng',
        component: () => import(/* webpackChunkName: "usermanager" */'./UserManagerView.vue'),
        permission: Permission.CanSeeUserManager
    }
   ]
module.exports = childRoutes;