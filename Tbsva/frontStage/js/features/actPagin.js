// PAGINS
let titlez = $('.titlez'), categoryz = $('.categoryz'), yearz = $('.yearz'), monthz = $('.monthz'), dayz = $('.dayz'), contentz = $('.contentz'), editorz = $('#editorz');
let navPicturez = $('.navPicturez'), imgLens = 8, sliders = $('.sliders'), slidez, slideCheck = false; // 導覽圖 目前最多為 8 張
let addrs, addrArr = new Array(), datez;
CONNECT = 'activity';
// 接收資料，做渲染、處理
function process(data) {
    titlez.html(data.name);
    categoryz.html(data.categoryName);
    datez = data.createDate.split(' ')[0];
    yearz.html(`${datez.split('-')[0]}<span>年</span>`);
    monthz.html(`${datez.split('-')[1]}<span>月</span>`);
    dayz.html(`${datez.split('-')[2]}<span>日</span>`);
    // 導覽圖
    slidez = "";
    for (let i = 0; i < imgLens; i++) {
        if (data[`imageURL0${i + 1}`] !== "" && data[`imageURL0${i + 1}`] !== null) {
            slideCheck = true; // 開啟
            slidez += `
                <div class="swiper-slide">
                    <div class="images">
                        <img src="${data[`imageURL0${i + 1}`]}?${paramRandoms()}">
                    </div>
                </div>
            `;
        }
    };
    sliders.html(slidez);
    if (slideCheck == true) {
        navPicturez.addClass('active');
    };
    html = "";
    // Content
    html = quill.setContents(JSON.parse(data.content).ops);
    editorz.html(quill.root.innerHTML);
};
// NOTFOUND
function fails() { };
$().ready(function () {
    let numz = request('id');
    if (numz) {
        let dataObj = {
            "Methods": "GET",
            "APIs": URL,
            "CONNECTs": `${CONNECT}/${numz}`,
            "QUERYs": "",
            "Sends": "",
            "Counts": "",
        };
        getPageDatas(dataObj).then(res => {
            // DOSOMETHING
            if (res !== null) {
                process(res);
            } else {
                fails();
            };
        }, rej => {
            if (rej == "NOTFOUND") { };
        });
    };
});