//Helper methods go here
export default {
    methods: {
        noSpace: function(evt) {
            evt = (evt) ? evt : window.event;
            var charCode = (evt.which) ? evt.which : evt.keyCode;
            if (charCode == 32) {
                evt.preventDefault();;
            } else {
                return true;
            }
        }
    }
}