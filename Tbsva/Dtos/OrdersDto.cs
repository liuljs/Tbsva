using WebShopping.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebShopping.Dtos
{
    /// <summary>
    /// 訂單資訊(傳送)
    /// </summary>
    public class SendOrdersGetDto
    {
        /// <summary>
        /// 訂單Id (EX: #0000001)
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 建立時間
        /// </summary>
        public string Creation_Date { get; set; }

        /// <summary>
        /// 訂購日
        /// </summary>
        public string Purchase_Date { get; set; }

        /// <summary>
        /// 訂單總金額
        /// </summary>
        public decimal Order_Total { get; set; }

        /// <summary>
        /// 運費
        /// </summary>
        public decimal Delivery_Fee { get; set; }

        /// <summary>
        /// 授權金額
        /// </summary>
        public decimal Auth_Total { get; set; }

        /// <summary>
        /// 付款方式  0、無  1、信用卡  2、 ATM轉帳  3、超商帳單代收   4、7-11   5、全家   6、萊爾富
        /// </summary>
        public string Pay_Type { get; set; }

        /// <summary>
        /// 訂單狀態Id
        /// </summary>
        public string Order_Status_Id { get; set; }

        /// <summary>
        /// 訂單狀態
        /// </summary>
        public string Order_Status { get; set; }

        /// <summary>
        /// 預計出貨時間
        /// </summary>
        public string Delivery_Date { get; set; }

        /// <summary>
        /// 預計抵達時間
        /// </summary>
        public string Arrival_Date { get; set; }

        /// <summary>
        /// 授權時間
        /// </summary>
        public string Pay_Date { get; set; }

        /// <summary>
        /// 配送方式  0、無  1、店到店  2、宅配
        /// </summary>
        public string Delivery_Type { get; set; }

        /// <summary>
        /// 收件者_名字        
        /// </summary>
        public string Receiver_Name { get; set; }

        /// <summary>
        /// 收件者_電話        
        /// </summary>
        public string Receiver_Phone { get; set; }

        /// <summary>
        /// 收件者_住址        
        /// </summary>
        public string Receiver_Address { get; set; }

        /// <summary>
        /// 收件者_信箱       
        /// </summary>
        public string Receiver_Email { get; set; }

        /// <summary>
        /// 繳費期限       
        /// </summary>
        public string Pay_End_Date { get; set; }

        /// <summary>
        /// 消費者備註        
        /// </summary>
        public string Memo_Customer { get; set; }

        /// <summary>
        /// 付款方式  1、信用卡  2、 ATM轉帳  3、超商帳單代收   4、7-11   5、全家   6、萊爾富
        /// </summary>
        public int Pay_Type_Id { get; set; }

        /// <summary>
        /// 付款方式，信用卡改傳01
        /// </summary>
        public string Pay_Type_sId
        {
            get
            {
                if (Pay_Type_Id == 1) return "01";
                return $"{Pay_Type_Id}";
            }
        }

        [StringLength(10)]
        /// <summary>
        /// 銀行代碼
        /// </summary>
        public string Bank_No { get; set; }

        /// <summary>
        /// (超商|銀行)名稱
        /// </summary>
        public string Bank_Name
        {
            get
            {
                switch (Pay_zg)
                {
                    case "2":
                        switch (Bank_No)
                        {
                            case "006": return "合作金庫";
                            case "008": return "華南商銀";
                            case "013": return "國泰世華";
                            case "822": return "中國信託";
                        }
                        break;
                    case "4":
                    case "5": return "ibon";
                    case "6": return "FamiPort";
                    case "9": return "OK-go";
                    case "10": return "LifeET";
                }

                return "";
            }
        }

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
        /// 發票資訊
        /// </summary>
        public string Invoice { get; set; }

        /// <summary>
        /// 訂單明細
        /// </summary>
        public List<SendOrderItemsGetDto> Items { get; set; }
    }

    /// <summary>
    /// 訂單明細(傳送)
    /// </summary>
    public class SendOrderItemsGetDto {
        /// <summary>
        /// 訂單明細編號
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 訂單編號
        /// </summary>
        public int Order_Id { get; set; }

        /// <summary>
        /// 產品編號
        /// </summary>
        public int Spu_Id { get; set; }

        /// <summary>
        /// 規格編號
        /// </summary>
        public int Sku_Id { get; set; }


        /// <summary>
        /// 產品名
        /// </summary>
        public string Spu { get; set; }

        /// <summary>
        /// 規格名
        /// </summary>
        public string Sku { get; set; }

        /// <summary>
        /// 購買的價格
        /// </summary>
        public int Price { get; set; }

        /// <summary>
        /// 購買數量
        /// </summary>
        public int quantity { get; set; }

        /// <summary>
        /// 產品說明
        /// </summary>
        public string content { get; set; }

        /// <summary>
        /// 產品封面圖
        /// </summary>
        public string product_cover { get; set; }
    }

    /// <summary>
    /// 訂單資訊有條件式(收到)
    /// </summary>
    public class RecvOrdersGetDto
    {
        [Required]
        [RegularExpression("[1-9][0-9]*", ErrorMessage = "主類別{0}必須為正整數")]
        /// <summary>
        /// 訂單Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 搜尋建立時間_開始
        /// </summary>
        public DateTime Creation_Date_Start { get; set; }
        
        /// <summary>
        /// <summary>
        /// 搜尋建立時間_結束
        /// </summary>
        public DateTime Creation_Date_End { get; set; }

    }

    /// <summary>
    /// 訂單詳情資訊(傳送)
    /// </summary>
    public class SendOrderDetailGetDto
    {
        /// <summary>
        /// 訂單Id (EX: #0000001)
        /// </summary>
        public string Id { get; set; }
        
        [Required, StringLength(30)]
        /// <summary>
        /// 購買人姓名        
        /// </summary>
        public string Name { get; set; }

        [Required, StringLength(15)]
        /// <summary>
        /// 購買人電話號碼        
        /// </summary>
        public string Phone { get; set; }

        [StringLength(50)]
        /// <summary>
        /// 購買人住址        
        /// </summary>
        public string Address { get; set; }

        [Required, StringLength(50), EmailAddress]
        /// <summary>
        /// 購買人信箱
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// 配送方式  0、無  1、店到店  2、宅配
        /// </summary>
        public string Delivery_Type { get; set; }

        /// <summary>
        /// 付款方式  0、無  1、信用卡  2、 ATM轉帳  3、超商帳單代收   4、7-11   5、全家   6、萊爾富
        /// </summary>
        public string Pay_Type { get; set; }

        /// <summary>
        /// 訂單狀態Id
        /// </summary>
        public byte Order_Status_Id { get; set; }

        /// <summary>
        /// 訂單狀態
        /// </summary>
        public string Order_Status { get; set; }

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
        /// 授權金額
        /// </summary>
        public decimal Auth_Total { get; set; }

        /// <summary>
        /// 付款方式  1、信用卡  2、 ATM轉帳  3、超商帳單代收   4、7-11   5、全家   6、萊爾富
        /// </summary>
        public int Pay_Type_Id { get; set; }

        /// <summary>
        /// 付款方式，信用卡改傳01
        /// </summary>
        public string Pay_Type_sId
        {
            get
            {
                if (Pay_Type_Id == 1) return "01";
                return $"{Pay_Type_Id}";
            }
        }

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

        [StringLength(20)]
        /// <summary>
        /// 繳費代碼        
        /// </summary>
        public string Payment_No { get; set; }

        /// <summary>
        /// 訂單是否已取消        
        /// </summary>
        public byte Is_Cancel { get; set; }

        /// <summary>
        /// 繳費期限       
        /// </summary>
        public string Pay_End_Date { get; set; }

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

        /// <summary>
        /// 建立時間
        /// </summary>
        public string Creation_Date { get; set; }

        /// <summary>
        /// 更新時間
        /// </summary>
        public string Update_Date { get; set; }

        /// <summary>
        /// 訂購日
        /// </summary>
        public string Purchase_Date { get; set; }

        /// <summary>
        /// 預計出貨時間
        /// </summary>
        public string Delivery_Date { get; set; }

        /// <summary>
        /// 預計抵達時間
        /// </summary>
        public string Arrival_Date { get; set; }
    }

    /// <summary>
    /// 加入購物車(傳送)
    /// </summary>
    public class SendAddCartDto
    {
        /// <summary>
        /// 產品Id
        /// </summary>
        [Required]
        public int spu_id { get; set; }
        /// <summary>
        /// 規格Id
        /// </summary>
        [Required]
        public int sku_id { get; set; }
        /// <summary>
        /// 購買數量
        /// </summary>
        [Required]
        public int qty { get; set; }
        /// <summary>
        /// 會員編號
        /// </summary>
        public Guid member_id { get; set; }
        /// <summary>
        /// 訂單編號
        /// </summary>
        public int orders_id { get; set; }
    }

    /// <summary>
    /// 加入購物車(傳送)
    /// 含總計
    /// </summary>
    public class RecvMasterCartDto
    {
        /// <summary>
        /// 金額總計
        /// </summary>
        public int Total
        {
            get
            {
                int result = 0;
                if (Items != null)
                {
                    foreach (var i in Items)
                    {
                        result += Convert.ToInt32(i.price * i.quantity);
                    }
                }
                return result;
            }
        }
        /// <summary>
        /// 特價金額總計
        /// </summary>
        public int Discount_Total
        {
            get
            {
                int result = 0;
                if (Items != null)
                {
                    foreach (var i in Items)
                    {
                            result += i.amount;
                    }
                }
                return result;
            }
        }
        /// <summary>
        /// 數量合計
        /// </summary>
        public int Count
        {
            get
            {
                int result = 0;
                if (Items != null)
                {
                    foreach (var i in Items)
                    {
                        result += i.quantity;
                    }
                }
                return result;
            }
        }
        public List<RecvCartDto> Items { get; set; }
    }

    /// <summary>
    /// 加入購物車(傳送)
    /// </summary>
    public class RecvCartDto
    {
        /// <summary>
        /// 購物車品項Id
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// 訂單編號(購物車編號)
        /// </summary>
        public int orders_id { get; set; }

        /// <summary>
        /// 規格Id
        /// </summary>
        public int sku_id { get; set; }

        /// <summary>
        /// 產品名稱
        /// </summary>
        public string spu { get; set; }

        /// <summary>
        /// 規格名稱
        /// </summary>
        public string sku { get; set; }

        /// <summary>
        /// 售價
        /// </summary>
        public decimal price { get; set; }
        /// <summary>
        /// 特價
        /// </summary>
        public decimal discount_price { get; set; }

        ///// <summary>
        ///// 特價(無用)
        ///// </summary>
        //public decimal discount { get; set; }

        /// <summary>
        /// 購買數量
        /// </summary>
        public int quantity { get; set; }

        /// <summary>
        /// 庫存量
        /// </summary>
        public int stock_qty { get; set; }

        /// <summary>
        /// 小計金額
        /// </summary>
        public int amount
        {
            get
            {
                if (discount_price > 0)
                    return Convert.ToInt32(discount_price) * quantity;
                return Convert.ToInt32(price) * quantity;
            }
        }

        /// <summary>
        /// 加入日期
        /// </summary>
        public string creation_date { get; set; }

        /// <summary>
        /// 產品說明
        /// </summary>
        public string content { get; set; }

        /// <summary>
        /// 產品Id
        /// </summary>
        public int spu_id { get; set; }

        /// <summary>
        /// 產品封面圖
        /// </summary>
        public string product_cover { get; set; }

    }

    /// <summary>
    /// 更改訂單狀態
    /// </summary>
    public class ChangeOrderStatusDto
    {
        /// <summary>
        /// 購物車Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 訂單狀態Id
        /// </summary>
        public int Order_Status_Id { get; set; }


        /// <summary>
        /// 管理者備註
        /// </summary>
        public string memo { get; set; }

    }

    /// <summary>
    /// 訂單查詢條件
    /// </summary>
    public class OrderQuery
    {
        public int id { get; set; }
        public int[] order_status_id { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public int count { get; set; }
        public int page { get; set; }
    }
}