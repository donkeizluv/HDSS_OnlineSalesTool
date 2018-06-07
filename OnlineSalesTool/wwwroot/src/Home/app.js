import Vue from 'vue'
// import VModal from 'vue-js-modal'

import store from './store'
import router from './router'
import App from './Component/AppRoot.vue'
import vBPopover from 'bootstrap-vue/es/directives/popover/popover';
import Toasted from 'vue-toasted'

//Directives
Vue.directive('b-popover', vBPopover);
Vue.use(Toasted,
    {
        duration: 3333,
        position: 'top-center',
        theme: 'primary',
        iconPack: 'fontawesome'
    });
new Vue({
    store: store,
    router: router,
    el: '#app',
    render: h => h(App)
});
