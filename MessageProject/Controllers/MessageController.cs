using Dapper;
using MessageProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Data;


namespace MessageProject.Controllers
{
    [Authorize(Policy = "RequireLoggedIn")]
    public class MessageController : Controller
    {
        //private readonly AppDbContext _context;



        private readonly IDbConnection _connection;
        public MessageController(IDbConnection connection)
        {
            //_context = context;

            _connection = connection;
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
            ViewBag.Id = id;    
            return View();
        }

        public IActionResult GetDetail(int id)
        {

            var param = new DynamicParameters();
            param.Add("@Id", id);
            string query = "SELECT ID , Title , CONTENT,UserName FROM Messages WHERE id=@Id";
            var message = _connection.QueryFirstOrDefault<Message>(query, param, commandType: CommandType.Text);
            
            ICollection<Reply> reply = (ICollection<Reply>)_connection.Query<Reply>("usp_Replys_GetReply", param, commandType: CommandType.StoredProcedure);
            message.Replys = reply;

             return Json(message);           
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
            string storedProcedure = "usp_Messages_Add";
            var param=new DynamicParameters();
            param.Add("@Title", title);
            param.Add("@Content", content);
            param.Add("@UserName", userName);
            param.Add("@Date", date);

            int result= _connection.Execute(storedProcedure, param, commandType: CommandType.StoredProcedure);

            //string query = "INSERT INTO Messages (Title, Content, UserName, Date) VALUES (@Title, @Content, @UserName, @Date);";
            //int result = _connection.Execute(query, newMessage);
            //_context.Messages.Add(newMessage);
            //int result = _context.SaveChanges();

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
            int pageIndex = (id-1) * pageSize;
            //var messages = _context.Messages.OrderByDescending(m=>m.Id).Include(m => m.Replys).Skip(pageIndex * pageSize).Take(pageSize);
            string storedProcedure = "usp_Messages_GetList";
            var param = new DynamicParameters();
            param.Add("@PageIndex", pageIndex);
            param.Add("@PageSize", pageSize);

            var messages = _connection.Query(storedProcedure, param, commandType: CommandType.StoredProcedure);
           

            return Json(messages.ToList());
        }

        /// <summary>
        /// 取得留言板頁碼
        /// </summary>
        /// <returns></returns>
        public IActionResult GetMessagePageCount()
        {
            string query = "SELECT COUNT(Id) FROM Messages";
            var count = _connection.QueryFirstOrDefault<int>(query, commandType: CommandType.Text);

            //int messages = _context.Messages.Count();
            int pageCount = (int)Math.Ceiling((double)count / 10);
            return Json(pageCount);
        }

        /// <summary>d
        /// 新增留言回應
        /// </summary>
        /// <param name="reply">回應內容</param>
        /// MessageId 留言的ID
        /// <returns></returns>
        public IActionResult CreateReplyMessage([FromBody]Reply reply)
        {
            string storedProcedure = "usp_Replys_Add";
            var param= new DynamicParameters();
            param.Add("@MessageId",reply.MessageId);
            param.Add("@UserName", User.Identity.Name);
            param.Add("@ReplyContent", reply.ReplyContent);
            param.Add("@Date", DateTime.Now);
            //_context.Replys.Add(newReply);
            //int result= _context.SaveChanges();
            int result =_connection.Execute(storedProcedure, param, commandType: CommandType.StoredProcedure);
            if (result > 0)
            {
                return Ok();
            }
            else
            {
                return NotFound("Reply do not Add.");
            }

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

            var param = new DynamicParameters();
            param.Add("@Id", id);
            string query = "SELECT UserName FROM Messages WHERE Id = @id";
            var message= _connection.QueryFirstOrDefault<Message>(query,param,commandType: CommandType.Text);

            if (CheckPermission(message.UserName))
            {
                if (message != null)
                {
                    string deleteQuery = "DELETE  FROM Messages WHERE Id = @id";
                    var count=   _connection.Execute(deleteQuery, param, commandType: CommandType.Text);
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
            var queryParam = new DynamicParameters();
            queryParam.Add("@id", reply.Id);
          
            var param = new DynamicParameters();
            param.Add("@Id", reply.Id);
            param.Add("@ReplyContent", reply.ReplyContent);

            string query = "SELECT id,userName FROM Replys WHERE id= @Id";
            var result=_connection.QueryFirstOrDefault<Reply>(query, queryParam, commandType: CommandType.Text);
            
            //var replyToUpdate = _context.Replys.FirstOrDefault(r => r.Id == reply.Id);
            if(CheckPermission(result.UserName)) 
            {
                if (result!= null)
                {
                    
                    _connection.Execute("usp_Replys_UpdateReplyContent", param, commandType: CommandType.StoredProcedure);
                    return Ok("Reply content updated successfully.");
                }
                else
                {
                    return NotFound("Reply not found.");
                }
            }
            return NotFound("No Premission.");

        }

        /// <summary>
        /// 刪除回覆
        /// </summary>
        /// <param name="id">回覆訊息的ID序號</param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult DeleteReply(int id)
        {

            var param =new DynamicParameters();
            param.Add("@p_Id",id);
            string query = "SELECT UserName FROM Replys Where Id=@p_Id";
            var reply=  _connection.QueryFirstOrDefault<Reply>(query,param, commandType: CommandType.Text);
            if (CheckPermission(reply.UserName))
            {
                if (reply != null)
                {
                    string storedProcedure = "usp_Replys_Delete";
                    string deleteQuery = "DELETE FROM Replys Where Id =@p_Id";
                    var count= _connection.Execute(storedProcedure, param, commandType: CommandType.StoredProcedure);
                    return Ok("Reply content updated successfully.");
                }
                else
                {
                    return NotFound("Reply not found.");
                }
            }
            return NotFound("No Permission.");
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
            
            string query = "SELECT Id,UserName FROM Messages WHERE Id=@Id";
            var queryParam = new DynamicParameters();
            queryParam.Add("@Id", message.Id);
            var param = new DynamicParameters();
            param.Add("@Id", message.Id);
            param.Add("@Content", message.Content);

            var result= _connection.QueryFirstOrDefault<Message>(query, queryParam, commandType: CommandType.Text);           
            if (CheckPermission(result.UserName))
            {
                if (result != null)
                {
                    _connection.Execute("usp_Messages_UpdateContent", param, commandType: CommandType.StoredProcedure);
                    return Ok("Reply content updated successfully.");
                }
                else
                {
                    return NotFound("Reply not found.");
                }
            }
            return NotFound("No Permission.");
        }

        public bool CheckPermission(string userName)
        {
            return (userName == User.Identity.Name || User.IsInRole("Admin"));
        }
    }
}
