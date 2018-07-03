export const Roles = {
    CA: "CA",
    BDS: "BDS",
    ADMIN: "ADMIN",
    ASM: "ASM"
};
export const ConstStorage = {
    //Local store
    TokenStorage: 'user-token',
    IdentityStorage: 'id',
    ExpireStorage: 'exp'
    //AbilityStoreage: 'ablity',
    //RoleStoreage: 'role'

};
export const Permission = {
    //Main views
    CaseListing: 'CaseListing',
    ScheduleAssigner: 'ScheduleAssigner',
    Management: 'Management',
    //Case view
    CustomerConfirm: "CustomerConfirm",
    EnterContractNumber: "EnterContractNumber",
    AssignCase: "AssignCase",
    //Schedules
    CreateSchedule: 'CreateSchedule',
    EditSchedule: 'EditSchedule',
    //Pos manager
    PosManager: 'PosManager',
    CreatePOS: 'CreatePOS',
    UpdatePOS: 'UpdatePOS',
    //User manager
    UserManager: 'UserManager',
    CreateUser: 'CreateUser',
    UpdateUser: 'UpdateUser',
    //Info
    SystemInfo: 'SystemInfo'
}
