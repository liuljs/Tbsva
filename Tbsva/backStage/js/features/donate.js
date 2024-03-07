//
let methodz = '01', setUpz = '',
    insert = `
        <div class="card">
            <div class="card-header">
                <div class="row">
                    <div class="col-12 form-group py-0">
                        <h2 class="text-primary d-inline mb-0">新增捐款項目<span class="notes">（＊號為必填欄位）</span></h2>
                    </div>
                </div>
            </div>
            <div class="card-body">
                <div class="row">
                    <div class="form-group col-md-4">
                        <label>項目類別<span class="markz"></span></label>
                        <select class="form-control custom-select classifies addClassifyz" id="inputGroupSelect01">
                            <option value="preset">選擇類別</option>
                            <option value="1">供佛齋僧</option>
                            <option value="2">經書助印</option>
                            <option value="3">慈善公益</option>
                            <option value="4">繳交年費</option>
                        </select>
                    </div>
                    <div class="form-group col-md-4">
                        <label>項目名稱<span class="markz"></span></label>
                        <input type="text" class="form-control addTitlez" placeholder="項目名稱（建議字數在 20 個字元內）">
                    </div>
                    <div class="form-group col-md-4">
                        <label>項目金額<span class="notes">（單位：新台幣）</span></label>
                        <div class="fieldUnits units">
                            <input type="text" class="form-control numberz addAmountz" placeholder="填寫 0 元或不填，則改為由捐款方自由捐款">
                            <span>元</span>
                        </div>
                    </div>
                </div>
            </div>
            <div class="card-footer">
                <div class="row">
                    <div class="form-group col-12 text-right align-self-end">
                        <button type="submit" class="btn btn-primary btnAddz">
                            <i class="fad fa-file-plus"></i> 新增
                        </button>
                    </div>
                </div>
            </div>
        </div>
    `;
// 宣告要帶入的欄位
let list = $('.list');
let donateType = 1, enabled = 1, topz = 0; // 主類別為一般捐款 狀態預設為顯示 置頂預設為不置頂 
// 驗證
function dataUpdateCheck(aId, classify, title) {
    if (aId.trim() === '') {
        return check = false, errorText = '請確認必填欄位皆有填寫！';
    }
    if (classify.val().trim() === '') {
        classify.focus();
        return check = false, errorText = '請確認類別欄位有確實填寫！';
    }
    if (title.val().trim() === '' || title.val().trim().length > 40) {
        title.focus();
        return check = false, errorText = '請確認名稱欄位有確實填寫、格式是否正確（長度不得超過 40 個字元）！';
    }
    else {
        return check = true, errorText = "";
    }
};
CONNECT = 'DonateRelatedItem';
// 接收資料，做渲染、處理
function process(data) {
    html = "";
    for (let i = 0; i < data.length; i++) {
        html += `
            <tr data-numz="${data[i].donateRelatedItemId}" data-sortz="${data[i].sort}">
                <td data-title="編號">
                    <div>${data[i].id}</div>
                </td>
                <td data-title="項目類別" class="fieldLefts">
                    <div class="fullFields">
                        <select class="form-control custom-select classifies inputStatus editClassifyz" id="inputGroupSelect01" disabled>
                            <option value="preset">選擇類別</option>
                            <option value="1">供佛齋僧</option>
                            <option value="2">經書助印</option>
                            <option value="3">慈善公益</option>
                            <option value="4">繳交年費</option>
                        </select>
                    </div>
                </td>
                <td data-title="項目名稱" class="fieldLefts">
                    <div class="fullFields">
                        <input type="text" class="form-control inputStatus editTitlez" value="${data[i].title}" placeholder="項目名稱（建議字數在 20 個字元內）" disabled>
                    </div>
                </td>
                <td data-title="項目金額" class="fieldLefts">
                    <div class="fieldUnits units">
                        <input type="text" class="form-control inputStatus numberz editAmountz text-right" value="${data[i].amount}" disabled>
                        <span>元</span>
                    </div>
                </td>
                <td data-title="狀態">
                    <div class="form-check pr-0">
                        <div class="custom-control custom-switch">
                            <input type="checkbox" class="custom-control-input inputStatus statusz" id="${i}switch" value="${data[i].enabled}" disabled>
                            <label class="custom-control-label" for="${i}switch">開啟</label>
                        </div>
                    </div>
                </td>
                <td data-title="功能">
                    <div class="btns">${setUpz}</div>
                </td>
            </tr>
        `;
    };
    list.html(html);
    //
    for (let i = 0; i < data.length; i++) {
        // 類別顯示
        $('.editClassifyz').eq(i).val(data[i].secondary !== undefined ? data[i].secondary : "preset");
        // 顯示狀態
        if ($('.statusz').eq(i).val() == "1") {
            $('.statusz').eq(i).prop('checked', true);
        } else {
            $('.statusz').eq(i).prop('checked', false);
        };
    }
    // Edit
    $(document).on('click', '.btnEditz', function (e) {
        e.preventDefault(), e.stopPropagation();
        let trz = $(this).parents('tr');
        let numz = trz.data('numz');
        if ($('.btns').hasClass('active')) {
            alert('請確認您是否有其他的內容尚未儲存！');
        }
        else {
            // Setup
            trz.find('.btns').addClass('active').html('').append(`
                <a class="btn btn-success btn-sm btnSavez" href="#"><i class="fal fa-save"></i> 存儲</a> 
                <a class="btn btn-danger btn-sm btnDelz" href="#"><i class="fas fa-trash"></i> 刪除</a>
                <a class="btn btn-sm btnCancel btnCancelz" href="#"><i class="far fa-window-close"></i> 取消</a>
            `);
            // Original
            let orgClassify = trz.find('.editClassifyz').val(), orgTitle = trz.find('.editTitlez').val(), orgAmount = trz.find('.editAmountz').val(), orgStatus = trz.find('.statusz').val();
            // Input Enable
            trz.find('.inputStatus').prop('disabled', false);
            // 顯示狀態控制
            trz.find('.statusz').on('change', function () {
                if ($(this).prop('checked') == true) {
                    $(this).val("1");
                } else {
                    $(this).val("0");
                }
            });
            // Save
            trz.find('.btnSavez').on('click', function (e) {
                e.preventDefault();
                let dataObj = {
                    "donateRelatedItemId": numz,
                    "primary": donateType,
                    "secondary": trz.find('.editClassifyz').val().trim(),
                    "title": trz.find('.editTitlez').val().trim(),
                    "amount": trz.find('.editAmountz').val().trim(),
                    "enabled": trz.find('.statusz').val().trim(),
                    "sort": trz.data('sortz'),
                    "first": topz
                };
                if (confirm('您確定要修改嗎？')) {
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
            })
            // Delete
            trz.find('.btnDelz').on('click', function (e) {
                e.preventDefault();
                let dataObj = {
                    "donateRelatedItemId": numz
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
                };
            });
            // Cancel
            trz.find('.btnCancelz').on('click', function (e) {
                e.preventDefault();
                if (confirm("您尚未儲存內容，確定要取消嗎？")) {
                    // 還原欄位資料
                    trz.find('.editClassifyz').val(orgClassify), trz.find('.editTitlez').val(orgTitle), trz.find('.editAmountz').val(orgAmount), trz.find('.statusz').val(orgStatus);
                    if (trz.find('.statusz').val() == "1") {
                        trz.find('.statusz').prop('checked', true);
                    } else {
                        trz.find('.statusz').prop('checked', false);
                    };
                    // 轉為編輯按鈕
                    trz.find('.btns').removeClass('active').html('').append(`
                        <a class="btn btn-sm btn-warning btnEditz" href="#"><i class="far fa-edit"></i> 編輯</a>
                    `);
                    // 權限欄位的禁用、啟用
                    trz.find('.inputStatus').prop('disabled', true);
                }
            });
        };
    });
    // 資料載入完成後
    loadingCompleted();
};
// NOTFOUND
function fails() {
    html = "";
    html = `
        <tr class="none">
            <td colspan="6" class="none">
                <span>目前沒有任何的內容。</span>
            </td>
        </tr> 
    `;
    list.html(html);
    // 資料載入完成後
    loadingCompleted();
};
$().ready(function () {
    //
    let dataObj = {
        "Methods": "GET",
        "APIs": URL,
        "CONNECTs": `${CONNECT}?primary=${donateType}`,
        "QUERYs": "",
        "Sends": "",
        "Counts": ""
    };
    getPageDatas(dataObj).then(res => {
        if (res !== null) {
            // DOSOMETHING
            process(res);
        } else {
            fails();
        };
    }, rej => {
        if (rej == "NOTFOUND") { };
    });
    //
    $(document).on('click', '.btnAddz', function (e) {
        e.preventDefault();
        // 
        dataObj.CONNECTs = `${CONNECT}`;
        dataUpdateCheck(idz, $('.addClassifyz'), $('.addTitlez'));
        if (check == true) {
            let dataObj = {
                "primary": donateType,
                "secondary": $('.addClassifyz').val().trim(),
                "title": $('.addTitlez').val().trim(),
                "amount": $('.addAmountz').val().trim() !== "" ? $('.addAmountz').val().trim() : 0,
                "first": topz,
                "enabled": enabled
            };
            if (confirm("您確定要新增嗎？")) {
                let xhr = new XMLHttpRequest();
                xhr.onload = function () {
                    if (xhr.status == 200 || xhr.status == 201) {
                        alert('新增成功！');
                        location.reload();
                    } else {
                        alert("錯誤訊息 " + xhr.status + "：您的連線異常，請重新登入！");
                        location.reload();
                    };
                };
                xhr.open('POST', `${URL}${CONNECT}`, true);
                // xhr.setRequestHeader('Content-Type', 'multipart/form-data');
                xhr.send(dataObj);
            };
        } else {
            alert(errorText);
        };
    });
});