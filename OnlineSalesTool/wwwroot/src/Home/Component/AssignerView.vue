<!--Assigner view-->
<template id="assignerview">
    <div>
        <div class="row">
            <div class="col-lg-4 mx-auto">
                <div class="form-group">
                    <div class="form-inline">
                        <!--Select POS-->
                        <select class="form-control" v-model="SelectedPos" v-on:change="SelectedPosChanged">
                            <option disabled>Chọn POS</option>
                            <option v-for="pos in POSs" v-bind:key="pos.PosId" v-bind:value="pos.PosId">{{pos.PosName}} - {{pos.PosCode}}</option>
                        </select>
                        <!--Select prev month-->
                        <select class="form-control" v-model="SelectedPrevSchedule">
                            <option disabled>Chọn tháng</option>
                            <option v-for="prev in CurrentPrevSchedules" v-bind:key="prev.MonthYear" v-bind:value="prev.MonthYear">{{prev.DisplayMonthYear}}</option>
                        </select>
                        <button class="btn btn-primary ml-2" v-on:click="LoadPrevScheduleClicked" v-bind:disabled="!CanExecuteLoadSchedule">
                            <span>Xem</span>
                        </button>
                    </div>
                </div>
            </div>
        </div>
        <div class="row">
            <div class="col">
                <assigner v-bind:shifts="CurrentShifts" v-bind:users="Users" v-bind:days.sync="CurrentDays"/>
            </div>
        </div>
    </div>
</template>
<script>
    import axios from 'axios'
    import assigner from './Assigner.vue'
    //import moment from 'moment'

    //Api
    import API_Const from '../API'

    export default {
        name: 'AssignerView',
        template: '#assignerview',
        components: {
            'assigner' : assigner
        },
        created: async function () {
            //Fetch VM
            try {
                //Get VM
                var response = await axios.get(API_Const.AssignerVmAPI);
                var vm = response.data;
                //Set POSs
                this.POSs = vm.POSs;
                //Set users
                this.Users = vm.Users;
            } catch (e) {
                //Show some sort of alert
                console.log(e);
            }
        },
        data: function() {
            return {
                POSs: [],
                Users: [],

                //selected identifier
                SelectedPos: null,
                SelectedPrevSchedule: null,

                ReadOnly: false,
                //Current displaying
                CurrentPOS: null,
                CurrentPrevSchedules: null,
                CurrentShifts: [],
                CurrentDays: []
            };
        },
        computed: {
            CanExecuteLoadSchedule: function () {
                if (this.SelectedPrevSchedule)
                    return true;
                return false;
            }
        },
        methods: {
            SelectedPosChanged: function () {
                console.log('pos changed');
                //Unset selected schedule
                this.SelectedPrevSchedule = null;
                //Get POS object
                this.CurrentPOS = this.POSs.filter(p => p.PosId == this.SelectedPos)[0];
                //console.log(this.CurrentPOS);
                //Set displaying schedules
                this.CurrentPrevSchedules = this.CurrentPOS.PreviousMonthSchedules;
                //Set displaying shifts
                this.CurrentShifts = this.CurrentPOS.Shifts;
                if (this.CurrentPOS.HasCurrentMonthSchedule) {
                    //Readony mode
                    this.ReadOnly = true;
                }
                else {
                    //Create new mode
                    this.ReadOnly = false;
                }
            },

            LoadPrevScheduleClicked: function () {
                //Get schedule container
                var scheduleContainer = this.CurrentPrevSchedules.find(s => s.MonthYear == this.SelectedPrevSchedule);
                //Transform to displaying days
                //Create empty array of each days
                var arr = Array(scheduleContainer.TotalDaysOfMonth).fill(1).map((x, y) => x + y);
                var days = arr.map(d => {
                    return {
                        Day: d,
                        Shifts: this.CurrentShifts.map(s => {
                            return {
                                Name: s.Name,
                                ShiftId: s.ShiftId,
                                Assign: {label: null, value: null}
                            }})
                    }
                });
                //Map saved schedule to empty days
                days.forEach(day => {
                    var shifts = scheduleContainer.Schedules.filter(shift => shift.Day == day.Day);
                    shifts.forEach(s => {
                        var shift = day.Shifts.find(shift => shift.ShiftId == s.ShiftId);
                        //Find user
                        var user = this.Users.find(u => u.UserId == s.UserId);
                        if (user) {
                            shift.Assign.value = user.UserId;
                            shift.Assign.label = user.DisplayName;
                        }
                    });
                });
                //console.log(days);

                //Display
                this.CurrentDays = days;
            }
        }
    }
</script>
<style scoped>

</style>