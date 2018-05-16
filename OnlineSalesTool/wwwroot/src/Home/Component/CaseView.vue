<template id="caseview">
    <div>
        <!--Search bar-->
        <div class="row top-margin">
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
    import searchBar from './SearchBar.vue'
    export default {
        template: '#caseview',
        components: {
            'search-bar': searchBar
        },

        beforeRouteEnter(to, from, next) {
            next(async me => {
                //this component
                if (!me.VM)
                    me.init();
            })
        },

        computed: {

        },
        data: () => {
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
                console.log('init case view');
            },
            submitSearch: function(model) {
                console.log(that.searchProps);
            }
        }
    }
</script>
<style scoped>
    .top-margin {
        margin-top: 15px;
    }

    .borderless {
        border: none !important;
    }
</style>