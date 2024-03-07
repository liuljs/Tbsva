// EDITORS
let layoutImagez = $('.layoutImagez');
let titlez = $('.titlez'), editorz = $('.editorz');
CONNECT = "4";
$().ready(function () {
    let dataObj = {
        "Methods": "GET",
        "APIs": URL,
        "CONNECTs": `graphicsEditorR${CONNECT}`,
        "QUERYs": "",
        "Sends": "",
        "Counts": ""
    };
    getPageDatas(dataObj).then(res => {
        // DOSOMETHING
        if (res !== null) {
            html = "";
            // TITLE
            if (res.title !== null && res.title !== "") {
                titlez.html(res.title);
            } else { };
            // IMAGE
            for (let i = 0; i < layoutImagez.length; i++) {
                if (res[`imageURL0${i + 1}`] !== null) {
                    layoutImagez.eq(i).find('.imagez').attr('src', res[`imageURL0${i + 1}`]);
                    layoutImagez.eq(i).addClass('active');
                };
            };
            // CONTENT
            var el = document.createElement('div');
            for (let i = 0; i < editorz.length; i++) {
                let quill = new Quill(el, {});
                if (JSON.parse(res[`content${i + 1}`]) !== null && JSON.parse(res[`content${i + 1}`]) !== "") {
                    let writing = JSON.parse(res[`content${i + 1}`]).ops;
                    if (writing.length > 0) {
                        html = quill.setContents(writing);
                        editorz.eq(i).html(quill.root.innerHTML);
                    };
                };
            };
        } else { };
    }, rej => {
        if (rej == "NOTFOUND") { };
    });
});