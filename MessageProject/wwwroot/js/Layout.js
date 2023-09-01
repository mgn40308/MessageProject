const layout = Vue.createApp({
    el: "#navbarContent",
    data() {
        return {
            isAdmin: false,
            isAuthorize: false
        }
    },
    created() {
        this.CheckIsAuthorize();
        this.CheckIsAdmin();
    },
    methods: {
        CheckIsAuthorize() {
            var self = this
            $.ajax({
                url: "/Member/CheckIsAuthorize",
                type: "GET",
                contentType: "application/json",
                success: function (data) {
                    self.isAuthorize = data;
                },
                error: function (error) {
                    console.error("Error:", error);
                }
            });
        },
        CheckIsAdmin() {
            var self = this
            $.ajax({
                url: "/Member/CheckIsAdmin",
                type: "GET",
                contentType: "application/json",
                success: function (data) {
                    self.isAdmin = data;
                },
                error: function (error) {
                    console.error("Error:", error);
                }
            });
        },

    },
});
layout.mount("#navbarContent");




