// PAGINS
let titlez = $('.titlez'), subTitlez = $('.subTitlez'), contentz = $('.contentz'), imagez = $('.imagez'), editorz = $('#editorz');

CONNECT = 'directorIntroduction';
// 接收資料，做渲染、處理
function process(data) {
    //
    titlez.html(data.name);
    subTitlez.html(data.subtitle);
    contentz.html(data.brief);
    imagez.attr('src', data.imageURL);
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