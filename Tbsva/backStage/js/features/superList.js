//
let methodz = '02', setUpz = '',
    insert = `
        <a class="btn btn-primary ml-2" href="./supervisor_02.html">
            <i class="fad fa-file-plus"></i> 新增成員
        </a>
    `;
// 宣告要帶入的欄位
let list = $('.list'), statusz;
let btnSearchz = $('.btnSearchz'), btnAllz = $('.btnAllz'), classifyz = $('.classifyz'), searchz = $('.searchz');
CONNECT = "articleContent";
// 接收資料，做渲染、處理
function process(data) {
    html = "";
    for (let i = 0; i < data.length; i++) {
        // 狀態
        if (data[i].enabled == "1") {
            statusz = '<span class="text-success">開啟</span>';
        } else {
            statusz = '<span class="text-danger">關閉</span>';
        };
        html += `
            <tr data-numz="${data[i].id}">
                <td data-title="編號" class="text-center">
                    <div>${i + 1}</div>
                </td>
                <td data-title="建立日期" class="fieldLefts">
                    <div class="fullFields">${dateChange(data[i].creationDate)}</div>
                </td>
                <td data-title="理事職位" class="fieldLefts">
                    <div class="fullFields">
                        <p class="mb-0 abridged1">${data[i].title}</p>
                    </div>
                </td>
                <td data-title="理事姓名" class="fieldLefts">
                    <div class="fullFields">
                        <p class="mb-0 abridged1">${data[i].subtitle}</p>
                    </div>
                </td>
                <td data-title="類別" class="text-center fieldLefts" data-numz="${data[i].articleCategoryId}">
                    <div class="fullFields">
                        <p class="mb-0 classify">${data[i].articleCategoryName}</p>
                    </div>
                </td>
                <td data-title="狀態" class="text-center"><span data-statusz="${data[i].enabled}">${statusz}</span></td>
                <td data-title="設定">
                    <div class="btns">${setUpz}</div>
                </td>
            </tr>
        `;
    };
    list.html(html);
    // Edit
    $('.btnEditz').on('click', function (e) {
        e.preventDefault();
        if (idz) {
            // 點擊編輯後，將要編輯的編號儲存於瀏覽器（LocalStorage 或 SessionStorage）
            let supNum = $(this).parents('tr').data('numz');
            localStorage.setItem("supNum", supNum);
            let numz = localStorage.getItem("supNum");
            if (numz) { // 確認有將消息編號存入 Storage
                location.href = "./supervisor_04.html"; // 跳轉至編輯頁面
            };
        };
    });
    // Delete
    $('.btnDelz').on('click', function (e) {
        e.preventDefault(); // 取消 a 預設事件
        let numz = $(this).parents('tr').data('numz'); // 取得要刪除的
        if (confirm("您確定要刪除嗎？")) {
            let xhr = new XMLHttpRequest();
            xhr.onload = function () {
                if (xhr.status == 200 || xhr.status == 204) {
                    alert('刪除成功！');
                    location.reload();
                } else {
                    alert("錯誤訊息 " + xhr.status + "：您的連線異常，請重新登入！");
                };
            };
            xhr.open('DELETE', `${URL}${CONNECT}/${numz}`, true)
            xhr.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded');
            xhr.send(null);
        };
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
            <td colspan="7" class="txt-left none">
                <span>目前沒有任何的內容。</span>
            </td>
        </tr>
    `;
    list.html(html);
    paginations.find('div').html(curPage(current, pageLens, pageCount));
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
    // 清除 localStorag 中可能留著的 supNum
    if (localStorage.getItem('supNum')) {
        localStorage.removeItem('supNum');
    };
    // 類別
    let clsObj = {
        "Methods": "GET",
        "APIs": URL,
        "CONNECTs": `ArticleCategory`,
        "QUERYs": "",
        "Counts": listSize,
        "Sends": "",
    };
    getPageDatas(clsObj).then(res => {
        // DOSOMETHING
        html = `<option value="preset" selected>選擇類別 - </option>`;
        if (res !== null) {
            for (let i = 0; i < res.length; i++) {
                html += `
                    <option value="${res[i].id}">${res[i].name}</option>
                `;
            };
        };
        classifyz.html(html);
    }, rej => {
        if (rej == "NOTFOUND") {
            html = `<option value="preset" selected>選擇類別 - </option>`;
            classifyz.html(html);
        };
    });
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
                dataObj.CONNECTs = `${CONNECT}?articleCategoryId=${clsNum}&BuyerName=${clsName}`;
            } else if (clsNum !== "" || clsName !== "") {
                dataObj.CONNECTs = `${CONNECT}?articleCategoryId=${clsNum !== "" ? clsNum : ""}&BuyerName=${clsName !== "" ? clsName : ""}`;
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
                                dataObj.QUERYs = `${CONNECT}?count=${listSize}&page=${pageRcd}&articleCategoryId=${clsNum}&BuyerName=${clsName}`;
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
                                dataObj.QUERYs = `${CONNECT}?count=${listSize}&page=${pageRcd}&articleCategoryId=${clsNum}&BuyerName=${clsName}`;
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
                                    dataObj.QUERYs = `${CONNECT}?count=${listSize}&page=${pageRcd}&articleCategoryId=${clsNum}&BuyerName=${clsName}`;
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