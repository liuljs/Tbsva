using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebShopping.Dtos
{
    public class IndexSlideshowDto
    {
        //// 自動流水號編號
        public int id { get; set; }

        //// 輪播編號
        public Guid slideId { get; set; }

        //// 輪播圖片名稱
        public string name { get; set; }

        //// 滿版圖片檔案fullImage
        public string imageURL01 { get; set; }

        //// 平板圖片檔案tabletImage
        public string imageURL02 { get; set; }

        //// 手機圖片檔案smPhoneImage
        public string imageURL03 { get; set; }

        //// 連結網址
        public string hyperlink { get; set; }

        //// 置頂狀態（0 關閉；1 開啟）
        public byte first { get; set; }

        //// 排序，預設為流水號編號
        public int sort { get; set; }

        //// 啟用狀態（0 關閉；1 開啟）
        public byte enabled { get; set; }

        public string creationDate { get; set; }

        //public DateTime? updatedDate { get; set; }
    }
}