// 宣告要帶入的欄位
let list = $('.list'), payz, tradez, statusz;
let namez = $('.namez'), areaz = $('.areaz'), concatAddrz = $('.concatAddrz'), phonez = $('.phonez'), cellz = $('.cellz'), mailz = $('.mailz');
let recordz = $('.recordz'), notez = $('.notez');
let payMethodz = $('.payMethodz'), payEndDatez = $('.payEndDatez'), receiptz = $('.receiptz'), receiptTitlez = $('.receiptTitlez'), sendAddrz = $('.sendAddrz'), amountz = $('.amountz'), agreez = $('.agreez');
let donateMethodz, payStatusz, qtyz, concatz, sendz, addrArr;
let iNow, payEndDate, truez = `<span>是</span>`, falsez = `<span>否</span>`, notz = `<span> - </span>`, immediatez = `<span> 即時付款 </span>`;
let btnSearchz = $('.btnSearchz'), btnAllz = $('.btnAllz'), classifyz = $('.classifyz'), searchz = $('.searchz');

CONNECT = 'donate';
// 接收資料，做渲染、處理
function process(data) {
    // 目前時間
    let thisDate = new Date().toISOString().split('T')[0];
    iNow = Date.parse(thisDate).valueOf();
    html = "";
    for (let i = 0; i < data.length; i++) {
        // 顯示狀態
        if (data[i].needAnonymous == 1) {
            statusz = '<span class="text-success">顯示</span>';
        } else {
            statusz = '<span class="text-danger">關閉</span>';
        };
        // 繳款狀態
        if (data[i].payType == "01") { // 線上刷卡
            if (data[i].PayStatus == 2) {
                payz = `
                    <span data-statusz="${data[i].PayStatus}">已繳款</span>
                `;
            } else {
                payz = `
                    <span data-statusz="${data[i].PayStatus}">已失效</span>
                `;
            };
        } else {
            // 最後繳款日期
            if (data[i].PayEndDate !== null) {
                payEndDate = Date.parse(data[i].PayEndDate.split("T")[0]).valueOf();
            };
            if (data[i].PayStatus == 1) {
                payz = `
                    <div data-statusz="${data[i].PayStatus}" class="btns justify-content-center">
                        <button data-numz="${data[i].payType}" type="button" class="btn btn-sm btn-info btnPayz">繳款</button>
                    </div>
                `;
            } else if (data[i].PayStatus == 2) {
                payz = `
                    <span data-statusz="${data[i].PayStatus}" class="text-info">已繳款</span>
                `;
            } else if (data[i].PayStatus == 3 || payEndDate < iNow) {
                payz = `
                    <span data-statusz="${data[i].PayStatus}">已逾期</span>
                `;
            } else if (data[i].PayStatus == 4) {
                payz = `
                    <span data-statusz="${data[i].PayStatus}">已失效</span>
                `;
            };
        };
        // 交易狀態
        if (data[i].DonateStatus == 1) {
            tradez = `
                <div data-statusz="${data[i].DonateStatus}" class="btns justify-content-center">
                    <button type="button" class="btn btn-sm btn-secondary btnFinishz">結案</button>
                </div>
            `;
        } else if (data[i].DonateStatus == 2) {
            tradez = `
                <span data-statusz="${data[i].DonateStatus}" class="text-secondary">已結案</span>
            `;
        }
        html += `
            <tr data-numz="${data[i].orderId}">
                <td data-title="編號" class="text-center">
                    <div>${data[i].orderId}</div>
                </td>
                <td data-title="捐款日期" class="fieldLefts">
                    <div class="fullFields">${dateChange(data[i].orderDate)}</div>
                </td>
                <td data-title="捐款姓名" class="fieldLefts">
                    <div class="fullFields">
                        <p class="mb-0 abridged1">${data[i].buyerName}</p>
                    </div>
                </td>
                <td data-title="捐款方式" class="fieldLefts">
                    <div class="fullFields">
                        <p data-numz="${data[i].donateType}" class="mb-0 text-center">${donateTypeDisplay(data[i].donateType)}</p>
                    </div>
                </td>
                <td data-title="顯示狀態" class="text-center">
                    <span data-statusz="${data[i].enabled}">${statusz}</span>
                </td>
                <td data-title="繳款狀態" class="text-center">
                    <div class="status pays">
                        ${payz}
                    </div>
                </td>
                <td data-title="交易狀態" class="text-center">
                    <div class="trades">
                        ${tradez}
                    </div>
                </td>
                <td data-title="設定">
                    <div class="btns">
                        <a class="btn btn-warning btn-sm btnDetailz" data-toggle="modal" data-target="#recordDetails">
                            <i class="far fa-edit"></i> 詳情
                        </a> 
                    </div>
                </td>
            </tr>
        `;
    };
    list.html(html);
    // Pay
    $('.btnPayz').on('click', function () {
        let status = $(this).parents('.status');
        let numz = $(this).parents('tr').data('numz');
        let payz = $(this).data('numz');

        let dataObj = {
            "id": numz,
        }
        if (confirm(`點擊繳款按鈕，代表此筆訂單的繳款狀態已完成（無法回復狀態）
請再次確認是否繳款！`)) {
            //
            let CONNECT;
            if (payz == 83 || payz == 13) { // 銀行匯款
                CONNECT = 'bankRemittance';
            } else {
                CONNECT = 'virtualAtmSuperMarket';
            };
            let xhr = new XMLHttpRequest();
            xhr.onload = () => {
                if (xhr.status == 200 || xhr.status == 204) {
                    status.html(`
                        <span class="text-info">已繳款</span>
                    `);
                    location.reload();
                } else {
                    alert("錯誤訊息 " + xhr.status + "：您的連線異常，請重新登入！");
                    location.reload();
                };
            };
            xhr.open('PUT', `${URL}${CONNECT}/${numz}`, true)
            xhr.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded');
            xhr.send($.param(dataObj));
        };
    })
    // Finish
    $('.btnFinishz').on('click', function () {
        let status = $(this).parents('.status');
        let numz = $(this).parents('tr').data('numz');
        let dataObj = {
            "id": numz,
        }
        if (confirm(`點擊結案按鈕，代表此筆訂單的交易狀態已完成（無法回復狀態）
請再次確認是否完成！`)) {
            let xhr = new XMLHttpRequest();
            xhr.onload = () => {
                if (xhr.status == 200 || xhr.status == 204) {
                    status.html(`
                        <span class="text-secondary">已結案</span>
                    `);
                    location.reload();
                } else {
                    alert("錯誤訊息 " + xhr.status + "：您的連線異常，請重新登入！");
                    location.reload();
                };
            };
            xhr.open('PUT', `${URL}${CONNECT}/${numz}`, true)
            xhr.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded');
            xhr.send($.param(dataObj));
        };
    });
    // Detail
    $('.btnDetailz').on('click', function () {
        // 取得訂單的詳細情況
        let numz = $(this).parents('tr').data('numz');
        let xhr = new XMLHttpRequest();
        xhr.onload = function () {
            if (xhr.status == 200) {
                if (xhr.responseText !== "" && xhr.responseText !== "[]") {
                    let callBackData = JSON.parse(xhr.responseText);
                    // 捐款者
                    namez.html(callBackData.buyerName);
                    // 隸屬地區
                    areaz.html(areasDisplay(callBackData.affiliatedArea));
                    // 聯絡地址
                    if (callBackData.address1 != "" && callBackData.address1 != "[]") {
                        concatz = "";
                        addrArr = JSON.parse(callBackData.address1);
                        for (let i = 0; i < addrArr.length; i++) {
                            concatz += addrArr[i];
                        };
                    } else {
                        addrArr = [];
                        concatz = notz;
                    };
                    concatAddrz.html(concatz);
                    // 聯絡電話
                    phonez.html(callBackData.buyerPhone);
                    // 手機
                    if (callBackData.mobilePhone) {
                        cellz.html(callBackData.mobilePhone);
                    } else {
                        cellz.html(notz);
                    };
                    // 電子信箱
                    mailz.html(callBackData.buyerEmail);
                    // 捐款項目
                    html = "";
                    for (let i = 0; i < callBackData.DonateRelatedItemRecord.length; i++) {
                        if (callBackData.DonateRelatedItemRecord.qty !== "") {
                            qtyz = `${callBackData.DonateRelatedItemRecord[i].qty}`;
                        } else {
                            qtyz = notz;
                        }
                        html += `
                                <div data-numz="${callBackData.DonateRelatedItemRecord[i].orderId}" class="items">
                                    <div class="parts">
                                        <div class="heads">
                                            <p class="mainTitle">${callBackData.DonateRelatedItemRecord[i].title}</p>
                                        </div>
                                    </div>
                                    <div class="parts">
                                        <div class="qties">
                                            <div class="names">
                                                <p class="mainTitle">數量</p>
                                            </div>
                                            <p class="qty">${qtyz}</p>
                                        </div>
                                    </div>
                                    <div class="parts">
                                        <div class="subTotals">
                                            <div class="names">
                                                <p class="mainTitle">小計</p>
                                            </div>
                                            <p class="subTotal">
                                                <span>${thousands(callBackData.DonateRelatedItemRecord[i].amount)}</span>
                                                <span>元</span>
                                            </p>
                                        </div>
                                    </div>
                                </div> 
                            `;
                    };
                    recordz.html(html);
                    // 備註說明
                    if (callBackData.remark !== "") {
                        notez.html(callBackData.remark);
                    } else {
                        notez.html(notz);
                    };
                    // 付款方式
                    payMethodz.data('numz', callBackData.payType);
                    payMethodz.html(callBackData.payTypeName.name);
                    // 付款期限
                    if (callBackData.PayEndDate !== null) {
                        payEndDatez.html(`${callBackData.PayEndDate.split('T')[0]} 前 需要繳款完成`);
                    } else {
                        payEndDatez.html(immediatez);
                    }
                    // 開立收據
                    if (callBackData.needReceipt == "1") {
                        receiptz.html(truez);
                        receiptTitlez.html(callBackData.receiptTitle);
                    } else {
                        receiptz.html(falsez);
                        receiptTitlez.html(notz);
                    };
                    // 寄件地址
                    if (callBackData.address3 != "" && callBackData.address3 != "[]") {
                        sendz = "";
                        addrArr = JSON.parse(callBackData.address3);
                        for (let i = 0; i < addrArr.length; i++) {
                            sendz += addrArr[i];
                        };
                    } else {
                        addrArr = [];
                        sendz = notz;
                    };
                    sendAddrz.html(sendz);
                    // 捐款金額
                    amountz.html(thousands(callBackData.amount));
                    // 是否同意公開資訊
                    if (callBackData.needAnonymous) {
                        agreez.html(truez);
                    } else {
                        agreez.html(falsez);
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
// NOTFOUND
function fails() {
    html = "";
    html = `
        <tr class="none">
            <td colspan="8">
                <span>目前沒有任何的內容。</span>
            </td>
        </tr>
    `;
    list.html(html);
    paginations.find('.pagez').html(curPage(current, pageLens, pageCount));
    // 資料載入完成後
    loadingCompleted();
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
    let dataObj = {
        "Methods": "GET",
        "APIs": URL,
        "CONNECTs": CONNECT,
        "QUERYs": `${CONNECT}?count=${listSize}&page=${pageRcd}`,
        "Counts": listSize,
        "Sends": "",
    };
    init(dataObj);
    // Cancel
    $(document).on('click', '.btnCancelz, .close', function () {
        let trz = $(this).parents('.modal-content');
        $.map(trz.find('.editInput'), function (item, index) {
            if (item.value !== "") {
                return check = false;
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
                dataObj.CONNECTs = `${CONNECT}?DonateType=${clsNum}&BuyerName=${clsName}`;
            } else if (clsNum !== "" || clsName !== "") {
                dataObj.CONNECTs = `${CONNECT}?DonateType=${clsNum !== "" ? clsNum : ""}&BuyerName=${clsName !== "" ? clsName : ""}`;
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
                                dataObj.QUERYs = `${CONNECT}?count=${listSize}&page=${pageRcd}&DonateType=${clsNum}&BuyerName=${clsName}`;
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
                                dataObj.QUERYs = `${CONNECT}?count=${listSize}&page=${pageRcd}&DonateType=${clsNum}&BuyerName=${clsName}`;
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
                                    dataObj.QUERYs = `${CONNECT}?count=${listSize}&page=${pageRcd}&DonateType=${clsNum}&BuyerName=${clsName}`;
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