//
let methodz = '01', setUpz = '',
    insert = `
        <div class="card">
            <div class="card-header">
                <div class="row">
                    <div class="col-12 form-group py-0">
                        <h2 class="text-primary d-inline mb-0">新增歷程</h2>
                        <span class="notes">（以下除「導覽圖」外皆必填欄位）</span>
                    </div>
                </div>
            </div>
            <div class="card-body">
                <div class="row">
                    <div class="form-group col-md-12">
                        <label>歷程標題<span class="markz"></span></label>
                        <input type="text" class="form-control addTitlez" placeholder="歷程標題（建議字數在 15 個字元內）">
                    </div>
                    <div class="form-group col-md-12">
                        <label>歷程日期<span class="markz"></span></label>
                        <input type="month" class="form-control addDatez">
                    </div>
                    <div class="form-group col-md-12">
                        <label>歷程內容<span class="markz"></span></label>
                        <textarea type="text" class="form-control textareas addContentz" placeholder="歷程內容（建議字數在 300 個字元內）"></textarea>
                    </div>
                    <div class="form-group col-md-12">
                        <div class="previews noLives">
                            <div class="parts">
                                <label>導覽圖 01<span class="notes">（圖片大小：800 X 600）</span></label>
                                <input type="file" class="form-control addPicImagez" accept="image/*">
                                <!-- <div class="titleImages samplez"></div> -->
                            </div>
                            <div class="parts">
                                <label>導覽圖 02<span class="notes">（圖片大小：800 X 600）</span></label>
                                <input type="file" class="form-control addPicImagez" accept="image/*">
                                <!-- <div class="titleImages samplez"></div> -->
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="card-footer">
                <div class="row">
                    <div class="form-group col-12">
                        <div class="btns justify-content-end">
                            <button type="button" class="btn btn-primary btnAddz">
                                <i class="fad fa-file-plus"></i> 新增
                            </button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    `;
// 宣告要帶入的欄位
let list = $('.list'), paginationz = $('.paginationz');
let navPics01, navPics02;
// 驗證
function dataUpdateCheck(aId, title, date, content) {
    if (aId.trim() === '') {
        return check = false, errorText = '請確認必填欄位皆有填寫！';
    }
    if (title.val().trim() === '' || title.val().trim().length > 40) {
        title.focus();
        return check = false, errorText = '請確認歷程標題欄位是否有確實填寫、格式是否正確（長度不得超過 40 個字元）！';
    }
    if (date.val().trim() === '0000-00' || date.val().trim() === '') {
        date.focus();
        return check = false, errorText = '請確認歷程時間欄位是否有確實選取！';
    }
    if (content.val().trim() === '' || content.val().trim().length > 300) {
        content.focus();
        return check = false, errorText = '請確認歷程內容欄位是否有確實填寫、格式是否正確（長度不得超過 300 個字元）！';
    }
    else {
        return check = true, errorText = "";
    }
};
CONNECT = 'timeMachine';
let enabled = 1, topz = 0; // 狀態預設開啟 置頂預設不開放
let datez, formatz;
// 接收資料，做渲染、處理
function process(data) {
    html = "";
    for (let i = 0; i < data.length; i++) {
        // Date Format
        datez = Date.parse(dateChange(data[i].course)).valueOf();
        formatz = new Date(datez).format('yyyy-MM');
        if (data[i].imageURL01 !== null) {
            navPics01 = `<img class="sampleImagez" src="${data[i].imageURL01}?editz${paramRandoms()}">`;
        } else {
            navPics01 = `
                <img src="./img/samples/sample800x600.jpg">
            `;
        };
        if (data[i].imageURL02 !== null) {
            navPics02 = `<img class="sampleImagez" src="${data[i].imageURL02}?editz${paramRandoms()}">`;
        } else {
            navPics02 = `
                <img src="./img/samples/sample800x600.jpg">
            `;
        };
        html += `
            <div class="card" data-numz="${data[i].timeMachineId}" data-sortz="${data[i].sort}">
                <div class="card-header">
                    <div class="row">
                        <div class="col-12 form-group py-0 d-flex justify-content-between align-items-center">
                            <h4 class="text-info mb-0">
                                <span>編號 ${data[i].id}</span>
                            </h4>
                            <div class="custom-control custom-switch">
                                <input type="checkbox" class="custom-control-input inputStatus statusz" id="${data[i].id}listtatus${i}" value="${data[i].enabled}" disabled>
                                <label class="custom-control-label" for="${data[i].id}listtatus${i}">開啟</label>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="card-body">
                    <div class="row">
                        <!--<div class="form-group d-flex justify-content-end col-12">
                            <label class="mr-2 col-form-label">排序</label>
                            <input type="text" class="form-control text-right inputStatus sort sortz" value="${data[i].sort}" disabled>
                        </div>-->
                        <div class="form-group col-md-12">
                            <label>歷程標題<span class="notes">（建議字數在 15 個字元內）</span><span class="markz"></span></label>
                            <input type="text" class="form-control inputStatus editTitlez" placeholder="歷程標題（建議字數在 15 個字元內）" value="${data[i].name}" disabled>
                        </div>
                        <div class="form-group col-md-12">
                            <label>歷程日期<span class="markz"></span></label>
                            <input type="month" class="form-control inputStatus editDatez" value="${formatz}" disabled>
                        </div>
                        <div class="form-group col-md-12">
                            <label>歷程內容<span class="notes">（建議字數在 300 個字元內）</span><span class="markz"></span></label>
                            <textarea type="text" class="form-control textareas halfs inputStatus editContentz" placeholder="歷程內容（建議字數在 300 個字元內）" disabled>${data[i].brief}</textarea> 
                        </div>
                        <div class="form-group col-12">
                            <div class="previews previewz">
                                <div data-srcz="${data[i].imageURL01}" class="parts">
                                    <label>導覽圖 01<span class="notes">（圖片大小：800 X 600）</span><span class="markz"></span></label>
                                    <input type="file" class="form-control inputStatus editPicImagez" accept="image/*" disabled>
                                    <div class="titleImages samplez">
                                        ${navPics01}
                                    </div>
                                </div>
                                <div data-srcz="${data[i].imageURL02}" class="parts">
                                    <label>導覽圖 02<span class="notes">（圖片大小：800 X 600）</span><span class="markz"></span></label>
                                    <input type="file" class="form-control inputStatus editPicImagez" accept="image/*" disabled>
                                    <div class="titleImages samplez">
                                        ${navPics02}
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="card-footer">
                    <div class="row">
                        <div class="form-group col-12 pt-0">
                            <div class="dates">
                                <span>建立日期</span>                                
                                <span class="date">${dateChange(data[i].creationDate)}</span>
                            </div>
                            <div class="btns justify-content-end">${setUpz}</div>
                        </div>
                    </div>
                </div>
            </div>
        `;
    };
    list.html(html);
    // 狀態顯示
    let chk = $('.statusz');
    for (let i = 0; i < chk.length; i++) {
        if (chk.eq(i).val() == "1") {
            chk.eq(i).prop('checked', true);
        } else {
            chk.eq(i).prop('checked', false);
        };
    };
    // 資料載入完成後
    loadingCompleted();
};
// NOTFOUND
function fails() {
    paginationz.addClass('disabled');
    // 資料載入完成後
    loadingCompleted();
};
$().ready(function () {
    //
    let dataObj = {
        "Methods": "GET",
        "APIs": URL,
        "CONNECTs": CONNECT,
        "QUERYs": `${CONNECT}?count=${listSize}&page=${pageRcd}`,
        "Counts": listSize,
        "Sends": "",
    };
    // 產生第一次的分頁器
    getTotalPages(dataObj).then(res => {
        if (res !== 0) {
            pageLens = res;
            paginations.find('.pagez').html(curPage(current, pageLens, pageCount));
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
                        dataObj.QUERYs = `${CONNECT}?count=${listSize}&page=${pageRcd}`;
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
                                alert("錯誤訊息 " + xhr.status + "：您的連線異常，請重新登入！");
                                location.reload();
                            };
                        });
                    };
                } else {
                    if (num !== pageRcd) { // 如果不是點同一頁碼的話
                        // 1. 記錄當下頁碼
                        pageRcd = num;
                        // 2. 條件
                        dataObj.QUERYs = `${CONNECT}?count=${listSize}&page=${num}`;
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
                                alert("錯誤訊息 " + xhr.status + "：您的連線異常，請重新登入！");
                                location.reload();
                            };
                        });
                    } else { };
                };
            });
            paginations.find('.pagez li:first-child').trigger('click');
        } else {
            pageLens = 0; // 資料筆數為 0 頁數為 0
            fails();
        };
    }, rej => {
        if (rej == "NOTFOUND") { // ? 當伺服器無資料時 || 當伺服器發生問題時
            pageLens = 0; // 資料筆數為 0 頁數為 0
            fails();
            // alert("錯誤訊息 " + xhr.status + "：您的連線已逾期，請重新登入！");
            // location.reload();
        };
    });
    // 上傳圖片 驗證
    $(document).on('change', '.imgFilez', function () {
        let file = $(this);
        let parents = file.parents('.parts');
        if (file.val() !== "") {
            if (imgUpdateCheck(file)) {
                // if (window.URL !== undefined) {
                //     let url = window.URL.createObjectURL(file[0].files[0]);
                //     let currentImages = `
                //             <img class="sampleImagez" src="${url}">
                //         `;
                //     parents.find('.samplez').html(currentImages);
                // };
            };
        } else {
            // 回復預設
            parents.find('.samplez').html('');
            // 清空 file 的值
            file.val('');
        };
    });
    // Add 新增
    $(document).on('click', '.btnAddz', function (e) {
        e.preventDefault();
        // 驗證
        dataUpdateCheck(idz, $('.addTitlez'), $('.addDatez'), $('.addContentz'));
        if (check == true) {
            let dataObj = new FormData();
            dataObj.append('name', $('.addTitlez').val().trim());
            dataObj.append('course', $('.addDatez').val().trim());
            dataObj.append('brief', $('.addContentz').val().trim());
            dataObj.append('enabled', enabled);
            dataObj.append('navPics01', $('.addPicImagez')[0].files[0]);
            dataObj.append('navPics02', $('.addPicImagez')[1].files[0]);
            dataObj.append('first', topz);
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
                // xhr.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded');
                xhr.send(dataObj);
            }
        } else {
            alert(errorText);
        };
    });
    // Edit
    $(document).on('click', '.btnEditz', function (e) {
        e.preventDefault(), e.stopPropagation();
        let trz = $(this).parents('.card');
        let numz = trz.data('numz');
        if ($('.btns').hasClass('active')) {
            alert('請確認您是否有其他的內容尚未儲存！');
        }
        else {
            trz.find('.btns').addClass('active').html('').append(`
                    <a class="btn btn-success btn-sm btnSavez" href="#"><i class="fal fa-save"></i> 儲存</a> 
                    <a class="btn btn-danger btn-sm btnDelz" href="#"><i class="fas fa-trash"></i> 刪除</a>
                    <a class="btn btn-sm btnCancel btnCancelz" href="#"><i class="far fa-window-close"></i> 取消</a>
            `);
            // 原資料
            let orgStatus = trz.find('.statusz').val(), orgTitle = trz.find('.editTitlez').val(), orgDate = trz.find('.editDatez').val(), orgContent = trz.find('.editContentz').val();
            let orgImageURL01 = trz.find('.previewz .parts').eq(0).data('srcz'), orgImageURL02 = trz.find('.previewz .parts').eq(1).data('srcz');
            // Input Enable
            trz.find('.inputStatus').prop('disabled', false);
            // 更新圖片
            trz.find('.editPicImagez').on('change', function () {
                let file = $(this);
                let parents = file.parents('.parts');
                if (file.val() !== "") {
                    if (imgUpdateCheck(file)) {
                        if (window.URL !== undefined) {
                            let url = window.URL.createObjectURL(file[0].files[0]);
                            let currentImages = `
                            <img class="sampleImagez" src="${url}">
                        `;
                            parents.find('.samplez').html(currentImages);
                        };
                    };
                } else {
                    // 回復預設
                    parents.find('.samplez').html(`
                        <img src="./img/samples/sample800x600.jpg">
                    `);
                    // 清空 file 的值
                    file.val('');
                };
            });
            // Save 儲存
            trz.find('.btnSavez').on('click', function (e) {
                e.preventDefault();
                // 驗證
                dataUpdateCheck(idz, trz.find('.editTitlez'), trz.find('.editDatez'), trz.find('.editContentz'));
                if (check == true) {
                    let dataObj = {
                        "timeMachineId": numz,
                        "name": trz.find('.editTitlez').val(),
                        "course": trz.find('.editDatez').val(),
                        "brief": trz.find('.editContentz').val(),
                        "enabled": trz.find('.statusz').val(),
                        "first": topz,
                        "sort": trz.data('sortz')
                    };
                    if (confirm("您確定要修改嗎？")) {
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
                        xhr.open('PUT', `${URL}${CONNECT}/${numz}`, true);
                        xhr.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded');
                        xhr.send($.param(dataObj));
                    }
                } else {
                    alert(errorText);
                };
            });
            // Delete 刪除  
            trz.find('.btnDelz').on('click', function (e) {
                e.preventDefault(); // 取消 a 預設事件
                let dataObj = {
                    "timeMachineId": numz
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
                    // 還原欄位資料
                    trz.find('.statusz').val(orgStatus), trz.find('.editTitlez').val(orgTitle), trz.find('.editDatez').val(orgDate), trz.find('.editContentz').val(orgContent);
                    if (orgImageURL01 !== null) {
                        trz.find('.previewz .parts').eq(0).find('.samplez').html(`
                            <img class="sampleImagez" src="${orgImageURL01}">
                        `);
                    } else {
                        trz.find('.previewz .parts').eq(0).find('.samplez').html(`
                            <img src="./img/samples/sample800x600.jpg">
                        `);
                    }
                    if (orgImageURL02 !== null) {
                        trz.find('.previewz .parts').eq(1).find('.samplez').html(`
                            <img class="sampleImagez" src="${orgImageURL02}">
                        `);
                    } else {
                        trz.find('.previewz .parts').eq(1).find('.samplez').html(`
                            <img src="./img/samples/sample800x600.jpg">
                        `);
                    }
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
    // 分類狀態控制
    $(document).on('change', '.statusz', function () {
        if ($(this).prop('checked') == true) {
            $(this).val("1");
        } else {
            $(this).val("0");
        }
    });
});
