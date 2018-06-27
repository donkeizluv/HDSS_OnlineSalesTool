<template id="dynamicSelect">
    <div>
        <v-select :disabled="disabled"
                  :options="options"
                  @search="onSearch"
                  :value="value"
                  v-on="inputListeners"
                  :searchable="allowSearch"
                  :filterable="filterable"
                  :label="label">
        </v-select>
    </div>
</template>
<script>
    import axios from 'axios'
    import vSelect from 'vue-select'
    import debounce from 'lodash.debounce'

    export default {
        name: 'dynamicSelect',
        template: '#dynamicSelect',
        components: {
            'v-select': vSelect
        },
        props: {
            'disabled': {
                type: Boolean,
                default: false
            },
            'api': {
                type: String,
                required: true
            },
            'filterable': {
                type: Boolean,
                default: true
            },
            'clearonblur': {
                type: Boolean,
                default: true
            },
            'label': {
                type: String,
                default: 'label'
            },
            value: null
        },
        computed: {
            inputListeners () {
                var vm = this
                // `Object.assign` merges objects together to form a new object
                return Object.assign({},
                    // We add all the listeners from the parent
                    this.$listeners,
                    // Then we can add custom listeners or override the
                    // behavior of some listeners.
                    {
                        // This ensures that the component works with v-model
                        input (event) {
                            //console.log(event);
                            vm.$emit('input', event);
                        }
                    }
                )
            }
        },
        data () {
            return {
                options: [],
                allowSearch: true,
            };
        },
        
        methods: {
            onSearch (search, loading) {
                loading(true);
                this.search(loading, search, this);
            },
            search: debounce(async (loading, search, vm) =>  {
                let { data } = await axios.get(`${vm.api}${escape(search)}`)
                vm.options = data;
                loading(false);
            }, 235),
           
        }
    }
</script>