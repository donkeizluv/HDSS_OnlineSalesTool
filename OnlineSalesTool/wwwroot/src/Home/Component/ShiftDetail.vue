<!--Shift detail cell-->
<template id="shiftdetail">
    <div v-bind:class="BorderColor">
        <div class="card-header">
            <div>
                <span class="float-left">Ngày {{day.Day}}</span>
                <div class="float-right">
                    <i v-show="!readonly" v-if="IsAllSet" class="fas fa-check-circle text-success"></i>
                    <i v-else class="fas fa-exclamation-circle text-danger"></i>
                </div>
            </div>
            
        </div>
        <div class="card-body">
            <div v-for="shift in day.Shifts" v-bind:key="shift.ShiftId">
                <div class="d-flex flex-column">
                    <span class="text-secondary">{{shift.Name}}</span>
                    <v-select v-bind:disabled="readonly"
                              v-bind:options="UsersLeft"
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
            users: {
                type: Array
            },
            day: {
                type: Object
            },
            readonly: {
                type: Boolean,
                default: false
            }
        },
        data: function () {
            return {
                DefaultCardClass: 'card card-width mb-3'
            }
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
                //console.log(assigned);
                var left = this.users.filter(u => !assigned.includes(u.UserId));
                return left.map(u => { return { label: u.DisplayName, value: u.UserId }; });
            },
            BorderColor: function () {
                if (this.readonly)
                    return `readonly-border-color ${this.DefaultCardClass}`;
                else {
                    if (this.IsAllSet) {
                        return `border-success  ${this.DefaultCardClass}`;
                    }
                    return `border-danger  ${this.DefaultCardClass}`;

                }
            },
            IsAllSet: function () {
                return this.day.Shifts.every(d => {
                    if (!d.Assign) return false;
                    if (!d.Assign.value) return false;
                    return true;
                });
            }
        }
    }
</script>
<style scoped>
    .editable-bg {
        background-color: #c4e4ff!important
    }
    .card-width {
        width: 18rem
    }
    .readonly-border-color {
        border-color: rgb(232, 232, 242)!important
    }
</style>