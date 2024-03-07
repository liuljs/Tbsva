let list = $('.list'), notez;
let searchz = $('.searchz'), btnSearchz = $('.btnSearchz'), btnAllz = $('.btnAllz');
CONNECT = 'addTbsva';
// 接收資料，做渲染、處理
function process(data) {
    html = "";
    for (let i = 0; i < data.length; i++) {
        if (data[i].notez !== "") {
            notez = `${data[i].notez}`;
        } else {
            notez = " - ";
        }
        html += `
            <tr>
                <td data-title="會員編號">
                    <p title="${data[i].tId}">${data[i].tId}</p>
                </td>
                <td data-title="會員名稱">
                    <p class="abridged1" title="${data[i].namez}">${data[i].namez}</p>
                </td>
                <td data-title="隸屬地區">
                    <p title="${areasDisplay(data[i].affiliatedAreaz)}">${areasDisplay(data[i].affiliatedAreaz)}</p>
                </td>
                <td data-title="備註">
                    <p class="abridged2" title="${notez}">${notez}</p>
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
            <td colspan="4">
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
        return check = false, errorText = '請確認查詢的會員名稱有確填寫！';
    } else {
        return check = true, errorText = '';
    };
};
//
function init(dataObj) {
    // 1. 使用搜尋，清空目前記錄的篩選 && 每次使用都要清除頁碼紀錄
    clsRcd = "", pageRcd = "";
    // 2. 重置條件
    dataObj.CONNECTs = `${CONNECT}`;
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
        if (rej == "NOTFOUND") { };
    });
}
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
            let clsName = searchz.val().trim() !== "" ? searchz.val().trim() : "";
            dataObj.CONNECTs = `${CONNECT}?count=${listSize}&page=${pageRcd}&namez=${clsName}`;
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
                                dataObj.QUERYs = `${CONNECT}?count=${listSize}&page=${pageRcd}&namez=${clsName}`; // 篩選的條件
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
                                        alert("錯誤訊息 " + xhr.status + "：您的連線已逾期，請重新登入！");
                                        location.reload();
                                    };
                                });
                            };
                        } else {
                            if (clsName !== clsRcd) {
                                // 1.重設頁碼紀錄為 Current && 重新記錄目前的分類
                                clsRcd = clsName, pageRcd = current;
                                // 2. 條件
                                dataObj.QUERYs = `${CONNECT}?count=${listSize}&page=${pageRcd}&namez=${clsName}`; // 篩選的條件
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
            searchz.val(''); // 清空搜尋欄位
            init(dataObj);
        };
    });
});