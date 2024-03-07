// MISSIONARY
let list = $('.list'), limitz;
let navPics01, navPics02;
CONNECT = 'timeMachine';
html = "", preview = "";
// 接收資料，做渲染、處理
function process(data) {
    for (let i = 0; i < data.length; i++) {
        if (data[i].imageURL01 !== null) {
            navPics01 = `
                <div class="preview">
                    <div class="images active">
                        <img class="imagez" src="${data[i].imageURL01}">
                    </div>
                </div>
            `;
        } else {
            navPics01 = `
                <div class="preview">
                    <div class="images"></div>
                </div>
            `;
        }
        if (data[i].imageURL02 !== null) {
            navPics02 = `
                <div class="preview">
                    <div class="images active">
                        <img class="imagez" src="${data[i].imageURL02}">
                    </div>
                </div>
            `;
        } else {
            navPics02 = `
                <div class="preview">
                    <div class="images"></div>
                </div>
            `;
        }
        if (data[i].imageURL01 == null && data[i].imageURL02 == null) {
            preview = `previews none`;
        } else {
            preview = `previews`;
        }
        html += `
            <div class="items itemz">
                <div class="timeLine">
                    <p class="year yearz">${data[i].course.split('/')[0]}</p>
                    <p class="month monthz">${data[i].course.split('/')[1]}月</p>
                    <div class="decCircles">
                        <div class="circles"></div>
                    </div>
                </div>
                <div class="heads">
                    <p class="mainTitle titlez">${data[i].name}</p>
                </div>
                <div class="texts">
                    <p class="text contentz">${data[i].brief}</p>
                </div>
                <div class="${preview}">
                    ${navPics01} ${navPics02}
                    <div class="notes">
                        <div class="decRows"></div>
                        <p class="note">點擊圖片可單獨預覽</p>
                        <div class="decRows shorts"></div>
                    </div>
                </div>
            </div>
        `;
    };
    list.html(html);

    $('.images.active').on('click', function () {
        let targets = $(this).parents('.mains').find('.lightImagez');
        let srcs = $(this).find('.imagez').attr('src');
        if (targets.hasClass('active')) {
        } else {
            targets.addClass('active');
            targets.find('.imagez').attr('src', srcs);
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

                    setTimeout(() => {
                        // 清空
                        targets.find('.imagez').attr('src', '');
                    }, 500);
                }
            });
        };
    });
};
// NOTFOUND
function fails() {
    html = "";
    html = ``;
    // list.html(html);
    // paginations.find('.pagez').html(curPage(current, pageLens, pageCount));
    //
    $("#loadings").addClass('hidden');
};
$(() => {
    let dataObj = {
        "Methods": "GET",
        "APIs": URL,
        "CONNECTs": CONNECT,
        "QUERYs": `${CONNECT}?count=${listSize}&page=${pageRcd}`,
        "Counts": listSize,
        "Sends": "",
    };
    // init controller
    var controller = new ScrollMagic.Controller();
    // build scene
    var scene = new ScrollMagic.Scene({ triggerElement: "#demos #loadings", triggerHook: "onEnter" })
        .addTo(controller)
        .on("enter", function (e) {
            if (current == limitz) {
                $("#loadings").addClass('hidden');
                scene.destroy();
            } else {
                if (!$("#loadings").hasClass("active")) {
                    $("#loadings").addClass("active");

                    // simulate ajax call to add content using the function below
                    setTimeout(ajaxLoadings, 800, ++current);
                }
            }
        });
    function ajaxLoadings(nums) {
        getTotalPages(dataObj).then(res => {
            if (res !== 0) {
                limitz = res;
                dataObj.QUERYs = `${CONNECT}?count=${listSize}&page=${nums}`;
                getPageDatas(dataObj).then(res => {
                    // DO SOMETHING
                    if (res !== null) {
                        process(res);
                    } else {
                        fails();
                        scene.destroy();
                    };
                    // $('html,body').scrollTop(0);
                }, rej => {
                    if (rej == "NOTFOUND") {
                        fails();
                        scene.destroy();
                    };
                });
                // "loading" done -> revert to normal state
                scene.update(); // make sure the scene gets the new start position
                $("#loadings").removeClass("active");
            } else {
                pageLens = 0; // 資料筆數為 0 頁數為 0
                fails();
                scene.destroy();
            };
        }, rej => {
            if (rej == "NOTFOUND") { // ? 當伺服器無資料時 || 當伺服器發生問題時
                pageLens = 0; // 資料筆數為 0 頁數為 0
                fails();
                scene.destroy();
            };
        });
    };
    // 執行第一次
    ajaxLoadings(current);
});