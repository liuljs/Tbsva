// 宣告要帶入的欄位
let list = $('.list');
CONNECT = "video", videoCategory = "1";
let topMoviez = $('.topMoviez'), keyz;
// 接收資料，做渲染、處理
function process(data) {
    topMoviez.addClass('active');
    // 如果帶參數
    if (request("id")) {
        keyz = request("id");
    };
    html = "";
    for (let i = 0; i < data.length; i++) {
        // 如果有參數則顯示於優先頁首
        if (data[i].videoId == keyz) {
            topMoviez.find('.titlez').html(data[i].name);
            topMoviez.find('.categoryz').html(movCategoryDisplay(data[i].videoCategory));
            topMoviez.find('.datez').html(dateChangeSlash(data[i].createDate));
            topMoviez.find('.contentz').html(JSON.parse(data[i].content));
            // topMoviez.find('.iframez').attr('src', JSON.parse(data[i].video));
            topMoviez.find('.iframez').attr('src', data[i].video);
            //
            slideInit('none');
        };
        // 渲染
        html += `
            <a data-numz="${data[i].videoId}" data-srcz="${data[i].video}" class="items itemz">
                <div class="parts">
                    <div class="images">
                        <img src="./img/elements/samples/sample640x480.jpg">
                        <img class="cover imagez" src="${data[i].imageURL}?video${paramRandoms()}">
                    </div>
                </div>
                <div class="parts">
                    <div class="texts">
                        <p class="mainTitle titlez abridged2" title="${data[i].name}">${data[i].name}</p>
                    </div>
                    <div>
                        <div class="categories">
                            <p class="category categoryz">${movCategoryDisplay(data[i].videoCategory)}</p>
                            <div class="decRows"></div>
                        </div>
                        <div class="dates">
                            <p class="date datez">${dateChangeSlash(data[i].createDate)}</p>
                        </div>
                        <input type="hidden" class="contnet contnetz" value="${JSON.parse(data[i].content)}">
                    </div>
                </div>
            </a>
        `;
    };
    list.html(html);
    // 如果有參數
    if (request("id")) {
        // 優先點擊
        list.find(`.items[data-numz="${keyz}"]`).addClass('active').siblings().removeClass('active');
    } else {
        // 沒有則點擊第一筆
        let activez = list.find('.itemz').eq(0);
        activez.addClass('active').siblings().removeClass('active');
        // 顯示第一筆於頁首
        topMoviez.find('.titlez').html(activez.find('.titlez').html());
        topMoviez.find('.categoryz').html(activez.find('.categoryz').html());
        topMoviez.find('.datez').html(activez.find('.datez').html());
        topMoviez.find('.contentz').html(activez.find('.contnetz').val());
        topMoviez.find('.iframez').attr('src', activez.data('srcz'));
        //
        slideInit('none');
    };
    list.find('.itemz').on('click', function () {
        let numz = $(this).data('numz');
        if ($(this).hasClass('active')) {

        } else {
            location.href = `./event_04.html?id=${numz}`;
        };
    });
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
    //
    let dataObj = {
        "Methods": "GET",
        "APIs": URL,
        "CONNECTs": `${CONNECT}?videoCategory=${videoCategory}`,
        "QUERYs": `${CONNECT}?videoCategory=${videoCategory}&count=${listSize}&page=${pageRcd}`,
        "Counts": listSize,
        "Sends": "",
    };
    // 產生第一次的分頁器
    getTotalPages(dataObj).then(res => {
        if (res !== 0) {
            pageLens = res;
            paginations.find('.pagez').html(curPage(current, res, pageCount));
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
                        dataObj.QUERYs = `${CONNECT}?videoCategory=${videoCategory}&count=${listSize}&page=${pageRcd}`;
                        // 3. 取得點擊頁碼後要呈現的內容(要呈現的筆數)
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
                            if (rej == "NOTFOUND") { };
                        });
                    };
                } else {
                    if (num !== pageRcd) { // 如果不是點同一頁碼的話
                        // 1. 記錄當下頁碼
                        pageRcd = num;
                        // 2. 條件
                        dataObj.QUERYs = `${CONNECT}?videoCategory=${videoCategory}&count=${listSize}&page=${num}`;
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
            fails();
        };
    }, rej => {
        if (rej == "NOTFOUND") {
            fails();
        };
    });
});