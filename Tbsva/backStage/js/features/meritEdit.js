// 宣告要帶入的欄位
let topz = $('.topz'), titlez = $('.titlez'), imgFilez = $('.imgFilez'), titleImagez = $('.titleImagez'), titleSamplez = $('.titleSamplez'), linkz = $('.linkz'), URLz, uIdz, coverz, contentz = $('.contentz');
let btnBackOrgz = $('.btnBackOrgz'), disCover, orgTop, orgTitle, orgClassify, orgLink, orgContent;
let btnReturnz = $('.btnReturnz'), btnResetz = $('.btnResetz'), btnSavez = $('.btnSavez');
let enabled = 1, sortz;
CONNECT = "video2";
// 接收資料，做渲染、處理
function process(data) {
    // 顯示圖片 加上參數變數來破壞 cache
    disCover = data.imageURL + `?editz${paramRandoms()}`;
    // 原資料
    orgTop = data.first, orgTitle = data.name, orgLink = data.video, orgContent = data.content;
    // 顯示狀態
    topz.val(data.first);
    if (topz.val() == "1") {
        topz.prop('checked', true);
    } else {
        topz.prop('checked', false);
    };
    //顯示封面圖片
    titleSamplez.append(`<img class="cover" src="${disCover}">`);
    // 原 YouTube 的影片id、縮圖（或者是自行上傳的圖片）
    coverz = data.imageURL;
    if (coverz.indexOf('youtube') !== -1) {
        uIdz = data.video.slice(data.video.lastIndexOf('/') + 1);
    } else {
        uIdz = "";
    };
    // 標題
    titlez.val(data.name);
    // 影片網址
    linkz.val(data.video);
    // 排序
    sortz = data.sort;
    // 將取得的內容渲染至編輯頁面上
    contentz.val(JSON.parse(data.content));
    // 頁面載入完成 
    loadingCompleted();
};
// 驗證
function dataUpdateCheck(aId, title, link, content) {
    if (aId.trim() === '') {
        title.focus();
        $(document).scrollTop(0); // 置頂
        return check = false, errorText = '請確認必填欄位皆有填寫！';
    }
    if (title.val().trim() === "" || title.val().trim().length > 40) {
        title.focus();
        $(document).scrollTop(0); // 置頂
        return check = false, errorText = '請確認標題欄位有確實填寫、格式是否正確（長度不得超過 40 個字元）！';
    }
    if (link.val().trim() === "" || link.val().trim().indexOf('youtu.be') === -1 && link.val().trim().indexOf('youtube') === -1) {
        link.focus();
        $(document).scrollTop(0); // 置頂
        return check = false, errorText = '請確認連結欄位有確實填寫、格式是否正確（YouTube 的分享網址）！';
    }
    if (content.val() === "") {
        $(document).scrollTop(0); // 置頂
        return check = false, errorText = '請確認內容有確實填寫！';
    }
    else {
        return check = true, errorText = "";
    };
};
$().ready(function () {
    // 從 localStorage 取編號，用於呼叫要修改的訊息
    let numz = localStorage.getItem('merNum');
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
    // 影片縮圖
    linkz.on('change', function () {
        // 取連結網址 1.重新貼上一個新的 YouTube 分享網址 2.貼上原本以處理過的 YouTube 分享網址
        if (linkz.val().trim().indexOf('youtu.be') !== -1 || linkz.val().trim().indexOf('youtube') !== -1) {
            // 如果有先自行上傳，就清空
            titleImagez.val('');
            // 取得 YouTube 影片id、縮圖
            uIdz = linkz.val().trim().slice(linkz.val().trim().lastIndexOf('/') + 1);
            let currentImages = `
                    <img src="https://img.youtube.com/vi/${uIdz}/sddefault.jpg">
                `;
            titleSamplez.html(currentImages);
            // 上傳圖片 1.YouTube 圖片連結 2.自行上傳 檔案物件 
            coverz = `https://img.youtube.com/vi/${uIdz}/sddefault.jpg`;
        } else { // 如果是空值 || 如果不是 Youtube 分享網址
            uIdz = "";
            // 清空自行上傳
            titleImagez.val('');
            // 回復預設
            titleSamplez.html(`<img src="./img/samples/sample640x480.jpg">`);
            // 清空上傳圖片
            coverz = "";
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
                    coverz = file[0].files[0];
                };
            };
        } else {
            if (uIdz !== "" && uIdz !== undefined) {
                // 圖片預覽改成預設（YouTube 影片縮圖）
                let currentImages = `
                    <img src="https://img.youtube.com/vi/${uIdz}/sddefault.jpg">
                `;
                titleSamplez.html(currentImages);
                // 上傳圖片 1.YouTube 圖片連結 2.自行上傳 檔案物件
                coverz = `https://img.youtube.com/vi/${uIdz}/sddefault.jpg`;
            } else {
                if (linkz.val() == orgLink) {
                    // 回復預設
                    titleSamplez.html(`<img src="${disCover}"`);
                    coverz = disCover, uIdz = "";
                    // 清空 file 的值
                    file.val('');
                } else {
                    // 回復預設
                    titleSamplez.html(`<img src="./img/samples/sample640x480.jpg" alt="">`);
                    // 清空 file 的值
                    file.val('');
                    // 再次確認 1.清空上傳圖片 2.清空 uIdz
                    coverz = "", uIdz = "";
                };
            };
        };
    });
    // 回復原圖
    btnBackOrgz.on('click', function () {
        if (uIdz !== "" && uIdz !== undefined) {
            // 圖片預覽改成預設（YouTube 影片縮圖）
            let currentImages = `
                <img src="https://img.youtube.com/vi/${uIdz}/sddefault.jpg">
            `;
            titleSamplez.html(currentImages);
            // 清空 file 的值
            titleImagez.val('');
            // 上傳圖片 1.YouTube 圖片連結 2.自行上傳 檔案物件
            coverz = `https://img.youtube.com/vi/${uIdz}/sddefault.jpg`;
        } else {
            if (linkz.val() == orgLink) {
                titleSamplez.html(`<img src="${disCover}">`);
                titleImagez.val('');
                coverz = disCover;
            } else {
                // 回復預設
                titleSamplez.html(`<img src="./img/samples/sample640x480.jpg">`);
                // 清空 file 的值
                titleImagez.val('');
                // 再次確認 1.清空上傳圖片 2.清空 uIdz
                coverz = "", uIdz = "";
            };
        };
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
            // 原連結
            linkz.val(orgLink);
            // 清空已設的封面圖
            titleImagez.val('');
            // 回復原圖
            if (disCover !== "") {
                titleSamplez.html(`<img src="${disCover}">`);
                coverz = disCover;
            };
            // 原內容
            contentz.val(JSON.parse(orgContent));

            $(document).scrollTop(0); // 置頂
        };
    });
    // Save 儲存
    btnSavez.on('click', function (e) {
        e.preventDefault();
        // 驗證
        dataUpdateCheck(idz, titlez, linkz, contentz);
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
            // 取連結網址
            if (uIdz !== "") {
                URLz = `https://www.youtube.com/embed/${uIdz}`;
            } else {
                URLz = linkz.val().trim();
            };
            // 將要新增的消息內容傳送至後台資料庫中儲存
            let dataObj = new FormData();
            dataObj.append('video2Id', numz);
            dataObj.append('first', topz.val().trim());
            dataObj.append('name', titlez.val().trim());
            dataObj.append('video', URLz);
            dataObj.append('cover', coverz);
            dataObj.append('brief', notBriefz);
            dataObj.append('sort', sortz);
            dataObj.append('enabled', enabled);
            dataObj.append('content', JSON.stringify(contentz.val().trim()));
            dataObj.append('cNameArr', JSON.stringify(fNameArr));
            if (confirm("您確定要儲存修改嗎？")) {
                // 傳送
                let xhr = new XMLHttpRequest();
                xhr.onload = function () {
                    if (xhr.status == 200 || xhr.status == 204) {
                        alert("修改成功！");
                        location.href = './annualMerit_02.html';
                    } else {
                        if (JSON.parse(xhr.responseText).Message == "FirstOnlyOne") {
                            // alert("錯誤訊息 " + xhr.status + "：您的連線異常，請重新登入！");
                            alert("目前已有設定顯示的功德主方案囉（只能設定一筆）！");
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