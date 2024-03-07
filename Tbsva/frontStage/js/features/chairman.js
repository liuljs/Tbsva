// CHAIRMAN
let currentz = $('.currentz');
let list = $('.list');
CONNECT = 'directorIntroduction';
// 非對稱排版
function asymmetryLayouts() {
    let asymmetries = $('.asymmetries'), asymmetry = $('.asymmetry');
    let w = $(window).width() + 17;
    if (w > tabletWidth) {
        asymmetries.addClass('active');
        let pads = asymmetry.height() / 2;
        asymmetries.css('--pads', `${pads}px`);
    } else {
        asymmetries.removeClass('active');
    }
}
// 接收資料，做渲染、處理
function process(data) {
    html = "";
    // 如果有設定現任（現任一定會是第一筆）
    if (data[0].first == "1") {
        // 顯示現任區塊
        currentz.addClass('active');
    };
    for (let i = 0; i < data.length; i++) {
        // 現任
        if (data[i].first == "1") {
            currentz.find('.imagez').attr('src', `${data[i].imageURL}?chirman${paramRandoms()}`);
            currentz.find('.titlez').html(data[i].name);
            currentz.find('.subTitlez').html(data[i].subtitle);
            currentz.find('.contentz').html(data[i].brief);
            currentz.find('.btnMorez').attr('href', `./chaPagins.html?id=${data[i].directorIntroductionId}`);
        } else {
            // 歷屆
            html += `
                <a href="./chaPagins.html?id=${data[i].directorIntroductionId}" class="items">
                    <div class="parts border">
                        <div class="images">
                            <img src="./img/elements/samples/sample500x600.jpg">
                            <img class="cover" src="${data[i].imageURL}?chirman${paramRandoms()}">
                        </div>
                    </div>
                    <div class="parts border">
                        <div class="heads">
                            <p class="mainTitle abridged1" title="${data[i].name}">${data[i].name}</p>
                            <p class="subTitle abridged1" title=""${data[i].subtitle}>${data[i].subtitle}</p>
                        </div>
                        <div class="btns">
                            <div class="btnSecond btnMore">
                                <span>了解更多</span>
                                <span class="circle"><i class="fad fa-info-circle"></i></span>
                            </div>
                        </div>            
                    </div>
                </a>
            `;
        }
    };
    list.html(html);
    asymmetryLayouts();
    // resize
    $(window).on('resize', function () {
        asymmetryLayouts();
    });
};
// NOTFOUND
function fails() {
    html = "";
    html = `

    `;
    list.html(html);
    paginations.find('div').html(curPage(current, pageLens, pageCount));
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
                            if (rej == "NOTFOUND") { };
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