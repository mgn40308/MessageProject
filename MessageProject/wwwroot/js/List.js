//const app = Vue.createApp({
//        el: "#app",
//        data() {
//            return {
//                messages: [],
//                pageCount:"",
//                pageNumber:1,
//            }
//        },
//        created() {
//            this.GetMessageList();
//            this.GetMessagePageCount();

//        },
//        methods:{
//            GetMessageList(id) {
//                const self = this;
//                if (id !== self.pageNumber) {
//                    this.LoadPage(id);
//                }

//            },
//            LoadPage(id){
//                const self = this;
//                $.ajax({
//                    url: "/Message/GetMessageList/" + id,
//                    type: "Get",
//                    contentType: "application/json",
//                    success: function (data) {
//                        self.messages = data;
//                        $("td button").addClass("d-none")
//                        self.GetUserPerssion();
//                        if (id) {
//                            self.pageNumber = id;
//                        }
//                    },
//                    error: function (error) {
//                        console.error("Error:", error);
//                    }
//                });
//            },
//            GetUserPerssion() {
//                $.ajax({
//                    url: "/Message/GetUserPerssion",
//                    type: "Get",
//                    contentType: "application/json",
//                    success: function (data) {
//                        if (data == "all") {
//                            $("td").find("button").removeClass("d-none");
//                        } else {
//                            $("[name='" + data + "'").find("button").removeClass("d-none");
//                        }

//                    },
//                    error: function (error) {
//                        console.error("Error:", error);
//                    }
//                });
//            },
//            DeleteMessage(id) {
//                const self = this;
//                $.ajax({
//                    url: "/Message/DeleteMessage/"+id,
//                    type: "POST",
//                    contentType: "application/json",
//                    success: function (data) {
//                        self.LoadPage(self.pageNumber);
//                    },
//                    error: function (error) {
//                        console.error("Error:", error);
//                    }
//                });
//            },
//            GetMessagePageCount() {
//                const self = this;
//                    $.ajax({
//                    url: "/Message/GetMessagePageCount",
//                    type: "GET",
//                    contentType: "application/json",
//                    success: function (data) {
//                        self.pageCount = data;
//                    },
//                    error: function (error) {
//                        console.error("Error:", error);
//                    }
//                });
//            },
//            GetNextPage(page) {
//                if (page <= this.pageCount && page > 0) {
//                    this.GetMessageList(page);
//                }
//            },
//            FromtDate(date){
//                var timestamp = new Date(date);

//                var year = timestamp.getFullYear();
//                var month = timestamp.getMonth() + 1;
//                var day = timestamp.getDate();
//                var hours = timestamp.getHours();
//                var minutes = timestamp.getMinutes();

//                var formattedTimestamp = year + '/' + month + '/' + day + ' ' + hours + ':' + minutes;
//                return formattedTimestamp
//            }

//        },
//    });
//app.mount("#app");


const { createApp, ref, onMounted } = Vue;

const app = createApp({
    data() {
        return {
            messages: [],
            pageCount: "",
            pageNumber: 1,
        };
    },
    created() {
        this.GetMessageList();
        this.GetMessagePageCount();
    },
    methods: {
        GetMessageList(id) {
            if (id !== this.pageNumber) {
                this.LoadPage(id);
            }
        },
        LoadPage(id) {
            $.ajax({
                url: "/Message/GetMessageList/" + id,
                type: "Get",
                contentType: "application/json",
                success: (data) => {
                    this.messages = data;
                    $("td button").addClass("d-none");
                    this.GetUserPermission();
                    if (id) {
                        this.pageNumber = id;
                    }
                },
                error: (error) => {
                    console.error("Error:", error);
                }
            });
        },
        GetUserPermission() {
            $.ajax({
                url: "/Message/GetUserPermission",
                type: "Get",
                contentType: "application/json",
                success: (data) => {
                    if (data === "all") {
                        $("td").find("button").removeClass("d-none");
                    } else {
                        $("[name='" + data + "'").find("button").removeClass("d-none");
                    }
                },
                error: (error) => {
                    console.error("Error:", error);
                }
            });
        },
        DeleteMessage(id) {
            $.ajax({
                url: "/Message/DeleteMessage/" + id,
                type: "POST",
                contentType: "application/json",
                success: (data) => {
                    this.LoadPage(this.pageNumber);
                },
                error: (error) => {
                    console.error("Error:", error);
                }
            });
        },
        GetMessagePageCount() {
            $.ajax({
                url: "/Message/GetMessagePageCount",
                type: "GET",
                contentType: "application/json",
                success: (data) => {
                    this.pageCount = data;
                },
                error: (error) => {
                    console.error("Error:", error);
                }
            });
        },
        GetNextPage(page) {
            if (page <= this.pageCount && page > 0) {
                this.GetMessageList(page);
            }
        },
        FromtDate(date) {
            const timestamp = new Date(date);
            const year = timestamp.getFullYear();
            const month = timestamp.getMonth() + 1;
            const day = timestamp.getDate();
            const hours = timestamp.getHours();
            const minutes = timestamp.getMinutes();
            const formattedTimestamp = year + '/' + month + '/' + day + ' ' + hours + ':' + minutes;
            return formattedTimestamp;
        }
    },
});

app.mount("#app");



