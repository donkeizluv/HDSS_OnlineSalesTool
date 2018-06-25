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
                       v-bind:disabled="disabled"
                       type="search">
                <!--Clear button-->
                <span v-show="showClear"
                      v-on:click="clear"
                      class="searchclear fas fa-times"/>
                <!--Submit-->
                <span class="input-group-append">
                    <button class="btn input-group-text"
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
    name: "search-bar",
    template: "#search-bar",

    props: {
        disabled: {
            type: Boolean,
            default: false
        },
        items: {
            //value, name
            type: Array,
            required: true
        },
        submitOnClear: {
            type: Boolean,
            default: true
        }
        //'search': {
        //    type: Object,
        //    required: true
        //}
    },
    created() {
        //Default value
        if (this.items) this.model.filter = this.items[0].value;
    },
    computed: {
        showClear() {
            //Cant use if(this.filter) cuz "0" will cause false
            //return this.filter != null && this.filter != undefined;
            if (this.model.text) return true;
            return false;
        }
    },
    data() {
        return {
            model: {
                filter: "",
                text: ""
            }
        };
    },
    methods: {
        submitSearch() {
            if (this.disabled) return;
            this.$emit("submit", this.model);
        },
        clear() {
            this.model.text = "";
            if (this.submitOnClear) this.$emit("submit", this.model);
        }
    }
};
</script>
<style scoped>
.no-right-pad {
    padding-right: 0;
}

.no-padding {
    padding: 0;
}
.searchclear {
    position: absolute;
    right: 3rem;
    top: 0;
    bottom: 0;
    height: 1rem;
    margin: auto;
    font-size: 1rem;
    cursor: pointer;
    color: #ccc;
}
</style>