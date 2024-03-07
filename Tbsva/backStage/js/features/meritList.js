//
let methodz = '02', setUpz = '',
    insert = `
        <a class="btn btn-primary" href="./annualMerit_01.html">
            <i class="fad fa-file-plus"></i> 新增方案
        </a>
     `;
// 宣告要帶入的欄位
let list = $('.list'), topz, statusz;
CONNECT = 'video2';
// 接收資料，做渲染、處理
function process(data) {
    html = "";
    for (let i = 0; i < data.length; i++) {
        // 顯示
        if (data[i].first == 1) {
            topz = '<span class="text-primary">顯示</span>';
        } else {
            topz = '<span> - </span>';
        };
        // 狀態
        if (data[i].enabled == 1) {
            statusz = '<span class="text-success">開啟</span>';
        } else {
            statusz = '<span class="text-danger">關閉</span>';
        };
        html += `
            <tr data-numz="${data[i].video2Id}">
                <td data-title="編號" class="text-center">
                    <div>${data[i].id}</div>
                </td>
                <td data-title="建立日期" class="fieldLefts">
                    <div class="fullFields">${dateChange(data[i].createDate)}</div>
                </td>
                <td data-title="方案標題" class="fieldLefts">
                    <div class="fullFields">
                        <p class="mb-0 abridged1">${data[i].name}</p>
                    </div>
                </td>
                <td data-title="顯示" class="text-center">
                    <span data-topz="${data[i].first}">${topz}</span>
                </td>                
                <td data-title="狀態" class="text-center">
                    <span data-statusz="${data[i].enabled}">${statusz}</span>
                </td>
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
            // 點擊編輯後，將要編輯的消息編號儲存於瀏覽器（LocalStorage 或 SessionStorage）
            let merNum = $(this).parents('tr').data('numz');
            localStorage.setItem("merNum", merNum);
            let numz = localStorage.getItem("merNum");
            if (numz) { // 確認有將消息編號存入 Storage
                location.href = "./annualMerit_03.html"; // 跳轉至編輯頁面
            };
        };
    });
    // Delete
    $('.btnDelz').on('click', function (e) {
        e.preventDefault(); // 取消 a 預設事件
        let numz = $(this).parents('tr').data('numz');
        let dataObj = {
            "video2Id": numz
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
    // 資料載入完成後
    loadingCompleted();
};
// NOTFOUND
function fails() {
    html = "";
    html = `
        <tr class="none">
            <td colspan="6" class="txt-left none">
                <span>目前沒有任何的內容。</span>
            </td>
        </tr>
    `;
    list.html(html);
    paginations.find('.pagez').html(curPage(current, pageLens, pageCount));
    // 資料載入完成後
    loadingCompleted();
};
$().ready(function () {
    // 清除 localStorag 中可能留著的 merNum
    if (localStorage.getItem('merNum')) {
        localStorage.removeItem('merNum');
    };
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
                        pageRcd = pageRcd
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
});