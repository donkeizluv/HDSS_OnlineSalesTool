import Vue from 'vue'
// import VModal from 'vue-js-modal'
import Toasted from 'vue-toasted'
import store from './store'
import router from './router'
import App from './Component/AppRoot.vue'
import vBPopover from 'bootstrap-vue/es/directives/popover/popover';

//Directives
Vue.directive('b-popover', vBPopover);
//Extend & reg
// Vue.use(VModal, { dialog: true });
Vue.use(Toasted,
    {
        duration: 3333,
        position: 'top-center',
        theme: 'primary',
        iconPack: 'fontawesome'
    });
//init
new Vue({
    //mixins: [mixin],
    store: store,
    router: router,
    el: '#app',
    render: h => h(App)
});
