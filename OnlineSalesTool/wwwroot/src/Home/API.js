export default {
    //Auth
    Login: '/Account/Login',
    Ping: '/Account/Ping',
    //Get Assigner VM
    AssignerVmAPI: '/API/Shift/Get',
    SaveScheduleAPI: '/API/Shift/Create',
    //POS manager
    PosVM: '/API/Pos/Get',
    CreatePos: '/API/Pos/Create',
    UpdatePos: '/API/Pos/Update',
    //User manager
    UserVM: '/API/User/Get',
    CreateUser: '/API/User/Create',
    UpdateUser: '/API/User/Update',
    UserSearchSuggest: '/API/User/SearchSuggest?role={role}&q='
}
