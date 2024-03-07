// 欄位尾數帶 z 字，用於功能、拋接資訊

// 全域使用 Commons
let idz, localz, module, count, html;
let phoneWidth = 575, tabletWidth = 767, humburgerWidth = 1199;; // 手機斷點 || 平板電腦斷點 || 選單出現點
let URL = "https://tbsva.webshopping.vip/api/", IMGURL = "https://tbsva.webshopping.vip/backStage/img/", CONNECT, TABCONNECT; // API 位置 || 圖片檔位置 || API 名稱 || 類別名稱
let userAgent = navigator.userAgent;
// 判斷是否為 LINE 內建瀏覽器，如果是就加上 "openExternalBrowser=1"
if (userAgent.indexOf("Line") > -1) {
    location.href = window.location.href + "?" + "openExternalBrowser=1";
};
// 篩選、分頁器
let mainLens, pageLens;
let clsRcd, cls1Rcd, cls2Rcd, cls3Rcd, pageRcd, current = 1, listSize = 12, pageCount = 2;

// 驗證資訊
let errorTitle, errorText = "", check = true;
let limit = (1024 * 1024) * 5, imgLimit = 6; // 限制圖片大小 5MB || 圖片數量限制
const PhoneRegExp = /^(\d{2,3}-?|\(\d{2,3}\))\d{3,4}-?\d{4}$/;
const CellRegExp = /^09\d{2}-?(\d{6}|\d{3}-\d{3})$/;
const EmailRegExp = /^([\w]+)(.[\w]+)*@([\w]+)(.[\w]{2,3}){1,2}$/;
const NumberRegExp = /^[0-9]*$/;
const Rules = /^(?=.*[a-zA-Z])(?=.*\d)[a-zA-Z\d]{4,}$/; // 包含大小寫英數字，至少要 4 碼
const RulesSix = /^(?=.*[a-zA-Z])(?=.*\d)[a-zA-Z\d]{6,}$/; // 包含大小寫英數字，至少要 6 碼
const MonthArr = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec']; // 12 個月
// 1. 只能輸入數字
$(document).on('input', '.numberz', function () {
    $(this).val($(this).val().replace(/[^\d].*$/g, ''));
});
//
function dateChange(e) {
    return e.split(" ")[0].replace(/\//g, '').replace(/^(\d{4})(\d{2})(\d{2})$/, "$1-$2-$3");
}
function dateChangeSlash(e) {
    return e.split(" ")[0].replace(/\-/g, '/').replace(/^(\d{4})(\d{2})(\d{2})$/, "$1-$2-$3");
}
// QueryString 
function request(paras) {
    let url = location.href;
    let paraString = url.substring(url.indexOf("?") + 1, url.length).split("&");
    let paraObj = {}
    for (i = 0; j = paraString[i]; i++) {
        paraObj[j.substring(0, j.indexOf("=")).toLowerCase()] = j.substring(j.indexOf("=") + 1, j.length);
    };
    let returnValue = paraObj[paras.toLowerCase()];
    if (typeof (returnValue) == "undefined") {
        return "";
    } else {
        return returnValue;
    };
};
function requestAll(urlz) {
    let url = urlz;
    let paraString = url.substring(url.indexOf("?") + 1, url.length).split("&");
    let paraObj = {}
    if (paraString.length > 1) {
        for (i = 0; j = paraString[i]; i++) {
            paraObj[j.substring(0, j.indexOf("=")).toLowerCase()] = j.substring(j.indexOf("=") + 1, j.length);
        };
        return paraObj
    } else {
        return "";
    }

};
// 亂數參數
function paramRandoms() {
    return Math.floor(Math.random() * 50);
};
// HTML STATUS : 2XX (SUCCESS)
// 200 成功、201 新增成功、202 接受請求，未處裡、203 已處理，反饋可能非來自伺服器、204 已處理，未反饋、205 已處理、未反饋、206 已處理部分 GET 的請求
// FAIL STATUS : || ...
// Send Objects
// dataObj = {
//     "Methods": "", // 方法
//     "APIs": "", // API
//     "CONNECTs": "", // CONNECT 預設 || 總筆數用
//     "QUERYs":"", // 網址式：QUERY 內容篩選條件 (未填則為使用 CONNECTs)
//     "Sends": "", // 物件式：內容篩選條件 (GET 方法可 Null)
//     "Counts": "" // 頁面顯示筆數
// };
// 取得總頁數 ( 1. CONNECT 為全資料總頁數 || 2. QUERY 為篩選的資料總頁數 )
function getTotalPages(dataObj) {
    return new Promise((resolve, reject) => {
        let xhr = new XMLHttpRequest();
        xhr.onload = () => {
            if (xhr.status == 200) {
                if (xhr.responseText !== "" && xhr.responseText !== "[]") {
                    let callBackData = JSON.parse(xhr.responseText);
                    let pages = Math.ceil(callBackData.length / Number(dataObj.Counts)); // 資料總筆數 / 頁面顯示筆數 
                    resolve(pages); // 返回取得總頁數
                } else {
                    resolve(0); // 沒有資料，所以總頁數為 0
                };
            } else {
                reject("NOTFOUND"); // 沒有成功 || 資料筆數為 0 沒有檔案
            };
        };
        xhr.open(`${dataObj.Methods}`, `${dataObj.APIs}${(dataObj.CONNECTs !== "") ? dataObj.CONNECTs : dataObj.QUERYs}`, true);
        xhr.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded');
        xhr.send((dataObj.Sends !== "") ? $.param(dataObj.Sends) : null);
    });
};
// 點擊頁碼取得對應筆數的資料 || 取得內容
function getPageDatas(dataObj) {
    // 1. 動態顯示點擊後的資料
    return new Promise((resolve, reject) => {
        let xhr = new XMLHttpRequest();
        xhr.onload = () => {
            if (xhr.status == 200) {
                if (xhr.responseText !== "" && xhr.responseText !== "[]") {
                    let callBackData = JSON.parse(xhr.responseText);
                    resolve(callBackData); // 返回取得資料
                } else {
                    resolve(null); // 沒有資料
                };
            } else {
                reject("NOTFOUND"); // 沒有成功
            };
        };
        xhr.open(`${dataObj.Methods}`, `${dataObj.APIs}${(dataObj.QUERYs !== "") ? dataObj.QUERYs : dataObj.CONNECTs}`, true);
        xhr.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded');
        xhr.send((dataObj.Sends !== "") ? $.param(dataObj.Sends) : null);
    });
};
// 分頁器 Pagination    
let paginations = $('.pagination');
function curPage(iNow, lens, count) { // 當前頁碼 , 總頁數 , 頁面上產生幾個頁碼

    var pickz = `<li class="page-item active"><a class="page-link" data-num="${iNow}">` + iNow + `</a></li>`; // 指向當前頁碼
    // 迴圈產呈現在頁面上的頁碼，count 控制迴圈數，用以控制產生的頁碼數量
    for (let i = 1; i < count; i++) {
        if (iNow - i > 1) { // 當 iNow - i 小於 1 時，會產生小於 iNow 的頁碼 
            pickz = `<li class="page-item"><a class="page-link" data-num="${iNow - i}">` + (iNow - i) + `</a></li>` + pickz;
        }
        if (iNow + i < lens) { // 當 iNow - i 大於 1 時，會產生大於 iNow 的頁碼 
            pickz = pickz + `<li class="page-item"><a class="page-link" data-num="${iNow + i}">` + (iNow + i) + `</a></li>`;
        }
    }
    if (iNow == 1) { // 當前頁碼是1的時候，上一頁的按鈕鎖定
        paginations.find('li:first-child').addClass('disabled');
    }
    if (iNow - 3 > 0) { // 當 當前頁碼 大於3的話 前面的頁碼 省略
        pickz = `<li class="page-item disabled"><a class="page-link" data-num="none">...</a></li>` + pickz;
    }
    if (iNow > 1) { // 當 當前頁碼 大於1的話 要顯示第1頁的頁碼，並且開啟上一頁按鈕
        pickz = `<li class="page-item"><a class="page-link" data-num="1">1</a></li>` + pickz;
        paginations.find('li:first-child').removeClass('disabled');
    }
    if (iNow + 2 < lens) { // 當前頁碼 的下一頁之後 省略
        pickz = pickz + `<li class="page-item disabled"><a class="page-link" data-num="none">...</a></li>`;
    }
    if (iNow < lens) { // 當前頁碼 小於總頁碼的話 頁顯示最後1頁的頁碼，並且開啟下一頁按鈕
        pickz = pickz + `<li class="page-item"><a class="page-link" data-num="${lens}">` + lens + `</a></li>`;
        paginations.find('li:last-child').removeClass('disabled');
    }
    if (iNow == lens) { // 當 當前頁碼為最後一頁時，鎖定下一頁按鈕
        paginations.find('li:last-child').addClass('disabled');
    }
    return pickz;
};
// 金額每三位數加逗號
function thousands(num) {
    let str = num.toString();
    let reg = str.indexOf(".") > -1 ? /(\d)(?=(\d{3})+\.)/g : /(\d)(?=(?:\d{3})+$)/g;
    return str.replace(reg, "$1,");
};
// 影片類別顯示
function movCategoryDisplay(val) {
    if (val == 1) {
        return "結緣品影片";
    } else if (val == 2) {
        return "活動影片";
    } else if (val == 3) {
        return "媒體報導";
    };
};
// 捐款方式顯示
function donateTypeDisplay(val) {
    if (val == 1) {
        return "一般捐款";
    } else if (val == 2) {
        return "結緣捐贈";
    }
}
// 地區顯示
function areasDisplay(val) {
    if (val == 1) {
        return "北部";
    } else if (val == 2) {
        return "中部";
    } else if (val == 3) {
        return "南部";
    } else if (val == 4) {
        return "海外";
    };
}
$(() => {
    // 當前滑動位置
    let s;
    if (userAgent.indexOf("Safari") !== -1 && userAgent.indexOf("Chrome") == -1) {
        s = $('document,body,html').scrollTop();
    } else {
        s = $('body,html').scrollTop();
    };
    let w = $(window).width() + 17; // 加上 scrollbar 的寬度

    let breakFixedz = $('.breakFixedz'), tabsChange = $('.tabz.changed');
    let fixedPoint, stopedPoint;

    let fixedBackz = $('.fixedBackz');

    let stays; // 視窗暫置點

    // menus
    $(".hamburger").on("click", function () {
        // $(this).toggleClass("is-active");
        if ($(this).hasClass("is-active")) {
            $(this).removeClass("is-active");
            $(".phoneNavs").removeClass('active');
            $('.wrap').removeClass('active');
            $('.wrap').css({ 'top': 0 });
            $(window).scrollTop(stays);

            stays = $(window).scrollTop();
        } else {
            stays = $(window).scrollTop();

            $(this).addClass("is-active");
            $(".phoneNavs").addClass('active');
            $('.wrap').css({ 'top': -stays });
            $(window).scrollTop(stays);
            $('.wrap').addClass('active');
        };
    });
    // 寬度小於 1199px 變為 漢堡選單
    if (w < humburgerWidth) {
        if ($(".phoneNavs .nav ul li").hasClass("active")) {
            let mainLinks = $(".phoneNavs .nav ul li.active").find(".subNavs");
            mainLinks.slideDown("normal");
        }
    } else {
        $(".header .nav li:not(.unShowBacks).hovers").hover(
            function () {
                stays = $(window).scrollTop();
                //
                $(".subNavModals").addClass('active');
                $('.wrap').css({ 'top': -stays });
                $(window).scrollTop(stays);
                $('.wrap').addClass('active');

            }, function () {
                $(".subNavModals").removeClass('active');
                $('.wrap').removeClass('active');
                $('.wrap').css({ 'top': 0 });
                $(window).scrollTop(stays);

            }
        );
    };
    $(".phoneNavs .nav ul li").on("click", function (e) {
        // e.preventDefault();
        if ($(this).hasClass("active")) {
            $(this).removeClass("active");
            $(this).find(".subNavs").slideUp("normal");
        } else {
            $(this).addClass("active");
            $(this).find(".subNavs").slideToggle("normal");
        }
    });
    // 寬度小於 768px 取消 hover效果
    if (w < tabletWidth) {
        $('.wrap').removeClass('hovers');
    };
    // btnTop
    let btnTop = $('.btnTop'); // btnGoTop 回到頂端
    if (s > 1000) {
        btnTop.addClass('active');
    };
    btnTop.on('click', function () {
        $('html,body').animate({
            scrollTop: 0
        }, 1000);
    });
    // Tab Toggle
    function tabToggle() {
        let w = $(window).width() + 17;
        if (w > tabletWidth) {
            tabsChange.addClass('changed');
            let stoped = parseInt(breakFixedz.height() - $(window).height() + parseInt(tabsChange.css('--fixedTops')));

            let widths = tabsChange.parent().width(); // 取 Tabs 父層寬度（隨父層）
            tabsChange.css('--widths', `${widths}px`); // 給與 CSS 變數

            let reserved = tabsChange.css('--fixedTops');
            fixedPoint = parseInt(breakFixedz.offset().top - parseInt(reserved));
            stopedPoint = parseInt($('.breakStopz').offset().top - $(window).height() - 120);
            if (s > fixedPoint && s < stopedPoint) {
                tabsChange.removeClass('stoped').addClass('fixed');
            }
            else if (s >= stopedPoint) {
                tabsChange.css('--stopedBottom', `${stoped}px`);
                tabsChange.removeClass('fixed').addClass('stoped');
            } else {
                tabsChange.removeClass('fixed stoped');
            };
        } else {
            tabsChange.removeClass('changed fixed stoped');
        };
    };
    // Tabs exist 載入後執行一次
    if (tabsChange.length > 0 && breakFixedz.height() > 1000) { // Tabs exist && 當內容高度大於 1000
        tabToggle();
    };
    // resize 
    $(window).on('resize', function () {
        let w = $(window).width() + 17; // 加上 scrollbar 的寬度
        if (w < humburgerWidth) {
            if ($(".phoneNavs .nav ul li").hasClass("active")) {
                let mainLinks = $(".phoneNavs .nav ul li.active").find(".subNavs");
                mainLinks.slideDown("normal");
            };
        } else {
            
            if ($(".phoneNavs").hasClass("active")) {
                $(".hamburger").trigger('click');
            };

            $(".header .nav li:not(.unShowBacks).hovers").hover(
                function () {
                    $(".subNavModals").addClass('active');
                    $('.wrap').css({ 'top': -stays });
                    $(window).scrollTop(stays);
                    $('.wrap').addClass('active');

                }, function () {
                    $(".subNavModals").removeClass('active');
                    $('.wrap').removeClass('active');
                    $('.wrap').css({ 'top': 0 });
                    $(window).scrollTop(stays);
                }
            );
        };
        // 寬度小於 768px 取消 hover效果
        if (w < tabletWidth) {
            $('.wrap').removeClass('hovers');
        } else {
            $('.wrap').addClass('hovers');
        };
    });
    // scroll
    $(window).on('scroll', function () {
        if (userAgent.indexOf("Safari") !== -1 && userAgent.indexOf("Chrome") == -1) {
            s = $('document,body,html').scrollTop();
        } else {
            s = $('body,html').scrollTop();
        };
        // 加上 scrollbar 的寬度
        let w = $(window).width() + 17;
        // 
        if (w < tabletWidth) {
            let heights = screen.height;
            fixedBackz.css('--vHeights', `${heights}px`);
            //
            $('.wrap').removeClass('hovers');
        };
        if (w < phoneWidth) {
            if (s == 0) {
                if (!$('.btnSwitch').hasClass('active')) {

                } else {
                    $('.btnSwitch').addClass('active');
                    $('.btnDonates').addClass('active');
                }
                //
                btnTop.removeClass('active');
            }
            else if (s > 320) {
                if ($('.btnSwitch').hasClass('active')) {
                    $('.btnSwitch').removeClass('active');
                    $('.btnDonates').removeClass('active');
                };
                //
                btnTop.addClass('active');
            }
        } else {
            if (s > 1000) {
                btnTop.addClass('active');
            } else {
                btnTop.removeClass('active');
            };
        };
        // 
        if (tabsChange.length > 0 && breakFixedz.height() > 1000) {
            tabToggle();
        };
    });
    // btnSwitch
    let btnSwitch = $('.btnSwitch');
    btnSwitch.on('click', function () {
        $(this).toggleClass('active');
        if ($(this).hasClass('active')) {
            $('.btnDonates').addClass('active');
        } else {
            $('.btnDonates').removeClass('active');
        };
    });
    // btnEnlargez 
    let btnEnlargez = $('.btnEnlargez');
    btnEnlargez.on('click', function () {
        let targets = $(this).parents('.mains').find('.lightBoxz'), imgParents = $(this).parents('.parts');
        let srcs = imgParents.find('.imagez').attr('src');
        let titles = imgParents.find('.legends').html() ? imgParents.find('.legends').html() : imgParents.find('.imagez').data('titlez');
        if (targets.hasClass('active')) {
        } else {
            targets.addClass('active');
            targets.find('.imagez').attr('src', srcs);
            if (titles !== undefined && titles !== "") {
                targets.find('.titlez').html(titles);
            } else {
                targets.find('.titlez').html('');
            }
            //
            stays = $(window).scrollTop();
            $('.wrap').css({ 'top': -stays });
            $(window).scrollTop(stays);
            $('.wrap').addClass('active');
            //
            targets.find('.btnClose').on('click', function () {
                if (targets.hasClass('active')) {
                    targets.removeClass('active');
                    //
                    $('.wrap').removeClass('active');
                    $('.wrap').css({ 'top': 0 });
                    $(window).scrollTop(stays);
                }
            });
        };
    });
    // 錨點
    let anchors = $('.anchors');
    if (anchors.length > 0) { // 只能存在一個
        anchors[0].scrollIntoView(true);
    };
    // Tabs
    let tabs = $('.tabs '), switches = $('.switches');
    tabs.find('.tab').on('change', function () {
        let tabContent = $(this).parents('.rows').find('.tabContents');
        tabContent.find('.tabContent').eq($(this).index()).addClass('active').siblings().removeClass('active');
    });
    // Switches
    switches.find('.switch').on('change', function () {
        let switchContents = $(this).parents('.rows').find('.switchContents');
        if ($(this).find('.radioz').val() == 1) {
            switchContents.addClass('active');
        } else {
            switchContents.removeClass('active');
        }
    });
    // 已存在 checkbox
    let checkOption = $('.checkOptions .checkz');
    // checkOption.val('0'); // 預設為 false
    checkOption.on('change', function () {
        if ($(this).prop('checked') == true) {
            $(this).val("1");
        } else {
            $(this).val("0");
        }
    });
    // 所屬地區對應填寫的聯絡地址
    let affiliatedAreaz = $('.affiliatedAreaz'), addressz = $('.addressz');
    affiliatedAreaz.on('change', function () {
        if ($(this).val() == 4) {
            addressz.find('.twZipCodes').addClass('disabled')
        } else {
            addressz.find('.twZipCodes').removeClass('disabled')
        }
    });
    // 列表數字樣式
    let aras = $('ul.aras'), listType;
    for (let i = 0; i < aras.length; i++) {
        let nums = aras.eq(i).find('>li');
        for (let n = 0; n < nums.length; n++) {
            if (n < 9) {
                listType = `<div class="numTypes">0${n + 1}.</div>`;
            } else {
                listType = `<div class="numTypes">${n + 1}.</div>`;
            }
            nums.eq(n).prepend(listType)
        }
    };
});
// 定義一個名為 Slider 的全域變數
window.Slider = (function () {
    // 定義 Slider 對象
    let Slider = {};

    // I. 定義一個 TimerManager
    {
        // 1.) 構造
        function TimerManager() {
            this.timers = [];       // 保存區（定時器）
            this.targets = [];      // 保存區（目標值）
            this.paras = [];        // 保存區（定時器參數）
            this.check = false;
        };
        // 2.) 靜態方法：為 element 添加一個 TimerManager 實例
        TimerManager.makeInstance = function (element) {
            // 如果 element.__TimerManager__ 上沒有 TimerManager，就添加一個
            if (!element.__TimerManager__ || element.__TimerManager__.constructor != TimerManager) {
                element.__TimerManager__ = new TimerManager();
            }
        };
        // 3.) 擴展 TimerManager 原型，分別實現 add、active、next 方法 
        TimerManager.prototype.add = function (timer, target, para) {
            this.timers.push(timer);
            this.targets.push(target.toString); // 不知為何 PUSH 數值0 會無效
            this.paras.push(para);
            this.active();
        };
        TimerManager.prototype.active = function () {
            if (!this.check) {
                let timer = this.timers.shift(),    // 取出定時器
                    target = this.targets.shift(), // 取出目標值
                    paras = this.paras.shift();     // 取出定時器參數
                if (timer && target && paras) { // 如果定時器與參數皆存在
                    this.check = true;

                    // 傳入參數，執行定時器函數
                    timer(paras[0], paras[1], paras[2]);
                }
            }
        };
        TimerManager.prototype.next = function () {
            this.check = false;
            this.active();
        };
    };
    // II. 修改動畫函數並在定時器結束後調用 element.__TimerManager__.next()

    // 1.) 下拉（展開）
    function czSlideDown(element, targetHeight, time) {
        if (element.offsetHeight == targetHeight) { // 當元素等於目標初始值（收起時的高度值），執行下拉動作
            if (targetHeight == 0) {                    // 1. 顯示元素
                element.style.display = "block";  // 將元素顯示設定為顯示（元素可能設定為不顯示，高度為 0 || 高度直接設定為 0 ）
            }
            element.style.height = "auto";        // 將元素高度設定為 auto（要取得元素整體高度）
            let totalHeight = element.offsetHeight;     // 2. 保存總高度
            element.style.height = targetHeight + "px"; // 3. 再將元素高度設為目標初始值（收起時的高度值）
            // 定義一個變數保存元素當前高度
            let currentHeight = targetHeight;           // 定義當前元素高度為 目標高度
            // 計算每次增加的值
            let increment = totalHeight / (time / 10);
            // 開始循環定時器
            let timer = setInterval(function () {
                // 增加一部分高度
                currentHeight += increment;
                // 把當前高度賦值給 height 屬性
                element.style.height = currentHeight + "px";
                // 如果當前高度大於或等於總高度則關閉定時器
                if (currentHeight >= totalHeight) {
                    // 關閉定時器
                    clearInterval(timer);
                    // 把高度設置為總高度
                    element.style.height = totalHeight + "px";
                    if (element.__TimerManager__ && element.__TimerManager__.constructor == TimerManager) {
                        element.__TimerManager__.next();
                    }
                }
            }, 10);
        } else {    // 如果當前高度不為目標值的話，則直接執行下一個函數
            if (element.__TimerManager__ && element.__TimerManager__.constructor == TimerManager) {
                element.__TimerManager__.next();
            }
        };
    };
    // 2.) 上滑（收起）
    function czSlideUp(element, targetHeight, time) {
        if (element.offsetHeight > targetHeight) { // 只會有展開與收起兩種選項（除非 BUG），當大於目標初始值（收起時的高度值）則執行上滑動作
            // 獲取總元素高度
            let totalHeight = element.offsetHeight;
            // 定義一個變數保存元素當前高度
            let currentHeight = totalHeight;
            // 計算每次減去的值
            let decrement = totalHeight / (time / 10);
            // 開始循環定時器
            let timer = setInterval(function () {
                // 減去一部分高度
                currentHeight -= decrement;
                // 把當前高度賦值給 height 屬性
                element.style.height = currentHeight + "px";
                // 如果當前高度小於或等於目標值則關閉定時器
                if (currentHeight <= targetHeight) {
                    // 關閉定時器
                    clearInterval(timer);
                    // 當目標值為 0 把元素設定為不顯示
                    if (targetHeight == 0) { // 如果目標初始值（收起時的高度值）為 0，將元素設定為不顯示
                        element.style.display = "none";
                        // 把高度設置為總高度
                        element.style.height = totalHeight + "px";
                    } else {
                        element.style.height = targetHeight + "px"; // 將元素高度設定為目標初始值（收起時的高度值）
                    }
                    if (element.__TimerManager__ && element.__TimerManager__.constructor == TimerManager) {
                        element.__TimerManager__.next();
                    }
                }
            }, 10);
        } else {    // 如果當前高度為 0 或 特定值，則直接執行下一個函數
            if (element.__TimerManager__ && element.__TimerManager__.constructor == TimerManager) {
                element.__TimerManager__.next();
            }
        };
    };

    // III. 定義外部訪問端口

    // 1.) 下拉
    Slider.SlideDown = function (element, targets, time) {
        TimerManager.makeInstance(element);
        element.__TimerManager__.add(czSlideDown, targets, arguments);
        return this;
    };
    // 2.) 上滑
    Slider.SlideUp = function (element, targets, time) {
        TimerManager.makeInstance(element);
        element.__TimerManager__.add(czSlideUp, targets, arguments);
        return this;
    };
    // 返回 Slider 對象
    return Slider;
})();
function slideInit(targetHeight) {
    var slideContents = document.querySelectorAll('hiddenContents'), slideContent = document.querySelectorAll('.hiddenContent'), btnSlideToggle = document.querySelectorAll('.btnSlideToggle'), btnText = document.querySelectorAll('.btnSlideToggle span');
    for (let i = 0; i < slideContent.length; i++) {
        let targets = slideContent[i].style.display !== 'none' ? slideContent[i].offsetHeight : 0; // 當前元素顯示高度 || 目標值（收起時高度值）
        let durs = parseFloat(window.getComputedStyle(slideContent[i]).getPropertyValue('transition-duration'));
        if (targetHeight > targets || targetHeight == 'none') {
            // 註冊
            btnSlideToggle[i].onclick = function () {   // 元素高度值（展開時高度值） || 目標值（收起時高度值）
                if (slideContent[i].offsetHeight == targets) {
                    Slider.SlideDown(slideContent[i], targets, durs * 1000);
                    btnText[i].innerHTML = '顯示部分訊息';
                } else {
                    Slider.SlideUp(slideContent[i], targets, durs * 1000);
                    btnText[i].innerHTML = '顯示完整訊息';
                }
            };
        } else {
            // 關閉
            btnSlideToggle[i].classList.add('none');
        };
    };
};