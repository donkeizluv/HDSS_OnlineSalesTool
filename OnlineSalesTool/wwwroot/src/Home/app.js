import Vue from 'vue'

import VModal from 'vue-js-modal'
import Toasted from 'vue-toasted'
import vSelect from 'vue-select'
import appConst from './AppConst'
//import mixin from '../Home/mixin'
import store from './store'
import router from './router'
import App from './Component/AppRoot.vue'


//Extend & reg
Vue.component('v-select', vSelect)
Vue.use(VModal, { dialog: true });
Vue.use(Toasted,
    {
        duration: 3333,
        position: 'top-center',
        theme: 'primary',
        iconPack: 'fontawesome'
    });
//Registers globally
//Vue.mixin(mixin);
//init
new Vue({
    //mixins: [mixin],
    store: store,
    router: router,
    el: '#app',
    render: h => h(App)
});
