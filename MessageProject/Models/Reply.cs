namespace MessageProject.Models
{
    public class Reply
    {
        /// <summary>
        /// 回覆的ID序號
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 回覆的留言ID
        /// </summary>
        public int MessageId { get; set; }
        /// <summary>
        /// 回覆者
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 回覆內容
        /// </summary>
        public string ReplyContent { get; set; }
        /// <summary>
        /// 回覆時間
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// 回覆的留言
        /// </summary>
        public Message Message { get; set; }
    }
}
