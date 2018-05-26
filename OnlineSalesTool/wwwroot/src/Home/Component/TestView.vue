<template id="test">
    <div>
        <div class="row">
            <div class="col-lg-12 mx-auto">
                <div class="row">
                    <div class="col-6 mx-auto">
                        <d-select v-model="selected"
                                  v-bind:api="api"></d-select>
                    </div>
                </div>
                
            </div>
        </div>
    </div>
</template>
<script>
    import DynamicSelect from './DynamicSelect.vue'
    import API from '../API'

    export default {
        name: 'test-view',
        template: '#test',
        components: {
            'd-select': DynamicSelect
        },
        data: function () {
            return {
                selected: null,
                api: API.UserSearchSuggest.replace('{role}', 'BDS')
            }
        },
        methods: {
           

        }

    }
</script>
<style scoped>
    .top-margin {
        margin-top: 1rem
    }
</style>