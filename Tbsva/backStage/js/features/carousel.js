let methodz = '01', setUpz = '',
    insert = `
        <div class="card">
            <div class="card-header">
                <div class="row">
                    <div class="form-group col-12 py-0">
                        <h2 class="text-primary d-inline mb-0">新增輪播圖片<span class="notes">（滿版圖片為必填，如只提供滿版將會由滿版圖片自動裁切並置適應）</span></h2>
                    </div>
                </div>
            </div>
            <div class="card-body">
                <div class="row">
                    <div class="form-group col-md-12">
                        <label>滿版 16:9 顯示<span class="notes">（圖片大小：1600 X 900）</span><span class="markz"></span></label>
                        <input type="file" class="form-control imgFilez" accept="image/*">
                    </div>
                    <div class="form-group col-md-6">
                        <label>平板 1:1 顯示<span class="notes">（圖片大小：800 X 800）</label>
                        <input type="file" class="form-control imgFilez" accept="image/*">
                    </div>
                    <div class="form-group col-md-6">
                        <label>手機 1:1 顯示<span class="notes">（圖片大小：600 X 600）</label>
                        <input type="file" class="form-control imgFilez" accept="image/*">
                    </div>
                    <div class="form-group col-md-12">
                        <label>超連結網址</label>
                        <input type="text" class="form-control carouselUrlz" placeholder="超連結網址（非必填，http 或 https 為開頭）">
                    </div>
                </div>
            </div>
            <div class="card-footer">
                <div class="row">
                    <div class="form-group col-12 text-right">
                        <button type="button" class="btn btn-primary btnAddz">
                            <i class="fad fa-file-plus"></i> 新增
                        </button>
                    </div>
                </div>
            </div>
        </div>
    `;
// 宣告要帶入的欄位
let list = $('.list'), carousel;
let topz = 0, enabled = 1; // 預設為不置頂
// 驗證
function dataUpdateCheck(aId, name) {
    if (aId.trim() === '') {
        return check = false, errorText = '請確認必填欄位皆有填寫！';
    }
    if (name.val().trim() === '') {
        name.focus();
        return check = false, errorText = '請確認有選擇要上傳的輪播圖片（滿版圖片）！';
    }
    else {
        return check = true, errorText = "";
    };
};
CONNECT = 'indexSlideshow';
// 接收資料，做渲染、處理
function process(data) {
    html = "", carousel = "";
    for (let i = 0; i < data.length; i++) {
        // 滿版 必填
        carousel = `
            <div class="carouselImage">
                <img src="./img/samples/sample1600x900.jpg">
                <img class="cover" src="${data[i].imageURL01}">
                <p class="mainTitle">滿版 16:9 顯示</p>
            </div>
        `;
        // 平板 選填
        if (data[i].imageURL02 !== "" && data[i].imageURL02 !== null && data[i].imageURL02 !== undefined) {
            carousel += `
                <div class="carouselImage">
                    <img src="./img/samples/sample800x800.jpg">
                    <img class="cover" src="${data[i].imageURL02}">
                    <p class="mainTitle">平板 1:1 顯示</p>
                </div>
            `;
        }
        // 手機 選填
        if (data[i].imageURL03 !== "" && data[i].imageURL03 !== null && data[i].imageURL03 !== undefined) {
            carousel += `
                <div class="carouselImage">
                    <img src="./img/samples/sample800x800.jpg">
                    <img class="cover" src="${data[i].imageURL03}">
                    <p class="mainTitle">手機 1:1 顯示</p>
                </div>
            `;
        }
        html += `
            <tr data-numz="${data[i].slideId}" data-sortz="${data[i].sort}">
                <td data-title="編號" class="text-center">
                    <div>${data[i].id}</div>
                </td>
                <td data-title="建立日期" class="fieldLefts">
                    <div class="fullFields">${dateChange(data[i].creationDate)}</div>
                </td>
                <td data-title="顯示">
                    <div class="carouselImages">${carousel}</div>
                </td>
                <td data-title="連結">
                    <div class="fullFields">
                        <input type="text" class="form-control inputStatus linkz" value="${data[i].hyperlink}" disabled>
                    </div>
                </td>
                <td data-title="順序">
                    <div class="form-check pr-0">
                        <label class="form-check-label mr-0">
                            <input type="checkbox" class="form-check-input inputStatus topz" value="${data[i].first}" disabled>
                            <span class="form-check-sign">置頂</span>
                        </label>
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
    for (let i = 0; i < data.length; i++) {
        // 置頂狀態
        if ($('.topz').eq(i).val() == "1") {
            $('.topz').eq(i).prop('checked', true);
        } else {
            $('.topz').eq(i).prop('checked', false);
        };
        // 顯示狀態
        if ($('.statusz').eq(i).val() == "1") {
            $('.statusz').eq(i).prop('checked', true);
        } else {
            $('.statusz').eq(i).prop('checked', false);
        };
    };
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
                <a class="btn btn-success btn-sm btnSavez" href="#"><i class="fal fa-save"></i> 儲存</a> 
                <a class="btn btn-danger btn-sm btnDelz" href="#"><i class="fas fa-trash"></i> 刪除</a>
                <a class="btn btn-sm btnCancel btnCancelz" href="#"><i class="far fa-window-close"></i> 取消</a>
            `);
            // Original
            let orgLink = trz.find('.linkz').val(), orgTop = trz.find('.topz').val(), orgStatus = trz.find('.statusz').val()
            // Input Enable
            trz.find('.inputStatus').prop('disabled', false);
            // topz
            trz.find('.topz').on('change', function () {
                if ($(this).prop('checked') == true) {
                    $(this).val('1');
                } else {
                    $(this).val('0');
                };
            });
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
                let dataObj = new FormData();
                dataObj.append("slideId", numz);
                dataObj.append("hyperlink", trz.find('.linkz').val());
                dataObj.append("first", trz.find('.topz').val());
                dataObj.append("enabled", trz.find('.statusz').val());
                dataObj.append("sort", trz.data('sortz'));
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
                    // xhr.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded');
                    xhr.send(dataObj);
                };
            })
            // Delete
            trz.find('.btnDelz').on('click', function (e) {
                e.preventDefault();
                let dataObj = {
                    "slideId": numz
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
                    trz.find('.linkz').val(orgLink), trz.find('.topz').val(orgTop), trz.find('.statusz').val(orgStatus);
                    if (trz.find('.topz').val() == "1") {
                        trz.find('.topz').prop('checked', true);
                    } else {
                        trz.find('.topz').prop('checked', false);
                    };
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
    //
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
        "CONNECTs": CONNECT,
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
        if (rej == "NOTFOUND") {
            fails();
        };
    });
    $(document).on('change', '.imgFilez', function () {
        let file = $(this);
        imgUpdateCheck(file);
    });
    // 新增輪播圖
    $(document).on('click', '.btnAddz', function (e) {
        e.preventDefault();
        dataUpdateCheck(idz, $('.imgFilez').eq(0));
        if (check == true) {
            let dataObj = new FormData();
            dataObj.append("first", topz);
            dataObj.append('fullImage', $('.imgFilez')[0].files[0]); // 滿版 必填
            dataObj.append('tabletImage', $('.imgFilez')[1].files[0]); // 平板 選填
            dataObj.append('smPhoneImage', $('.imgFilez')[2].files[0]); // 手機 選填
            dataObj.append("enabled", enabled);
            dataObj.append("hyperlink", $('.carouselUrlz').val());

            if (confirm("您確定要新增嗎？")) {
                let xhr = new XMLHttpRequest();
                xhr.onload = function () {
                    if (xhr.status == 200 || xhr.status == 201) {
                        alert('新增成功！');
                        location.reload();
                    } else {
                        alert("錯誤訊息 " + xhr.status + "：您的連線異常，請重新登入！");
                        // location.reload();
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