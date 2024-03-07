// MISSIONARY
let list = $('.list'), categoryz = '法師';
CONNECT = 'knowledge2';
// 接收資料，做渲染、處理
function process(data) {
    html = "";
    for (let i = 0; i < data.length; i++) {
        html += `
            <tr>
                <td data-title="名稱">
                    <p class="abridged1" title="${data[i].title}">${data[i].title}</p>
                </td>
                <td data-title="位階">
                    <p title="${data[i].category}">${data[i].category}</p>
                </td>
                <td data-title="備註">
                    <p title="${data[i].content}">${data[i].content}</p>
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

    `;
    list.html(html);
    paginations.find('.pagez').html(curPage(current, pageLens, pageCount));
};
$(() => {
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
                            if (rej == "NOTFOUND") { };
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
                            if (rej == "NOTFOUND") { };
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
        };
    });
});