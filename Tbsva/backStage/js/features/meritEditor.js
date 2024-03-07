//
let methodz = '02', insert = '',
    setUpz = `
        <a href="#" class="btn btn-sm btn-danger btnResetz">
            <i class="fal fa-chalkboard"></i> 清空
        </a>
        <a href="#" class="btn btn-sm btn-success btnSavez">
            <i class="fal fa-save"></i> 儲存
        </a>
    `;
// 宣告要帶入的欄位
let titleFiledz = $('.titleFiledz');
let editorz = $('.editorz'), editorImagez = $('.editorImagez');
let btnResetz = $('.btnResetz'), btnSavez = $('.btnSavez');
let layoutDropz = $('.layoutDropz');
let fileArr = [];
let btnz = $('.btnz');
// 文編器
var toolbarOptions = [
    [{ header: [1, 2, 3, 4, 5, false] }],
    ['bold', 'italic', 'underline'],
    [{ color: [] }, { background: [] }],
    ['link', 'blockquote', 'code-block', 'image'],
    [{ list: 'ordered' }, { list: 'bullet' }, { 'align': [] }]
];
var toolbarOptionsNoImage = [
    [{ header: [1, 2, 3, 4, 5, false] }],
    ['bold', 'italic', 'underline'],
    [{ color: [] }, { background: [] }],
    ['link', 'blockquote', 'code-block'],
    [{ list: 'ordered' }, { list: 'bullet' }, { 'align': [] }]
];
CONNECT = "8";
// 驗證
function dataUpdateCheck(aId) {
    if (aId.trim() === '') {
        $(document).scrollTop(0); // 置頂
        return check = false, errorText = '請確認必填欄位皆有填寫！';
    }
    else {
        return check = true, errorText = "";
    }
};
// UPLOADS
function renderImages(index, main, files) {
    if (window.URL !== undefined) {
        let src;
        if (typeof files == 'object') {
            src = window.URL.createObjectURL(files[0]);
            main.find('.imgSizez').html(`${Math.round(files[0].size / 1024) + ' KB'}`);
        } else {
            src = files;
        };
        // 顯示圖片預覽
        main.find('.imagez').attr('src', src);
        main.find('.prevImagez').addClass('active');
        main.find('.upFilez').addClass('none');
        // 取消
        main.find('.btnCancelUpz').unbind().on('click', function (e) {
            e.preventDefault();
            // 顯示上傳區塊
            main.find('.imagez').attr('src', '');
            main.find('.imgSizez').html('');
            main.find('.prevImagez').removeClass('active');
            main.find('.upFilez').removeClass('none');
            // 清除陣列中要上傳的檔案 || 對應各個索引值中的檔案
            fileArr[index] = "";
        });
    };
};
// 
function process(data) {
    //
    if (editAuth == 1) {
        //
        $('.layoutEdits').addClass('hovers'), $('.layoutEditors').addClass('hovers');
        // 將取得的內容渲染至編輯頁面上
        if (data.title !== "" || data.title !== null) {
            titleFiledz.html(`
                <input type="text" class="form-control titles titlez" value="${data.title}" placeholder="標題內容..">
            `);
        } else {
            titleFiledz.html(`
                <input type="text" class="form-control titles titlez" value="" placeholder="標題內容..">
            `);
        };
        // 圖片顯示
        for (let i = 0; i < layoutDropz.length; i++) {
            if (data[`imageURL0${i + 1}`] !== null && data[`imageURL0${i + 1}`] !== "") {
                renderImages(i, layoutDropz.eq(i), data[`imageURL0${i + 1}`]);
            } else {

            };
        };
        // 文案顯示
        for (let i = 1; i <= editorz.length; i++) {
            let quill = new Quill(`#editor0${i}`, {
                modules: {
                    toolbar: toolbarOptionsNoImage
                },
                placeholder: '內容..',
                theme: 'snow'
            });
            if (JSON.parse(data[`content${i}`]) !== null && JSON.parse(data[`content${i}`]) !== "") {
                let writing = JSON.parse(data[`content${i}`]).ops;
                if (writing.length > 0) {
                    quill.setContents(writing);
                } else {
                    quill.setContents();
                };
            }
            // 在編輯器點擊圖片上傳，選擇好圖片時就上傳並且能夠以路徑的URL預覽
            let toolbar = quill.getModule('toolbar');
            toolbar.addHandler("image", function () { // 將 quill 編輯器的圖片功能轉為自訂義圖片上傳
                editorImagez.eq(i).click();
                editorImagez.eq(i).on('change', function () {
                    let file = $(this);
                    if (imgUpdateCheck(file)) {
                        // 2. 圖片格式為路徑：在編輯器點擊圖片上傳，選擇好圖片時就上傳並且回傳路徑用以預覽
                        let dataObj = new FormData();
                        dataObj.append('file', file[0].files[0]);
                        let xhr = new XMLHttpRequest();
                        xhr.onload = function () {
                            if (xhr.status == 200 || xhr.status == 201) {
                                if (xhr.responseText !== "" && xhr.responseText !== "[]") {
                                    let callBackData = JSON.parse(xhr.responseText);
                                    // 獲取編輯器當前 focus 的位置
                                    let selection = quill.getSelection(true);
                                    // 調用函式 insertEmbed 將圖片顯示於編輯器上
                                    quill.insertEmbed(selection.index, 'image', callBackData.imageURL); // path 為回傳值的路徑
                                };
                            } else {
                                alert("錯誤訊息 " + xhr.status + "：您的連線已逾期，請重新登入！");
                                location.reload();
                            };
                        };
                        xhr.open('POST', `${URL}graphicsEditorR${CONNECT}/image`, true);
                        // xhr.setRequestHeader('Content-Type', 'multipart/form-data');
                        xhr.send(dataObj);

                        file.val(''); // 重置檔案上傳欄位的內容，避免重複出現預覽圖片
                    };
                });
            });
        };
        // 可編輯
        btnz.html(setUpz);
        // Reset 重置
        $('.btnResetz').on('click', function (e) {
            e.preventDefault();
            if (confirm("您確定要將目前編輯的內容全部清空嗎？")) {
                // 文案
                for (let i = 1; i <= editorz.length; i++) {
                    let quill = new Quill(`#editor0${i}`);
                    if (quill.getLength() > 1) {
                        // 清空編輯器
                        let cntsLen = quill.getLength();
                        quill.deleteText(0, cntsLen);
                    };
                };
                // 圖片
                for (let i = 0; i < layoutDropz.length; i++) {
                    // 顯示上傳區塊
                    layoutDropz.eq(i).find('.imagez').attr('src', '');
                    layoutDropz.eq(i).find('.imgSizez').html('');
                    layoutDropz.eq(i).find('.prevImagez').removeClass('active');
                    layoutDropz.eq(i).find('.upFilez').removeClass('none');
                    // 清除陣列中要上傳的檔案 || 對應各個索引值中的檔案
                    fileArr[i] = "";
                }
                $(document).scrollTop(0); // 置頂
            };
        });
        // Save 儲存
        $('.btnSavez').on('click', function (e) {
            e.preventDefault();
            dataUpdateCheck(idz);
            if (check == true) {
                // 取得圖片的名稱包成陣列
                let file = $('.ql-editor').find('img');
                if (file) {
                    fNameArr = [];
                    for (let i = 0; i < file.length; i++) {
                        fNameArr.push(file.eq(i).attr('src').split('/').pop());
                    };
                };
                let dataObj = new FormData();
                // 標題
                if ($('.titlez').val().trim() !== "") {
                    dataObj.append('title', $('.titlez').val().trim());
                }
                // 文案
                for (let i = 1; i <= editorz.length; i++) {
                    let quill = new Quill(`#editor0${i}`);
                    if (quill.getLength() > 1) {
                        dataObj.append(`content${i}`, JSON.stringify(quill.getContents()));
                    };
                };
                // 圖片
                for (let i = 0; i < 8; i++) {
                    if (fileArr[i] !== undefined) {
                        dataObj.append(`navPics0${i + 1}`, fileArr[i]);
                    } else {
                        dataObj.append(`navPics0${i + 1}`, '');
                    };
                };
                // 文邊器內圖片
                dataObj.append('cNameArr', JSON.stringify(fNameArr));
                if (confirm("您確定要儲存修改嗎？")) {
                    // 傳送
                    let xhr = new XMLHttpRequest();
                    xhr.onload = function () {
                        if (xhr.status == 200 || xhr.status == 201) {
                            alert('儲存成功！')
                            location.reload();
                        } else {
                            alert("錯誤訊息 " + xhr.status + "：您的連線異常，請重新登入！");
                            // location.reload();
                        };
                    };
                    xhr.open('POST', `${URL}graphicsEditorR${CONNECT}`, true);
                    // xhr.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded');
                    xhr.send(dataObj);
                };

            } else {
                alert(errorText);
            };
        });
    } else {
        // 將取得的內容渲染至編輯頁面上
        if (data.title !== "" || data.title !== null) {
            titleFiledz.html(`
                <div class="titles">${data.title}</div>
            `);
        } else {
            titleFiledz.html(`
                <div class="titles"></div>
            `);
        };
        // 圖片顯示
        for (let i = 0; i < layoutDropz.length; i++) {
            if (data[`imageURL0${i + 1}`] !== null && data[`imageURL0${i + 1}`] !== "") {
                renderImages(i, layoutDropz.eq(i), data[`imageURL0${i + 1}`]);
                layoutDropz.eq(i).find('.imgControls').remove();
            } else {
                // 顯示基本底圖
                layoutDropz.eq(i).find('.prevImagez .imagez').remove();
                layoutDropz.eq(i).find('.prevImagez').addClass('bases active');
                layoutDropz.eq(i).find('.upFilez').addClass('none');
            };
        };
        // 文案顯示
        var el = document.createElement('div');
        for (let i = 0; i < editorz.length; i++) {
            let quill = new Quill(el, {});
            if (JSON.parse(data[`content${i + 1}`]) !== null && JSON.parse(data[`content${i + 1}`]) !== "") {
                let writing = JSON.parse(data[`content${i + 1}`]).ops;
                if (writing.length > 0) {
                    html = quill.setContents(writing);
                    editorz.eq(i).html(`<div class="ql-editor">${quill.root.innerHTML}</div>`);
                } else { };
            } else { };
        };
        // 可編輯
        btnz.html(setUpz);
    };
    // 資料載入完成後
    loadingCompleted();
};
// 
function fails() {
    // 資料載入完成後
    loadingCompleted();
};
$().ready(function () {
    //
    let dataObj = {
        "Methods": "GET",
        "APIs": URL,
        "CONNECTs": `graphicsEditorR${CONNECT}`,
        "QUERYs": "",
        "Sends": "",
        "Counts": ""
    };
    getPageDatas(dataObj).then(res => {
        // DO SOMETHING
        if (res !== null) {
            process(res);
        } else {
            fails();
        };
    }, rej => {
        if (rej == "NOTFOUND") { };
    });
    // 上傳檔案
    for (let i = 0; i < layoutDropz.length; i++) {
        // 拖曳
        layoutDropz.eq(i).bind('dragover', function (e) {
            e.originalEvent.preventDefault();
        });
        layoutDropz.eq(i).bind('drop', function (e) {
            e.originalEvent.preventDefault();
            let numz = $(this);
            let files = e.originalEvent.dataTransfer.files; // file
            // 前端限制上傳的檔案格式
            if (!files[0].type.match('image/*')) {
                alert('檔案格式錯誤，請上傳圖片檔格式！');
                return;
            }
            // 前端限制圖片大小
            else if (files[0].size > limit) {
                alert(`圖片大小不得超過 ${limit / (1024 * 1024)} MB`);
                return;
            };
            // 將上傳檔案放入陣列
            fileArr[i] = files[0];
            // 渲染
            renderImages(i, numz, files);
        });
        // 上傳
        layoutDropz.eq(i).find('.uploadImagez').on('change', function () {
            let e = $(this);
            let mainz = e.parents('.layoutDropz');
            if (imgUpdateCheck(e)) {
                let files = e[0].files;
                // 將上傳檔案放入陣列
                fileArr[i] = files[0];
                // 渲染
                renderImages(i, mainz, files);
            };
        });
    };
    // 在編輯器點擊圖片上傳，選擇好圖片時就上傳並且能夠以路徑的URL預覽
    // let editorImagez = $('#editorImagez');
    // let toolbar = quill.getModule('toolbar');
    // toolbar.addHandler("image", function () { // 將 quill 編輯器的圖片功能轉為自訂義圖片上傳
    //     editorImagez.click();
    //     editorImagez.on('change', function () {
    //         // 圖片格式為路徑：在編輯器點擊圖片上傳，選擇好圖片時就上傳並且回傳路徑用以預覽
    //         let file = $(this);
    //         if (imgUpdateCheck(file)) {
    //             let dataObj = new FormData();
    //             dataObj.append('file', file[0].files[0]);

    //             let xhr = new XMLHttpRequest();
    //             xhr.onload = function () {
    //                 if (xhr.status == 200 || xhr.status == 201) {
    //                     if (xhr.responseText !== "") {
    //                         let callBackData = JSON.parse(xhr.responseText);
    //                         // 獲取編輯器當前 focus 的位置
    //                         let selection = quill.getSelection(true);
    //                         // 調用函式 insertEmbed 將圖片顯示於編輯器上
    //                         quill.insertEmbed(selection.index, 'image', callBackData.imageURL); // path 為回傳值的路徑
    //                     };
    //                 } else {
    //                     alert("錯誤訊息 " + xhr.status + "：您的連線異常，請重新登入！");
    //                     location.reload();
    //                 };
    //             };
    //             xhr.open('POST', `${URL}textEditorR${CONNECT}/image`, true);
    //             // xhr.setRequestHeader('Content-Type', 'multipart/form-data');
    //             xhr.send(dataObj);

    //             file.val(''); // 重置檔案上傳欄位的內容，避免重複出現預覽圖片
    //         };
    //     });
    // });
});