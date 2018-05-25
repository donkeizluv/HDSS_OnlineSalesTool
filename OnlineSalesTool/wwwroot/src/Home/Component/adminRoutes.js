import PosManagerView from './PosManagerView.vue'
import UserManagerView from './UserManagerView.vue'

const childRoutes = [
    {
        path: '/POS',
        name: 'POS',
        display: 'POS',
        component: PosManagerView
    },
    {
        path: '/User',
        name: 'User',
        display: 'Người dùng',
        component: UserManagerView
    }
   ]
module.exports = childRoutes;