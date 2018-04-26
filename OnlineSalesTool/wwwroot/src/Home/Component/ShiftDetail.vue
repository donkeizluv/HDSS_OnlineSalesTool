<!--Shift detail cell-->
<template id="shiftdetail">
    <div class="card custom-border-color card-width">
        <div class="card-header">Ngày {{day.Day}}</div>
        <div class="card-body">
            <div v-for="shift in day.Shifts" v-bind:key="shift.ShiftId">
                <div>
                    <span class="text-secondary">{{shift.Name}}</span>
                    <v-select v-bind:options="UsersLeft"
                              v-model="shift.Assign"></v-select>
                </div>
            </div>
        </div>
    </div>
</template>
<script>
    export default {
        name: 'shiftdetail',
        template: '#shiftdetail',
        props: {
            //All shifts available
            //shifts: {
            //    type: Array
            //},
            //Users to assign to shifts
            users: {
                type: Array
            },
            day: {
                type: Object
            }
        },
        data: function () {
            return {
                //dayData
            };
        },
        computed: {
            //Return list of users left to assign
            UsersLeft: function () {
                //User left to assign
                var assigned = this.day.Shifts.map(a => {
                    if (a.Assign)
                        return a.Assign.value;
                    return -1;
                });
                console.log(assigned);
                var left = this.users.filter(u => !assigned.includes(u.UserId));
                return left.map(u => { return { label: u.DisplayName, value: u.UserId }; });
            }
        },
        watch: {
            //'day.assign': function (arr) {
            //    console.log(arr);
            //    //console.log(oldArr);
            //}
        },
        methods: {
            //Emit model change
            //SelectChanged: function (val) {
            //    //val is user Id
            //    console.log('select changed: ' + val);
            //    //Find if already assigned
            //    var index = this.assignDetail.indexOf(val);
            //    if (index > -1) {
            //        this.assignDetail[index] = val;
            //        return;
            //    }
            //    //Push to detail
            //    this.assignDetail.push(val);
            //}
        }
    }
</script>
<style scoped>
    .card-width {
        width: 18rem
    }
    .custom-border-color {
        border-color: rgb(232, 232, 232)!important
    }
</style>