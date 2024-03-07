// 宣告要帶入的欄位
let topz = $('.topz'), titlez = $('.titlez'), subTitlez = $('.subTitlez'), imgFilez = $('.imgFilez'), titleImagez = $('.titleImagez'), titleSamplez = $('.titleSamplez'), briefz = $('.briefz'), statusz = $('.statusz');
let btnBackOrgz = $('.btnBackOrgz'), disCover, orgTop, orgTitle, orgSubTitle, orgStatus, orgContent;
let sortz;
let btnReturnz = $('.btnReturnz'), btnResetz = $('.btnResetz'), btnSavez = $('.btnSavez');
CONNECT = 'directorIntroduction';
// 接收資料，做渲染、處理
function process(data) {
    // 顯示圖片 加上參數變數來破壞 cache
    disCover = data.imageURL + `?editz${paramRandoms()}`;
    // 原資料
    orgTop = data.first, orgTitle = data.name, orgSubTitle = data.subtitle, orgStatus = data.enabled, orgBrief = data.brief, orgContent = data.content;
    // 置頂狀態
    topz.val(data.first);
    if (topz.val() == "1") {
        topz.prop('checked', true);
    } else {
        topz.prop('checked', false);
    };
    //顯示封面圖片
    titleSamplez.append(`<img class="cover" src="${disCover}">`);
    // 標題
    titlez.val(data.name);
    // 副標題
    subTitlez.val(data.subtitle);
    // 顯示狀態
    statusz.val(data.enabled);
    if (statusz.val() == "1") {
        statusz.prop('checked', true);
    } else {
        statusz.prop('checked', false);
    };
    // 簡述
    briefz.val(data.brief);
    // 排序
    sortz = data.sort;
    // 將取得的內容渲染至編輯頁面上
    quill.setContents(JSON.parse(data.content).ops);
    // 頁面載入完成 
    loadingCompleted();
};
// NOTFOUND
function fails() { };
// 驗證
function dataUpdateCheck(aId, title, sub, brief, content) {
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
    if (sub.val().trim() === '' || sub.val().trim().length > 40) {
        sub.focus();
        $(document).scrollTop(0); // 置頂
        return check = false, errorText = '請確認副標題欄位有確實填寫、格式是否正確（長度不得超過 40 個字元）！';
    }
    if (brief.val().trim() === '' || brief.val().trim().length > 150) {
        brief.focus();
        $(document).scrollTop(0); // 置頂
        return check = false, errorText = '請確認簡述欄位有確實填寫、格式是否正確（長度不得超過 150 個字元）！';
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
    // 從 localStorage 取編號，用於呼叫要修改的訊息
    let numz = localStorage.getItem('chaNum');
    if (numz) {
        let dataObj = {
            "Methods": "GET",
            "APIs": URL,
            "CONNECTs": `${CONNECT}/${numz}`,
            "QUERYs": "",
            "Counts": listSize,
            "Sends": "",
        };
        //
        getPageDatas(dataObj).then(res => {
            // DOSOMETHING
            if (res !== null) {
                process(res);
            } else { };
        }, rej => {
            if (rej == "NOTFOUND") { };
        });
    };
    // 返回
    btnReturnz.on('click', backToPrevious);
    // Top
    topz.on('change', function () {
        if ($(this).prop('checked') == true) {
            $(this).val('1');
        } else {
            $(this).val('0');
        };
    });
    // 顯示狀態控制
    statusz.on('change', function () {
        if ($(this).prop('checked') == true) {
            $(this).val("1");
        } else {
            $(this).val("0");
        }
    });
    // 上傳圖片 取得圖片路徑    
    titleImagez.on('change', function () {
        let file = $(this);
        if (file.val() !== "") {
            if (imgUpdateCheck(file)) {
                if (window.URL !== undefined) {
                    let url = window.URL.createObjectURL(file[0].files[0]);
                    let currentImages = `
                        <img src="${url}">
                    `;
                    titleSamplez.html(currentImages);
                };
            }
        } else {
            if (disCover !== "") {
                titleSamplez.html(`<img src="${disCover}">`);
                titleImagez.val('');
            };
        };
    });
    // 回復原圖
    btnBackOrgz.on('click', function (e) {
        e.preventDefault();
        if (disCover !== "") {
            titleSamplez.html(`<img src="${disCover}">`);
            titleImagez.val('');
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
                // 圖片格式為路徑：在編輯器點擊圖片上傳，選擇好圖片時就上傳並且回傳路徑用以預覽
                let dataObj = new FormData();
                dataObj.append('directorIntroductionId', numz)
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
                        alert("錯誤訊息 " + xhr.status + "：您的連線異常，請重新登入！");
                        location.reload();
                    };
                };
                xhr.open('PUT', `${URL}${CONNECT}/image/${numz}`, true);
                // xhr.setRequestHeader('Content-Type', 'multipart/form-data');
                xhr.send(dataObj);

                file.val(''); // 重置檔案上傳欄位的內容，避免重複出現預覽圖片
            }
        });
    });
    // Reset 重置
    btnResetz.on('click', function (e) {
        e.preventDefault()
        if (confirm("您確定要重置為原本的內容嗎？")) {
            // 置頂狀態
            topz.val(orgTop);
            if (topz.val() == "1") {
                topz.prop('checked', true);
            } else {
                topz.prop('checked', false);
            };
            // 原標題
            titlez.val(orgTitle);
            // 原副標題
            subTitlez.val(orgSubTitle);
            // 清空已設的封面圖
            titleImagez.val('');
            // 回復原圖
            if (disCover !== "") {
                titleSamplez.html(`<img src="${disCover}">`)
            };
            // 清空簡述
            briefz.val(orgBrief);
            // 顯示狀態
            statusz.val(orgStatus);
            if (statusz.val() == "1") {
                statusz.prop('checked', true);
            } else {
                statusz.prop('checked', false);
            };
            // 原編輯器內容
            quill.setContents(JSON.parse(orgContent).ops);

            $(document).scrollTop(0); // 置頂
        };
    });
    // Save 儲存
    btnSavez.on('click', function (e) {
        e.preventDefault();
        // 驗證
        dataUpdateCheck(idz, titlez, subTitlez, briefz, quill);
        if (check == true) {
            // 取得圖片的名稱包成陣列
            let file = $('.ql-editor').find('img');
            if (file) {
                fNameArr = [];
                for (let i = 0; i < file.length; i++) {
                    fNameArr.push(file.eq(i).attr('src').split('/').pop());
                };
            };
            // 將要新增的內容傳送至後台資料庫中儲存
            let dataObj = new FormData();
            dataObj.append('directorIntroductionId', numz);
            dataObj.append('first', topz.val().trim());
            dataObj.append('name', titlez.val().trim());
            dataObj.append('subtitle', subTitlez.val().trim());
            dataObj.append('enabled', statusz.val().trim());
            dataObj.append('cover', titleImagez[0].files[0]);
            dataObj.append('brief', briefz.val().trim());
            dataObj.append('sort', sortz);
            dataObj.append('content', JSON.stringify(quill.getContents()));
            dataObj.append('cNameArr', JSON.stringify(fNameArr));
            if (confirm("您確定要儲存修改嗎？")) {
                // 傳送
                let xhr = new XMLHttpRequest();
                xhr.onload = function () {
                    if (xhr.status == 200 || xhr.status == 204) {
                        alert("修改成功！");
                        location.href = './chairman_02.html';
                    } else {
                        if (JSON.parse(xhr.responseText).Message == "FirstOnlyOne") {
                            // alert("錯誤訊息 " + xhr.status + "：您的連線異常，請重新登入！");
                            alert("目前已有設定現任的理事長資訊囉（只能設定一筆）！");
                            // location.reload();
                        } else {
                            alert("錯誤訊息 " + xhr.status + "：您的連線異常，請重新登入！");
                            location.reload();
                        };
                    };
                };
                xhr.open('PUT', `${URL}${CONNECT}/${numz}`, true);
                // xhr.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded');
                xhr.send(dataObj);
            };
        } else {
            alert(errorText);
        };
    });
});