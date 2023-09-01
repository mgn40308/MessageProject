const app = Vue.createApp({
    el: "#app",
    data() {
        return {
            userName: "",
            newPassword: "",
            checkNewPassword: "",
            passwordMatch: false,
        }
    },
    computed: {
        isPasswordValid() {
            // 驗證密碼長度和包含大小寫英文字母
            const minLength = 6;
            const hasLowerCase = /[a-z]/.test(this.newPassword);
            const hasUpperCase = /[A-Z]/.test(this.newPassword);
            return this.newPassword.length >= minLength && hasLowerCase && hasUpperCase;
        },

    },
    methods: {
        CheckPasswordMatch() {
            this.passwordMatch = this.newPassword != this.checkNewPassword;
        },

        CheckForm() {
            let tipString = "";
            const self = this;
            if (!this.isPasswordValid) {
                tipString += "密碼長度至少六位，並包含大小寫英文。\n";
            }
            if (this.passwordMatch) {
                tipString += "請確認兩次密碼是否都正確輸入。\n";
            }
            if (tipString.length > 0) {
                alert(tipString);
            } else {
                this.PostAccount();
            }
        },
        PostAccount() {
            const currentPassword = $("#currentPassword").val();
            const newPassword = $("#newPassword").val();

            $.ajax({
                url: "/Member/UpdatePassword",
                type: "POST",
                contentType: "application/json",
                data: JSON.stringify({
                    CurrentPassword: currentPassword,
                    NewPassword: newPassword
                }),
                success: function (data) {
                    location.href = "/Message/List"
                },
                error: function (error) {
                    console.error("Error:", error);
                }
            });
        },
    },
});
app.mount("#app");