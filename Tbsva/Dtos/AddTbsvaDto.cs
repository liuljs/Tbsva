using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebShopping.Dtos
{
    public class AddTbsvaDto
    {
        //// 0.T0000001T加流水號編
        public string tId { get; set; }
        //// 1.流水號編號索引
        public int id { get; set; }
        //// 2.主索引鍵（P.K.）
        public Guid addtbsvaId { get; set; }
        //// 3.會員姓名
        public string namez { get; set; }
        //// 4.性別
        public byte gender { get; set; }
        //// 5.出生日期
        public string birthz { get; set; }
        //// 6.隸屬地區 0無1北部2中部3南部4海外
        public byte affiliatedAreaz { get; set; }
        //// 7.聯絡地址
        public string contactAddress { get; set; }
        //// 8.聯絡電話
        public string contactNumber { get; set; }
        //// 9.手機電話
        public string moblieNumber { get; set; }
        //// 10.電子信箱
        public string email { get; set; }
        //// 11.自我介紹（顯示於後台）前後台都有輸入欄
        public string contentz { get; set; }
        //// 12.備註內容（顯示於前台）
        public string notez { get; set; }
        //// 13.狀態(1顯示，0不顯示)
        public byte enabled { get; set; }
        //// 14.審核(0未通過, 1通過)
        public byte audit { get; set; }
        public string creationDate { get; set; }
    }
}