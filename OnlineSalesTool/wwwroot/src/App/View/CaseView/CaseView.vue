<template>
    <div>
        <!--Search bar-->
        <div class="row">
            <div class="col-sm-8 mx-auto">
                <search-bar v-bind:disabled="loading"
                            v-bind:items="searchProps"
                            v-on:submit="submitSearch"></search-bar>
            </div>
        </div>
        <!--Listing-->

    </div>
</template>
<script>
    import API from "../../API";
    //Permission
    import { Permission } from "../../AppConst";
    import searchBar from '../Shared/SearchBar.vue'
    import pagenav from "vuejs-paginate";
    import axios from "axios";
    import listingMix from "../Shared/listingViewMixins";

    export default {
        name: "CaseView",
        mixins: [listingMix],
        components: {
            "search-bar": searchBar
        },
        
        computed: {

        },
        data() {
            return {
                VM: null,
                searchProps: [
                    {
                        name: 'dummy',
                        value: 1
                    },
                    {
                        name: 'dummy2',
                        value: 2
                    }
                ],
                loading: false
            }
        },

        methods: {
            init: function () {
                //Load VM, check auth and stuff....
                //console.log('init case view');
            },
            submitSearch: function(model) {
                console.log(that.searchProps);
            }
        }
    }
</script>
<style scoped>
    
</style>