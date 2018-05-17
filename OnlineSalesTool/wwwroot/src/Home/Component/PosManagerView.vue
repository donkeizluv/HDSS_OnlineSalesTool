<template id="posmanager">
    <div>
        <!--Search bar-->
        <div class="row">
            <div class="col-sm-8 mx-auto">
                <!--<search-bar v-bind:disabled="isLoading"
                            v-bind:items="searchFilters"
                            v-on:submit="submitSearch"></search-bar>-->
            </div>
        </div>
        <!--Listing-->
        <div class="row">
            <div class="col-12">
                <div class="table-responsive">
                    <table class="table table-hover no-top-border">
                        <thead>
                            <tr>
                                <th>
                                    Tên POS
                                </th>
                                <th>
                                    Code
                                </th>
                                <th>
                                    Địa chỉ
                                </th>
                                <th>
                                    SĐT
                                </th>
                                <th>
                                    BDS
                                </th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr v-for="item in items" v-bind:key="item.PosId">
                                <td>
                                    <span>{{item.PosName}}</span>
                                </td>
                                <td>
                                    <span>{{item.PosCode}}</span>
                                </td>
                                <td>
                                    <span>{{item.Address}}</span>
                                </td>
                                <td>
                                    <span>{{item.Phone}}</span>
                                </td>
                                <td>
                                    <span>{{item.BDS.DisplayName}}</span>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </div>
            </div>
        </div>
    </div>
</template>
<script>
    import API from '../API'
    //Actions
    import { LOGOUT, CHECK_TOKEN_EXPIRE } from '../actions'
    //Mutation
    import { VM_POSMAN } from '../mutations'
    import axios from 'axios'

    export default {
        name: 'posManagerView',
        template: 'posmanager',

        beforeRouteEnter(to, from, next) {
            //console.log('enter pos man');
            next(async me => {
                if (!me.vm)
                    await me.init();
            })
        },
        computed: {
            //VM
            vm: function () {
                return this.$store.getters.vm_posman;
            }
        },
        data: function () {
            return {
                items: [],
                onPage: 1,
                filterBy: '',
                filterString: '',
                orderBy: '',
                orderAsc: true,
                items: [],
                totalRows: 0,
                totalPages: 0,

                searchFilters: [
                    { name: 'Tên POS', value: 'PosName' },
                    { name: 'Pos code', value: 'PosCode' },
                    { name: 'SĐT', value: 'Phone' },
                    { name: 'BDS', value: 'BDS' }
                ]
            }
        },
        methods: {
            init: async function () {
                await this.$store.dispatch(CHECK_TOKEN_EXPIRE);
                await this.loadVM();
            },
            loadVM: async function () {
                try {
                    let response = await axios.get(API.PosManagerVM);
                    console.log(response);
                    let vm = response.data;
                    //console.log(vm);
                    this.$store.commit(VM_POSMAN, vm);
                    this.items = vm.Items;
                } catch (e) {
                    this.$emit('showerror', 'Tải dữ liệu thất bại, vui lòng liên hệ IT.');
                }
            },
            submitSearch: function (model) {
                
            }
        }
    }
</script>
<style scoped>
    .no-top-border th{
        border-top: none!important;
    }
</style>