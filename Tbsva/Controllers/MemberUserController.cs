using WebShopping.Auth;
using WebShopping.Dtos;
using WebShopping.Models;
using WebShopping.Services;
using WebShoppingAdmin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace WebShopping.Controllers
{
    /// <summary>
    /// 前台會員管理
    /// </summary>
    [CustomAuthorize(Role.User)]
    public class MemberUserController : ApiController
    {
        private IMemberService m_IMemberService;

        public MemberUserController(IMemberService IMemberService)
        {
            m_IMemberService = IMemberService;
        }

        /// <summary>
        /// 取得會員基本資料(單一)
        /// </summary>
        /// <param name="id"> 會員的id </param>
        /// <returns></returns>        
        [HttpGet]
        public IHttpActionResult GetMember()
        {
            //只能取自己的
            Guid id = new Guid(User.Identity.Name);
            var member_ = m_IMemberService.GetMember(id);

            if (member_ != null)
            {
                SendMemberGetDto dto_ = ReturnGetMember(member_);

                return Ok(dto_);
            }
            else
            {
                return Ok();//StatusCode(HttpStatusCode.NotFound);
            }
        }

        /// <summary>
        /// 轉換成GetMember回傳的類別
        /// </summary>
        /// <param name="p_Member"> 會員基本資訊 </param>
        /// <returns></returns>
        private SendMemberGetDto ReturnGetMember(Member p_Member)
        {
            SendMemberGetDto dto_ = new SendMemberGetDto();
            dto_.Id = p_Member.Id;
            dto_.No = $"N{ p_Member.No.ToString("D5")}";
            dto_.Account = p_Member.Account;
            dto_.Name = p_Member.Name;
            dto_.Gender = p_Member.Gender;
            dto_.Creation_Date = p_Member.Creation_Date.ToString("yyyy/MM/dd HH:mm");
            dto_.Birthday = p_Member.Birthday.ToString("yyyy/MM/dd HH:mm");
            dto_.Phone = p_Member.Phone;
            dto_.Address = p_Member.Address;
            dto_.Blacklist = p_Member.Blacklist;
            if (p_Member.LastLogined != DateTime.MinValue)
                dto_.LastLogin = p_Member.LastLogined.ToString("yyyy/MM/dd HH:mm");

            return dto_;
        }

        //[HttpDelete]
        ///// <summary>
        ///// 刪除會員(1筆)               
        ///// </summary>
        ///// <param name="id"> 會員的id </param>
        ///// <returns></returns>               
        //public IHttpActionResult DeleteMember(Guid id)
        //{            
        //    try
        //    {
        //        var member_ = m_IMemberService.GetMember(id);
        //        if (member_ != null)
        //        {
        //            m_IMemberService.DeleteMember(id);

        //            return StatusCode(HttpStatusCode.NoContent);
        //        }
        //        else
        //        {
        //            return NotFound();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return Content(HttpStatusCode.InternalServerError, ex.Message);
        //    }
        //}

        [Route("MemberUser/AddMember")]
        [HttpPost]
        [AllowAnonymous]
        /// <summary>
        /// 新增(註冊)會員資訊
        /// </summary>
        /// <param name="p_RecvMemberAddDto"> 新增的會員資訊 </param>
        /// <returns></returns>
        public IHttpActionResult AddMember(RecvMemberAddDto p_RecvMemberAddDto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    MessageStatus status_ = m_IMemberService.AddMember(p_RecvMemberAddDto, false);

                    if (status_ == MessageStatus.None)
                    {
                        try
                        {
                            using (AuthUser obj = new AuthUser())
                            {
                                obj.UserLoginAction(p_RecvMemberAddDto.id.ToString(), p_RecvMemberAddDto.Name);
                            }
                        }
                        catch (Exception ex)
                        {
                            Helpers.SystemFunctions.WriteLogFile($"{ex.Message}\n{ex.StackTrace}");
                        }
                        return StatusCode(HttpStatusCode.NoContent);
                    }
                    else
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, Convert.ToInt32(status_).ToString()));
                    }
                }
                else
                {
                    var _errMsg = ModelState.Values.Where(x => x.Errors.Count > 0)?.FirstOrDefault()?.Errors.Select(y => y.ErrorMessage).FirstOrDefault();
                    return Content(HttpStatusCode.BadRequest, _errMsg);
                }
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPut]
        /// <summary>
        /// 更新會員資訊
        /// </summary>
        /// <param name="p_RecvMemberUpdateDto"> 更新的會員資訊 </param>
        /// <returns></returns>
        public IHttpActionResult UpdateMember([FromBody] RecvMemberUpdateDto p_RecvMemberUpdateDto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //只能取自己的
                    Guid id = new Guid(User.Identity.Name);

                    //string strReturn_ = string.Empty;

                    //if (id.ToString() == id2.ToString())
                    //strReturn_ = m_IMemberService.UpdateMember(id, p_RecvMemberUpdateDto, false);
                    //改成以下
                    MessageStatus status_ = m_IMemberService.UpdateMember(id, p_RecvMemberUpdateDto, false);
                    //else
                    //    strReturn_ = "不能修改他人資訊";

                    //if (strReturn_ == string.Empty)
                    //{
                    //    return StatusCode(HttpStatusCode.NoContent);
                    //}
                    //else
                    //{
                    //    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, strReturn_));
                    //}
                    //改成以下
                    if (status_ == MessageStatus.None)  //0沒有任何錯誤
                    {
                        return StatusCode(HttpStatusCode.NoContent);
                    }
                    else
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, status_.ToString()));
                    }

                }
                else
                {
                    var _errMsg = ModelState.Values.Where(x => x.Errors.Count > 0)?.FirstOrDefault()?.Errors.Select(y => y.ErrorMessage).FirstOrDefault();
                    return Content(HttpStatusCode.BadRequest, _errMsg);
                }
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [Route("MemberUser/ChangePassword")]
        [HttpPatch]
        /// <summary>
        /// 更新會員密碼
        /// </summary>
        /// <param name="p_RecvMemberPasswordDto"> 更新的會員密碼 </param>
        /// <returns></returns>
        public IHttpActionResult ChangePassword([FromBody] RecvMemberPasswordDto p_RecvMemberPasswordDto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //只能取自己的
                    Guid id = new Guid(User.Identity.Name);

                    string strReturn_ = string.Empty;

                    //if (id.ToString() == id2.ToString())
                    strReturn_ = m_IMemberService.ChangePassword(id, p_RecvMemberPasswordDto, false);
                    //else
                    //    strReturn_ = "不能修改他人資訊";

                    if (strReturn_ == string.Empty)
                    {
                        return StatusCode(HttpStatusCode.NoContent);
                    }
                    else
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, strReturn_));
                    }
                }
                else
                {
                    var _errMsg = ModelState.Values.Where(x => x.Errors.Count > 0)?.FirstOrDefault()?.Errors.Select(y => y.ErrorMessage).FirstOrDefault();
                    return Content(HttpStatusCode.BadRequest, _errMsg);
                }
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [Route("MemberUser/ForgetPassword")]
        [HttpPatch]
        [AllowAnonymous]
        /// <summary>
        /// 忘記會員密碼
        /// </summary>
        /// <param name="p_RecvForgetPasswordDto"> 更新的會員密碼 </param>
        /// <returns></returns>
        public IHttpActionResult ForgetPassword([FromBody] RecvForgetPasswordDto p_RecvForgetPasswordDto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string strReturn_ = string.Empty;

                    strReturn_ = m_IMemberService.ForgetPassword(p_RecvForgetPasswordDto);

                    if (strReturn_ == string.Empty)
                    {
                        return StatusCode(HttpStatusCode.NoContent);
                    }
                    else
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, strReturn_));
                    }
                }
                else
                {
                    var _errMsg = ModelState.Values.Where(x => x.Errors.Count > 0)?.FirstOrDefault()?.Errors.Select(y => y.ErrorMessage).FirstOrDefault();
                    return Content(HttpStatusCode.BadRequest, _errMsg);
                }
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// 新增追踪清單
        /// </summary>
        /// <returns></returns>
        [Route("MemberUser/AddWish")]
        [HttpPost]
        public IHttpActionResult AddWish()
        {
            try
            {
                if (ModelState.IsValid)
                {
                    HttpRequest _request = HttpContext.Current.Request; //取得使用者要求的Request物件
                    if (string.IsNullOrWhiteSpace(_request.Form.Get("sku_id")))
                        return Content(HttpStatusCode.BadRequest, "需要參數sku_id");
                    int sku_id = Convert.ToInt32(_request.Form.Get("sku_id"));

                    //只能取自己的
                    Guid id = new Guid(User.Identity.Name);

                    string strReturn_ = string.Empty;

                    strReturn_ = m_IMemberService.AddWish(new WishReceiveDto() { Member_Id = id, Sku_Id = sku_id });

                    if (strReturn_ == string.Empty)
                    {
                        return StatusCode(HttpStatusCode.NoContent);
                    }
                    else
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, strReturn_));
                    }
                }
                else
                {
                    var _errMsg = ModelState.Values.Where(x => x.Errors.Count > 0)?.FirstOrDefault()?.Errors.Select(y => y.ErrorMessage).FirstOrDefault();
                    return Content(HttpStatusCode.BadRequest, _errMsg);
                }
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// 取得追踪清單
        /// </summary>
        /// <returns></returns>
        [Route("MemberUser/GetWish")]
        [HttpGet]
        public IHttpActionResult GetWish()
        {
            //只能取自己的
            Guid id = new Guid(User.Identity.Name);
            List<WishReturnDto> list = m_IMemberService.GetWish(new WishReceiveDto() { Member_Id = id });

            if (list.Count > 0)
            {
                return Ok(list);
            }
            else
            {
                return Ok();
                //return NotFound();
            }
        }

        [HttpDelete]
        /// <summary>
        /// 新增追踪清單(1筆/全部)               
        /// </summary>
        /// <returns></returns>               
        public IHttpActionResult DeleteWish()
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //只能取自己的
                    Guid id = new Guid(User.Identity.Name);

                    int strReturn_ = 0;

                    HttpRequest _request = HttpContext.Current.Request; //取得使用者要求的Request物件
                    if (string.IsNullOrWhiteSpace(_request.Form.Get("sku_id")))
                    {
                        strReturn_ = m_IMemberService.DeleteAllWish(new WishReceiveDto() { Member_Id = id });
                    }
                    else
                    {
                        int sku_id = Convert.ToInt32(_request.Form.Get("sku_id"));
                        strReturn_ = m_IMemberService.DeleteWish(new WishReceiveDto() { Member_Id = id, Sku_Id = sku_id });
                    }

                    return Ok($"刪除{strReturn_}筆");
                }
                else
                {
                    var _errMsg = ModelState.Values.Where(x => x.Errors.Count > 0)?.FirstOrDefault()?.Errors.Select(y => y.ErrorMessage).FirstOrDefault();
                    return Content(HttpStatusCode.BadRequest, _errMsg);
                }
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.InternalServerError, ex.Message);
            }

        }

    }
}