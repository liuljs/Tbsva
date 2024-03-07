// DONATESEARCHS
let list = $('.list');
CONNECT = 'Donate/Donations';

let searchz = $('.searchz'), btnSearchz = $('.btnSearchz'), btnAllz = $('.btnAllz');
// 接收資料，做渲染、處理
function process(data) {
    html = "";
    for (let i = 0; i < data.length; i++) {
        html += `
            <tr>
                <td data-title="捐贈人名稱">
                    <p>${data[i].buyerName}</p>
                </td>
                <td data-title="隸屬地區">
                    <p>${areasDisplay(data[i].affiliatedArea)}</p>
                </td>
                <td data-title="捐贈項目">
                    <p>${donateTypeDisplay(data[i].donateType)}</p>
                </td>
                <td data-title="捐贈時間">
                    <p>${dateChangeSlash(data[i].orderDate)}</p>
                </td>
                <td data-title="捐贈金額">
                    <div class="units">
                        <div class="unit">
                            <span class="prices">${thousands(data[i].amount)}</span>
                            <span>元</span>
                        </div>
                    </div>
                </td>
                <td data-title="-">
                    <p></p>
                </td>
            </tr>
        `;
    };
    list.html(html);
};
// NOTFOUND
function fails() {
    html = "";
    html = `
        <tr class="none">
            <td colspan="6">
                <div class="">    
                    <p>
                        目前沒有任何的內容〈<span class="btnReturn btnReturnz">點擊返回</span>〉。
                    </p>
                </div>
            </td>
        </tr>
    `;
    list.html(html);
    paginations.find('.pagez').html(curPage(current, pageLens, pageCount));
    // return
    $('.btnReturnz').on('click', function () {
        let dataObj = {
            "Methods": "GET",
            "APIs": URL,
            "CONNECTs": CONNECT,
            "QUERYs": `${CONNECT}?count=${listSize}&page=${pageRcd}`,
            "Counts": listSize,
            "Sends": "",
        };
        init(dataObj);

        searchz.val(''); // 清空搜尋欄位
    });
};
// 類別查詢驗證
function categoryCheck(title) {
    if (title.val().trim() === '') {
        return check = false, errorText = '請確認查詢的捐款人名稱有確填寫！';
    } else {
        return check = true, errorText = '';
    };
};
function init(dataObj) {
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
                        // 1. 產生分頁器
                        paginations.find('.pagez').html(curPage(pageRcd, pageLens, pageCount));
                        pageRcd = pageRcd // 紀錄當下頁碼
                        // 2. 取得點擊頁碼後要呈現的內容(要呈現的筆數)
                        dataObj.QUERYs = `${CONNECT}?count=${listSize}&page=${pageRcd}`;
                        getPageDatas(dataObj).then(res => {
                            // DO SOMETHING
                            if (res !== null) {
                                process(res);
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
                        // 1. 產生分頁器
                        paginations.find('.pagez').html(curPage(num, pageLens, pageCount));
                        pageRcd = num // 記錄當下頁碼
                        // 2. 取得點擊頁碼後要呈現的內容(要呈現的筆數)
                        dataObj.QUERYs = `${CONNECT}?count=${listSize}&page=${num}`;
                        getPageDatas(dataObj).then(res => {
                            // DO SOMETHING
                            if (res !== null) {
                                process(res);
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
$(() => {
    //
    let dataObj = {
        "Methods": "GET",
        "APIs": URL,
        "CONNECTs": CONNECT,
        "QUERYs": `${CONNECT}?count=${listSize}&page=${pageRcd}`,
        "Counts": listSize,
        "Sends": "",
    };
    init(dataObj);
    // Searchs
    btnSearchz.on('click', function (e) {
        e.preventDefault();
        categoryCheck(searchz);
        if (check == true) {
            let clsName = searchz.val() !== "" ? searchz.val() : "";
            dataObj.CONNECTs = `${CONNECT}?count=${listSize}&page=${pageRcd}&BuyerName=${clsName}`;
            clsRcd = "", pageRcd = ""; // 使用搜尋，清空目前記錄的篩選 && 每次使用都要清除頁碼紀錄
            getTotalPages(dataObj).then(res => {
                if (res !== 0) {
                    pageLens = res; // 目前總頁數
                    paginations.find('.pagez').html(curPage(current, pageLens, pageCount));
                    // 點擊頁碼
                    paginations.unbind('click').on('click', 'li', function (e) {
                        e.preventDefault(), e.stopImmediatePropagation();
                        let num = $(this).find('a').data('num');
                        dataObj.Sends["count"] = dataObj.Counts; // 每次列出筆數
                        dataObj.QUERYs = `${CONNECT}?count=${listSize}&page=${pageRcd}&BuyerName=${clsName}`; // 篩選的條件
                        if (isNaN(num)) {
                            if (!$(this).hasClass("disabled")) {
                                if (num == "prev") {
                                    pageRcd--;
                                } else if (num == "next") {
                                    pageRcd++;
                                };
                                // 1. 產生分頁器
                                paginations.find('.pagez').html(curPage(pageRcd, pageLens, pageCount));
                                // 2. 取得點擊頁碼後要呈現的內容
                                pageRcd = pageRcd, dataObj.Sends.page = pageRcd; // 紀錄當下頁碼 || 上下頁：以記錄的頁碼來做拋接值
                                getPageDatas(dataObj).then(res => {
                                    // DO SOMETHING
                                    if (res !== null) {
                                        process(res);
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
                            if (clsName !== clsRcd) {
                                paginations.find('.pagez').html(curPage(current, pageLens, pageCount));
                                clsRcd = clsName, pageRcd = current; // 重設頁碼紀錄為 Current && 重新記錄目前的分類
                                // 2. 取得點擊頁碼後要呈現的內容(要呈現的筆數)
                                getPageDatas(dataObj).then(res => {
                                    // DO SOMETHING
                                    if (res !== null) {
                                        process(res);
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
                                    // 1. 產生分頁器
                                    paginations.find('.pagez').html(curPage(num, pageLens, pageCount));
                                    // 2. 取得點擊頁碼後要呈現的內容(要呈現的筆數)
                                    pageRcd = num, dataObj.Sends.page = num // 記錄當下頁碼 || 傳送的頁碼
                                    getPageDatas(dataObj).then(res => {
                                        // DO SOMETHING
                                        if (res !== null) {
                                            process(res);
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
                    clsRcd = clsName;
                };
            }, rej => {
                if (rej == "NOTFOUND") {
                    fails();
                    clsRcd = clsName;
                };
            });
        } else {
            alert(errorText);
            searchz.val(''); // 清空
        };
    });
    // All
    btnAllz.on('click', function (e) {
        e.preventDefault();
        if (clsRcd !== "" && clsRcd !== undefined) {
            clsRcd = "", pageRcd = ""; // 使用搜尋，清空目前記錄的篩選 && 每次使用都要清除頁碼紀錄
            dataObj.CONNECTs = `${CONNECT}`;
            dataObj.QUERYs = `${CONNECT}?count=${listSize}&page=${pageRcd}`;
            init(dataObj);
            searchz.val(''); // 清空搜尋欄位
        };

    });
});