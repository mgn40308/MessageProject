const app = Vue.createApp({
    el: "#app",
    data() {
        return {
            messages: [],
            isAdmin: false,
            detailId: "",
            identityName: ""
        }
    },
    created() {
        
        this.GetDetailContent()
    },
    methods: {
        PostReply() {
            var id = this.messages.id;
            const content = $("#replyTextarea").val();
            $.ajax({
                url: "/Message/CreateReplyMessage",
                type: "POST",
                contentType: "application/json",
                data: JSON.stringify({
                    "MessageId": id,
                    "ReplyContent": content,
                }),
                success: function (data) {
                    window.location.reload();
                },
                error: function (error) {
                    console.error("Error:", error);
                }
            });
        },
        EditContent(id) {
            $("#textarea_" + id).prop("readonly", false);
            $("#replyEdit_" + id).addClass("d-none");
            $("#replySubmit_" + id).removeClass("d-none");
        },
        EditSubmit(id) {
            $("#textarea_" + id).prop("readonly", true);
            $("#replyEdit_" + id).removeClass("d-none");
            $("#replySubmit_" + id).addClass("d-none");
            const content = $("#textarea_" + id).val();
            $.ajax({
                url: "/Message/UpdateReply",
                type: "POST",
                contentType: "application/json",
                data: JSON.stringify({
                    "Id": id,
                    "ReplyContent": content,
                }),
                success: function (data) {
                    console.log(data);
                },
                error: function (error) {
                    console.error("Error:", error);
                }
            });
        },
        DeleteReply(id) {
            $.ajax({
                url: "/Message/DeleteReply/" + id,
                type: "POST",
                contentType: "application/json",
                success: function (data) {
                    $("#reply_" + id).remove();
                },
                error: function (error) {
                    console.error("Error:", error);
                }
            });
        },
        GetUserPerssion() {
            const self = this;
            $.ajax({
                url: "/Message/GetUserPerssion",
                type: "Get",
                contentType: "application/json",
                success: function (data) {
                    if (data == "all") {
                        $("[name^='replyButton_']").removeClass("d-none");
                        self.isAdmin = true
                    } else {
                        const userName = self.messages.userName;
                        self.isAdmin = (self.identityName === userName);
                        $("[name='replyButton_" + data + "']").removeClass("d-none");
                    }
                },
                error: function (error) {
                    console.error("Error:", error);
                }
            });
        },
        MessageEdit() {
            $("#messageTextarea").prop("readonly", false);
            $("#messageSubmit").removeClass("d-none");
            $("#messageEdit").addClass("d-none");
        },
        MessageSubmit(id) {
            const content = $("#messageTextarea").val();
            $.ajax({
                url: "/Message/UpdateMessage",
                type: "POST",
                contentType: "application/json",
                data: JSON.stringify({
                    "Id": id,
                    "Content": content,
                }),
                success: function (data) {
                    $("#messageTextarea").prop("readonly", true);
                    $("#messageSubmit").addClass("d-none");
                    $("#messageEdit").removeClass("d-none");
                },
                error: function (error) {
                    console.error("Error:", error);
                }
            });
        },
        GetDetailContent() {
            const self = this;
            $.ajax({
                url: "/Message/GetDetail/" + self.detailId,
                type: "POST",
                contentType: "application/json",
                success: function (data) {
                    self.messages = data;
                    self.GetUserPerssion();
                },
                error: function (error) {
                    console.error("Error:", error);
                }
            });
        },
        GetPathParam() {
            const path = window.location.href;
            var match = path.match(/\/([^/]+)$/);
            var parameter = match[1];
            return parameter;
        }
    },
});
app.mount("#app");