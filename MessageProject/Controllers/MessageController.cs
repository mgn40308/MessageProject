using MessageProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySqlX.XDevAPI.Common;
using System.Linq;

namespace MessageProject.Controllers
{
    [Authorize(Policy = "RequireLoggedIn")]
    public class MessageController : Controller
    {
        private readonly AppDbContext _context;

        private readonly ApplicationDbContext _user;
        public MessageController(AppDbContext context, ApplicationDbContext user)
        {
            _context = context;
            _user = user;
        }

        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 留言板清單頁面
        /// </summary>
        /// <returns></returns>
        public IActionResult List()
        {
            return View();
        }

        /// <summary>
        /// 留言內容及回覆頁面
        /// </summary>
        /// <param name="id">留言的ID序號</param>
        /// <returns></returns>
        public IActionResult Detail(int id)
        {
            var messageWithReplies = _context.Messages
            .Include(m => m.Replys) // 加載關聯的 Replies 資料
            .FirstOrDefault(m => m.Id == id);
            var message = _context.Messages.FirstOrDefault(m => m.Id == id);
            ViewBag.Message = messageWithReplies;
            return View();
        }

        /// <summary>
        /// 新增留言頁面
        /// </summary>
        /// <returns></returns>
        public IActionResult AddMessage()
        {
            return View();
        }

        /// <summary>
        /// 聊天室頁面
        /// </summary>
        /// <returns></returns>
        public IActionResult Chat()
        {
            return View();
        }

        /// <summary>
        /// 新增留言
        /// </summary>
        /// <param name="form">前端form表單傳回應有</param>
        /// title  留言標題
        /// content 留言內容
        /// <returns></returns>
        public IActionResult CreateMessage(IFormCollection form)
        {
            string title = form["title"];
            string content = form["content"];
            string userName = User.Identity.Name;
            DateTime date = DateTime.Now;
            var newMessage = new Message
            {
                Title = title,
                Content = content,
                UserName = userName,
                Date = date
            };
            _context.Messages.Add(newMessage);
            int result = _context.SaveChanges();

            if (result>0)
            {
                return RedirectToAction("List", "Message");
            }
            else
            {
                return RedirectToAction("AddMessage", "Message");
            }          
        }

        /// <summary>
        /// 根據頁碼取得對應的留言資料
        /// </summary>
        /// <param name="id">頁碼</param>
        /// <returns></returns>
        public IActionResult GetMessageList(int id=1)
        {
            int pageSize = 10; // 每页显示的记录数
            int pageIndex = id-1;
            var messages = _context.Messages.OrderByDescending(m=>m.Id).Include(m => m.Replys).Skip(pageIndex * pageSize).Take(pageSize);
            return Json(messages.Select(x => new { 
                Id =x.Id, 
                Title =x.Title,
                Content=x.Content, 
                UserName =x.UserName,
                Date = x.Date.ToString("yyyy/MM/dd HH:mm:ss"),
                Replys=x.Replys,
            }).ToList());
        }

        /// <summary>
        /// 取得留言板頁碼
        /// </summary>
        /// <returns></returns>
        public IActionResult GetMessagePageCount()
        {
            int messages = _context.Messages.Count();
            int pageCount = (int)Math.Ceiling((double)messages/10);
            return Json(pageCount);
        }

        /// <summary>
        /// 新增留言回應
        /// </summary>
        /// <param name="reply">回應內容</param>
        /// MessageId 留言的ID
        /// <returns></returns>
        public IActionResult CreateReplyMessage([FromBody]Reply reply)
        {

                var newReply = new Reply
                {
                    MessageId = reply.MessageId,
                    UserName = User.Identity.Name,
                    ReplyContent = reply.ReplyContent,
                    Date = DateTime.Now
                };

            _context.Replys.Add(newReply);
            _context.SaveChanges();
            

            return RedirectToAction("List");
        }

        /// <summary>
        /// 確認使用者是否有權限修改刪除
        /// </summary>
        /// <returns></returns>
        public IActionResult GetUserPerssion()
        {
            if (User.IsInRole("Admin"))
            {
                return Json("all");
            }
            else
            {
                return Json(User.Identity.Name);
            }
          
        }

        /// <summary>
        /// 刪除留言以及相關回覆
        /// </summary>
        /// <param name="id">留言訊息的ID序號</param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult DeleteMessage(int id)
        {
            var messageWithReplies = _context.Messages
             .Include(m => m.Replys) // 加載關聯的 Replies 資料
             .FirstOrDefault(m => m.Id == id);
            if (messageWithReplies.UserName == User.Identity.Name|| User.IsInRole("Admin"))
            {
                if (messageWithReplies != null)
                {
                    _context.Messages.Remove(messageWithReplies);
                    _context.SaveChanges();
                    return Ok("Message and related replies deleted successfully.");
                }
                else
                {
                    return NotFound("Message not found.");
                }
            }
            return NotFound("Message not found.");

        }

        /// <summary>
        /// 編輯回覆
        /// </summary>
        /// <param name="reply"> 回覆內容</param>
        /// Id 回應訊息的ID序號
        /// <returns></returns>
        [HttpPost]
        public IActionResult UpdateReply([FromBody] Reply reply)
        {
            var replyToUpdate = _context.Replys.FirstOrDefault(r => r.Id == reply.Id);
            if (replyToUpdate != null)
            {
                replyToUpdate.ReplyContent = reply.ReplyContent;
                _context.SaveChanges();
                return Ok("Reply content updated successfully.");
            }
            else
            {
                return NotFound("Reply not found.");
            }
        }

        /// <summary>
        /// 刪除回覆
        /// </summary>
        /// <param name="id">回覆訊息的ID序號</param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult DeleteReply(int id)
        {
            var replyToUpdate = _context.Replys.FirstOrDefault(r => r.Id == id);
            if (replyToUpdate != null)
            {
                _context.Remove(replyToUpdate);
                _context.SaveChanges();
                return Ok("Reply content updated successfully.");
            }
            else
            {
                return NotFound("Reply not found.");
            }
        }

        /// <summary>
        /// 編輯留言
        /// </summary>
        /// <param name="message">留言內容</param>
        /// Id 訊息ID序號
        /// Content 修改過的留言內容
        /// <returns></returns>
        [HttpPost]
        public IActionResult UpdateMessage([FromBody] Message message)
        {
            var replyToUpdate = _context.Messages.FirstOrDefault(r => r.Id == message.Id);
            if (replyToUpdate != null)
            {
                replyToUpdate.Content = message.Content;
                _context.SaveChanges();
                return Ok("Reply content updated successfully.");
            }
            else
            {
                return NotFound("Reply not found.");
            }
        }
    }
}
