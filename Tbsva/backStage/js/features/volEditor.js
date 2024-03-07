// 宣告要帶入的欄位
let btnResetz = $('.btnResetz'), btnSavez = $('.btnSavez');
CONNECT = "Terms";
// 驗證
function dataUpdateCheck(aId, contentz) {
    if (aId.trim() === '') {
        $(document).scrollTop(0); // 置頂
        return check = false, errorText = '請確認必填欄位皆有填寫！';
    }
    if (contentz.getLength() === 1) {
        $(document).scrollTop(0); // 置頂
        return check = false, errorText = '請確認編輯的內容有確實填寫！';
    }
    else {
        return check = true, errorText = "";
    }
};
$().ready(function () {
    //
    let dataObj = {
        "Methods": "GET",
        "APIs": URL,
        "CONNECTs": CONNECT,
        "QUERYs": "",
        "Sends": "",
        "Counts": ""
    };
    getPageDatas(dataObj).then(res => {
        // DO SOMETHING
        if (res !== null) {
            // 將取得的內容渲染至編輯頁面上
            quill.setContents(JSON.parse(res[0].Content).ops);
            // 資料載入完成後
            loadingCompleted();
        } else {
            quill.setContents();
            // 資料載入完成後
            loadingCompleted();
        };
    }, rej => {
        if (rej == "NOTFOUND") { };
    });
    // 在編輯器點擊圖片上傳，選擇好圖片時就上傳並且能夠以路徑的URL預覽
    let editorImagez = $('#editorImagez');
    let toolbar = quill.getModule('toolbar');
    toolbar.addHandler("image", function () { // 將 quill 編輯器的圖片功能轉為自訂義圖片上傳
        editorImagez.click();
        editorImagez.on('change', function () {
            // 圖片格式為路徑：在編輯器點擊圖片上傳，選擇好圖片時就上傳並且回傳路徑用以預覽
            let file = $(this);
            if (imgUpdateCheck(file)) {
                let dataObj = new FormData();
                dataObj.append('uploadImage', file[0].files[0]);

                let xhr = new XMLHttpRequest();
                xhr.onload = function () {
                    if (xhr.status == 200 || xhr.status == 201) {
                        if (xhr.responseText !== "") {
                            let callBackData = JSON.parse(xhr.responseText);
                            // 獲取編輯器當前 focus 的位置
                            let selection = quill.getSelection(true);
                            // 調用函式 insertEmbed 將圖片顯示於編輯器上
                            quill.insertEmbed(selection.index, 'image', callBackData.Image_Link); // path 為回傳值的路徑
                        };
                    } else {
                        alert("錯誤訊息 " + xhr.status + "：您的連線異常，請重新登入！");
                        location.reload();
                    };
                };
                xhr.open('POST', `${URL}${CONNECT}/AddImage`, true);
                // xhr.setRequestHeader('Content-Type', 'multipart/form-data');
                xhr.send(dataObj);

                file.val(''); // 重置檔案上傳欄位的內容，避免重複出現預覽圖片
            };
        });
    });
    // Reset 重置
    btnResetz.on('click', function (e) {
        e.preventDefault();
        if (confirm("您確定要將目前編輯的內容全部清空嗎？")) {
            // 清空編輯器
            let cntsLen = quill.getLength();
            quill.deleteText(0, cntsLen);

            $(document).scrollTop(0); // 置頂
        };
    });
    // Save 儲存
    btnSavez.on('click', function (e) {
        e.preventDefault();
        dataUpdateCheck(idz, quill);
        if (check == true) {
            // 取得圖片的名稱包成陣列
            let file = $('.ql-editor').find('img');
            if (file) {
                fNameArr = [];
                for (let i = 0; i < file.length; i++) {
                    fNameArr.push(file.eq(i).attr('src').split('/').pop());
                };
            };
            let dataObj = {
                "Content": JSON.stringify(quill.getContents()),
                'fNameArr': fNameArr
            };
            if (confirm("您確定要儲存修改嗎？")) {
                // 傳送
                let xhr = new XMLHttpRequest();
                xhr.onload = function () {
                    if (xhr.status == 200 || xhr.status == 201) {
                        alert('儲存成功！')
                        location.reload();
                    } else {
                        alert("錯誤訊息 " + xhr.status + "：您的連線異常，請重新登入！");
                        location.reload();
                    };
                };
                xhr.open('POST', `${URL}${CONNECT}/AddContent`, true);
                xhr.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded');
                xhr.send($.param(dataObj));
            };
        } else {
            alert(errorText);
        };
    });
});