using MySql.Data.MySqlClient;
using System.Security.Cryptography;
using System.Text;

namespace MessageProject.Models
{
    public class MemberModel
    {
        public static string EncryptionPassword(string password)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(password);
            byte[] hash = SHA256.HashData(bytes);
            StringBuilder sb = new StringBuilder();
            foreach (byte b in hash)
            {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }

        //public static bool CreateAccount(string username, string password)
        //{
        //    string connString = SQLHelper.MySqlConnectString;
        //    string encrypPassword = EncryptionPassword(password);
        //    using MySqlConnection connection = new(connString);
        //    connection.Open();

        //    // 建立插入 SQL 語句
        //    string insertQuery = "INSERT INTO UserAccounts (Username, Password ,Permission) VALUES (@Username, @Password,@Permission)";

        //    // 建立 MySqlCommand 對象
        //    using MySqlCommand cmd = new(insertQuery, connection);
        //    // 添加參數
        //    cmd.Parameters.AddWithValue("@Username", username);
        //    cmd.Parameters.AddWithValue("@Password", encrypPassword);
        //    cmd.Parameters.AddWithValue("@Permission", 1);
        //    // 執行插入操作
        //    int rowsAffected = cmd.ExecuteNonQuery();
        //    return rowsAffected > 0;
        //}

    }

    
}
