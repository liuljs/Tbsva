//
let methodz = '01', setUpz = '',
    insert = `
        <div class="card">
            <div class="card-header">
                <div class="row">
                    <div class="col-12 form-group py-0">
                        <h2 class="text-primary d-inline mb-0">新增法師<span class="notes">（＊號為必填欄位）</span></h2>
                    </div>
                </div>
            </div>
            <div class="card-body">
                <div class="row">
                    <div class="form-group col-md-3">
                        <label>戒牒號碼<span class="markz"></span></label>
                        <input type="text" class="form-control addIdNumberz" placeholder="戒牒號碼">
                    </div>
                    <div class="form-group col-md-3">
                        <label>法師名稱<span class="markz"></span></label>
                        <input type="text" class="form-control addTitlez" placeholder="法師名稱（建議字數在 6 個字元內）">
                    </div>
                    <!-- <div class="form-group col-md-2">
                        <label>位階<span class="markz"></span></label>
                        <input type="text" class="form-control addSubTitlez" placeholder="位階">
                    </div> -->
                    <!-- <div class="form-group col-md-2">
                        <label>位階<span class="markz"></span></label>
                        <select class="form-control custom-select classifies addSubTitlez" id="inputGroupSelect01">
                            <option value="preset">選擇位階</option>
                            <option value="法師">法師</option>
                            <option value="教授師">教授師</option>
                            <option value="講師">講師</option>
                            <option value="助教">助教</option>
                        </select>
                    </div> -->
                    <div class="form-group col-md-6">
                        <label>備註內容</label>
                        <input type="text" class="form-control addNotez" placeholder="備註內容（建議字數在 100 個字元以內）">
                    </div>
                </div>
            </div>
            <div class="card-footer">
                <div class="row">
                    <div class="form-group col-12 text-right align-self-end">
                        <button type="button" class="btn btn-primary btnAddz"><i class="fad fa-file-plus"></i> 新增</button>
                    </div>
                </div>
            </div>
        </div>
    `;
// 宣告要帶入的欄位
let list = $('.list');
let categoryz = '法師', enabled = 1, topz = 0;
// 驗證
function dataInsertCheck(aId, number, title, category, note) {
    if (aId.trim() === '') {
        return check = false, errorText = '請確認必填欄位皆有填寫！';
    }
    if (number.val().trim() === '' || number.val().trim().length > 40) {
        number.focus();
        $(document).scrollTop(0); // 置頂
        return check = false, errorText = '請確認戒牒號碼欄位有確實填寫、格式是否正確（長度不得超過 40 個字元）！';
    }
    if (title.val().trim() === '' || title.val().trim().length > 40) {
        title.focus();
        return check = false, errorText = '請確認名稱欄位有確實填寫、格式是否正確（長度不得超過 40 個字元）！';
    }
    if (category === '') {
        category.focus();
        return check = false, errorText = '請確認位階欄位有確實填寫、格式是否正確（長度不得超過 40 個字元）！';
    }
    if (note.val().trim().length > 100) {
        note.focus();
        return check = false, errorText = '請確認備註欄位格式是否正確（長度不得超過 40 個字元）！';
    }
    else {
        return check = true, errorText = "";
    };
};
function dataUpdateCheck(aId, number, title, category, note, sort) {
    if (aId.trim() === '') {
        return check = false, errorText = '請確認必填欄位皆有填寫！';
    }
    if (number.val().trim() === '' || number.val().trim().length > 40) {
        number.focus();
        $(document).scrollTop(0); // 置頂
        return check = false, errorText = '請確認戒牒號碼欄位有確實填寫、格式是否正確（長度不得超過 40 個字元）！';
    }
    if (title.val().trim() === '' || title.val().trim().length > 40) {
        title.focus();
        return check = false, errorText = '請確認名稱欄位有確實填寫、格式是否正確（長度不得超過 40 個字元）！';
    }
    if (category === '') {
        category.focus();
        return check = false, errorText = '請確認位階欄位有確實填寫、格式是否正確（長度不得超過 40 個字元）！';
    }
    if (note.val().trim().length > 100) {
        note.focus();
        return check = false, errorText = '請確認備註欄位格式是否正確（長度不得超過 40 個字元）！';
    }
    if (sort.val().trim() === '') {
        sort.focus();
        return check = false, errorText = '請確認排序欄位有確實填寫！';
    }
    else {
        return check = true, errorText = "";
    }
};
CONNECT = 'knowledge2';
// 接收資料，做渲染、處理
function process(data) {
    html = "";
    for (let i = 0; i < data.length; i++) {
        html += `
            <tr data-numz="${data[i].knowledgetId}">
                <td data-title="編號" class="text-center">
                    <div>${data[i].id}</div>
                </td>
                <td data-title="戒牒號碼" class="fieldLefts">
                    <div class="fullFields">
                        <input type="text" class="form-control inputStatus editIdNumberz" value="${data[i].number}" disabled>
                    </div>
                </td>
                <td data-title="法師名稱" class="fieldLefts">
                    <div class="fullFields">
                        <input type="text" class="form-control inputStatus editTitlez" value="${data[i].title}" placeholder="人員名稱（建議字數在 6 個字元內）"disabled>
                    </div>
                </td>
                <td data-title="備註內容" class="fieldLefts">
                    <div class="fullFields">
                        <input type="text" class="form-control inputStatus editNotez" value="${data[i].content}" placeholder="備註內容（建議字數在 100 個字元以內）" disabled>
                    </div>
                </td>
                <td data-title="排序" class="text-center fieldSorts fieldLefts">
                    <div class="fullFields">
                        <input type="text" class="form-control text-center inputStatus editSortz numberz" value="${data[i].sort}" disabled>
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
        // 顯示狀態
        if ($('.statusz').eq(i).val() == "1") {
            $('.statusz').eq(i).prop('checked', true);
        } else {
            $('.statusz').eq(i).prop('checked', false);
        };
    };
    // 資料載入完成後
    loadingCompleted();
};
// NOTFOUND
function fails() {
    html = "";
    html = `
        <tr class="none">
            <td colspan="7" class="none">
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
        "CONNECTs": `${CONNECT}?category=${categoryz}`,
        "QUERYs": `${CONNECT}?category=${categoryz}&count=${listSize}&page=${pageRcd}`,
        "Counts": listSize,
        "Sends": "",
    };
    // 產生第一次的分頁器
    getTotalPages(dataObj).then(res => {
        if (res !== 0) {
            pageLens = res;
            paginations.find('.pagez').html(curPage(current, res, 3));
            // 點擊頁碼
            paginations.unbind('click').on('click', 'li', function (e) {
                e.preventDefault(), e.stopImmediatePropagation();
                let num = $(this).find('a').data('num');
                if (isNaN(num)) {
                    if (!$(this).hasClass("disabled")) {
                        if (num == "prev") {
                            pageRcd--;
                        } else if (num == "next") {
                            pageRcd++;
                        };
                        // 1. 紀錄當下頁碼
                        pageRcd = pageRcd;
                        // 2. 條件
                        dataObj.QUERYs = `${CONNECT}?category=${categoryz}&count=${listSize}&page=${pageRcd}`;
                        // 3. 取得點擊頁碼後要呈現的內容(要呈現的筆數)
                        getPageDatas(dataObj).then(res => {
                            // DO SOMETHING
                            if (res !== null) {
                                process(res);
                                // 4. 產生新分頁器
                                paginations.find('.pagez').html(curPage(pageRcd, pageLens, pageCount));
                            } else {
                                fails();
                            };
                            $('html,body').scrollTop(0);
                        }, rej => {
                            if (rej == "NOTFOUND") {
                                alert("錯誤訊息 " + xhr.status + "：您的連線已逾期，請重新登入！");
                                location.reload();
                            };
                        });
                    };
                } else {
                    if (num !== pageRcd) { // 如果不是點同一頁碼的話
                        // 1. 記錄當下頁碼
                        pageRcd = num;
                        // 2. 條件 
                        dataObj.QUERYs = `${CONNECT}?category=${categoryz}&count=${listSize}&page=${num}`;
                        // 3. 取得點擊頁碼後要呈現的內容(要呈現的筆數)
                        getPageDatas(dataObj).then(res => {
                            // DO SOMETHING
                            if (res !== null) {
                                process(res);
                                // 4. 產生新分頁器
                                paginations.find('.pagez').html(curPage(num, pageLens, pageCount));
                            } else {
                                fails();
                            };
                            $('html,body').scrollTop(0);
                        }, rej => {
                            if (rej == "NOTFOUND") {
                                alert("錯誤訊息 " + xhr.status + "：您的連線已逾期，請重新登入！");
                                location.reload();
                            };
                        });
                    } else { };
                };
            });
            paginations.find('.pagez li:first-child').trigger('click');
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
    $(document).on('click', '.btnAddz', function (e) {
        e.preventDefault();
        dataInsertCheck(idz, $('.addIdNumberz'), $('.addTitlez'), categoryz, $('.addNotez'));
        if (check == true) {
            let dataObj = {
                "number": $('.addIdNumberz').val().trim(),
                "title": $('.addTitlez').val().trim(),
                "category": categoryz,
                "content": $('.addNotez').val().trim() !== "" ? $('.addNotez').val().trim() : "-",
                "enabled": enabled,
                "first": topz
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
                xhr.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded');
                xhr.send($.param(dataObj));
            };
        } else {
            alert(errorText);
        };
    });
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
            let orgIdNumber = trz.find('.editIdNumberz').val(), orgTitle = trz.find('.editTitlez').val(), orgNote = trz.find('.editNotez').val(), orgStatus = trz.find('.statusz').val(), orgSort = trz.find('.editSortz').val();
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
                dataUpdateCheck(idz, trz.find('.editIdNumberz'), trz.find('.editTitlez'), categoryz, trz.find('.editNotez'), trz.find('.editSortz'));
                if (check == true) {
                    let dataObj = {
                        "knowledgetId": numz,
                        "number": trz.find('.editIdNumberz').val().trim(),
                        "title": trz.find('.editTitlez').val().trim(),
                        "category": categoryz,
                        "content": trz.find('.editNotez').val().trim() !== "" ? trz.find('.editNotez').val().trim() : "-",
                        "sort": trz.find('.editSortz').val().trim(),
                        "enabled": trz.find('.statusz').val().trim(),
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
                                // location.reload();
                            };
                        };
                        xhr.open('PUT', `${URL}${CONNECT}/${numz}`, true);
                        xhr.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded');
                        xhr.send($.param(dataObj));
                    };
                } else {
                    alert(errorText);
                };
            })
            // Delete
            trz.find('.btnDelz').on('click', function (e) {
                e.preventDefault();
                let dataObj = {
                    "knowledgetId": numz
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
                    trz.find('.editIdNumberz').val(orgIdNumber), trz.find('.editTitlez').val(orgTitle), trz.find('.editNotez').val(orgNote), trz.find('.editSortz').val(orgSort), trz.find('.statusz').val(orgStatus);
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
});