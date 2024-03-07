// ACTIVITY
let list = $('.list'), tabz = $('.tabz');
CONNECT = "activity", TABCONNECT = "activity/category";
// 接收資料，做渲染、處理
function process(data) {
    html = "";
    for (let i = 0; i < data.length; i++) {
        html += `
            <div class="items border">
                <a href="./activity.html?id=${data[i].activityId}" class="item border">
                    <div class="parts border">
                        <div class="images">
                            <img src="./img/elements/samples/sample540x450.jpg">
                            <img src="${data[i].cover}?activity${paramRandoms()}" class="cover">
                        </div>
                    </div>
                    <div class="parts border">
                        <div class="dates">
                            <div class="categories">
                                <p class="category">${data[i].categoryName}</p>
                                <div class="decRows"></div>
                            </div>
                            <p class="date">${dateChangeSlash(data[i].createDate)}</p>
                        </div>
                        <div class="texts">
                            <p class="mainTitle abridged2" title="${data[i].name}">${data[i].name}</p>
                        </div>
                    </div>
                </a>
            </div>
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
    // 如果資料數為 0 筆，則為 NOTFOUND 
    clsRcd = "NOTFOUND";
};
$(() => {
    //
    let dataObj = {
        "Methods": "GET",
        "APIs": URL,
        "CONNECTs": TABCONNECT,
        "QUERYs": "",
        "Counts": "",
        "Sends": "",
    };
    // Classification
    getPageDatas(dataObj).then(res => {
        // DOSOMETHING
        html = `
            <li>
                <a data-numz="all">
                    <i class="fad fa-sun-cloud"></i>
                    <span>所有活動</span>
                </a>
            </li>
        `;
        if (res !== null) {
            for (let i = 0; i < res.length; i++) {
                html += `
                    <li>
                        <a data-numz="${res[i].categoryId}">
                            <i class="fad fa-sun-cloud"></i>
                            <span>${res[i].name}</span>
                        </a>
                    </li>
                `;
            };
        };
        tabz.html(html);
        tabz.find('li').on('click', function (e) {
            e.preventDefault(), e.stopImmediatePropagation();
            $(this).addClass('active').siblings().removeClass('active');
            let clsNum = $(this).find('a').data('numz');
            if (clsNum == 'all') {
                clsNum = "";
            };
            // 判斷點擊
            if (clsNum !== clsRcd) {
                pageRcd = ""; // 重置頁碼紀錄
                dataObj = {
                    "Methods": "GET",
                    "APIs": URL,
                    "CONNECTs": `${CONNECT}?categoryId=${clsNum}`,
                    "QUERYs": `${CONNECT}?categoryId=${clsNum}&count=${listSize}&page=${current}`,
                    "Counts": listSize,
                    "Sends": "",
                };
                // 產生第一次的分頁器 (產生以此分類為主的所有筆數)
                getTotalPages(dataObj).then(res => {
                    pageLens = res; // 目前總頁數
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
                                dataObj.QUERYs = `${CONNECT}?categoryId=${clsRcd}&count=${listSize}&page=${pageRcd}`; // 條件：以此分類為主且目前點擊的頁碼要呈現的內容
                                // 3. 取得點擊頁碼後要呈現的內容
                                getPageDatas(dataObj).then(res => {
                                    // DOSOMETHING
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
                            if (clsNum !== clsRcd) {
                                // 1. 重設頁碼紀錄為 Current && 重新記錄目前的分類
                                clsRcd = clsNum, pageRcd = current;
                                // 2. 條件
                                dataObj.QUERYs = `${CONNECT}?categoryId=${clsRcd}&count=${listSize}&page=${current}`; // 條件：以此分類為主且目前點擊的頁碼要呈現的內容
                                // 3. 取得點擊頁碼後要呈現的內容(要呈現的筆數)
                                getPageDatas(dataObj).then(res => {
                                    // DOSOMETHING
                                    if (res !== null) {
                                        process(res);
                                        // 4. 產生新分頁
                                        paginations.find('.pagez').html(curPage(current, pageLens, pageCount));
                                    } else {
                                        fails();
                                    };
                                }, rej => {
                                    if (rej == "NOTFOUND") {
                                        fails();
                                    };
                                });
                            } else {
                                if (num !== pageRcd) { // 如果不是點同一頁碼的話
                                    // 1. 記錄當下頁碼
                                    pageRcd = num;
                                    // 2. 條件
                                    dataObj.QUERYs = `${CONNECT}?categoryId=${clsNum}&count=${listSize}&page=${num}`; // 條件：以此分類為主且目前點擊的頁碼要呈現的內容
                                    // 3. 取得點擊頁碼後要呈現的內容(要呈現的筆數)
                                    getPageDatas(dataObj).then(res => {
                                        // DOSOMETHING
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
                                            fails();
                                        };
                                    });
                                } else { };
                            }
                        };
                    });
                    paginations.find('.pagez li:first-child').trigger('click');
                }, rej => {
                    if (rej == "NOTFOUND") {
                        fails();
                    };
                });
            } else { };
        });
        tabz.find('li').eq(0).trigger('click');
    });
});