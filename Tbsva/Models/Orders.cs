using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebShopping.Models
{  
    /// <summary>
    /// 訂單
    /// </summary>
    public class Orders
    {
        /// <summary>
        /// 付款方式  對照表
        /// </summary>
        //public string[] Pay_Type_Array = { "無", "信用卡", "ATM轉帳", "超商帳單代收", "7-11", "全家", "萊爾富" };
        public Dictionary<int, string> Pay_Type_Array = new Dictionary<int, string>() {
            {0, "無"} ,
            {1, "信用卡"},
            {2, "ATM轉帳"},
            {3, "超商帳單代收"},
            {4, "7-11"},
            {5, "全家"},
            {6, "萊爾富"},
            {7, "超商代碼"},
            {13, "LINE Pay"}
        };

        /// <summary>
        /// 配送方式 對照表
        /// </summary>
        public string[] Delivery_Type_Array = { "無", "店到店", "宅配" };

        /// <summary>
        ///  訂單狀態 對照表
        /// </summary>
        public Dictionary<int, string> Order_Status_Dic = new Dictionary<int, string>()
                {   {11, "待付款-買家尚未付款"}, {12, "待付款-逾期未付"}, {13, "待付款-付款失敗"}, {14, "待付款-賣家取消"}, {15, "待付款-買家取消"}, {16, "待付款-買家成功付款"},
                    {21, "待出貨-未出貨"}, {22, "待出貨-備貨中"}, {23, "待出貨-已出貨"},
                    {31, "運送中-未送達"}, {32, "運送中-已送達"},
                    {41, "已完成-未取貨"}, {42, "已完成-已取貨"},
                    {51, "取消-待回應"},   {52, "取消-已取消"},
                    {99, "購物車"}
                };
                                                                
        /// <summary>
        /// 訂單Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 會員Id
        /// </summary>        
        public Guid Member_Id { get; set; }

        /// <summary>
        /// 訂單狀態Id
        /// </summary>
        public byte Order_Status_Id { get; set; }

        /// <summary>
        /// 訂單詳情(購買物品內容)
        /// </summary>
        public List<Order_Item> Order_Detail { get; set; }

        /// <summary>
        /// 訂單總金額
        /// </summary>
        public decimal Order_Total { get; set; }

        /// <summary>
        /// 運費
        /// </summary>
        public decimal Delivery_Fee { get; set; }

        /// <summary>
        /// 付款方式  1、信用卡  2、 ATM轉帳  3、超商帳單代收   4、7-11   5、全家   6、萊爾富
        /// </summary>
        public int Pay_Type_Id { get; set; }

        /// <summary>
        /// 配送方式  1、店到店  2、宅配
        /// </summary>
        public int Delivery_Type_Id { get; set; }

        /// <summary>
        /// 回饋點數
        /// </summary>
        public int Point { get; set; }

        [MaxLength]
        /// <summary>
        /// 商家備註        
        /// </summary>
        public string Memo_Store { get; set; }

        [MaxLength]
        /// <summary>
        /// 消費者備註        
        /// </summary>
        public string Memo_Customer { get; set; }
        
        /// <summary>
        /// 訂單是否已刪除     
        /// </summary>
        public byte Deleted { get; set; }

        [StringLength(10)]
        /// <summary>
        /// 銀行代碼
        /// </summary>
        public string Bank_No { get; set; }

        [StringLength(3)]
        /// <summary>
        /// 子付款方式
        /// </summary>
        public string Pay_zg { get; set; }

        [StringLength(20)]
        /// <summary>
        /// 繳費代碼(ATM | CVS)
        /// </summary>
        public string Payment_No { get; set; }

        /// <summary>
        /// 訂單是否已取消        
        /// </summary>
        public byte Is_Cancel { get; set; }

        /// <summary>
        /// 訂單是否已退貨     
        /// </summary>
        public byte Is_Return { get; set; }

        /// <summary>
        /// 繳費期限       
        /// </summary>
        public DateTime Pay_End_Date { get; set; }

        [StringLength(100)]
        /// <summary>
        /// 退款資訊        
        /// </summary>
        public string Refund { get; set; }

        /// <summary>
        /// 商家退款金額    
        /// </summary>
        public decimal Refund_Amount { get; set; }

        [StringLength(30)]
        /// <summary>
        /// 收件者_名字        
        /// </summary>
        public string Receiver_Name { get; set; }

        [StringLength(15)]
        /// <summary>
        /// 收件者_電話        
        /// </summary>
        public string Receiver_Phone { get; set; }

        [StringLength(50)]
        /// <summary>
        /// 收件者_住址        
        /// </summary>
        public string Receiver_Address { get; set; }

        [StringLength(100)]
        /// <summary>
        /// 收件者_信箱       
        /// </summary>
        public string Receiver_Email { get; set; }

        /// <summary>
        /// 建立時間
        /// </summary>
        public DateTime Creation_Date { get; set; }

        /// <summary>
        /// 訂購日
        /// </summary>
        public DateTime? Purchase_Date { get; set; }

        /// <summary>
        /// 更新時間
        /// </summary>
        public DateTime Update_Date { get; set; }

        /// <summary>
        /// 預計出貨時間
        /// </summary>
        public DateTime Delivery_Date { get; set; }

        /// <summary>
        /// 預計抵達時間
        /// </summary>
        public DateTime Arrival_Date { get; set; }

        /// <summary>
        /// 付款時間
        /// </summary>
        public DateTime Pay_Date { get; set; }

        /// <summary>
        /// 購買人資訊
        /// </summary>
        public Member MemberInfo { get; set; }

        /// <summary>
        /// 發票資訊
        /// </summary>
        public string Invoice { get; set; }
    }

    /// <summary>
    /// 訂單詳情
    /// </summary>
    public class Order_Item
    {
        /// <summary>
        /// 訂單詳情id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 訂單id
        /// </summary>
        public int Orders_Id { get; set; }

        /// <summary>
        /// 產品id
        /// </summary>
        public int Sku_Id { get; set; }

        /// <summary>
        /// 產品名稱
        /// </summary>
        public string Spu { get; set; }

        /// <summary>
        /// 規格名稱
        /// </summary>
        public string Sku { get; set; }


        /// <summary>
        /// 價格    
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 優惠價格    
        /// </summary>
        public decimal Discount { get; set; }

        /// <summary>
        /// 訂購數量
        /// </summary>
        public int Quantity { get; set; }
        
        /// <summary>
        /// 建置時間
        /// </summary>
        public DateTime Creation_Date { get; set; }
        
        /// <summary>
        /// 附言
        /// </summary>
        public string Content { get; set; }
    }

    public class OrderMemo {
        /// <summary>
        /// 要修改的訂單編號
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 要增加的備註
        /// </summary>
        public string Memo { get; set; }

        /// <summary>
        /// 管理員編號
        /// </summary>
        public Guid Manager_Id { get; set; }
    }
}