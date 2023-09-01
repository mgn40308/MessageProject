
//app.AddData = function () {
//    app.$data.refenData = newData;
//    var newData= {
//        connection: null,
//        selfID: "",
//        message: "",
//        sendToID: "",
//        IDList: [],
//        content:[]
//    }
//    app.$data.refenData = newData;
//    //app.AddNewData(newData);
//}
//app.AddData()
//app.StartConnection = function () {
//    app.refenData.connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

//    app.refenData.connection.start().then(() => {
//        console.log("Hub 連線完成");
//    }).catch((err) => {
//        alert('連線錯誤: ' + err.toString());
//    });

//    app.refenData.connection.on("UpdList", (jsonList) => {
//        this.refenData.IDList = JSON.parse(jsonList);
//    });

//    app.refenData.connection.on("UpdSelfID", (id) => {
//        this.refenData.selfID = id;
//    });

//    app.refenData.connection.on("UpdContent", (msg) => {
//        this.refenData.content.push(msg);
//    });
//}

//app.SendMessage = function () {
//    this.refenData.connection.invoke("SendMessage", this.refenData.selfID, this.refenData.message, this.refenData.sendToID).then(() => {
//        $("#message").val("");
//        var chatContent = $("#chatContent");
//        $("#chatContent").scrollTop(chatContent[0].scrollHeight);
//    })
//        .catch((err) => {
//            alert('傳送錯誤: ' + err.toString());
//    });
//}
//app.StartConnection();
const app = Vue.createApp({
    el: "#app",
    data() {
        return {
            connection: null,
            selfID: "",
            message: "",
            sendToID: "",
            IDList: [],
            content: [],
    
        };
    },
    methods: {
        StartConnection() {
            this.connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

            this.connection.start().then(() => {
                console.log("Hub 連線完成");
            }).catch((err) => {
                alert('連線錯誤: ' + err.toString());
            });

            this.connection.on("UpdList", (jsonList) => {
                this.IDList = JSON.parse(jsonList);
            });

            this.connection.on("UpdSelfID", (id) => {
                this.selfID = id;
            });

            this.connection.on("UpdContent", (msg) => {
                this.content.push(msg);
            });
        },
        SendMessage() {
            this.connection.invoke("SendMessage", this.selfID, this.message, this.sendToID).then(() => {
                $("#message").val("");
                var chatContent = $("#chatContent");
                $("#chatContent").scrollTop(chatContent[0].scrollHeight);
            })
            .catch((err) => {
                    alert('傳送錯誤: ' + err.toString());
            });
        }
    },
    mounted() {
        this.StartConnection();
    }
});
app.mount("#app");






