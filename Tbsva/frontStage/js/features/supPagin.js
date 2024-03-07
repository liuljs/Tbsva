// PAGINS
let titlez = $('.titlez'), subTitlez = $('.subTitlez'), contentz = $('.contentz'), categoryz = $('.categoryz'), editorz = $('#editorz');

CONNECT = 'articleContent';
// 接收資料，做渲染、處理
function process(data) {
    //
    titlez.html(data.title);
    subTitlez.html(data.subtitle);
    categoryz.html(data.articleCategoryName);
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