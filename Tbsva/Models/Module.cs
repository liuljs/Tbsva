using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Data;
using ecSqlDBManager;
using System.Data.SqlClient;
using System.Web.Http;

namespace WebShoppingAdmin.Models
{
    public class Module : BaseModel
    {
        public Guid Id { get; set; }

        /// <summary>
        /// 代號e01 編輯類別  f01財務類別 m01管理類別
        /// </summary>
        [Required]
        [StringLength(3)]
        public string authId { get; set; }

        /// <summary>
        /// 選單名稱
        /// </summary>
        [Required]
        [StringLength(30)]
        public string name { get; set; }

        /// <summary>
        /// 對應的網頁carousel.html
        /// </summary>
        [Required]
        [StringLength(50)]
        public string path { get; set; }

        /// <summary>
        /// 屬於什麼front  back有的要標記給前端用
        /// </summary>
        [StringLength(10)]
        public string belong { get; set; }

        /// <summary>
        /// 是否顯示
        /// </summary>
        [Required]
        [StringLength(1)]
        //[RegularExpression(@"[0]|[1]")]
        public string view { get; set; }

        /// <summary>
        /// 是否編輯
        /// </summary>
        [Required]
        [StringLength(1)]
        //[RegularExpression(@"[0]|[1]")]
        public string edit { get; set; }


        /// <summary>
        /// 關連目錄editCategory,financialCategory,managerCategory
        /// </summary>
        [Required]
        [StringLength(20)]
        public string category { get; set; }


        //[Required]
        //[StringLength(1)]
        //[RegularExpression(@"[Y]|[N]")]
        //public string act_view { get; set; }

        //public string act_add { get; set; }

        //[Required]
        //[StringLength(1)]
        //[RegularExpression(@"[Y]|[N]")]
        //public string act_edt { get; set; }

        //[Required]
        //[StringLength(1)]
        //[RegularExpression(@"[Y]|[N]")]
        //public string act_del { get; set; }

        public DataTable Get()
        {
            SqlCommand sql = new SqlCommand(@"
            SELECT  [id], [authId], [name], [path], [belong], [view], [edit], [category]
            FROM [MODULE] order by authId ASC");

            DataTable result = db.GetResult(sql);

            return result;
        }
    }   
}