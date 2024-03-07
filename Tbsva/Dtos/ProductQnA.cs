using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebShopping.Dtos
{
    public class Spu_QnA
    {
        /// <summary>
        /// 流水編號
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// 產品Id
        /// </summary>
        public int spu_id { get; set; }

        /// <summary>
        /// 會員Id
        /// </summary>
        public Guid member_id { get; set; }

        /// <summary>
        /// 會員帳號
        /// </summary>
        public string account { get; set; }

        /// <summary>
        /// 管理者Id
        /// 回覆人
        /// </summary>
        public Guid manager_id { get; set; }

        /// <summary>
        /// 問題
        /// </summary>
        public string quection { get; set; }

        /// <summary>
        /// 回答
        /// </summary>
        public string answer { get; set; }

        /// <summary>
        /// 建立日期
        /// </summary>
        public DateTime created_at { get; set; }

        /// <summary>
        /// 回覆日期
        /// </summary>
        public DateTime? updated_at { get; set; }

        /// <summary>
        /// 公開可見
        /// </summary>
        public byte Is_View { get; set; }

    }
    public class Spu_QnA_Send: Spu_QnA
    {
         /// <summary>
        /// 建立日期
        /// </summary>
        public new string created_at { get; set; }

        /// <summary>
        /// 回覆日期
        /// </summary>
        public new string updated_at { get; set; }

    }
    public class ProductQuestion
    {
        /// <summary>
        /// 產品Id
        /// </summary>
        public int spu_id { get; set; }

        /// <summary>
        /// 會員Id
        /// </summary>
        public Guid member_id { get; set; }

        /// <summary>
        /// 問題
        /// </summary>
        public string quection { get; set; }
    }
    public class ProductAnswer
    {
        /// <summary>
        /// 流水編號
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// 管理者Id
        /// 回覆人
        /// </summary>
        public Guid manager_id { get; set; }

        /// <summary>
        /// 回答
        /// </summary>
        public string answer { get; set; }

        /// <summary>
        /// 公開可見
        /// </summary>
        public byte Is_View { get; set; }
    }

    public class QueryProductAnswer
    {
        /// <summary>
        /// 產品Id
        /// </summary>
        public int? spu_id { get; set; }

        /// <summary>
        /// 會員Id
        /// </summary>
        public Guid? member_id { get; set; }

        /// <summary>
        /// 是否是公開顯示
        /// </summary>
        public Boolean Is_View { get; set; }

    }
}