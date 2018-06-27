export default {
    //Auth
    Login: '/Account/Login',
    Ping: '/Account/Ping',
    //Get Assigner VM
    AssignerVmAPI: '/API/Shift/Get',
    ScheduleDetailAPI: '/API/Shift/GetDetails?id={id}',
    SaveScheduleAPI: '/API/Shift/Create',
    //POS manager
    PosVM: '/API/Pos/Get',
    CreatePos: '/API/Pos/Create',
    UpdatePos: '/API/Pos/Update',
    CheckCode: '/API/Pos/Check?q={code}',
    //User manager
    UserVM: '/API/User/Get',
    CreateUser: '/API/User/Create',
    UpdateUser: '/API/User/Update',
    UserSearchSuggest: '/API/User/Suggest?role={role}&q=',
    CheckUsername: '/API/User/Check?q={name}',
    //Case
    CaseVM: '/API/Case/Get',
    CaseUpdateIndusCode: '/API/Case/UpdateCode',
    CaseCustomerConfirm: '/API/Case/CustomerConfirm'
}
