using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebShopping.Dtos
{
    //[MetadataType(typeof(newsItemAttributes))]       // [MetadataType(typeof(對應的 類別名稱))]
    //可將驗證抽離出來放在newsItemAttributes，news就可只放欄位屬性就好
    public class LotusDto
    {
        //// 流水號編號
        public int id { get; set; }

        //// 活動編號 活動類別另設 ID 唯一索引用
        public Guid lotusId { get; set; }

        //// 圖片名稱image_name cover封面圖片檔案
        public string cover { get; set; }

        public string title { get; set; }

        //// 副標題
        public string subTitle { get; set; }

        //// 簡述
        public string brief { get; set; }

        //// 文集冊號
        public string number { get; set; }

        //// 推薦標題  
        public string recommendTitle { get; set; }

        //// 推薦序
        public string recommend { get; set; }

        public string content { get; set; }

        //// 必填: 置頂狀態（0 關閉；1 開啟）
        public byte first { get; set; }

        //// 非必填排序，預設為流水號編號
        public int sort { get; set; }

        //// 必填: 啟用狀態（0 關閉；1 開啟）
        public byte enabled { get; set; }
        public string createDate { get; set; }

        //// 文集資訊欄位1~8
        public string information01 { get; set; }
        public string information02 { get; set; }
        public string information03 { get; set; }
        public string information04 { get; set; }
        public string information05 { get; set; }
        public string information06 { get; set; }
        public string information07 { get; set; }
        public string information08 { get; set; }

        //// 導覽圖片1~8
        public string imageURL01 { get; set; }
        public string imageURL02 { get; set; }
        public string imageURL03 { get; set; }
        public string imageURL04 { get; set; }
        public string imageURL05 { get; set; }
        public string imageURL06 { get; set; }
        public string imageURL07 { get; set; }
        public string imageURL08 { get; set; }


    }
}