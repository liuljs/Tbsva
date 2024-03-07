// 宣告要帶入的欄位
let idNumberz = $('.idNumberz'), titlez = $('.titlez'), imgFilez = $('.imgFilez'), titleImagez = $('.titleImagez'), titleSamplez = $('.titleSamplez'), briefz = $('.briefz');
let categoryz = "上師";
let btnReturnz = $('.btnReturnz'), btnResetz = $('.btnResetz'), btnAddz = $('.btnAddz');
let enabled = 1, topz = 0; // 預設為顯示 || 未置頂 
CONNECT = 'knowledge';
// 驗證
function dataUpdateCheck(aId, number, title, sub, file, brief, content) {
    if (aId.trim() === '') {
        title.focus();
        $(document).scrollTop(0); // 置頂
        return check = false, errorText = '請確認必填欄位皆有填寫！';
    }
    if (number.val().trim() === '' || number.val().trim().length > 40) {
        number.focus();
        $(document).scrollTop(0); // 置頂
        return check = false, errorText = '請確認戒牒號碼欄位有確實填寫、格式是否正確（長度不得超過 40 個字元）！';
    }
    if (title.val().trim() === '' || title.val().trim().length > 40) {
        title.focus();
        $(document).scrollTop(0); // 置頂
        return check = false, errorText = '請確認名稱欄位有確實填寫、格式是否正確（長度不得超過 40 個字元）！';
    }
    if (sub === '' || sub.length > 40) {
        sub.focus();
        $(document).scrollTop(0); // 置頂
        return check = false, errorText = '請確認位階欄位有確實填寫、格式是否正確（長度不得超過 40 個字元）！';
    }
    if (file.val().trim() === '') {
        file.focus();
        $(document).scrollTop(0); // 置頂
        return check = false, errorText = '請確認有選擇上傳的封面圖！';
    }
    if (brief.val().trim() === '' || brief.val().trim().length > 100) {
        brief.focus();
        $(document).scrollTop(0); // 置頂
        return check = false, errorText = '請確認簡述欄位有確實填寫、格式是否正確（長度不得超過 100 個字元）！';
    }
    if (content.getLength() === 1) {
        $(document).scrollTop(0); // 置頂
        return check = false, errorText = '請確認內容有確實填寫！';
    }
    else {
        return check = true, errorText = "";
    }
};
$().ready(function () {
    // 頁面載入完成 
    loadingCompleted();
    // 返回
    btnReturnz.on('click', backToPrevious);
    // Reset 重置
    btnResetz.on('click', function (e) {
        e.preventDefault();
        if (confirm("您確定要將目前編輯的內容全部清空嗎？")) {
            // 置頂
            // topz.prop('checked', false), topz.val('0');
            // 清空戒牒號碼
            idNumberz.val('');
            // 清空標題
            titlez.val('');
            // 清空位階
            // categoryz.val('');
            // 清空封面圖
            titleImagez.val('');
            // 回復預設
            titleSamplez.html(`<img src="./img/samples/sample500x600.jpg">`);
            // 清空簡述
            briefz.val('');
            // 清空編輯器
            let cntsLen = quill.getLength();
            quill.deleteText(0, cntsLen);

            $(document).scrollTop(0); // 置頂
        };
    });
    // Top
    // topz.on('change', function () {
    //     if ($(this).prop('checked') == true) {
    //         $(this).val('1');
    //     } else {
    //         $(this).val('0');
    //     };
    // });
    // 上傳圖片 取得圖片路徑
    // 上傳圖片 取得圖片路徑
    titleImagez.on('change', function () {
        let file = $(this);
        if (file.val() !== "") {
            if (imgUpdateCheck(file)) {
                if (window.URL !== undefined) {
                    let url = window.URL.createObjectURL(file[0].files[0]);
                    let currentImages = `
                        <img class="sampleImagez" src="${url}">
                    `;
                    titleSamplez.html(currentImages);
                };
            };
        } else {
            // 回復預設
            titleSamplez.html(`
                <img src="./img/samples/sample500x600.jpg">
            `);
            // 清空 file 的值
            file.val('');
        };
    });
    // Add 新增
    btnAddz.on('click', function (e) {
        e.preventDefault();
        // 驗證
        dataUpdateCheck(idz, idNumberz, titlez, categoryz, titleImagez, briefz, quill);
        if (check == true) {
            // 取得圖片的名稱包成陣列
            let file = $('.ql-editor').find('img');
            if (file) {
                fNameArr = [];
                for (let i = 0; i < file.length; i++) {
                    fNameArr.push(file.eq(i).attr('src').split('/').pop());
                };
            } else {
                fNameArr = [];
            };
            // 將要新增的內容傳送至後台資料庫中儲存
            let dataObj = new FormData();
            dataObj.append('first', topz);
            dataObj.append('number', idNumberz.val().trim());
            dataObj.append('title', titlez.val().trim());
            dataObj.append('category', categoryz);
            dataObj.append('enabled', enabled);
            dataObj.append('cover', titleImagez[0].files[0]);
            dataObj.append('brief', briefz.val().trim());
            dataObj.append('content', JSON.stringify(quill.getContents()));
            dataObj.append('cNameArr', JSON.stringify(fNameArr));
            if (confirm("您確定要新增嗎？")) {
                // 傳送
                let xhr = new XMLHttpRequest();
                xhr.onload = function () {
                    if (xhr.status == 200 || xhr.status == 201) {
                        alert("新增成功！");
                        location.href = './missionary_01.html';
                    } else {
                        alert("錯誤訊息 " + xhr.status + "：您的連線異常，請重新登入！");
                        location.reload();
                    };
                };
                xhr.open('POST', `${URL}${CONNECT}`, true);
                // xhr.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded');
                xhr.send(dataObj);
            };
        } else {
            alert(errorText);
        };
    });
    // 在編輯器點擊圖片上傳，選擇好圖片時就上傳並且能夠以路徑的URL預覽
    let editorImagez = $('#editorImagez');
    let toolbar = quill.getModule('toolbar');
    toolbar.addHandler("image", function () { // 將 quill 編輯器的圖片功能轉為自訂義圖片上傳
        editorImagez.click();
        editorImagez.on('change', function () {
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
                xhr.open('POST', `${URL}${CONNECT}/image`, true);
                // xhr.setRequestHeader('Content-Type', 'multipart/form-data');
                xhr.send(dataObj);

                file.val(''); // 重置檔案上傳欄位的內容，避免重複出現預覽圖片
            }
        });
    });
});