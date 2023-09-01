const app = Vue.createApp({
    el: "#app",
    data() {
        return {
            userName: "",
            passwordValue: "",
            checkPasswordValue: "",
            passwordMatch: false,
            accountRepeat: false
        }
    },
    computed: {
        isPasswordValid() {
            // 驗證密碼長度和包含大小寫英文字母
            const minLength = 6;
            const hasLowerCase = /[a-z]/.test(this.passwordValue);
            const hasUpperCase = /[A-Z]/.test(this.passwordValue);
            return this.passwordValue.length >= minLength && hasLowerCase && hasUpperCase;
        },
        CheckForm() {
            const self = this;
            let tipString = "";
            if (!this.isPasswordValid) {
                tipString += "密碼長度至少六位，並包含大小寫英文。\n";
            }
            if (this.passwordMatch) {
                tipString += "請確認兩次密碼是否都正確輸入。\n";
            }
            if (tipString.length > 0) {
                alert(tipString);

            } else {
                self.PostAccount();
                return true;
            }
        }
    },
    methods: {
        CheckPasswordMatch() {
            this.passwordMatch = this.passwordValue != this.checkPasswordValue;
        },
        PostAccount() {
            const userName = $("#userName").val();
            const password = $("#password").val();

            $.ajax({
                url: "/Member/GetRegisterAccount",
                type: "POST",
                contentType: "application/json",
                async: false,
                data: JSON.stringify({
                    UserName: userName,
                    Password: password
                }),
                success: function (data) {
                    location.href = "/Member/Login"
                },
                error: function (error) {
                    console.error("Error:", error);
                }
            });
        },
        CheckAccount() {
            const userName = this.userName;
            const self = this;
            $.ajax({
                url: "/Member/CheckUserExists/" + userName,
                type: "POST",
                contentType: "application/json",
                success: function (data) {
                    self.accountRepeat = data;
                },
                error: function (error) {
                    console.error("Error:", error);
                }
            });
        }

    }
});
app.mount("#app");