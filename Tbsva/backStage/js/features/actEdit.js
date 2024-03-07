// 宣告要帶入的欄位
let topz = $('.topz'), titlez = $('.titlez'), picz = $('.picz'), imgFilez = $('.imgFilez'), titleImagez = $('.titleImagez'), picImagez = $('.picImagez'), picParents = picImagez.parents('.fileinput'), imgLens = picImagez.length, titleSamplez = $('.titleSamplez'), samplez = $('.samplez'), classifyz = $('.classifyz');
let statusz = $('.statusz'), sortz;

let btnBackOrgz = $('.btnBackOrgz'), disCover, disImageArr, orgTop, orgSort, orgTitle, orgClassify, orgStatus, orgContent;
let btnReturnz = $('.btnReturnz'), btnResetz = $('.btnResetz'), btnSavez = $('.btnSavez');

CONNECT = "activity";
// 接收資料，做渲染、處理
function process(data) {
    // 紀錄、顯示圖片、加上參數變數來破壞 cache
    disCover = data.cover + `?editz${paramRandoms()}`;
    disImageArr = [data.imageURL01, data.imageURL02, data.imageURL03, data.imageURL04, data.imageURL05, data.imageURL06, data.imageURL07, data.imageURL08];
    // 原資料
    orgTop = data.first, orgTitle = data.name, orgClassify = data.categoryId, orgStatus = data.enabled, orgContent = data.content;
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
    // 類別
    classifyz.val(data.categoryId);
    // 顯示狀態
    statusz.val(data.enabled);
    if (statusz.val() == "1") {
        statusz.prop('checked', true);
    } else {
        statusz.prop('checked', false);
    };
    // 導覽圖
    for (let i = 0; i < imgLens; i++) {
        if (data[`imageURL0${i + 1}`] !== "" && data[`imageURL0${i + 1}`] !== null) {
            picParents.eq(imgLens - 1 - i).addClass('fileinput-exists').removeClass('fileinput-new');
            picParents.eq(imgLens - 1 - i).find('.fileinput-preview').html(`<img src="${disImageArr[i]}?editz${paramRandoms()}">`);
        };
    };
    // 將取得的內容渲染至編輯頁面上
    quill.setContents(JSON.parse(data.content).ops);
    // 排序：不做使用（預設值）
    sortz = data.sort;
    // 頁面載入完成 
    loadingCompleted();
};
// 驗證
function dataUpdateCheck(aId, title, classify, content) {
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
    if (classify.val().trim() === 'preset') {
        classify.focus();
        $(document).scrollTop(0); // 置頂
        return check = false, errorText = '請確認類別欄位有確實填寫！';
    }
    if (content.getLength() === 1) {
        $(document).scrollTop(0); // 置頂
        return check = false, errorText = '請確認消息內容有確實填寫！';
    }
    else {
        return check = true, errorText = "";
    }
};
$().ready(function () {
    // 從 localStorage 取編號，用於呼叫要修改的訊息
    let numz = localStorage.getItem('actNum');
    // 取得類別
    let clsObj = {
        "Methods": "GET",
        "APIs": URL,
        "CONNECTs": `${CONNECT}/category`,
        "QUERYs": "",
        "Counts": listSize,
        "Sends": "",
    };
    getPageDatas(clsObj).then(res => {
        // DOSOMETHING
        html = `<option value="preset" selected>選擇分類</option>`;
        if (res !== null) {
            for (let i = 0; i < res.length; i++) {
                html += `
                    <option value="${res[i].categoryId}">${res[i].name}</option>
                `;
            };
        };
        classifyz.html(html);
        if (numz) {
            //
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
                // DO SOMETHING
                if (res !== null) {
                    process(res);
                } else { };
            }, rej => {
                if (rej == "NOTFOUND") { };
            });
        };
    }, rej => {
        if (rej == "NOTFOUND") { };
    });
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
    //
    imgFilez.on('change', function () {
        let file = $(this);
        if (file[0].files.length !== 0) {
            imgUpdateCheck(file); // 檢查
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
                dataObj.append('activityId', numz)
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
        e.preventDefault();
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
            // 原類別
            classifyz.val(orgClassify)
            // 清空已設的封面圖
            titleImagez.val('');
            // 回復原圖
            if (disCover !== "") {
                titleSamplez.html(`<img src="${disCover}">`)
            };
            // 顯示狀態
            statusz.val(orgStatus);
            if (statusz.val() == "1") {
                statusz.prop('checked', true);
            } else {
                statusz.prop('checked', false);
            };
            // 回復導覽圖片
            for (let i = 0; i < disImageArr.length; i++) {
                if (disImageArr[i] !== "" && disImageArr[i] !== null) {
                    picParents.eq(imgLens - 1 - i).addClass('fileinput-exists').removeClass('fileinput-new');
                    picParents.eq(imgLens - 1 - i).find('.fileinput-preview').html(`<img src="${disImageArr[i]}?editz${paramRandoms()}">`);
                } else {
                    picParents.eq(imgLens - 1 - i).find('.picImagez').val('');
                    picParents.eq(imgLens - 1 - i).addClass('fileinput-new').removeClass('fileinput-exists');
                };
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
        dataUpdateCheck(idz, titlez, classifyz, quill);
        if (check == true) {
            // 取得圖片的名稱包成陣列
            let file = $('.ql-editor').find('img');
            if (file) {
                fNameArr = [];
                for (let i = 0; i < file.length; i++) {
                    fNameArr.push(file.eq(i).attr('src').split('/').pop());
                };
            };
            // 將要新增的消息內容傳送至後台資料庫中儲存
            let dataObj = new FormData();
            dataObj.append('activityId', numz);
            dataObj.append('first', topz.val().trim());
            dataObj.append('sort', sortz);
            dataObj.append('cover', titleImagez[0].files[0]);
            dataObj.append('name', titlez.val().trim());
            dataObj.append('categoryId', classifyz.val().trim());
            dataObj.append('brief', notBriefz);
            dataObj.append('enabled', statusz.val().trim());
            for (let i = 0; i < imgLens; i++) {
                if (picImagez[i].files[0] !== undefined) { // 如果有 新增 或 修改 的圖片
                    dataObj.append(`navPics0${i + 1}`, picImagez[i].files[0]);
                } else { // 如果沒有就保持原來的 (原有圖片 或者 "")
                    dataObj.append(`navPics0${i + 1}`, disImageArr[i]);
                };
            };
            dataObj.append('content', JSON.stringify(quill.getContents()));
            dataObj.append('cNameArr', JSON.stringify(fNameArr));
            if (confirm("您確定要儲存修改嗎？")) {
                // 傳送
                let xhr = new XMLHttpRequest();
                xhr.onload = function () {
                    if (xhr.status == 200 || xhr.status == 204) {
                        alert("修改成功！");
                        location.href = './activity_03.html';
                    } else {
                        alert("錯誤訊息 " + xhr.status + "：您的連線異常，請重新登入！")
                        location.reload();
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