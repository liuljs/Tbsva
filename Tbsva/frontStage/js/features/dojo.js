// ACTIVITY
let googleMaps = $('.googleMaps'), list = $('.list'), tabz = $('.tabz');
let iconz, mapInfoz, morez, movez, addrs, addrArr = new Array();
CONNECT = "taiwanDojo", TABCONNECT = "taiwanDojo/category";
// 接收資料，做渲染、處理
function process(data) {
    html = "";
    for (let i = 0; i < data.length; i++) {
        // 地圖
        if (data[i].video !== "") {
            movez = `
                <a target="_blank" class="control btnSecond btnShowMapz">
                    <span>地圖</span>
                    <span class="circle"><i class="fad fa-map-marked-alt"></i></span>
                </a>
            `;
            mapInfoz = `
                <a href="${data[i].video}" target="_blank" class="control btnSecond">
                    <span>連結</span>
                    <span class="circle"><i class="fad fa-external-link"></i></span>
                </a>
            `;
        } else {
            movez = `
                <a target="_blank" class="control btnSecond btnShowMapz hiddens">
                    <span>地圖</span>
                    <span class="circle"><i class="fad fa-map-marked-alt"></i></span>
                </a>
            `;
            mapInfoz = `
                <a href="${data[i].video}" target="_blank" class="control btnSecond hiddens">
                    <span>連結</span>
                    <span class="circle"><i class="fad fa-external-link"></i></span>
                </a>
            `;
        };
        // 小圖示 || 資訊按鈕
        if (data[i].icon !== 0) {
            // 小圖示
            // iconz = `
            //     <span class="images moreInfos">
            //         <i class="fas fa-map-marker-alt"></i>
            //     </span>
            // `;
            // 資訊按鈕
            morez = `
                <a href="./dojPagins.html?id=${data[i].taiwanDojoId}" class="control btnSecond btnInfo">
                    <span>資訊</span>
                    <span class="circle"><i class="fad fa-info-circle"></i></span>
                </a>
            `;
        } else {
            // 小圖示
            // iconz = "";
            // 資訊按鈕
            morez = `
                <a href="./dojPagins.html?id=${data[i].taiwanDojoId}" class="control btnSecond btnInfo hiddens">
                    <span>資訊</span>
                    <span class="circle"><i class="fad fa-info-circle"></i></span>
                </a>
            `;
        };
        // Show Address
        if (data[i].address != "" && data[i].address != "[]") {
            addrs = "";
            addrArr = JSON.parse(data[i].address);
            for (let i = 0; i < addrArr.length; i++) {
                addrs += addrArr[i];
            };
        } else {
            addrArr = [];
            addrs = `<span> - </span>`;
        };
        html += `
            <tr data-numz="${addrs}">
                <td data-title="名稱">
                    <div>
                        <span class="images moreInfos">
                            <i class="fas fa-map-marker-alt"></i>
                        </span>
                        <p class="abridged1 titlez" title="${data[i].name}">${data[i].name}</p>
                    </div>
                </td>
                <td data-title="地區">
                    <p class="abridged1" title="${data[i].categoryName}">${data[i].categoryName}</p>
                </td>
                <td data-title="地址">
                    <p title="${addrs}">${addrs}</p>
                </td>
                <td data-title="功能">
                    <div class="btns">
                        <div class="controls">
                            ${movez}${mapInfoz}${morez}
                        </div>
                    </div>
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
    // 如果資料數為 0 筆，則為 NOTFOUND 
    clsRcd = "NOTFOUND";
};
$(function () {
    // Classification
    let dataObj = {
        "Methods": "GET",
        "APIs": URL,
        "CONNECTs": TABCONNECT,
        "QUERYs": "",
        "Counts": "",
        "Sends": "",
    };
    getPageDatas(dataObj).then(res => {
        // DOSOMETHING
        html = `
            <li>
                <a data-numz="all">
                    <i class="fad fa-map-marker-alt"></i>
                    <span>所有地區</span>
                </a>
            </li>
        `;
        if (res !== null) {
            for (let i = 0; i < res.length; i++) {
                html += `
                    <li>
                        <a data-numz="${res[i].categoryId}">
                            <i class="fad fa-map-marker-alt"></i>
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
                                        // 4. 產生新分頁器
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

// Google
function init() {
    let centerz = { lat: 23.9663957, lng: 120.6894082 }; // 中心點：起始位子
    let geocoder = new google.maps.Geocoder();
    let mapz = new google.maps.Map(
        document.getElementById('googleMap'), {
        center: centerz,
        zoom: 15, // 1 - 20 數字愈大，地圖愈詳細；反之愈小，地圖愈全面
        mapTypeId: 'roadmap',
        /*
            roadmap 顯示默認道路地圖視圖。
            satellite 顯示 Google 地球衛星圖像。
            hybrid 顯示正常和衛星視圖的混合。
            terrain 顯示基於地形信息的物理地圖。
        */
        clickableIcons: false

    });
    // 渲染各道場的資訊的標記以及 infoWindow
    let dataObj = {
        "Methods": "GET",
        "APIs": URL,
        "CONNECTs": CONNECT,
        "QUERYs": "",
        "Counts": "",
        "Sends": "",
    };
    getPageDatas(dataObj).then(res => {
        if (res.length > 0 && res !== null) {
            Array.prototype.forEach.call(res, i => {
                let latLng = new google.maps.LatLng(i.longitude, i.latitude);
                let markerz = new google.maps.Marker({
                    position: latLng,
                    map: mapz,
                    animation: google.maps.Animation.DROP, // DROP掉下來、BOUNCE一直彈跳
                    // title: 'title', // 顯示 title 
                    draggable: false // true、false可否拖拉
                });
                // Show Address
                if (i.address != "" && i.address != "[]") {
                    addrs = "";
                    addrArr = JSON.parse(i.address);
                    for (let i = 0; i < addrArr.length; i++) {
                        addrs += addrArr[i];
                    };
                } else {
                    addrArr = [];
                    addrs = `<span> - </span>`;
                };
                //
                let infoWindowz = new google.maps.InfoWindow({
                    content: `
                        <fieldset class="fieldsets">
                            <legend class="legends">
                               <p class="mainTitle">${i.name}</p>
                            </legend>
                            <div class="infoWindow">
                                <div class="texts">
                                    <ul class="">
                                        <li>
                                            <div class="text">
                                                <p class="outset">地區</p>
                                                <p class="infos">${i.categoryName}</p>
                                            </div>
                                        </li>
                                        <li>
                                            <div class="text">
                                                <p class="outset">地點</p>
                                                <p class="infos">${addrs}</p>
                                            </div>
                                        </li>
                                    </ul>
                                </div>
                            </div>
                        </fieldset>
                    `, // 支援 html 標籤
                });
                markerz.addListener("mouseover", function () {
                    infoWindowz.open({
                        anchor: markerz,
                        map: mapz,
                        shouldFocus: true
                    });
                });
                markerz.addListener("mouseout", function () {
                    infoWindowz.close();
                });
                markerz.addListener("click", function () {
                    location.href = `./dojPagins.html?id=${i.taiwanDojoId}`;
                });
                // 點擊即顯示於地圖
                $(document).on('click', `.list tr[data-numz="${addrs}"] .btnShowMapz`, function () {

                    let address = $(this).parents('tr').data('numz');

                    $(window).scrollTop(googleMaps.offset().top - 80);
                    // 定位
                    geocode({ address: address });
                    // 顯示資訊
                    infoWindowz.open({
                        anchor: markerz,
                        map: mapz,
                        shouldFocus: true
                    });
                });
            });
        };
    });
    //
    function geocode(request) {
        geocoder
            .geocode(request)
            .then((result) => {
                const { results } = result;

                mapz.setCenter(results[0].geometry.location);

                return results;
            })
            .catch((e) => {
                alert("Geocode was not successful for the following reason: " + e);
            });
    };
};

// google.maps.event.addDomListener(
//     window, 'load', init);

// 非同步載入（Asynchronously Loading）API
function loadScript(ks) {
    let scriptz = document.createElement('script');
    scriptz.type = 'text/javascript';
    scriptz.src = `https://maps.googleapis.com/maps/api/js?key=${ks}&v=3.exp&callback=init`;

    document.body.appendChild(scriptz);
};
let ks = "AIzaSyAH4VbojiSt4WLdZbfC1nKGfOJrsR3zyGc";
window.onload = loadScript(ks);
