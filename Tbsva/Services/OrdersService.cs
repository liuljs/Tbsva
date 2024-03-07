using WebShopping.Connection;
using WebShopping.Helpers;
using WebShopping.Models;
using WebShopping.Dtos;
using WebShopping.Services;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebShopping.Services
{
    public class OrdersService : IOrdersService
    {
        IDapperHelper m_DapperHelper;

        public OrdersService(IDapperHelper p_IDrapperHelp)
        {
            m_DapperHelper = p_IDrapperHelp;
        }

        /// <summary>
        /// 取得訂單(無條件的)
        /// </summary>
        /// <returns> List<Orders> </returns>
        public List<Orders> GetOrders()
        {
            List<Orders> orders_ = new List<Orders>();

            string sql_ = @"SELECT * FROM [ORDERS]                            
                            WHERE [DELETED] <> 1 AND order_status_id<>99";

            orders_ = m_DapperHelper.QuerySetSql<Orders>(sql_).ToList();

            return orders_;            
        }

        /// <summary>
        /// 會員取得訂單(無條件的)
        /// </summary>
        /// <returns> List<Orders> </returns>
        public List<Orders> MemberGetOrders(Guid memner_id, int? p_id, int[] p_order_status_id, DateTime? p_startDate, DateTime? p_endDate, int? p_count, int? p_page)
        {
            List<Orders> orders_ = new List<Orders>();

            string strConditionSql_ = $"WHERE [DELETED] <> 1 AND order_status_id<>99 AND MEMBER_ID='{memner_id.ToString()}' ";

            string pages = "";
            if (p_count != null && p_count>0 && p_page != null && p_page>0)
            {
                int startRowIndex = 0;
                startRowIndex = Convert.ToInt32(p_page - 1) * Convert.ToInt32(p_count);
                pages = $" OFFSET {startRowIndex} ROWS FETCH NEXT {p_count} ROWS ONLY ";
            }

            //條件判斷
            if (p_id != null && p_id > 0)
                strConditionSql_ += $@"AND ID={p_id}";

            if (p_order_status_id != null && p_order_status_id.Count() > 0)
            {
                strConditionSql_ += $@"AND (";
                foreach (var sid in p_order_status_id)
                    strConditionSql_ += $@" order_status_id={sid} OR";
                strConditionSql_ = strConditionSql_.Remove(strConditionSql_.Length - 2);
                strConditionSql_ += $@")";
            }

            if (p_startDate != null && p_startDate > DateTime.MinValue)
                strConditionSql_ += $@" AND CREATION_DATE >= '{Convert.ToDateTime(p_startDate).ToString("yyyy-MM-dd HH:mm:ss")}'";

            if (p_endDate != null && p_endDate > DateTime.MinValue)
                strConditionSql_ += $@" AND CREATION_DATE < '{Convert.ToDateTime(p_endDate).AddDays(1).ToString("yyyy-MM-dd HH:mm:ss")}'";

            string sql_ = $@"SELECT * FROM [ORDERS]                            
                            {strConditionSql_}
                            Order by creation_date desc
                            {pages}";

            orders_ = m_DapperHelper.QuerySetSql<Orders>(sql_).ToList();

            return orders_;
        }

        /// <summary>
        /// 取得訂單明細
        /// </summary>
        /// <param name="orders_id"></param>
        /// <returns></returns>
        public List<SendOrderItemsGetDto> MemberGetOrderItems(int orders_id)
        {
            List<SendOrderItemsGetDto> items_ = new List<SendOrderItemsGetDto>();

            string sql_ = $@"Select o.*,p.id as Spu_Id,p.product_cover from [order_item] o left join SKU k on o.sku_id=k.id left join SPU p on k.spu_id=p.id
                            Where orders_id={orders_id}";

            items_ = m_DapperHelper.QuerySetSql<SendOrderItemsGetDto>(sql_).ToList();

            return items_;
        }

        /// <summary>
        /// 取得訂單(條件的)
        /// </summary>
        /// <param name="p_id">訂單id</param>
        /// <param name="p_startDate">訂單建立時間_開始</param>
        /// <param name="p_endDate">訂單建立時間_結束</param>
        /// <param name="p_count"> 一頁幾筆 </param>
        /// <param name="p_page"> 第幾頁 </param>
        /// <returns></returns>
        public List<Orders> GetOrdersByCondition(int? p_id, int[] p_order_status_id, DateTime? p_startDate, DateTime? p_endDate, int? p_count, int? p_page)
        {
            List<Orders> orders_ = new List<Orders>();

            string strConditionSql_ = @"WHERE order_status_id<>99 ";
            string pages = "";
            if (p_count != null && p_count > 0 && p_page != null && p_page > 0)
            {
                int startRowIndex = 0;
                startRowIndex = Convert.ToInt32(p_page - 1) * Convert.ToInt32(p_count);
                pages = $" OFFSET {startRowIndex} ROWS FETCH NEXT {p_count} ROWS ONLY ";
            }

            //條件判斷
            if (p_id != null && p_id > 0)
                strConditionSql_ += $@"AND ID={p_id}";

            if (p_order_status_id != null && p_order_status_id.Count() > 0)
            {
                strConditionSql_ += $@"AND (";
                foreach (var sid in p_order_status_id)
                    strConditionSql_ += $@" order_status_id={sid} OR";
                strConditionSql_ = strConditionSql_.Remove(strConditionSql_.Length - 2);
                strConditionSql_ += $@")";
            }

            if (p_startDate != null && p_startDate > DateTime.MinValue)
                strConditionSql_ += $@" AND CREATION_DATE >= '{Convert.ToDateTime(p_startDate).ToString("yyyy-MM-dd HH:mm:ss")}'";

            if (p_endDate != null && p_endDate > DateTime.MinValue)
                strConditionSql_ += $@" AND CREATION_DATE < '{Convert.ToDateTime(p_endDate).AddDays(1).ToString("yyyy-MM-dd HH:mm:ss")}'";


            string sql_ = $@"SELECT * FROM [ORDERS] 
                            {strConditionSql_}
                            ORDER BY id desc
                            {pages}";

            orders_ = m_DapperHelper.QuerySetSql<Orders>(sql_).ToList();

            return orders_;
        }

        /// <summary>
        /// 會員取得訂單(條件的)
        /// </summary>
        /// <param name="p_id">訂單id</param>
        /// <param name="p_order_status_id">狀態id</param>
        /// <param name="p_startDate">訂單建立時間_開始</param>
        /// <param name="p_endDate">訂單建立時間_結束</param>
        /// <param name="p_count"> 一頁幾筆 </param>
        /// <param name="p_page"> 第幾頁 </param>
        /// <returns></returns>
        public List<Orders> MemberGetOrdersByCondition(Guid memner_id, int? p_id, int? p_order_status_id, DateTime? p_startDate, DateTime? p_endDate, int? p_count, int? p_page)
        {
            List<Orders> orders_ = new List<Orders>();

            string strConditionSql_ = $@"WHERE [DELETED] <> 1 AND order_status_id<>99 AND MEMBER_ID='{memner_id.ToString()}' ";
            string pages = "";
            if (p_count != null && p_count > 0 && p_page != null && p_page > 0)
            {
                int startRowIndex = 0;
                startRowIndex = Convert.ToInt32(p_page - 1) * Convert.ToInt32(p_count);
                pages = $" OFFSET {startRowIndex} ROWS FETCH NEXT {p_count} ROWS ONLY ";
            }

            //條件判斷
            if (p_id != null)
                strConditionSql_ += $@" AND ID={p_id}";

            if (p_order_status_id != null)
                strConditionSql_ += $@" AND order_status_id={p_order_status_id}";

            if (p_startDate != null)
                strConditionSql_ += $@" AND CREATION_DATE >= '{Convert.ToDateTime(p_startDate).ToString("yyyy-MM-dd HH:mm:ss")}'";

            if (p_endDate != null)
                strConditionSql_ += $@" AND CREATION_DATE < '{Convert.ToDateTime(p_endDate).AddDays(1).ToString("yyyy-MM-dd HH:mm:ss")}'";

            string sql_ = $@"SELECT * FROM [ORDERS] {strConditionSql_}
                            ORDER BY id desc
                            {pages}";

            orders_ = m_DapperHelper.QuerySetSql<Orders>(sql_).ToList();

            return orders_;
        }

        /// <summary>
        /// 取得訂單詳情
        /// </summary>
        /// <param name="p_id"></param>
        /// <returns></returns>
        public Orders GetOrderDetail(int? p_id)
        {
            Orders order_ = new Orders();

            order_.Id = (int)p_id;

            string sql_ = string.Format(@"SELECT * FROM [ORDERS] A 
                                          JOIN [ORDER_ITEM] B ON A.ID = B.ORDERS_ID 
                                          JOIN [MEMBER] C ON A.MEMBER_ID = C.ID 
                                          WHERE A.ID = {0}", p_id);

            try
            {
                using (var _cn = new ConnectionFactory().CreateConnection())
                {
                    var orderDictionary_ = new Dictionary<int, Orders>();

                    order_ = _cn.Query<Orders, Order_Item, Member, Orders>(
                               sql_,
                               (order, order_detail, member) =>
                               {
                                   Orders orderEntry_;

                                   if (!orderDictionary_.TryGetValue(order.Id, out orderEntry_))
                                   {
                                       orderEntry_ = order;
                                       orderEntry_.MemberInfo = member;
                                       orderEntry_.Order_Detail = new List<Order_Item>();
                                       orderDictionary_.Add(orderEntry_.Id, orderEntry_);
                                   }
                                   orderEntry_.Order_Detail.Add(order_detail);
                                   return orderEntry_;
                               },
                               splitOn: "ID").FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return order_;
        }


        ///// <summary>
        ///// 取消訂單  
        ///// </summary>
        ///// <returns> bool </returns>
        //public bool CancelOrder()
        //{
        //    return true;
        //}

        /// <summary>
        /// 管理者更改訂單備註
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public int AdminUpdateOrderMemo(OrderMemo order)
        {
            //string _sql = $@"
            //    UPDATE [ORDERS] SET 
            //    memo_store=ISNULL(memo_store,'')+CHAR(13)+CHAR(10)+@Memo,update_date=GETDATE()
            //    WHERE ID=@Id
            //";
            string _sql = $@"
                UPDATE [ORDERS] SET 
                memo_store=@Memo,update_date=GETDATE()
                WHERE ID=@Id
            ";
            int i = m_DapperHelper.ExecuteSql(_sql, order);
            return i;
        }

        /// <summary>
        /// 管理者更新訂單狀態
        /// </summary>
        /// <param name="order"></param>
        /// <param name="msg"></param>
        /// <param name="memo"></param>
        /// <returns></returns>
        public int AdminUpdateOrderStatus(Orders order, string msg, string memo,bool IsbackStore, Guid Manager_Id)
        {
            string cancelSQL = "";
            string memoSQL = "";
            string arrivalSQL = "";
            //if (!string.IsNullOrEmpty(memo)) {
            //    //memoSQL = $",memo_store=ISNULL(memo_store,'')+CHAR(13)+CHAR(10)+'{memo}'";
            order.Memo_Store = memo;
            memoSQL = $",memo_store=@memo_store";
            if (order.Order_Status_Id == 21) {
                arrivalSQL = $@" ,delivery_date='{DateTime.Today.AddDays(3).ToString("yyyy/MM/dd")}' 
                        ,arrival_date='{DateTime.Today.AddDays(4).ToString("yyyy/MM/dd")}' ";
            }
            //}
            if (IsbackStore)
            {
                //取消時,回充庫存
                cancelSQL = @"Update sku set 
                            stock_qty=stock_qty+quantity,
                            sell_qty=sell_qty-quantity
				            from sku s inner join order_item o on orders_id=@id and s.id=sku_id";
            }
            string _sql = $@"
                {cancelSQL}

                UPDATE [ORDERS] SET 
                order_status_id=@Order_Status_Id,update_date=GETDATE()
                ,Is_Cancel=@Is_Cancel,Is_Return=@Is_Return
                {memoSQL}
                {arrivalSQL}
                WHERE ID=@Id

                INSERT INTO [transaction](member_id,order_id,transaction_status_id,creation_date,content)
                VALUES('{Manager_Id}',@Id,@Order_Status_Id,getdate(),'{msg}')
            ";
            int i = m_DapperHelper.ExecuteSql(_sql, order);
            return i;
        }
        /// <summary>
        /// 會員更新訂單狀態
        /// 取消訂單/訂單退貨
        /// </summary>
        /// <returns> bool </returns>
        public int MemberUpdateOrderStatus(Orders order, string msg)
        {
            string cancelSQL = "";
            if (order.Order_Status_Id == 51 && order.Is_Cancel == 1)
            {
                //取消時,回充庫存
                cancelSQL = @"Update sku set 
                            stock_qty=stock_qty+quantity,
                            sell_qty=sell_qty-quantity
				            from sku s inner join order_item o on orders_id=@id and s.id=sku_id";
            }
            string _sql = $@"
                {cancelSQL}

                UPDATE [ORDERS] SET 
                order_status_id=@Order_Status_Id,update_date=GETDATE()
                ,Is_Cancel=@Is_Cancel,Is_Return=@Is_Return
                WHERE ID=@Id

                INSERT INTO [transaction](member_id,order_id,transaction_status_id,creation_date,content)
                VALUES(@Member_Id,@Id,@Order_Status_Id,getdate(),'{msg}')
            ";
            int i = m_DapperHelper.ExecuteSql(_sql, order);
            return i;

        }

        /// <summary>
        /// 加入購物車
        /// </summary>
        /// <param name="p_SendAddCartDto"></param>
        /// <returns>購物車商品數</returns>
        public int AddCart(SendAddCartDto p_SendAddCartDto) {
            //1.檢查規格是否存在
            string sql_ = @"Select * from sku where id=@sku_id and spu_id=@spu_id";
            Sku sku = m_DapperHelper.QuerySqlFirstOrDefault<SendAddCartDto, Sku>(sql_, p_SendAddCartDto);
            if (sku == null) throw new Exception("規格不存在!");
            if (p_SendAddCartDto.qty > sku.Stock_Qty) throw new Exception($"庫存量[{sku.Stock_Qty}]不足!");

            //2.取得購物車id(訂單主表裡，狀態為99的資料，若無就新增一筆)
            sql_ = @"SELECT * FROM [ORDERS] 
                            WHERE [DELETED] = 0
                            AND [ORDER_STATUS_ID]=99
                            AND [MEMBER_ID]=@MEMBER_ID";
            List<Orders> orders_ = new List<Orders>();
            orders_ = m_DapperHelper.QuerySetSql<SendAddCartDto, Orders>(sql_, p_SendAddCartDto).ToList();

            int orders_id = 0;
            if (orders_.Count == 0)
            {//2.1無訂單
                //sql_ = @"INSERT INTO [ORDERS](member_id,order_status_id,order_total,pay_type_id,delivery_type_id,receiver_name,receiver_phone,receiver_address)
                //        VALUES(@member_id,99,0,1,1,'','','')
                //        select scope_identity()";
                sql_ = @"INSERT INTO [ORDERS](member_id,order_status_id,order_total)
                        VALUES(@member_id,99,0)
                        select scope_identity()";
                orders_id = m_DapperHelper.QuerySingle(sql_, p_SendAddCartDto);
                ;
            }
            else
            {//2.2訂單存在
                orders_id = orders_[0].Id;
            }

            //3.寫入明細
            //TODO:考慮寫入discount_price,discount_percent
            p_SendAddCartDto.orders_id = orders_id;
            sql_ = @"Declare @spu nvarchar(500)
                     Declare @sku nvarchar(500)
                     Declare @content nvarchar(500)
                     Declare @price decimal(8, 0)

                     Select @spu_id=spu_id,@sku=title,@price=sell_price from sku where id=@sku_id
                     Select @spu=title,@content=describe from spu where id=@spu_id

                     IF NOT EXISTS(SELECT * FROM [order_item] WHERE orders_id=@orders_id and sku_id=@sku_id)
                       INSERT INTO [order_item](orders_id,sku_id,spu,sku,price,quantity,content) VALUES(@orders_id,@sku_id,@spu,@sku,@price,@qty,@content)
                         ELSE
                       UPDATE [order_item] SET spu=@spu,sku=@sku,price=@price,quantity=@qty,content=@content WHERE orders_id=@orders_id and sku_id=@sku_id
                    Select Count(1) from [order_item] WHERE orders_id=@orders_id ";
            int i = m_DapperHelper.QuerySingle(sql_, p_SendAddCartDto);
            return i;
        }

        /// <summary>
        /// 取得購物車
        /// </summary>
        /// <returns> List<Orders> </returns>
        public List<RecvCartDto> GetCart(Guid member_id)
        {
            List<RecvCartDto> orders_ = new List<RecvCartDto>();

            string sql_ = $@"Select a.*,discount_price,k.spu_id,p.product_cover,k.stock_qty 
                from order_item a 
                left join sku k on a.sku_id=k.id 
				left join spu p on p.id=k.spu_id
                where orders_id=(Select id from orders where order_status_id=99 and member_id='{member_id}') ";

            orders_ = m_DapperHelper.QuerySetSql<RecvCartDto>(sql_).ToList();

            return orders_;
        }

        /// <summary>
        /// 更改購物車數量
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public int UpdateCart(CardUpdateDto dto)
        {
            string sql_ = "update order_item set quantity=@quantity where id=@id";
            int result = m_DapperHelper.ExecuteSql(sql_, dto);
            return result;
        }

        /// <summary>
        /// 刪除購物車品項
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public int DeleteCart(CardUpdateDto dto)
        {
            string sql_ = "delete order_item where id=@id";
            int result = m_DapperHelper.ExecuteSql(sql_, dto);
            return result;
        }

        /// <summary>
        /// 更新購買資訊(購物車,仍未轉訂單)
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public int UpdatePurchase(PurchaseDto dto)
        {
            //dto.delivery_type_id
            string sql_ = @"
                declare @total decimal(8,0),@deliveryFee decimal(8,0)

                --設定總金額
                Select @total=sum(price*quantity) from order_item
                where orders_id=(Select id from orders where member_id=@member_id and order_status_id=99)

                --設定運費
				if(@total>=1000)
					set @deliveryFee=0
				else
					if(@delivery_type_id=1)
						set @deliveryFee=60
					else 
						set @deliveryFee=80

                UPDATE [ORDERS] SET
                order_total=@total,delivery_fee=@deliveryFee,pay_type_id=@pay_type_id,delivery_type_id=@delivery_type_id,delivery_time=@delivery_time,memo_customer=@memo_customer
                ,purchaser_name=@purchaser_name,purchaser_sex=@purchaser_sex,purchaser_phone=@purchaser_phone,purchaser_tel=@purchaser_tel,purchaser_address=@purchaser_address,purchaser_email=@purchaser_email
                ,receiver_name=@receiver_name,receiver_sex=@receiver_sex,receiver_phone=@receiver_phone,receiver_tel=@receiver_tel,receiver_address=@receiver_address,receiver_email=@receiver_email
                ,invoice=@invoice,update_date=getdate()
                WHERE [DELETED] = 0 AND [ORDER_STATUS_ID]=99 AND [MEMBER_ID]=@MEMBER_ID";

            int result = m_DapperHelper.ExecuteSql(sql_, dto);
            return result;
        }

        /// <summary>
        /// 取得訂單資料
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public PurchaseDto GetPurchase(PurchaseDto dto)
        {
            string sql_ = @"Select * FROM [ORDERS] Where id=@id";

            PurchaseDto result = m_DapperHelper.QuerySqlFirstOrDefault<PurchaseDto>(sql_, dto);
            return result;
        }

        /// <summary>
        /// 新增購買資訊(購物車轉訂單)
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public PurchaseDto CreatPurchase(PurchaseDto dto)
        {
            string sql_ = @"
                Declare @order_id int
                Select @order_id=id from orders where member_id=@member_id and order_status_id=99

				IF NOT EXISTS(Select stock_qty=stock_qty-quantity
				from sku s inner join order_item o on orders_id=@order_id and s.id=sku_id
				where stock_qty-quantity<0)
                BEGIN
				    Update sku set 
                    stock_qty=stock_qty-quantity,
                    sell_qty=sell_qty+quantity
				    from sku s inner join order_item o on orders_id=@order_id and s.id=sku_id

                    UPDATE [ORDERS] SET
                    order_status_id=11,purchase_date=getdate(),update_date=getdate()
                    WHERE id=@order_id

                    Select @order_id
                END
                ELSE
                    Select 0
                ";

            int order_id = m_DapperHelper.QuerySingle(sql_, dto);
            if (order_id == 0) return null;//庫存不足

            dto.id = order_id;
            PurchaseDto result = GetPurchase(dto);

            return result;
        }
    }
}