using MessageProject.Models;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Identity;
using System.Collections.Concurrent;

namespace MessageProject.Hubs
{
    public class ChatHub:Hub
    {
        // 用戶連線 ID 列表
        public static ConcurrentDictionary<string, string> UserConnectionMap = new ConcurrentDictionary<string, string>();

        /// <summary>
        /// 連線事件
        /// </summary>
        /// <returns></returns>
        public override async Task OnConnectedAsync()
        {
            string? name= Context.User?.Identity?.Name;
            if (name != null)
            {
                if (!UserConnectionMap.ContainsKey(name))
                {
                    UserConnectionMap[name] = Context.ConnectionId;
                }
            }

            // 更新連線 ID 列表
            string jsonString = JsonConvert.SerializeObject(UserConnectionMap.Keys.ToList());
            await Clients.All.SendAsync("UpdList", jsonString);

            // 更新個人 ID
            await Clients.Client(Context.ConnectionId).SendAsync("UpdSelfID", Context.User?.Identity?.Name);

            // 更新聊天內容
            await Clients.All.SendAsync("UpdContent", $"{Context.User.Identity?.Name} 已加入聊天室");

            await base.OnConnectedAsync();
        }

        /// <summary>
        /// 離線事件
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public override async Task OnDisconnectedAsync(Exception? ex)
        {
            string? name = Context.User?.Identity?.Name;
            if (name != null)
            {
                if (UserConnectionMap.ContainsKey(name))
                {
                    string userName = name;
                    UserConnectionMap.TryRemove(userName, out _);
                }
            }
                
           
            // 更新連線 ID 列表
            string jsonString = JsonConvert.SerializeObject(UserConnectionMap.Keys.ToList());
            await Clients.All.SendAsync("UpdList", jsonString);

            // 更新聊天內容
            await Clients.All.SendAsync("UpdContent",  $"{Context.User?.Identity?.Name} 已離開聊天室");

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
                if (UserConnectionMap.TryGetValue(sendToID, out string? receiverConnectionId))
                {
                    await Clients.Client(receiverConnectionId).SendAsync("UpdContent",$"{selfID} 私訊向你說:{message}");


                    await Clients.Client(senderConnectionId).SendAsync("UpdContent", $"你向 {sendToID} 私訊說:{message}");
                }
                else
                {
    
                    await Clients.Client(senderConnectionId).SendAsync("UpdContent", "使用者不在線上");
                }
            }
        }
    }
}
