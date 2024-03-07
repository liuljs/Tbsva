// PAGINS
let titlez = $('.titlez'), subTitlez = $('.subTitlez'), contentz = $('.contentz'), mapz = $('.mapz'), editorz = $('#editorz');
let addrs, addrArr = new Array();
CONNECT = 'taiwanDojo';
// 接收資料，做渲染、處理
function process(data) {
    //
    mapz.html(data.name);
    titlez.html(data.categoryName);

    // Show Address
    if (data.address != "" && data.address != "[]") {
        addrs = "";
        addrArr = JSON.parse(data.address);
        for (let i = 0; i < addrArr.length; i++) {
            addrs += addrArr[i];
        };
    } else {
        addrArr = [];
        addrs = `<span> - </span>`;
    };
    subTitlez.html(addrs);
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