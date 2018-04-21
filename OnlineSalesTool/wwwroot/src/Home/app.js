import Vue from 'vue'

import VModal from 'vue-js-modal'
import Toasted from 'vue-toasted'
//import VueProgressBar from 'vue-progressbar'

import appConst from './AppConst'
//import mixin from '../Home/mixin'
import store from './store'
import router from './router'
import App from './Component/AppRoot.vue'


//Extend & reg
Vue.use(VModal, { dialog: true });
Vue.use(Toasted,
    {
        duration: 3333,
        position: 'top-center',
        theme: 'primary',
        iconPack: 'fontawesome'
    });
//Vue.use(VueProgressBar, {
//    color: 'rgb(143, 255, 199)',
//    failedColor: 'red',
//    height: '2px'
//})
//Registers globally
//Vue.mixin(mixin);
//Init
new Vue({
    //mixins: [mixin],
    store: store,
    router: router,
    el: '#app',
    render: h => h(App)
});
