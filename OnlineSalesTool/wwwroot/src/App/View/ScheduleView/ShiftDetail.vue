﻿<!--Shift detail cell-->
<template id="shiftdetail">
    <div :class="borderColor">
        <div class="card-header">
            <div>
                <span class="float-left">Ngày {{day.Day}}</span>
                <span class="float-right">
                    <light v-show="!readonly" :state="isAllSet"/>
                </span>
            </div>
            
        </div>
        <div class="card-body">
            <div v-for="shift in day.Shifts" :key="shift.ShiftId">
                <div class="d-flex flex-column">
                    <span class="text-secondary">{{shift.Name}} {{shift.ExtName}}</span>
                    <v-select :disabled="readonly"
                              :options="usersLeft"
                              v-model="shift.Assign"></v-select>
                </div>
            </div>
        </div>
    </div>
</template>
<script>
    import vSelect from 'vue-select'
    import light from '../Shared/ValidLight.vue'
    export default {
        name: 'shiftdetail',
        template: '#shiftdetail',
        components: {
            'v-select': vSelect,
            'light': light
        },
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
        data() {
            return {
                DefaultCardClass: 'card card-width mb-3'
            }
        },
        computed: {
            //Return list of users left to assign
            usersLeft() {
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
            borderColor() {
                if (this.readonly)
                    return `readonly-border-color ${this.DefaultCardClass}`;
                else {
                    if (this.isAllSet) {
                        return `border-success  ${this.DefaultCardClass}`;
                    }
                    return `border-danger  ${this.DefaultCardClass}`;

                }
            },
            isAllSet() {
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