<template id="loginTemplate">
    <div class="row login-top-margin">
        <div class="col-lg-10 mx-auto">
            <div class="row">
                <div class="col-md-4 mx-auto">
                    <div class="card">
                        <div class="card-header">
                            <div class="text-center">
                                <h5 class="no-margin">Đăng nhập</h5>
                            </div>
                        </div>
                        <div class="card-body">
                            <div class="form-group">
                                <!--Username-->
                                <div class="input-group mb-3">
                                    <div class="input-group-prepend">
                                        <div class="input-group-text">
                                            <i class="fas fa-user" aria-hidden="true" id="basic-addon1" />
                                        </div>
                                    </div>
                                    <input :disabled="loading"
                                           v-model="username"
                                           type="text"
                                           @keyup.enter="login" 
                                           class="form-control"
                                           placeholder="Tên đăng nhập"
                                           aria-describedby="basic-addon1">
                                </div>
                                <!--Pwd-->
                                <div class="input-group mb-3">
                                    <div class="input-group-prepend">
                                        <div class="input-group-text">
                                            <i class="fas fa-key" aria-hidden="true" id="basic-addon2" />
                                        </div>
                                    </div>
                                    <input :disabled="loading"
                                           v-model="pwd" 
                                           type="password" 
                                           placeholder="Mật khẩu" 
                                           @keyup.enter="login" 
                                           class="form-control" 
                                           aria-describedby="basic-addon2">
                                </div>
                                <p id="status" class="text-center text-danger" style="height: 15px;">{{status}}</p>
                                <button :disabled="!canSubmit || loading" @click="login" class="btn btn-primary btn-block">
                                    <i v-if="loading" class="fas fa-spinner fa-pulse"></i>
                                    <span v-else>Login</span>
                                </button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>
<script>
    import { LOGIN } from '../../actions'
    export default {
        name: 'login',
        template: '#loginTemplate',
        computed: {
            canSubmit () {
                if (this.username && this.pwd)
                    return true;
                return false;
            },
            loading () {
                return this.$store.getters.isloading;
            }
        },
        data () {
            return {
                username: '',
                pwd: '',
                status: '',
            }
        },
        methods: {
            login: async function () {
                if (!this.canSubmit) return;
                this.status = ''; //Clear status
                try {
                    await this.$store.dispatch(LOGIN, { username: this.username, pwd: this.pwd });
                    this.$router.push('/');
                } catch (e) {
                    //console.log(e);
                    this.status = 'Đăng nhập thất bại';
                }
            }
        }
    }
</script>
<style scoped>
    .login-top-margin {
        margin-top: 9rem !important;
    }
    .no-margin{
        margin: 0;
    }
</style>