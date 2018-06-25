import Vue from 'vue'
import store from './store'
import router from './router'
import App from './View/AppRoot.vue'
import Toasted from 'vue-toasted'
import BootstrapVue from 'bootstrap-vue'
//CSS
import 'bootstrap/dist/css/bootstrap.css'
import 'bootstrap-vue/dist/bootstrap-vue.css'
import '../../css/site.css'
Vue.use(BootstrapVue);
Vue.use(Toasted ,{
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
