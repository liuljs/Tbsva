//
let methodz = '01', setUpz = '',
    insert = `
        <div class="card">
            <div class="card-header">
                <div class="row">
                    <div class="col-12 form-group py-0">
                        <h2 class="text-primary d-inline mb-0">新增主分類</h2>
                        <span class="notes">（＊號為必填欄位）</span>
                    </div>
                </div>
            </div>
            <div class="card-body">
                <div class="row">
                    <div class="form-group col-12">
                        <label>主分類名稱<span class="markz"></span></label>
                        <input type="text" class="form-control primNamez" placeholder="分類名稱（建議字數在 10 個字元內）">
                    </div>
                </div>
            </div>
            <div class="card-footer">
                <div class="row">
                    <div class="form-group col-12">
                        <div class="btns justify-content-end">
                            <button type="button" class="btn btn-primary btnAddPrimz">
                                <i class="fad fa-file-plus"></i> 新增
                            </button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    `;
// 宣告要帶入的欄位
let primaries = $('.primaries');

let edits = $('.edits'), edits_content = $('.edits_content');
let list = $('.list');
let secMax = 20;
let secs = "";
let minors = $('.minors');
let mins;
let clsMinArr = new Array(), clsAddArr = new Array(), clsDelArr = new Array();
let editaction, addClsId;
let enabled = '1';

CONNECT = 'articleCategory';
// 接收資料，做渲染、處理
function process(data) {
    html = "";
    for (let i = 0; i < data.length; i++) {
        html += `
            <div data-numz="${data[i].id}" class="card">
                <div class="card-header">
                    <div class="row">
                        <div class="form-group col-12 py-0 d-flex justify-content-between align-items-center">
                            <h4 class="text-info d-inline mb-0">
                                <span>主分類 ${data[i].id}</span>
                            </h4>
                            <div class="custom-control custom-switch">
                                <input type="checkbox" id="primSwitch${i}" class="custom-control-input inputStatus statusz" value="${data[i].enabled}" disabled>
                                <label class="custom-control-label" for="primSwitch${i}">開啟</label>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="card-body">
                    <div class="row sorts">
                        <div class="sort form-group col-md-8 d-flex align-items-center">
                            <label class="mr-2">主分類名稱<span class="markz"></span></label>
                            <input type="text" class="form-control inputStatus titlez" value="${data[i].name}" placeholder="分類名稱（建議字數在 10 個字元內）" disabled>
                        </div>
                        <div class="sort form-group col-md-4 d-flex justify-content-end">
                            <label class="mr-2 col-form-label">排序</label>
                            <input type="text" class="form-control text-right inputStatus sortz" value="${data[i].sort}" disabled>
                        </div>
                    </div>
                </div>
                <div class="card-footer">
                    <div class="row">
                        <div class="form-group col-sm-12">
                            <div class="btns justify-content-end">${setUpz}</div>
                        </div>
                    </div>
                </div>
            </div>
        `;
    };
    list.html(html);
    // 資料載入完成後
    loadingCompleted();
};
// NOTFOUND
function fails() {
    // 資料載入完成後
    loadingCompleted();
};
// 驗證
function dataInsertCheck(aId, title) {
    if (aId.trim() === '') {
        return check = false, errorText = '請確認必填欄位皆有填寫！';
    }
    if (title.val().trim() === '' || title.val().length > 20) {
        title.focus();
        return check = false, errorText = '請確認分類名稱是否確實填寫、格式使否正確（長度不得超過 20 字元）！';
    }
    else {
        return check = true, errorText = "";
    };
};
function dataUpdateCheck(aId, title, sort) {
    if (aId.trim() === '') {
        return check = false, errorText = '請確認必填欄位皆有填寫！';
    }
    if (title.val().trim() === '' || title.val().length > 20) {
        title.focus();
        return check = false, errorText = '請確認分類名稱是否確實填寫、格式使否正確（長度不得超過 20 字元）！';
    }
    if (sort.val().trim() === '') {
        sort.focus();
        return check = false, errorText = '請確認分類排序是否確實填寫！';
    }
    else {
        return check = true, errorText = "";
    }
};
$().ready(function () {
    //
    let dataObj = {
        "Methods": "GET",
        "APIs": URL,
        "CONNECTs": `${CONNECT}`,
        "QUERYs": "",
        "Counts": "",
        "Sends": "",
    };
    //
    getPageDatas(dataObj).then(res => {
        // DOSOMETHING
        if (res !== null) {
            process(res);
            // 主分類、次分類狀態顯示
            let chk = $('.statusz');
            for (let i = 0; i < chk.length; i++) {
                if (chk.eq(i).val() == "1") {
                    chk.eq(i).prop('checked', true);
                } else {
                    chk.eq(i).prop('checked', false);
                };
            };
            // Edit Primary
            $(document).on('click', '.btnEditz', function (e) {
                e.preventDefault(), e.stopPropagation();
                let trz = $(this).parents('.card');
                let numz = trz.data('numz');
                if ($('.card-footer .btns').hasClass('active')) {
                    alert('請確認您是否有其他的內容尚未儲存！');
                } else {
                    // 欄位
                    trz.find('.card-footer .btns').addClass('active').html('').append(`
                        <a class="btn btn-sm btn-success btnSavez" href="#"><i class="fal fa-save"></i> 儲存</a>
                        <a class="btn btn-sm btn-danger btnDelz" href="#"><i class="fas fa-trash"></i> 刪除</a>
                        <a class="btn btn-sm btnCancel btnCancelz" href="#"><i class="far fa-window-close"></i> 取消</a>
                    `);
                    // 權限欄位的禁用、啟用
                    trz.find('.inputStatus').prop('disabled', false);
                    // 原資料
                    let orgPrimName = trz.find('.titlez').val(), orgPrimSort = trz.find('.sortz').val(), orgPrimStatus = trz.find('.statusz').val();
                    // Save Primary
                    trz.find('.btnSavez').on('click', function (e) {
                        e.preventDefault();
                        dataUpdateCheck(idz, trz.find('.titlez'), trz.find('.sortz'));
                        if (check == true) {
                            let dataObj = {
                                "categoryId": numz,
                                "name": trz.find('.titlez').val().trim(),
                                "sort": trz.find('.sortz').val().trim(),
                                "enabled": trz.find('.statusz').val().trim()
                            };
                            if (confirm("您確定要儲存修改嗎？")) {
                                let xhr = new XMLHttpRequest();
                                xhr.onload = function () {
                                    if (xhr.status == 200 || xhr.status == 204) {
                                        alert("儲存成功！");
                                        location.reload();
                                    } else {
                                        alert("錯誤訊息 " + xhr.status + "：您的連線異常，請重新登入！");
                                        location.reload();
                                    };
                                };
                                xhr.open('PUT', `${URL}${CONNECT}/${numz}`, true);
                                xhr.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded');
                                xhr.send($.param(dataObj));
                            };
                        } else {
                            alert(errorText);
                        };
                    });
                    // Delete
                    trz.find('.btnDelz').on('click', function (e) {
                        e.preventDefault(); // 取消 a 預設事件
                        let dataObj = {
                            "categoryId": numz
                        };
                        if (confirm("您確定要刪除嗎？")) {
                            let xhr = new XMLHttpRequest();
                            xhr.onload = function () {
                                if (xhr.status == 200 || xhr.status == 204) {
                                    alert("刪除成功！");
                                    location.reload();
                                } else {
                                    alert("錯誤訊息 " + xhr.status + "：您的連線異常，請重新登入！");
                                    location.reload();
                                };
                            };
                            xhr.open('DELETE', `${URL}${CONNECT}/${numz}`, true)
                            xhr.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded');
                            xhr.send($.param(dataObj));
                        }
                    });
                    // Cancel
                    trz.find('.btnCancelz').on('click', function (e) {
                        e.preventDefault();
                        if (confirm("尚未儲存更改的內容，確定要取消嗎？")) {
                            // 還原欄位原本資料
                            trz.find('.titlez').val(orgPrimName), trz.find('.sortz').val(orgPrimSort), trz.find('.statusz').val(orgPrimStatus);
                            if (trz.find('.statusz').val() == "1") {
                                trz.find('.statusz').prop('checked', true);
                            } else {
                                trz.find('.statusz').prop('checked', false);
                            };
                            // 轉為編輯按鈕
                            trz.find('.card-footer .btns').removeClass('active').html('').append(`
                                <a class="btn btn-sm btn-warning btnEditz" href="#"><i class="far fa-edit"></i> 編輯</a>
                            `);
                            // 權限欄位的禁用、啟用
                            trz.find('.inputStatus').prop('disabled', true);
                        }
                    });
                };
            });
            // 分類狀態控制
            $(document).on('change', '.statusz', function () {
                if ($(this).prop('checked') == true) {
                    $(this).val("1");
                } else {
                    $(this).val("0");
                };
            });
        } else {
            fails();
        };
    }, rej => {
        if (rej == "NOTFOUND") {
            fails();
            // alert("錯誤訊息 " + xhr.status + "：您的連線已逾期，請重新登入！");
            // location.reload();
        };
    });
    // Add Prim
    $(document).on('click', '.btnAddPrimz', function () {
        dataInsertCheck(idz, $('.primNamez'));
        if (check == true) {
            let dataObj = {
                "name": $('.primNamez').val(),
                "enabled": enabled,
            }
            if (confirm("您確定要新增嗎？")) {
                let xhr = new XMLHttpRequest();
                xhr.onload = function () {
                    if (xhr.status == 200 || xhr.status == 201) {
                        alert("新增成功！");
                        location.reload();
                    } else {
                        alert("錯誤訊息 " + xhr.status + "：您的連線異常，請重新登入！");
                        location.reload();
                    };
                };
                xhr.open('POST', `${URL}${CONNECT}`, true);
                xhr.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded');
                xhr.send($.param(dataObj));
            }
        } else {
            alert(errorText);
        }
    });
});
