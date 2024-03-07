// PAGINS
let titlez = $('.titlez'), yearz = $('.yearz'), monthz = $('.monthz'), dayz = $('.dayz'), contentz = $('.contentz'), editorz = $('#editorz');
let addrs, addrArr = new Array(), datez;
CONNECT = 'News';
// 接收資料，做渲染、處理
function process(data) {
    titlez.html(data.Title);
    datez = data.Date.split(' ')[0];
    yearz.html(`${datez.split('-')[0]}<span>年</span>`);
    monthz.html(`${datez.split('-')[1]}<span>月</span>`);
    dayz.html(`${datez.split('-')[2]}<span>日</span>`);
    html = "";
    // Content
    html = quill.setContents(JSON.parse(data.Contents).ops);
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