<!--Assigner view-->
<template id="assignerview">
    <div>
        <div>
            <div class="row">
                <div class="col-lg-5 mx-auto">
                    <div class="form-row">
                        <div class="form-inline mx-auto">
                            <!--Select pos-->
                            <label>POS: </label>
                            <select class="form-control m-2"
                                    v-model="selectedPos"
                                    v-on:change="onPosChanged"
                                    v-bind:disabled="createMode">
                                <option disabled>Pos...</option>
                                <option v-for="pos in poses"
                                        v-bind:key="pos.PosId"
                                        v-bind:value="pos.PosId">
                                    {{composePOSLabel(pos)}}
                                </option>
                            </select>
                            <!--Select prev month-->
                            <label>Tháng: </label>
                            <select class="form-control m-2"
                                    v-model="selectedPrevSchedule"
                                    v-on:change="reloadPrevSchedule"
                                    v-bind:disabled="createMode">
                                <option disabled>Tháng...</option>
                                <option v-for="prev in currentPrevSchedules"
                                        v-bind:key="prev.PosScheduleId"
                                        v-bind:value="prev.PosScheduleId">
                                    {{prev.DisplayMonthYear}}
                                </option>
                            </select>
                        </div>
                        <div class="form-inline mx-auto">
                            <button class="btn btn-sm btn-primary m-2"
                                    type="button"
                                    v-on:click="createNewSchedule"
                                    v-bind:disabled="!canCreateNewSchedule">
                                <i class="fas fa-plus"></i>
                            </button>
                            <button class="btn btn-sm btn-warning m-2"
                                    type="button"
                                    v-bind:disabled="!canEditSchedule">
                                <i class="fas fa-edit"></i>
                            </button>
                            <button class="btn btn-sm btn-success m-2"
                                    type="button"
                                    v-on:click="saveSchedule"
                                    v-bind:disabled="!canSaveSchedule">
                                <i class="fas fa-check"></i>
                            </button>
                            <button class="btn btn-sm btn-danger m-2"
                                    type="button"
                                    v-on:click="cancelSchedule"
                                    v-bind:disabled="!canCancelSchedule">
                                <i class="fas fa-trash"></i>
                            </button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="row">
            <div class="col">
                <!--Title-->
                <h4 v-show="isModeTextVisible" class="text-center mb-md-3 mt-md-3">
                    {{modeText.mode}}
                    <span class="badge badge-success">
                        {{modeText.pos.PosName}}
                    </span>
                    <span class="badge badge-primary">
                        {{modeText.pos.PosCode}}
                    </span>
                    <span class="badge badge-danger">
                        {{modeText.monthYear}}
                    </span>
                </h4>
            </div>
        </div>
        <div class="row">
            <div class="col">
                <!--Schedule details-->
                <div class="d-flex justify-content-around flex-wrap">
                    <template v-for="item in currentDays">
                        <shift-detail v-bind:key="item.Day"
                                    v-bind:display="shiftDetailDisplay"
                                    v-bind:users="users"
                                    v-bind:readonly="readOnly"
                                    v-bind:day.sync="item" />
                    </template>
                </div>
            </div>
        </div>
    </div>
</template>
<script>
    import axios from 'axios'
    import shiftdetail from './ShiftDetail.vue'
    //import moment from 'moment'
    //date-fns
    import setDate from 'date-fns/set_date'
    import format from 'date-fns/format'
    //Api
    import API_Const from '../API'
    //Const
    import { Permission } from '../AppConst'

    export default {
        name: 'AssignerView',

        template: '#assignerview',

        components: {
            'shift-detail': shiftdetail
        },
        mounted: function () {
            //Init & load vm
            this.init();
        },
        data: function () {
            return {
                //Maybe store VM in vuex?
                //VM data
                poses: [],
                users: [],
                //Sys
                systemMonthYear: null,
                totalDaysOfMonth: null,
                //Display bindings
                currentPOS: null,
                //Binding depends on POS
                currentPrevSchedules: null,
                currentShifts: [],
                currentDays: [],

                //selected identifier
                selectedPos: null,
                selectedPrevSchedule: null,

                createMode: false,
                readOnly: true,
                shiftDetailDisplay: false,
                //Current displaying

                //Creating, viewing...
                modeText: {
                    mode: '',
                    pos: {},
                    monthYear: null
                },
                isModeTextVisible: false
            };
        },

        computed: {
            //Vuex permission
            canCreate: function () {
                //Must have shifts to create schedule
                if (this.currentShifts.length < 1) return false;
                return this.$store.getters.can(Permission.CreateSchedule);
            },
            canEdit: function () {
                return this.$store.getters.can(Permission.EditSchedule);
            },

            isLoading: function () {
                return this.$store.getters.Loading;
            },

            canEditSchedule: function () {
                if (!this.canEdit) return false;
                if (this.createMode) return false;
                return !!this.selectedPos && !!this.selectedPrevSchedule;
            },
            canCreateNewSchedule: function () {
                if (!this.canCreate) return false;
                if (this.createMode) return false;
                if (this.currentPOS)
                    return !this.currentPOS.HasCurrentMonthSchedule;
                return false;
            },
            canSaveSchedule: function () {
                if (this.readOnly) return false;
                //Check if all shifts of days are set
                return this.currentDays.every(d => d.Shifts.every(s => {
                    if (!s.Assign) return false;
                    if (!s.Assign.value) return false;
                    return true;
                }));
            },
            canCancelSchedule: function () {
                //Back to view mode
                return this.createMode
            }
        },

        methods: {
            init: async function () {
                await this.loadVM();
                //Display first prev schedule of 1st POS
                if (this.poses.length > 0) {
                    //Defaults
                    this.selectedPos = this.poses[0].PosId;
                    this.reloadSelectedPos();
                    this.selectDefaultSchedule();
                }
                //console.log(setDate(this.systemMonthYear, 1));
            },
            loadVM: async function () {
                //Fetch VM
                try {
                    //Get VM
                    let { data } = await axios.get(API_Const.AssignerVmAPI);
                    //console.log(data);
                    //Set poses
                    this.poses = data.POSs;
                    //Set users
                    this.users = data.Users;
                    //Set sys ref
                    this.systemMonthYear = data.SystemMonthYear;
                    this.totalDaysOfMonth = data.TotalDaysOfMonth;
                } catch (e) {
                    this.$emit('showerror', 'Tải dữ liệu thất bại, vui lòng liên hệ IT.');
                    //Disabled function
                    //TODO: Better mechanism to do this
                    this.poses = [];
                    this.users = [];
                }
            },
            onPosChanged: function () {
                this.reloadSelectedPos();
                this.selectDefaultSchedule();
            },
            reloadSelectedPos: function () {
                if (this.createMode) return;
                //Find POS
                this.currentPOS = this.poses.find(p => p.PosId == this.selectedPos);
                if (!this.currentPOS) return; //Someone tries to screw up the app
                //Set affective Shifts
                this.currentShifts = this.currentPOS.Shifts;
                //Set displaying available prev schedules
                this.currentPrevSchedules = this.currentPOS.PreviousMonthSchedules;
            },
            selectDefaultSchedule: async function (posScheduleId = -1) {
                //If prev sche is available, display 1st schedule
                if (this.currentPrevSchedules.length > 0) {
                    if (posScheduleId == -1) {
                        //If not specify then load first
                        this.selectedPrevSchedule = this.currentPrevSchedules[0].PosScheduleId;
                    }
                    else {
                        this.selectedPrevSchedule = posScheduleId;
                    }
                    await this.reloadPrevSchedule();
                    return;
                }
                this.selectedPrevSchedule = null;
                this.clearSchedule();
            },
            clearSchedule: function () {
                this.currentDays = [];
                this.hideModeText();
            },
            createNewSchedule: function () {
                if (!this.canCreateNewSchedule) return;
                this.createMode = true;
                this.readOnly = false;
                //Create new....
                //If currentDays are loaded then check if those days belong to same pos
                let emptyDays = this.createEmptyDays(this.totalDaysOfMonth, this.currentShifts);
                let flattenUsers = this.users.map(u => u.UserId);
                let flattenShifts = this.currentShifts.map(s => s.ShiftId);
                if (this.currentPOS.PosId == this.selectedPos) {
                    //Recontruct loaded day to current month array
                    let reconstruct = [];
                    //Replace empty days with loaded days if they are same day
                    for (let d of emptyDays) {
                        //Refill values of empty days with loaded one
                        let loaded = this.currentDays.find(l => l.Day == d.Day);
                        if (loaded) {
                            //Re-built shifts of loaded with current shifts & re-fill values if possible
                            let emptyShifts = d.Shifts;
                            //Refill loaded values to empty
                            emptyShifts.forEach(rs => {
                                //Find shift
                                let loadedShift = loaded.Shifts.find(l => l.ShiftId == rs.ShiftId);
                                if (loadedShift) {
                                    //If user still under mangement then re-use assign values
                                    if (flattenUsers.includes(loadedShift.Assign.value)) {
                                        rs.Assign = loadedShift.Assign;
                                    }
                                }
                            });
                            loaded.Shifts = emptyShifts;
                            reconstruct.push(loaded); //Reuse loaded
                        }
                        else {
                            //No loaded found, use empty instead
                            reconstruct.push(d)
                        }
                    }
                    this.currentDays = reconstruct;
                }
                else {
                    //Happens if selected pos changed without updating currentPOS
                    this.currentDays = emptyDays;
                }
                this.showModeText('Tạo ca trực mới: ', this.currentPOS, format(this.systemMonthYear, 'MM-YYYY'))
            },
            saveSchedule: async function () {
                if (!this.canSaveSchedule) return;
                let api = API_Const.SaveScheduleAPI;
                //Contructor:
                //ScheduleContainer(int targetPos, DateTime targetMonthYear, IEnumerable<ShiftSchedulePOCO> schedules)
                let postObject = {
                    targetPos: this.currentPOS.PosId, //Target pos of this schedule
                    //Must be same default time of non specified time of C# Datetime in order to re-select after saved successfully
                    //Target month of this schedule, day must be 1
                    targetMonthYear: format(setDate(this.systemMonthYear, 1), 'YYYY-MM-DD[T00:00:00]'),
                    //targetMonthYear: moment(this.systemMonthYear).date(1).format('YYYY-MM-DD[T00:00:00]'),
                    schedules: []
                };
                let schedules = this.toLinearShiftDetail(this.currentDays);
                //console.log(schedules);
                postObject.schedules = schedules;
                try {
                    var { data } = await axios.post(API_Const.SaveScheduleAPI, postObject);
                    this.$emit('showsuccess', 'Tạo lịch làm việc mới thành công!');
                    //Set current view to newly added schedule
                    //Reload VM
                    await this.loadVM();
                    //Back to view
                    this.createMode = false;
                    this.readOnly = true;
                    this.reloadSelectedPos();
                    await this.selectDefaultSchedule(data);
                } catch (e) {
                    //Show error mess
                    console.log(e);
                    this.$emit('showinfo', 'Có lỗi trong quá trình tạo lịch làm việc.');
                }

            },
            //Private
            //Transform to linear shift detail format
            toLinearShiftDetail: function (days) {
                return days.reduce((acc, d) => {
                    let shiftDetails = d.Shifts.map(s => {
                        return {
                            Day: d.Day,
                            User: {
                                UserId: s.Assign.value
                            },
                            Shift: {
                                ShiftId: s.ShiftId
                            }
                        };
                    });
                    //acc.push(...shiftDetails);
                    return acc.concat(shiftDetails);
                }, []);
            },
            cancelSchedule: function () {
                if (!this.createMode) return;
                //Cancel create mode & reload prev schedule
                this.createMode = false;
                this.readOnly = true;
                this.currentDays = [];
                //this.selectedPrevSchedule = null;
                //Reload prev schedule
                this.reloadPrevSchedule();
            },
            //Internal use
            createEmptyDays: function (dayCount, shifts) {
                let arr = Array(dayCount).fill(1).map((x, y) => x + y);
                let days = arr.map(d => {
                    return {
                        Day: d,
                        Shifts: shifts.map(s => {
                            return {
                                Name: s.Name,
                                ExtName: s.ExtName,
                                ShiftId: s.ShiftId,
                                Assign: null
                            }
                        })
                    }
                });
                return days;
            },
            reloadPrevSchedule: async function () {
                //If no prev sche selected, clear display
                if (!this.selectedPrevSchedule) {
                    this.hideModeText();
                    this.clearSchedule();
                    return;
                }
                //Get schedule container
                let scheduleContainer = this.currentPrevSchedules
                    .find(s => s.PosScheduleId === this.selectedPrevSchedule);
                if (!scheduleContainer) throw 'Cant get scheduleContainer: ' + this.selectedPrevSchedule;
                //Load & attach detail if not available
                if (!scheduleContainer.hasOwnProperty('details')) {
                    scheduleContainer.details = await this.loadScheduleDetails(scheduleContainer.PosScheduleId);
                }
                //console.log(scheduleContainer);
                //Group Shifts of ShiftDetails
                let containerShifts = scheduleContainer.details.reduce((ac, sc) => {
                    if (!ac.some(v => v.ShiftId == sc.Shift.ShiftId))
                        ac.push(sc.Shift);
                    return ac;
                }, []);
                //console.log(containerShifts);
                //Create empty array of each days
                let emptyDays = this.createEmptyDays(scheduleContainer.TotalDaysOfMonth, containerShifts);
                //Transform to displaying days
                //Map linear schedule details to empty day objects
                emptyDays.forEach(emptyDay => {
                    let containerShifts = scheduleContainer.details.filter(shift => shift.Day === emptyDay.Day);
                    //console.log(containerShifts);
                    containerShifts.forEach(cs => {
                        let shift = emptyDay.Shifts.find(shift => shift.ShiftId === cs.Shift.ShiftId);
                        shift.Assign = { value: cs.User.UserId, label: cs.User.DisplayName };
                    });
                });
                //console.log(days);
                //Display
                this.currentDays = emptyDays;
                this.showModeText('Xem ca trực: ', this.currentPOS, scheduleContainer.DisplayMonthYear)
                //this.ModeName = `Xem ca trực: ${this.currentPOS.PosCode} ${scheduleContainer.DisplayMonthYear}`;
            },
            loadScheduleDetails: async function (id) {
                try {
                    var { data } = await axios.get(API_Const.ScheduleDetailAPI.replace('{id}', id));
                    //console.log(data);
                    return data;

                } catch (e) {
                    this.$emit('showinfo', 'Tải chi tiết lịch làm việc thất bại.');
                }
            },
            showModeText: function (mode, pos, monthYear) {
                this.modeText.mode = mode;
                this.modeText.pos = pos;
                this.modeText.monthYear = monthYear;
                this.isModeTextVisible = true;
            },
            hideModeText: function () {
                this.modeText.mode = '';
                this.modeText.pos = {};
                this.modeText.monthYear = '';
                this.isModeTextVisible = false;
            },
            composePOSLabel: function (pos) {
                return pos.PosCode + (pos.HasCurrentMonthSchedule ? '' : ' (chưa xếp)');
                //Too long
                //return `${pos.PosName}-${pos.PosCode}` + (pos.HasCurrentMonthSchedule ? '' : ' (chưa xếp)');
            }
        }
    }
</script>