// 宣告要帶入的欄位
let topz = $('.topz'), titlez = $('.titlez'), subTitlez = $('.subTitlez'), amountz = $('.amountz'), imgFilez = $('.imgFilez'), titleImagez = $('.titleImagez'), titleSamplez = $('.titleSamplez'), briefz = $('.briefz');
let btnResetz = $('.btnResetz'), btnAddz = $('.btnAddz'), btnReturnz = $('.btnReturnz');
let donateType = 2, secondType = 0, enabled = 1; // 預設為顯示 主類別為結餘捐蹭 狀態預設為顯示
CONNECT = 'donateRelatedItem';
// 驗證
function dataUpdateCheck(aId, title, sub, file, amount, brief, content) {
    if (aId.trim() === '') {
        title.focus();
        $(document).scrollTop(0); // 置頂
        return check = false, errorText = '請確認必填欄位皆有填寫！';
    }
    if (title.val().trim() === '' || title.val().trim().length > 40) {
        title.focus();
        $(document).scrollTop(0); // 置頂
        return check = false, errorText = '請確認標題欄位有確實填寫、格式是否正確（長度不得超過 40 個字元）！';
    }
    if (sub.val().trim() === '' || sub.val().trim().length > 100) {
        sub.focus();
        $(document).scrollTop(0); // 置頂
        return check = false, errorText = '請確認取得辦法欄位有確實填寫、格式是否正確（長度不得超過 100 個字元）！';
    }
    if (file.val().trim() === '') {
        file.focus();
        $(document).scrollTop(0); // 置頂
        return check = false, errorText = '請確認有選擇上傳的封面圖！';
    }
    if (amount.val().trim() === '') {
        amount.focus();
        $(document).scrollTop(0); // 置頂
        return check = false, errorText = '請確認結緣品金額欄位有確實填寫！';
    }
    if (brief.val().trim() === '') {
        brief.focus();
        $(document).scrollTop(0); // 置頂
        return check = false, errorText = '請確認簡述欄位有確實填寫！';
    }
    if (content.getLength() === 1) {
        $(document).scrollTop(0); // 置頂
        return check = false, errorText = '請確認內容有確實填寫！';
    }
    else {
        return check = true, errorText = "";
    };
};
$().ready(function () {
    // 頁面載入完成 
    loadingCompleted();
    // 返回
    btnReturnz.on('click', backToPrevious);
    // Reset 重置
    btnResetz.on('click', function (e) {
        e.preventDefault()
        if (confirm("您確定要將目前編輯的內容全部清空嗎？")) {
            // 置頂
            topz.prop('checked', false), topz.val('0');
            // 清空標題
            titlez.val('');
            // 清空取得辦法
            subTitlez.val('');
            // 清空封面圖
            titleImagez.val('');
            // 回復預設
            titleSamplez.html(`<img src="./img/samples/sample800x640.jpg">`);
            // 清空金額
            amountz.val('');
            // 清空簡述
            briefz.val('');
            // 清空編輯器
            let cntsLen = quill.getLength();
            quill.deleteText(0, cntsLen);

            $(document).scrollTop(0); // 置頂
        };
    });
    // Top
    topz.on('change', function () {
        if ($(this).prop('checked') == true) {
            $(this).val('1');
        } else {
            $(this).val('0');
        };
    });
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
                <img src="./img/samples/sample800x640.jpg">
            `);
            // 清空 file 的值
            file.val('');
        };
    });
    // Add 新增
    btnAddz.on('click', function (e) {
        e.preventDefault();
        // 驗證
        dataUpdateCheck(idz, titlez, subTitlez, titleImagez, amountz, briefz, quill);
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
            dataObj.append('primary', donateType);
            dataObj.append('secondary', secondType);
            dataObj.append('first', topz.val().trim());
            dataObj.append('title', titlez.val().trim());
            dataObj.append('content1', subTitlez.val().trim());
            dataObj.append('cover', titleImagez[0].files[0]);
            dataObj.append('amount', amountz.val().trim());
            dataObj.append('content2', briefz.val().trim());
            dataObj.append('enabled', enabled);
            dataObj.append('content3', JSON.stringify(quill.getContents()));
            dataObj.append('cNameArr', JSON.stringify(fNameArr));
            if (confirm("您確定要新增嗎？")) {
                // 傳送
                let xhr = new XMLHttpRequest();
                xhr.onload = function () {
                    if (xhr.status == 200 || xhr.status == 201) {
                        alert("新增成功！");
                        location.href = './donation_02.html';
                    } else {
                        if (JSON.parse(xhr.responseText).Message == "FirstOnlyOne") {
                            // alert("錯誤訊息 " + xhr.status + "：您的連線異常，請重新登入！");
                            alert("目前已有設定置頂的結緣品項哦（只能設定一筆）！");
                            // location.reload();
                        } else {
                            alert("錯誤訊息 " + xhr.status + "：您的連線異常，請重新登入！");
                            // location.reload();
                        };
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