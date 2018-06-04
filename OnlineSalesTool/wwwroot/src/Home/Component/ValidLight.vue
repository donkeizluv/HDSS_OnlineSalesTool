<template>
    <span class="self-center self-padding">
        <span v-if="state">
            <i class="fas fa-check-circle text-success"></i>
        </span>
        <span v-else>
            <i class="fas fa-exclamation-circle text-danger"></i>
        </span>
    </span>
</template>
<script>
    export default {
        name: 'ValidLight',
        props: {
            state: {
                type: Boolean,
                required: true
            }
        }
    }
</script>
<style scoped>
.self-center{
    align-self: center;
}
.self-padding{
    padding-left: 4px;
    padding-right: 4px;
}
</style>
