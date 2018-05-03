<!--Assigner view-->
<template id="assignerview">
    <div>
        <div class="row">
            <div class="col-lg-8 mx-auto">
                <div class="form-group">
                    <div class="form-inline">
                        <!--Select POS-->
                        <label>POS: </label>
                        <select class="form-control m-2"
                                v-model="SelectedPos" 
                                v-on:change="SelectedPosChanged"
                                v-bind:disabled="CreateMode">
                            <option disabled>POS...</option>
                            <option v-for="pos in POSs"
                                    v-bind:key="pos.PosId"
                                    v-bind:value="pos.PosId">
                                {{ComposePOSLabel(pos)}}
                            </option>
                        </select>
                        <!--Select prev month-->
                        <label>Tháng: </label>
                        <select class="form-control m-2"
                                v-model="SelectedPrevSchedule"
                                v-on:change="LoadPrevSchedule"
                                v-bind:disabled="CreateMode">
                            <option disabled>Tháng...</option>
                            <option v-for="prev in CurrentPrevSchedules" 
                                    v-bind:key="prev.MonthYear" 
                                    v-bind:value="prev.MonthYear">{{prev.DisplayMonthYear}}</option>
                        </select>
                        <form class="form-inline">
                            <!--<button class="btn m-2" type="button" v-on:click="LoadPrevSchedule" v-bind:disabled="!CanLoadSchedule">
                                <span>Xem</span>
                            </button>-->
                            <button class="btn btn-sm btn-primary m-2"
                                    type="button"
                                    v-on:click="CreateNewSchedule"
                                    v-bind:disabled="!CanCreateNewSchedule">
                                <i class="fas fa-plus"></i>
                            </button>
                            <button class="btn btn-sm btn-warning m-2"
                                    type="button"
                                    disabled="disabled">
                                <i class="fas fa-edit"></i>
                            </button>
                            <button class="btn btn-sm btn-success m-2"
                                    type="button"
                                    v-on:click="SaveSchedule"
                                    v-bind:disabled="!CanSaveSchedule">
                                <i class="fas fa-check"></i>
                            </button>
                            <button class="btn btn-sm btn-danger m-2"
                                    type="button"
                                    v-on:click="CancelSchedule"
                                    v-bind:disabled="!CanCancelSchedule">
                                <i class="fas fa-trash"></i>
                            </button>
                        </form>
                    </div>
                </div>
            </div>
        </div>
        <div class="row">
            <div class="col">
                <!--Title-->
                <h4 v-show="IsModeTextVisible" class="text-center mb-md-3">
                    {{ModeText.Name}}
                    <span class="badge badge-success">
                        {{ModeText.POS}}
                    </span>
                    <span class="badge badge-danger">
                        {{ModeText.MonthYear}}
                    </span>
                </h4>
            </div>
        </div>
        <div class="row">
            <div class="col">
                <!--Schedule details-->
                <div class="d-flex justify-content-around flex-wrap">
                    <template v-for="item in CurrentDays">
                        <shift-detail v-bind:display="ShiftDetailDisplay"
                                      v-bind:users="Users"
                                      v-bind:readonly="ReadOnly"
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
    import moment from 'moment'

    //Api
    import API_Const from '../API'

    export default {
        name: 'AssignerView',
        template: '#assignerview',
        components: {
            'shift-detail': shiftdetail
        },
        created: async function () {
            await this.LoadVM();
        },
        data: function() {
            return {
                POSs: [],
                Users: [],
                //Sys
                SystemMonthYear: null,
                SystemMonthYearDisplay: null,
                TotalDaysOfMonth: null,

                //selected identifier
                SelectedPos: null,
                SelectedPrevSchedule: null,

                CreateMode: false,
                ReadOnly: true,
                ShiftDetailDisplay: false,
                //Current displaying

                //Creating, viewing...
                ModeText: {
                    Name: '',
                    POS: '',
                    MonthYear: null
                },
                IsModeTextVisible: false,

                //Bindings
                CurrentPOS: null,
                CurrentPrevSchedules: null,
                CurrentShifts: [],
                CurrentDays: []
            };
        },
        computed: {
            CanLoadSchedule: function () {
                if (this.SelectedPrevSchedule)
                    return true;
                return false;
            },
            CanCreateNewSchedule: function () {
                if (this.CreateMode) return false;
                if (this.CurrentPOS)
                    return !this.CurrentPOS.HasCurrentMonthSchedule;
                return false;
            },
            CanSaveSchedule: function () {
                if (this.ReadOnly) return false;
                //Check if all shifts of days are set
                return this.CurrentDays.every(d => d.Shifts.every(s => {
                    if (!s.Assign) return false;
                    if (!s.Assign.value) return false;
                    return true;
                }));
            },
            CanCancelSchedule: function () {
                //Back to view mode
                return this.CreateMode
            }
        },
        methods: {
            LoadVM: async function () {
                //Fetch VM
                try {
                    //Get VM
                    var response = await axios.get(API_Const.AssignerVmAPI);
                    var vm = response.data;
                    //console.log(vm);
                    //Set POSs
                    this.POSs = vm.POSs;
                    //Set users
                    this.Users = vm.Users;
                    //Set sys ref
                    this.SystemMonthYear = vm.SystemMonthYear;
                    this.SystemMonthYearDisplay = vm.SystemMonthYearDisplay;
                    this.TotalDaysOfMonth = vm.TotalDaysOfMonth;
                } catch (e) {
                    //Show some sort of alert
                    //this.$router.app.$emit('showerror', 'Tải dữ liệu thất bại, vui lòng liên hệ IT.');
                    this.$emit('showerror', 'Tải dữ liệu thất bại, vui lòng liên hệ IT.');
                    //Disabled function
                    //TODO: Better mechanism to do this
                    this.POSs = null;
                    this.Users = null;
                    //console.log(e);
                }
            },
            SelectedPosChanged: function () {
                if (this.CreateMode) return;
                //Unset selected schedule
                this.SelectedPrevSchedule = null;
                //Get POS object
                this.CurrentPOS = this.POSs.find(p => p.PosId == this.SelectedPos);
                if (!this.CurrentPOS) return; //Someone tries to screw up the app
                //console.log(this.CurrentPOS);
                //Set displaying schedules
                this.CurrentPrevSchedules = this.CurrentPOS.PreviousMonthSchedules;
                //Set displaying shifts
                this.CurrentShifts = this.CurrentPOS.Shifts;
                //Clear title
                this.HideModeText();
            },
            ClearSchedule: function () {
                this.CurrentDays = [];
            },
            CreateNewSchedule: function () {
                if (!this.CanCreateNewSchedule) return;
                this.CreateMode = true;
                this.ReadOnly = false;
                //Create new....
                //If CurrentDays are loaded then check if those days belong to same POS
                var emptyDays = this.GetEmptyDays(this.TotalDaysOfMonth, this.CurrentShifts);
                var flattenUsers = this.Users.map(u => u.UserId);
                var flattenShifts = this.CurrentShifts.map(s => s.ShiftId);
                if (this.CurrentPOS.PosId == this.SelectedPos) {
                    //Reuse loaded day
                    var reconstruct = [];
                    //Replace empty days with loaded days if they are same day
                    for(let d of emptyDays) {
                        //Refill values of empty days with loaded one
                        var loaded = this.CurrentDays.find(l => l.Day == d.Day);
                        if (loaded) {
                            //Re-built shifts of loaded with current shifts & re-fill values if possible
                            var emptyShifts = d.Shifts;
                            //Refill loaded values to empty
                            emptyShifts.forEach(rs => {
                                //Find shift
                                var loadedShift = loaded.Shifts.find(l => l.ShiftId == rs.ShiftId);
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
                    this.CurrentDays = reconstruct;
                }
                else {
                    //Happens if selected pos changed without updating CurrentPOS
                    this.CurrentDays = emptyDays;
                }
                this.ShowModeText('Tạo ca trực mới: ', this.CurrentPOS.PosCode, this.SystemMonthYearDisplay)
                //this.ModeName = `Tạo ca trực mới: ${this.CurrentPOS.PosCode} ${this.SystemMonthYearDisplay}`;
            },
            SaveSchedule: async function () {
                if (!this.CanSaveSchedule) return;
                var api = API_Const.SaveScheduleAPI;
                //Transform to linear shift detail format
                //Contructor:
                //ScheduleContainer(int targetPos, DateTime targetMonthYear, IEnumerable<ShiftSchedulePOCO> schedules)
                var postObject = {
                    targetPos: this.CurrentPOS.PosId, //Target POS of this schedule
                    //Must be same default time of non specified time of C# Datetime in order to re-select after saved successfully
                    //MomentJS, C# default time if not specified is 12:00:00, 00:00:00 respectively
                    targetMonthYear: moment(this.SystemMonthYear).date(1).format('YYYY-MM-DD[T00:00:00]'), //Target month of this schedule, day must be 1
                    schedules: []
                };
                var schedules = this.CurrentDays.reduce((acc, d) => {
                    let shiftDetails = d.Shifts.map(s => {
                        return {
                            ShiftDate: moment(this.SystemMonthYear).date(d.Day).format(),
                            User: {
                                UserId: s.Assign.value
                            },
                            Shift: {
                                ShiftId: s.ShiftId
                            }
                        };
                    });
                    acc.push(...shiftDetails);
                    return acc;
                }, []);
                //console.log(schedules);
                postObject.schedules = schedules;
                try {
                    await axios.post(API_Const.SaveScheduleAPI, postObject);
                    this.$emit('showsuccess', 'Tạo lịch làm việc mới thành công!');
                    //Set current view to newly added schedule
                    //Reload VM
                    var prevPos = this.SelectedPos;
                    await this.LoadVM();
                    //Back to view
                    this.CancelSchedule();
                    //Set POS then PrevSchedules
                    this.SelectedPos = prevPos;
                    this.SelectedPosChanged();
                    this.SelectedPrevSchedule = postObject.targetMonthYear;
                    this.LoadPrevSchedule();
                } catch (e) {
                    //Show error mess
                    //console.log(e);
                    this.$emit('showinfo', 'Có lỗi trong quá trình tạo lịch làm việc.');
                }

            },
            CancelSchedule: function () {
                if (!this.CreateMode) return;
                //Clean up ....
                this.CreateMode = false;
                this.ReadOnly = true;
                this.CurrentDays = [];
                this.SelectedPrevSchedule = null;
                this.HideModeText();
            },
            //Internal use
            GetEmptyDays: function (dayCount, shifts) {
                var arr = Array(dayCount).fill(1).map((x, y) => x + y);
                var days = arr.map(d => {
                    return {
                        Day: d,
                        Shifts: shifts.map(s => {
                            return {
                                Name: s.Name,
                                ShiftId: s.ShiftId,
                                //Assign: { label: null, value: null }
                                Assign: null
                            }
                        })
                    }
                });
                return days;
            },
            LoadPrevSchedule: function () {
                if (this.CreateMode) return;
                if (!this.CanLoadSchedule) { //????
                    this.ClearSchedule();
                    return;
                }
                //Get schedule container
                var scheduleContainer = this.CurrentPrevSchedules.find(s => s.MonthYear == this.SelectedPrevSchedule);
                if (!scheduleContainer) throw 'Cant get scheduleContainer';
                //console.log(scheduleContainer);
                //Group Shifts of ShiftDetails
                var containerShifts = scheduleContainer.Schedules.reduce((ac, sc) => {
                    if (!ac.some(v => v.ShiftId == sc.Shift.ShiftId))
                        ac.push(sc.Shift);
                    return ac;
                }, []);
                //console.log(containerShifts);
                //Create empty array of each days
                var emptyDays = this.GetEmptyDays(scheduleContainer.TotalDaysOfMonth, containerShifts);
                //Transform to displaying days
                //Map linear schedule details to empty day objects
                emptyDays.forEach(emptyDay => {
                    var containerShifts = scheduleContainer.Schedules.filter(shift => shift.Day == emptyDay.Day);
                    //console.log(containerShifts);
                    containerShifts.forEach(cs => {
                        var shift = emptyDay.Shifts.find(shift => shift.ShiftId == cs.Shift.ShiftId);
                        shift.Assign = { value: cs.User.UserId, label: cs.User.DisplayName };
                    });
                });
                //console.log(days);
                //Display
                this.CurrentDays = emptyDays;
                this.ShowModeText('Xem ca trực: ', this.CurrentPOS.PosCode, scheduleContainer.DisplayMonthYear)
                //this.ModeName = `Xem ca trực: ${this.CurrentPOS.PosCode} ${scheduleContainer.DisplayMonthYear}`;
            },
            ShowModeText: function (name, pos, monthYear) {
                this.ModeText.Name = name;
                this.ModeText.POS = pos;
                this.ModeText.MonthYear = monthYear;
                this.IsModeTextVisible = true;
            },
            HideModeText: function () {
                this.ModeText.Name = '';
                this.ModeText.POS = '';
                this.ModeText.MonthYear = '';
                this.IsModeTextVisible = false;
            },
            ComposePOSLabel: function (pos) {
                return pos.PosCode + (pos.HasCurrentMonthSchedule ? '' : ' (chưa xếp)');
                //Too long
                //return `${pos.PosName}-${pos.PosCode}` + (pos.HasCurrentMonthSchedule ? '' : ' (chưa xếp)');
            }
        }
    }
</script>