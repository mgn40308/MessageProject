using MessageProject.Models;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Identity;

namespace MessageProject.Hubs
{
    public class ChatHub:Hub
    {
        // 用戶連線 ID 列表
        public static Dictionary<string, string> userConnectionMap = new Dictionary<string, string>();
        // 用戶連線 UserName 列表
        public static List<string> ConnUserNameList = new List<string>();


        /// <summary>
        /// 連線事件
        /// </summary>
        /// <returns></returns>
        public override async Task OnConnectedAsync()
        {

            if (ConnUserNameList.Where(p => p == Context.User.Identity.Name).FirstOrDefault() == null)
            {
                userConnectionMap[Context.User.Identity.Name] = Context.ConnectionId;
                ConnUserNameList.Add(Context.User.Identity.Name);
            }
            // 更新連線 ID 列表
            string jsonString = JsonConvert.SerializeObject(ConnUserNameList);
            await Clients.All.SendAsync("UpdList", jsonString);

            // 更新個人 ID
            await Clients.Client(Context.ConnectionId).SendAsync("UpdSelfID", Context.User.Identity.Name);

            // 更新聊天內容
            await Clients.All.SendAsync("UpdContent", $"{Context.User.Identity.Name} 已加入聊天室");

            await base.OnConnectedAsync();
        }

        /// <summary>
        /// 離線事件
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public override async Task OnDisconnectedAsync(Exception ex)
        {
            string id = ConnUserNameList.Where(p => p == Context.User.Identity.Name).FirstOrDefault();
            if (id != null)
            {
                ConnUserNameList.Remove(id);
            }
            // 更新連線 ID 列表
            string jsonString = JsonConvert.SerializeObject(ConnUserNameList);
            await Clients.All.SendAsync("UpdList", jsonString);

            // 更新聊天內容
            await Clients.All.SendAsync("UpdContent",  $"{Context.User.Identity.Name} 已離開聊天室");

            await base.OnDisconnectedAsync(ex);
        }

        /// <summary>
        /// 傳遞訊息
        /// </summary>
        /// <param name="user"></param>
        /// <param name="message"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task SendMessage(string selfID, string message, string sendToID)
        {

            string senderConnectionId = Context.ConnectionId;

            if (string.IsNullOrEmpty(sendToID))
            {
                await Clients.All.SendAsync("UpdContent", $"{selfID} 說:{message}");
            }
            else
            {
                if (userConnectionMap.TryGetValue(sendToID, out string receiverConnectionId))
                {
                    // 发送消息给接收人
                    await Clients.Client(receiverConnectionId).SendAsync("UpdContent",$"{selfID} 私訊向你說:{message}");

                    // 发送消息给发送人
                    await Clients.Client(senderConnectionId).SendAsync("UpdContent", $"你向 {sendToID} 私訊說:{message}");
                }
                else
                {
                    // 接收人未连接
                    await Clients.Client(senderConnectionId).SendAsync("UpdContent", "使用者不在線上");
                }
            }
        }
    }
}
