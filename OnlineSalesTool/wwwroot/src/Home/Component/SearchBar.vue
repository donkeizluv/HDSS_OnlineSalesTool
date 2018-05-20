<template id="search-bar">
    <div class="form-inline justify-content-center">
        <div class="form-group">
            <select v-model="model.filter" class="form-control custom-select" v-bind:disabled="disabled">
                <option v-for="pair in items" v-bind:key="pair.value" v-bind:value="pair.value">{{pair.name}}</option>
            </select>
            <div class="input-group">
                <input v-on:keyup.enter="submitSearch"
                       v-model="model.text"
                       class="form-control"
                       placeholder="Từ khóa..."
                       v-bind:disabled="disabled"
                       type="search">
                <span class="input-group-btn">
                    <button class="btn btn-link"
                            type="button"
                            v-on:click="submitSearch">
                        <i class="fa fa-search"></i>
                    </button>
                </span>
            </div>
        </div>
    </div>
</template>
<script>
    export default {
        name: 'search-bar',
        template: '#search-bar',

        props: {
            'disabled': {
                type: Boolean,
                default: false
            },
            'items': { //value, name
                type: Array,
                required: true
            }
            //'search': {
            //    type: Object,
            //    required: true
            //}
        },
        created: function () {
            //Default value
            if(this.items)
                this.model.filter = this.items[0].value;
        },
        data: function () {
            return {
                model: {
                    filter: '',
                    text: ''
                }
            };
        },
        methods: {
            submitSearch: function () {
                if (this.disabled) return;
                this.$emit('submit', this.model);
            }
        }
    }
</script>
<style scoped>
    .no-right-pad {
        padding-right: 0;
    }

    .no-padding {
        padding: 0;
    }
</style>