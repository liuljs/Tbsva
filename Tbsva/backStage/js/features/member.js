//
let methodz = '02',
    setUpz = `
        <button class="btn btn-warning btn-sm mb-1 btnEditz" data-toggle="modal" data-target="#memberEdit"><i class="far fa-edit"></i> 編輯</button>
        <a class="btn btn-danger btn-sm mb-1 btnDelz" href="#"><i class="fas fa-trash"></i> 刪除</a>
    `,
    insert = `
        <button class="btn btn-primary" data-toggle="modal" data-target="#memberAdd">
            <i class="fal fa-user-plus"></i> 新增會員
        </button>
    `;
// 宣告要帶入的欄位
let list = $('.list'), edits = $('.edits');

let addStatusz = $('.addStatusz'), addAuditz = $('.addAuditz'), addNamez = $('.addNamez'), addGenderz = $('.addGenderz'), addBirthz = $('.addBirthz'), addClassifyz = $('.addClassifyz'), addAddressz = $('.addAddressz'), addPhonez = $('.addPhonez'), addCellz = $('.addCellz'), addMailz = $('.addMailz'), addContentz = $('.addContentz'), addNotez = $('.addNotez');
let memNumz = $('.memNumz'), editNumz = $('.editNumz'), editJoinz = $('.editJoinz'), editStatusz = $('.editStatusz'), editAuditz = $('.editAuditz'), editNamez = $('.editNamez'), editGenderz = $('.editGenderz'), editBirthz = $('.editBirthz'), editClassifyz = $('.editClassifyz'), editAddressz = $('.editAddressz'), editPhonez = $('.editPhonez'), editCellz = $('.editCellz'), editMailz = $('.editMailz'), editContentz = $('.editContentz'), editNotez = $('.editNotez');
let audit, enabled, addrs, gender, birth, cell, addrArr = new Array();
let btnAddz = $('.btnAddz'), btnSavez = $('.btnSavez')
let btnSearchz = $('.btnSearchz'), btnAllz = $('.btnAllz'), classifyz = $('.classifyz'), searchz = $('.searchz');
CONNECT = 'addTbsva';
// 接收資料，做渲染、處理
function process(data) {
    html = "";
    for (let i = 0; i < data.length; i++) {
        // 審核
        if (data[i].audit == 1) {
            audit = `<span class="text-secondary"> 已審核 </span>`;
        } else {
            audit = `<span class="text-danger"> 未審核 </span>`;
        };
        // 狀態
        if (data[i].enabled == 1) {
            enabled = `<span class="text-success"> 開啟 </span>`;
        } else {
            enabled = `<span class="text-danger"> 關閉 </span>`;
        };
        html += `
            <tr data-numz="${data[i].addtbsvaId}">
                <td data-title="會員編號" class="text-center">
                    <div>${data[i].tId}</div>
                </td>
                <td data-title="建立日期" class="fieldLefts">
                    <div class="fullFields">${dateChange(data[i].creationDate)}</div>
                </td>
                <td data-title="會員姓名" class="text-left fieldLefts">
                    <div class="fullFields">${data[i].namez}</div>
                </td>
                <td data-title="聯絡電話" class="text-left fieldLefts">
                    <div class="fullFields">${data[i].contactNumber}</div>
                </td>
                <td data-title="隸屬地區" class="text-center fieldLefts">
                    <div class="fullFields">${areasDisplay(data[i].affiliatedAreaz)}</div>
                </td>
                <td data-title="狀態" class="text-center" data-status="${data[i].enabled}">${enabled}</td>
                <td data-title="審核" class="text-center" data-statusz="${data[i].audit}">${audit}</td>
                <td data-title="設定">
                    <div class="btns">${setUpz}</div>
                </td>
            </tr>
        `;
    };
    list.html(html);
    // Delete 刪除
    $('.btnDelz').on('click', function (e) {
        e.preventDefault(); // 取消 a 預設事件
        let numz = $(this).parents('tr').data('numz');
        let dataObj = {
            "id": numz
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
                }
            };
            xhr.open('DELETE', `${URL}${CONNECT}/${numz}`, true);
            xhr.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded');
            xhr.send($.param(dataObj));
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
            <td colspan="8" class="txt-left none">
                <span>目前沒有任何的內容。</span>
            </td>
        </tr>
    `;
    list.html(html);
    paginations.find('.pagez').html(curPage(current, pageLens, pageCount));
    // 資料載入完成後
    loadingCompleted();
};
// 類別查詢驗證
function categoryCheck(classify, title) {
    if (classify.val().trim() === "preset" && title.val().trim() === '') {
        return check = false, errorText = '請確認查詢方式有確實選取或填寫！';
    } else {
        return check = true, errorText = '';
    };
};
// 驗證
function dataUpdateCheck(aId, name, birth, area, address, phone, mail) {
    if (aId.trim() === '') {
        errorText = '請確認必填欄位皆有填寫！';
        check = false;
        return;
    }
    if (name.val().trim() === '' || name.val().trim().length > 30) {
        name.focus();
        return check = false, errorText = '請確認會員姓名是否確實填寫，或格式是否正確（長度不得超過 30 個字元）！';
    }
    if (birth.val().trim() === '' || birth.val().trim() === '0001-01-01') {
        birth.focus();
        return check = false, errorText = '請確認出生日期是否確實選取！';
    }
    if (area.val().trim() === 'preset') {
        area.focus();
        return check = false, errorText = '請確認隸屬地區是否確實選擇！';
    }
    if (address.find('.twZipCodes').twzipcode('get', 'county,district') === '' || address.find('.inputz').val() === '') {
        address.find('.inputz').focus();
        return check = false, errorText = '請確認聯絡地址是否確實選取、詳細地址是否確實填寫！';
    }
    if (phone.val().trim() === '' || PhoneRegExp.test(phone.val()) === false) {
        phone.focus();
        return check = false, errorText = '請確認聯絡電話是否確實填寫、格式是否正確！';
    }
    if (mail.val().trim() === '' || EmailRegExp.test(mail.val()) === false) {
        mail.focus();
        return check = false, errorText = '請確認電子信箱是否確實填寫、格式是否正確！';
    }
    else {
        return check = true, errorText = "";
    }
};
// init 
function init(dataObj) {
    let cls = "";
    clsRcd = "", pageRcd = ""; // 使用搜尋，清空目前記錄的篩選 && 每次使用都要清除頁碼紀錄
    dataObj.CONNECTs = CONNECT;
    // 產生第一次的分頁器
    getTotalPages(dataObj).then(res => {
        if (res !== 0) {
            pageLens = res; // 目前總頁數
            paginations.find('.pagez').html(curPage(current, pageLens, pageCount));
            // 點擊頁碼
            paginations.unbind('click').on('click', 'li', function (e) {
                e.preventDefault(), e.stopImmediatePropagation();
                let num = $(this).find('a').data('num');
                dataObj.Sends["count"] = dataObj.Counts; // 每次列出筆數
                if (isNaN(num)) {
                    if (!$(this).hasClass("disabled")) {
                        if (num == "prev") {
                            pageRcd--;
                        } else if (num == "next") {
                            pageRcd++;
                        };
                        // 1. 紀錄當下頁碼 || 上下頁：以記錄的頁碼來做拋接值
                        pageRcd = pageRcd, dataObj.Sends.page = pageRcd;
                        // 2. 條件
                        dataObj.QUERYs = `${CONNECT}?count=${listSize}&page=${pageRcd}`;
                        // 3. 取得點擊頁碼後要呈現的內容
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
                    if (cls !== clsRcd) {
                        // 1. 重設頁碼紀錄為 Current && 重新記錄目前的分類
                        clsRcd = cls, pageRcd = current;
                        // 2. 條件
                        dataObj.QUERYs = `${CONNECT}?count=${listSize}&page=${pageRcd}`;
                        // 3. 取得點擊頁碼後要呈現的內容(要呈現的筆數)
                        getPageDatas(dataObj).then(res => {
                            // DO SOMETHING
                            if (res !== null) {
                                process(res);
                                // 4. 產生新分頁器
                                paginations.find('.pagez').html(curPage(current, pageLens, pageCount));
                            } else {
                                fails();
                            };
                        }, rej => {
                            if (rej == "NOTFOUND") {
                                alert("錯誤訊息 " + xhr.status + "：您的連線異常，請重新登入！");
                                location.reload();
                            };
                        });
                    } else {
                        if (num !== pageRcd) { // 如果不是點同一頁碼的話
                            // 1. 記錄當下頁碼 || 傳送的頁碼 
                            pageRcd = num, dataObj.Sends.page = num;
                            // 2. 條件
                            dataObj.QUERYs = `${CONNECT}?count=${listSize}&page=${pageRcd}`;
                            // 3. 取得點擊頁碼後要呈現的內容(要呈現的筆數)
                            getPageDatas(dataObj).then(res => {
                                // DO SOMETHING
                                if (res !== null) {
                                    process(res);
                                    // 4. 產生分頁器
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
                    }
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
};
$().ready(function () {
    // 列表
    let dataObj = {
        "Methods": "GET",
        "APIs": URL,
        "CONNECTs": CONNECT,
        "QUERYs": `${CONNECT}?count=${listSize}&page=${pageRcd}`,
        "Counts": listSize,
        "Sends": "",
    };
    init(dataObj);
    // 所屬地區對應填寫的聯絡地址
    let affiliatedAreaz = $('.affiliatedAreaz'), addressz = $('.addressz');
    affiliatedAreaz.on('change', function () {
        if ($(this).val() == 4) {
            addressz.find('.twZipCodes').addClass('disabled')
        } else {
            addressz.find('.twZipCodes').removeClass('disabled')
        }
    });
    // Add
    btnAddz.on('click', function () {
        // twzipcode 取值
        addrArr = [];
        addAddressz.find('.twZipCodes').twzipcode('get', function (county, district, zipcode) {
            if (county !== "") {
                addrArr.push(county);
                if (district !== "") {
                    addrArr.unshift(zipcode);
                    addrArr.push(district);
                };
            };
            return addrArr;
        });
        if (addAddressz.find('.inputz').val() !== "") {
            addrArr.push(addAddressz.find('.inputz').val().trim());
        };
        // 驗證
        dataUpdateCheck(idz, addNamez, addBirthz, addClassifyz, addAddressz, addPhonez, addMailz,);
        if (check == true) {
            let dataObj = {
                "namez": addNamez.val().trim(),
                "gender": $('.addGenderz:checked').val().trim(),
                "birthz": addBirthz.val().trim(),
                "affiliatedAreaz": addClassifyz.val().trim(),
                "contactAddress": JSON.stringify(addrArr),
                "contactNumber": addPhonez.val().trim(),
                "moblieNumber": addCellz.val().trim(),
                "email": addMailz.val().trim(),
                "contentz": addContentz.val().trim(),
                "notez": addNotez.val().trim(),
                "enabled": addEnabled.val().trim(),
                "audit": addAuditz.val().trim()
            };

            let xhr = new XMLHttpRequest();
            xhr.onload = function () {
                if (xhr.status == 200 || xhr.status == 204) {
                    alert("新增成功！");
                    location.reload();
                } else {
                    alert("錯誤訊息 " + xhr.status + "：您的連線異常，請重新登入！");
                    location.reload();
                };
            };
            xhr.open('POST', `${URL}${CONNECT}/AddMember`, true);
            xhr.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded');
            xhr.send($.param(dataObj));
        } else {
            alert(errorText);
        };
    });
    // Edit 編輯
    $(document).on('click', '.btnEditz', function (e) {
        e.preventDefault();
        // 點擊編輯後，直接取頁面上的資料動態產生再編輯燈箱中
        let numz = $(this).parents('tr').data('numz');
        let xhr = new XMLHttpRequest();
        xhr.onload = function () {
            if (xhr.status == 200) {
                if (xhr.responseText !== "" && xhr.responseText !== "[]") {
                    let callBackData = JSON.parse(xhr.responseText);
                    memNumz.html(callBackData.tId);
                    editNumz.val(callBackData.tId);
                    editNumz.attr('data-numz', callBackData.addtbsvaId);
                    editJoinz.val(dateChange(callBackData.creationDate));
                    editAuditz.val(callBackData.audit);
                    // 審核狀態
                    if (editAuditz.val() == "1") {
                        editAuditz.prop('checked', true);
                    } else {
                        editAuditz.prop('checked', false);
                    };
                    editStatusz.val(callBackData.enabled);
                    // 會員狀態
                    if (editStatusz.val() == "1") {
                        editStatusz.prop('checked', true);
                    } else {
                        editStatusz.prop('checked', false);
                    };
                    editNamez.val(callBackData.namez);
                    $(`.editGenderz[value="${callBackData.gender}"]`).prop('checked', true);
                    editBirthz.val(dateChange(callBackData.birthz));
                    editClassifyz.val(callBackData.affiliatedAreaz);
                    editPhonez.val(callBackData.contactNumber);
                    if (callBackData.moblieNumber !== "") {
                        editCellz.val(callBackData.moblieNumber);
                    } else {
                        editCellz.val('');
                    };
                    editMailz.val(callBackData.email);
                    if (callBackData.contentz !== "") {
                        editContentz.val(callBackData.contentz);
                    };
                    if (callBackData.notez) {
                        editNotez.val(callBackData.notez);
                    };
                    // 審核控制
                    editAuditz.on('change', function () {
                        if ($(this).prop('checked') == true) {
                            $(this).val("1");
                        } else {
                            $(this).val("0");
                        }
                    });
                    // 台灣地址 下拉式選單
                    $('.addressz').find('.twZipCodes').twzipcode({
                        zipcodeIntoDistrict: true, // 郵遞區號自動顯示在地區
                        css: ["custom-select form-control counties", "custom-select form-control districts"], // 自訂 "城市"、"地區" class 名稱 
                        countyName: "city", // 自訂城市 select 標籤的 name 值
                        districtName: "town" // 自訂地區 select 標籤的 name 值
                    });
                    // 隸屬地區
                    editClassifyz.val(callBackData.affiliatedAreaz);
                    // Set Contact Address
                    if (callBackData.contactAddress !== "" && callBackData.contactAddress !== "[]") {
                        let addrArr = JSON.parse(callBackData.contactAddress);
                        editAddressz.find('.twZipCodes').twzipcode('set', {
                            'zipcode': addrArr[0],
                            'county': addrArr[1],
                            'district': addrArr[2]
                        });
                        editAddressz.find('.inputz').val(addrArr[3]);
                    };
                } else { };
            } else {
                alert("錯誤訊息 " + xhr.status + "：您的連線異常，請重新登入！");
                location.reload();
            };
        };
        xhr.open('GET', `${URL}${CONNECT}/${numz}`, true);
        xhr.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded');
        xhr.send(null);
    });
    // Save 儲存
    btnSavez.on('click', function (e) {
        e.preventDefault();
        let numz = $(this).parents('.edits').find('.editNumz').data('numz');
        // twzipcode 取值
        addrArr = [];
        editAddressz.find('.twZipCodes').twzipcode('get', function (county, district, zipcode) {
            if (county !== "") {
                addrArr.push(county);
                if (district !== "") {
                    addrArr.unshift(zipcode);
                    addrArr.push(district);
                };
            };
            return addrArr;
        });
        if (editAddressz.find('.inputz').val() !== "") {
            addrArr.push(editAddressz.find('.inputz').val());
        };
        // // 驗證
        dataUpdateCheck(idz, editNamez, editBirthz, editClassifyz, editAddressz, editPhonez, editMailz,);
        // 
        if (check == true) {
            let dataObj = {
                "addtbsvaId": numz,
                "audit": editAuditz.val().trim(),
                "enabled": editStatusz.val().trim(),
                "namez": editNamez.val().trim(),
                "gender": $('.editGenderz:checked').val().trim(),
                "birthz": editBirthz.val().trim(),
                "affiliatedAreaz": editClassifyz.val().trim(),
                "contactAddress": JSON.stringify(addrArr),
                "contactNumber": editPhonez.val().trim(),
                "moblieNumber": editCellz.val().trim(),
                "email": editMailz.val().trim(),
                "contentz": editContentz.val().trim(),
                "notez": editNotez.val().trim(),
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
            };
        } else {
            alert(errorText);
        };
    });
    // Cancel
    $(document).on('click', '.btnCancelz, .close', function (e) {
        e.preventDefault();
        let trz = $(this).parents('.modal-content');
        $.map(trz.find('.editInput'), function (item, index) {
            if (item.value !== "") {
                return check = false
            };
        });
        if (check == true) {
            // 欄位未填任何內容，直接關閉
            trz.find('.closez').trigger('click');
            check = true; // 重置 check
        } else {
            // 資訊皆是渲染，可直接清空
            if (confirm("您尚未儲存內容，是否直接關閉？")) {
                // 清空欄位的值
                trz.find('.editInput').val('');
                // 關閉
                trz.find('.closez').trigger('click');
                check = true; // 重置 check
            };
        };
    });
    // Searchs
    btnSearchz.on('click', function (e) {
        e.preventDefault();
        categoryCheck(classifyz, searchz);
        if (check == true) {
            let clsNum = classifyz.val() !== "preset" ? classifyz.val() : "";
            let clsName = searchz.val() !== "" ? searchz.val() : "";
            let cls = clsNum + clsName;
            // 條件
            if (clsNum !== "" && clsName !== "") {
                dataObj.CONNECTs = `${CONNECT}?articleCategoryId=${clsNum}&namez=${clsName}`;
            } else if (clsNum !== "" || clsName !== "") {
                dataObj.CONNECTs = `${CONNECT}?articleCategoryId=${clsNum !== "" ? clsNum : ""}&namez=${clsName !== "" ? clsName : ""}`;
            };
            // 使用搜尋，清空目前記錄的篩選 && 每次使用都要清除頁碼紀錄
            clsRcd = "", pageRcd = "";
            getTotalPages(dataObj).then(res => {
                if (res !== 0) {
                    pageLens = res; // 目前總頁數
                    paginations.find('.pagez').html(curPage(current, pageLens, pageCount));
                    // 點擊頁碼
                    paginations.unbind('click').on('click', 'li', function (e) {
                        e.preventDefault(), e.stopImmediatePropagation();
                        let num = $(this).find('a').data('num');
                        dataObj.Sends["count"] = dataObj.Counts; // 每次列出筆數
                        if (isNaN(num)) {
                            if (!$(this).hasClass("disabled")) {
                                if (num == "prev") {
                                    pageRcd--;
                                } else if (num == "next") {
                                    pageRcd++;
                                };
                                // 1. 紀錄當下頁碼 || 上下頁：以記錄的頁碼來做拋接值
                                pageRcd = pageRcd, dataObj.Sends.page = pageRcd;
                                // 2. 條件
                                dataObj.QUERYs = `${CONNECT}?count=${listSize}&page=${pageRcd}&articleCategoryId=${clsNum}&namez=${clsName}`;
                                // 3. 取得點擊頁碼後要呈現的內容
                                getPageDatas(dataObj).then(res => {
                                    // DO SOMETHING
                                    if (res !== null) {
                                        process(res);
                                        // 4. 產生分頁器
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
                            if (cls !== clsRcd) {
                                // 1. 重設頁碼紀錄為 Current && 重新記錄目前的分類
                                clsRcd = cls, pageRcd = current;
                                // 2. 條件
                                dataObj.QUERYs = `${CONNECT}?count=${listSize}&page=${pageRcd}&articleCategoryId=${clsNum}&namez=${clsName}`;
                                // 3. 取得點擊頁碼後要呈現的內容(要呈現的筆數)
                                getPageDatas(dataObj).then(res => {
                                    // DO SOMETHING
                                    if (res !== null) {
                                        process(res);
                                        // 4. 產生新分頁器
                                        paginations.find('.pagez').html(curPage(current, pageLens, pageCount));
                                    } else {
                                        fails();
                                    };
                                }, rej => {
                                    if (rej == "NOTFOUND") {
                                        alert("錯誤訊息 " + xhr.status + "：您的連線已逾期，請重新登入！");
                                        location.reload();
                                    };
                                });
                            } else {
                                if (num !== pageRcd) { // 如果不是點同一頁碼的話
                                    // 1. 記錄當下頁碼 || 傳送的頁碼
                                    pageRcd = num, dataObj.Sends.page = num;
                                    // 2. 條件
                                    dataObj.QUERYs = `${CONNECT}?count=${listSize}&page=${pageRcd}&articleCategoryId=${clsNum}&namez=${clsName}`;
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
                            }
                        };
                    });
                    paginations.find('.pagez li:first-child').trigger('click');
                } else {
                    fails();
                    clsRcd = cls;
                };
            }, rej => {
                if (rej == "NOTFOUND") {
                    fails();
                    clsRcd = cls;
                };
            });
        } else {
            alert(errorText);
            searchz.val(''); // 清空
        };
    });
    // All
    btnAllz.on('click', function () {
        classifyz.prop('value', 'preset'), searchz.val('');
        init(dataObj);
    });
});