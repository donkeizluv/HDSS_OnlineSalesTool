<template id="loginTemplate">
    <div class="row h-100 login-top-margin">
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
                                    <input v-model="Username" type="text" class="form-control" placeholder="Tên đăng nhập" aria-describedby="basic-addon1">
                                </div>
                                <!--Pwd-->
                                <div class="input-group mb-3">
                                    <div class="input-group-prepend">
                                        <div class="input-group-text">
                                            <i class="fas fa-key" aria-hidden="true" id="basic-addon2" />
                                        </div>
                                    </div>
                                    <input v-model="Pwd" type="password" placeholder="Mật khẩu" v-on:keyup.enter="Login" class="form-control" aria-describedby="basic-addon2">
                                </div>
                                <p id="status" class="text-center text-danger" style="height: 15px;">{{Status}}</p>
                                <button v-bind:disabled="!CanSubmit" v-on:click="Login" class="btn btn-primary btn-block">
                                    <span>Login</span>
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
    import { LOGIN } from '../actions'
    export default {
        name: 'login',
        template: '#loginTemplate',
        computed: {
            CanSubmit: function () {
                if (this.Username && this.Pwd)
                    return true;
                return false;
            }
        },
        data: function () {
            return {
                Username: '',
                Pwd: '',
                Status: '',
            }
        },
        methods: {
            Login: async function () {
                if (!this.CanSubmit) return;
                try {
                    await this.$store.dispatch(LOGIN, { username: this.Username, pwd: this.Pwd });
                } catch (e) {
                    //console.log(e);
                    this.Status = 'Đăng nhập thất bại';
                }
            }
        }
    }
</script>
<style scoped>
    .login-top-margin {
        margin-top: 12rem !important;
    }
    .no-margin{
        margin: 0;
    }
</style>