//Helper methods go here
export default {
    methods: {
        noSpace(evt) {
            evt = evt ? evt : window.event;
            var charCode = evt.which ? evt.which : evt.keyCode;
            if (charCode == 32) {
                evt.preventDefault();
            } else {
                return true;
            }
        },
        thousandSep(x) {
            if(!x) return x;
            return x.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
        }
    }
};
