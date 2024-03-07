using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebShopping.Dtos
{
    public class ConvertFormsDto
    {
        //// 0.C0000001 C加流水號編
        public string cId { get; set; }
        //// 1.流水號編號索引
        public int id { get; set; }
        //// 2.另設 ID 唯一索引用
        public Guid convertFormsId { get; set; }
        //// 3.中文姓名
        public string namez { get; set; }
        //// 4.性別
        public byte gender { get; set; }
        //// 5.英文姓名
        public string enNamez { get; set; }
        //// 6.皈依者 1本人2家人3祖先4亡者5纏身靈6怨親債主7水子靈8土地公9地基主10地靈11地神12寵物
        public byte convertz { get; set; }
        //// 7.出生日期
        public string birthz { get; set; }
        //// 8.隸屬地區 0無1北部2中部3南部4海外
        public byte affiliatedAreaz { get; set; }
        //// 9.戶籍地址
        public string residenceAddress { get; set; }
        //// 10.聯絡地址
        public string contactAddress { get; set; }
        //// 11.聯絡電話
        public string contactNumber { get; set; }
        //// 12.手機電話
        public string moblieNumber { get; set; }
        //// 13.電子信箱
        public string email { get; set; }
        //// 14.是否寄發證書必填1是0否
        public byte sendCertificate { get; set; }
        //// 15.寄件地址
        public string sendAddress { get; set; }
        //// 16.備註內容
        public string remarks { get; set; }
        //// 17.狀態(1顯示，0不顯示)
        public byte enabled { get; set; }
        //// 18.審核(0未通過, 1通過)
        public byte audit { get; set; }
        //// 19.新增時間
        public string creationDate { get; set; }
        //// 20.修改時間
        //public DateTime? updatedDate { get; set; }
    }
}