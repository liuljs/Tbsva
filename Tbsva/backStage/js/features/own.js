//
let methodz = '01', setUpz = '',
    insert = `
        <div class="card">
            <div class="card-header">
                <div class="row">
                    <div class="form-group py-0">
                        <h2 class="text-primary d-inline mb-0">新增管理者</h2>
                        <span class="notes">（以下皆必填欄位）</span>
                    </div>
                </div>
            </div>
            <div class="card-body">
                <div class="row">
                    <div class="form-group col-md-4">
                        <label>管理者帳號<span class="markz"></span></label>
                        <input type="text" class="form-control addIdz" placeholder="登入用帳號（建議字數在 20 個字元內）">
                    </div>
                    <div class="form-group col-md-4">
                        <label>名字<span class="markz"></span></label>
                        <input type="text" class="form-control addNamez" placeholder="名子或暱稱（建議字數在 20 個字元內）">
                    </div>
                    <div class="form-group col-md-4">
                        <label>電子信箱<span class="notes">（密碼將再新增成功後寄至信箱）</span><span class="markz"></span></label>
                        <input type="text" class="form-control addMailz">
                    </div>
                </div>
            </div>
            <div class="card-footer">
                <div class="row">
                    <div class="form-group col-12">
                        <div class="btns justify-content-end">
                            <button type="button" class="btn btn-primary btnAddz"><i class="far fa-user-tie"></i> 新增</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    `;
// 宣告要帶入的欄位
let list = $('.list');
// 驗證
function dataUpdateCheck(aId, id, name, mail) {
    if (aId.trim() === '') {
        id.focus();
        return check = false, errorText = '請確認必填欄位皆有填寫！';
    }
    if (id.val().trim() === '' || id.val().trim().length > 20) {
        id.focus();
        return check = false, errorText = '請確認使用者ID是否確實填寫、格式是否正確（長度不得超過 20 個字元）！';
    }
    if (name.val().trim() === '' || name.val().trim().length > 20) {
        name.focus();
        return check = false, errorText = '請確認使用者名稱是否確實填寫、格式是否正確（長度不得超過 20 個字元）！';
    }
    if (mail.val().trim() === '' || EmailRegExp.test(mail.val()) === false) {
        mail.focus();

        return check = false, errorText = '請確認信箱是否確實填寫、格式是否正確！';
    }
    else {
        return check = true, errorText = "";
    }
};
function manPswCheck(psw) {
    if (psw.val().trim() === '' || Rules.test(psw.val()) === false) {
        psw.focus();
        return check = false, errorText = '請確認新密碼是否確實填寫，或格式是否正確！';
    }
    else {
        return check = true, errorText = "";
    }
};
$().ready(function () {
    let xhr = new XMLHttpRequest();
    xhr.onload = function () {
        if (xhr.status == 200) {
            let callBackData = JSON.parse(xhr.responseText);
            if (callBackData.Result == "OK") {
                // 渲染出所有的管理者帳號
                for (let i = 0; i < callBackData.Content.length; i++) {
                    html += `
                        <tr data-numz="${callBackData.Content[i].id}">
                            <td data-title="管理者帳號">
                                <div class="btnThirds d-flex justify-content-between align-items-center">
                                    <p class="mb-0 accounts accountz">${callBackData.Content[i].account}</p>
                                </div>
                            </td>
                            <td data-title="名稱" class="fieldLeft">
                                <div class="fullFields">
                                    <input type="text" class="form-control inputStatus namez" value="${callBackData.Content[i].name}" disabled>
                                </div>
                            </td> 
                            <td data-title="電子信箱" class="fieldLeft">
                                <div class="fullFields">
                                    <input type="email" class="form-control inputStatus mailz" value="${callBackData.Content[i].email}" disabled>
                                </div>
                            </td>
                            <td class="attitude" data-title="狀態">
                                <div class="custom-control custom-switch">
                                    <input type="checkbox" class="custom-control-input inputStatus statusz" id="customSwitch${i}" value="${callBackData.Content[i].enabled}" disabled>
                                    <label class="custom-control-label" for="customSwitch${i}">開啟</label>
                                </div>
                            </td>
                            <td data-title="設定">
                                <div class="btns">${setUpz}</div>
                            </td>
                        </tr>
                    `;
                };
                list.html(html);
                // 管理者帳號的狀態顯示
                let chk = $('.statusz');
                for (let i = 0; i < chk.length; i++) {
                    if (chk.eq(i).val() == "1") {
                        chk.eq(i).prop('checked', true);
                    } else {
                        chk.eq(i).prop('checked', false);
                    };
                };
                // Edit 取得 window 物件上所有編輯按鈕（靜態、第一次產生、第N次動態產生）
                $(document).on('click', '.btnEditz', function (e) {
                    e.preventDefault(), e.stopPropagation();// 取消 a 預設事件 // 取消冒泡 Bubble 事件
                    let trz = $(this).parents('tr'); // 宣告當下欄位
                    if ($('td .btns').hasClass('active')) { // 如果有欄位在編輯狀態的話，其他欄位就不能用
                        alert('請確認您是否有其他的內容尚未儲存！');
                    } else {
                        // 原資料
                        let orgMail = trz.find('.mailz').val(), orgName = trz.find('.namez').val(), orgStatus = trz.find('.statusz').val();

                        // 點擊編輯後 -> 1.啟用欄位的編輯 2.動態產生儲存、刪除
                        trz.find('input.inputStatus').prop('disabled', false); // 啟用欄位的編輯
                        // 1.新增一個判斷更動與否的class 2.動態產生 儲存、刪除
                        trz.find('.btns').addClass('active').html('').append(`
                                <a class="btn btn-success btn-sm btnSavez" href="#"><i class="fal fa-save"></i> 存儲</a> 
                                <a class="btn btn-danger btn-sm btnDelz" href="#"><i class="fas fa-trash"></i> 刪除</a>
                                <a class="btn btn-sm btnCancel btnAccCancelz" href="#"><i class="far fa-window-close"></i> 取消</a>
                        `);
                        // Delete 當點擊編輯按鈕後，動態產生刪除按鈕
                        trz.find('.btnDelz').on('click', function (e) {
                            e.preventDefault(); // 取消 a 預設事件
                            let numz = trz.data('numz');
                            let dataObj = { // Post 物件
                                "account_id": idz,
                                "id": numz
                            };
                            if (confirm("您確定要刪除嗎？")) {
                                let xhr = new XMLHttpRequest();
                                xhr.onload = function () {
                                    if (xhr.status == 200) {
                                        let callBackData = JSON.parse(xhr.responseText);
                                        if (callBackData.Result == "OK") {
                                            alert('刪除成功！');
                                            location.reload();
                                        }
                                    } else {
                                        alert("錯誤訊息 " + xhr.status + "：您的連線異常，請重新登入！");
                                        location.reload();
                                    };
                                };
                                xhr.open('POST', `${URL}Manager/Delete`, true)
                                xhr.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded');
                                xhr.send($.param(dataObj));
                            }
                        });
                        // 管理者帳號的狀態控制
                        trz.find('.statusz').on('change', function () {
                            if ($(this).prop('checked') == true) {
                                $(this).val("1");
                            } else {
                                $(this).val("0");
                            }
                        });
                        // Save 當點擊編輯按鈕後，動態產生儲存按鈕
                        trz.find('.btnSavez').on('click', function (e) {
                            e.preventDefault(), e.stopImmediatePropagation(); // 取消 a 預設事件 || 取消捕獲 Capture 事件
                            dataUpdateCheck(idz, trz.find('.accountz'), trz.find('.namez'), trz.find('.mailz'));
                            if (check == true) {
                                let dataObj = {
                                    "account_id": trz.data('numz'),
                                    "account": trz.find('.accountz').val(),
                                    "name": trz.find('.namez').val(),
                                    "email": trz.find('.mailz').val(),
                                    "Enabled": trz.find('.statusz').val()
                                };
                                if (confirm("您確定要進行修改嗎（會在下次登入生效）?")) {
                                    let xhr = new XMLHttpRequest();
                                    xhr.onload = function () {
                                        if (xhr.status == 200) {
                                            let callBackData = JSON.parse(xhr.responseText);
                                            if (callBackData.Result == "OK") {
                                                alert('修改成功！');
                                                // 點擊儲存後 -> 1.禁用欄位的編輯 2.動態產生編輯
                                                if ($(this)) {
                                                    trz.find('input.inputStatus').prop('disabled', true); // 禁用欄位的編輯
                                                    // 1.移除判斷更動的class 2.動態產生編輯
                                                    trz.find('.btns').removeClass('active').html('').append(`
                                                        <a class="btn btn-warning btn-sm btnEditz" href="#"><i class="far fa-edit"></i> 編輯</a> 
                                                    `);
                                                };
                                                // 重新載入
                                                location.reload();
                                            } else {
                                                alert(callBackData.Content);
                                            }
                                        } else {
                                            alert("錯誤訊息 " + xhr.status + "：您的連線異常，請重新登入！");
                                            location.reload();
                                        };
                                    };
                                    xhr.open('POST', `${URL}Manager/Update`, true);
                                    xhr.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded');
                                    xhr.send($.param(dataObj));
                                };
                            } else {
                                alert(errorText);
                            }
                        });
                        // 顯示變更密碼
                        trz.find('.btnThirds').append(`
                            <a href="#" class="text-primary btnThird btnOpenPswz" data-toggle="modal" data-target="#manPswEdit">［密碼］</a>
                        `);
                        // 更新會員密碼
                        trz.find('.btnOpenPswz').on('click', function () {
                            $('.managerIdz').val(trz.data('numz'));
                            // 
                            $('.btnChangeManPswz').on('click', function () {
                                let trz = $(this).parents('.modal-content');
                                manPswCheck(trz.find('.newPasswordz'));
                                if (check == true) {
                                    let dataObj = {
                                        "id": trz.find('.managerIdz').val(),
                                        "new_password": trz.find('.newPasswordz').val(),
                                        "new_password_again": trz.find('.agnPasswordz').val()
                                    };
                                    if (confirm("您確定要進行修改嗎（會在下次登入生效）?")) {
                                        let xhr = new XMLHttpRequest();
                                        xhr.onload = function () {
                                            if (xhr.status == 200 || xhr.status == 204) {
                                                alert("修改成功！");
                                                location.reload();
                                            } else {
                                                alert("錯誤訊息 " + xhr.status + "：您的連線異常，請重新登入！");
                                                location.reload();
                                            };
                                        };
                                        xhr.open('POST', `${URL}Manager/ResetPassword`, true);
                                        xhr.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded');
                                        xhr.send($.param(dataObj));
                                    }
                                } else {
                                    alert(errorText);
                                };
                            });
                            // Cancel
                            $('.btnCancelz, .close').unbind('click').on('click', function (e) {
                                e.preventDefault(), e.stopPropagation();
                                let trz = $(this).parents('.modal-content');
                                if (confirm("尚未儲存內容，確定要取消嗎？")) {
                                    // 清空欄位資料
                                    trz.find('input[type="password"]').val('');
                                    $('.closez').trigger('click');
                                };
                            });
                        });
                        // Cancel
                        trz.find('.btnAccCancelz').on('click', function (e) {
                            e.preventDefault();
                            if (confirm("尚未儲存更改的內容，確定要取消嗎？")) {
                                // 還原欄位資料
                                trz.find('.mailz').val(orgMail);
                                trz.find('.namez').val(orgName);
                                trz.find('.statusz').val(orgStatus);
                                if (trz.find('.statusz').val() == "1") {
                                    trz.find('.statusz').prop('checked', true);
                                } else {
                                    trz.find('.statusz').prop('checked', false);
                                };
                                // 轉為編輯按鈕
                                trz.find('.btns').removeClass('active').html('').append(`
                                    <a class="btn btn-warning btn-sm btnEditz" href="#"><i class="far fa-edit"></i> 編輯</a>
                                `);
                                //
                                trz.find('.btnThirds .btnOpenPswz').remove();

                                // 權限欄位的禁用、啟用
                                trz.find('.inputStatus').prop('disabled', true);
                            };
                        });
                    };
                });
            } else {
                alert(callBackData.Content);
            };
            // 資料載入完成後
            loadingCompleted();
        } else { };
    };
    xhr.open('POST', `${URL}Manager/Get`, true);
    xhr.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded');
    xhr.send(null);

    // 新增管理者
    $(document).on('click', '.btnAddz', function () {
        dataUpdateCheck(idz, $('.addIdz'), $('.addNamez'), $('.addMailz'));
        if (check == true) { // 驗證通過的話，將資料放入 Post 物件
            let dataObj = {
                "account_id": idz,
                "account": $('.addIdz').val(),
                "name": $('.addNamez').val(),
                "email": $('.addMailz').val()
            };
            if (confirm("您確定要新增嗎？")) {
                let xhr = new XMLHttpRequest();
                xhr.onload = function () {
                    if (xhr.status == 200) {
                        let callBackData = JSON.parse(xhr.responseText);
                        if (callBackData.Result == "OK") {
                            alert('新增成功！');
                            location.reload();
                        } else {
                            alert(callBackData.Content);
                        }
                    } else {
                        alert("錯誤訊息 " + xhr.status + "：您的連線異常，請重新登入！");
                        location.reload();
                    };
                };
                xhr.open('POST', `${URL}Manager/Insert`, true);
                xhr.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded');
                xhr.send($.param(dataObj));
            }
        } else {
            alert(errorText);
        };
    });
});