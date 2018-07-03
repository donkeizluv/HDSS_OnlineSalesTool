//Finely define of what view/route allow for any roles
import { Permission } from './AppConst'

export default [
    {
        //Matches all
        role: '',
        can: [ 
            Permission.Management,
            Permission.SystemInfo,
            Permission.CaseListing,
            Permission.ScheduleAssigner
         ]
    },
    {
        role: 'CA',
        can: [
            Permission.CustomerConfirm,
            Permission.EnterContractNumber
        ]
    },
    {
        role: 'BDS',
        can: [
            Permission.CreateSchedule,
            Permission.PosManager,
            Permission.UserManager,
            Permission.AssignCase
        ]
    },
    {
        role: 'ADMIN',
        can: [
            //Views
            Permission.ScheduleAssigner,
            Permission.Management,
            //Management views
            Permission.PosManager,
            Permission.UserManager,
            //Management actions
            //Case
            Permission.AssignCase,
            //Schedules
            Permission.CreateSchedule,
            Permission.EditSchedule,
            //Users
            Permission.UpdateUser,
            Permission.CreateUser,
            //POSs
            Permission.CreatePOS,
            Permission.UpdatePOS
        ]
    }
]