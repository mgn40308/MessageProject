namespace MessageProject.Models
{
    public class Message
    {
        /// <summary>
        /// ID序號
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 留言標題
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 留言內容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 留言人
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 留言時間
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// 留言回覆
        /// </summary>
        public ICollection<Reply> Replys { get; set; }

    }
}
