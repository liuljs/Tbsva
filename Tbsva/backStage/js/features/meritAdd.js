// 宣告要帶入的欄位
let topz = $('.topz'), titlez = $('.titlez'), imgFilez = $('.imgFilez'), titleImagez = $('.titleImagez'), linkz = $('.linkz'), URLz, uIdz, coverz, titleSamplez = $('.titleSamplez'), contentz = $('.contentz');
let btnBackOrgz = $('.btnBackOrgz'), btnResetz = $('.btnResetz'), btnAddz = $('.btnAddz'), btnReturnz = $('.btnReturnz');
let enabled = 1;
CONNECT = 'video2';
// 驗證
function dataUpdateCheck(aId, title, link, content) {
    if (aId.trim() === "") {
        title.focus();
        $(document).scrollTop(0); // 置頂
        return check = false, errorText = '請確認必填欄位皆有填寫！';
    }
    if (title.val().trim() === "" || title.val().trim().length > 40) {
        title.focus();
        $(document).scrollTop(0); // 置頂
        return check = false, errorText = '請確認標題欄位有確實填寫、格式是否正確（長度不得超過 40 個字元）！';
    }
    if (link.val().trim() === "" || link.val().trim().indexOf('youtu.be') === -1) {
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
    }
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
            // 清空顯示
            topz.val('0'), topz.prop('checked', false);
            // 清空標題
            titlez.val('');
            // 清空封面圖
            titleImagez.val('');
            // 清空圖片紀錄
            URLz = "", uIdz = "", coverz = "";
            // 回復預設
            titleSamplez.html(`<img src="./img/samples/sample640x480.jpg">`);
            // 清空內容
            contentz.val('');
            // 清空影片連結
            linkz.val('');
            // 清空介紹內容
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
    // 影片縮圖
    linkz.on('change', function () {
        // 取連結網址
        if (linkz.val().trim().indexOf('youtu.be') !== -1) {
            // 如果有先使用自行上傳，就清空
            titleImagez.val('');
            // 取得 YouTube 影片id、縮圖
            uIdz = linkz.val().trim().slice(linkz.val().trim().lastIndexOf('/') + 1);
            let currentImages = `
                    <img class="sampleImagez" src="https://img.youtube.com/vi/${uIdz}/sddefault.jpg">
                `;
            titleSamplez.html(currentImages);
            // 上傳圖片 1.YouTube 圖片連結 2.自行上傳 檔案物件 
            coverz = `https://img.youtube.com/vi/${uIdz}/sddefault.jpg`;
        } else {
            // 清空 YouTube 影片id、縮圖、上傳圖片
            uIdz = "", coverz = "";
            // 清空自行上傳
            titleImagez.val('');
            // 回復預設
            titleSamplez.html(`<img src="./img/samples/sample640x480.jpg">`);
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
                    coverz = file[0].files[0];
                };
            };
        } else {
            if (uIdz !== "" && uIdz !== undefined) {
                // 圖片預覽改成預設（YouTube 影片縮圖）
                let currentImages = `
                    <img class="sampleImagez" src="https://img.youtube.com/vi/${uIdz}/sddefault.jpg">
                `;
                titleSamplez.html(currentImages);
                // 上傳圖片 1.YouTube 圖片連結 2.自行上傳 檔案物件
                coverz = `https://img.youtube.com/vi/${uIdz}/sddefault.jpg`;
            } else {
                // 回復預設
                titleSamplez.html(`<img src="./img/samples/sample640x480.jpg">`);
                // 清空 file 的值
                file.val('');
                // 再次確認 1.清空上傳圖片 2.清空 uIdz
                coverz = "", uIdz = "";
            };
        };
    });
    // 回復預設圖片
    btnBackOrgz.on('click', function () {
        if (uIdz !== "" && uIdz !== undefined) {
            // 圖片預覽改成預設（YouTube 影片縮圖）
            let currentImages = `
                <img class="sampleImagez" src="https://img.youtube.com/vi/${uIdz}/sddefault.jpg">
            `;
            titleSamplez.html(currentImages);
            // 清空 file 的值
            titleImagez.val('');
            // 上傳圖片 1.YouTube 圖片連結 2.自行上傳 檔案物件
            coverz = `https://img.youtube.com/vi/${uIdz}/sddefault.jpg`;
        } else {
            // 回復預設
            titleSamplez.html(`<img src="./img/samples/sample640x480.jpg">`);
            // 清空 file 的值
            titleImagez.val('');
            // 再次確認 1.清空上傳圖片 2.清空 uIdz
            coverz = "", uIdz = "";
        };
    });
    // Add 新增
    btnAddz.on('click', function (e) {
        e.preventDefault();
        // 驗證
        dataUpdateCheck(idz, titlez, linkz, titleImagez, contentz);
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
            // 確定 uIdz 不是空值，取連結網址
            if (uIdz !== "") {
                URLz = `https://www.youtube.com/embed/${uIdz}`;
            };
            // 將要新增的內容傳送至後台資料庫中儲存
            let dataObj = new FormData();
            dataObj.append('first', topz.val().trim());
            dataObj.append('name', titlez.val().trim());
            dataObj.append('video', URLz);
            dataObj.append('cover', coverz);
            dataObj.append('brief', notBriefz);
            dataObj.append('enabled', enabled);
            dataObj.append('content', JSON.stringify(contentz.val().trim()));
            dataObj.append('cNameArr', JSON.stringify(fNameArr));
            if (confirm("您確定要新增嗎？")) {
                // 傳送
                let xhr = new XMLHttpRequest();
                xhr.onload = function () {
                    if (xhr.status == 200 || xhr.status == 201) {
                        alert("新增成功！");
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
                xhr.open('POST', `${URL}${CONNECT}`, true);
                // xhr.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded');
                xhr.send(dataObj);
            };
        } else {
            alert(errorText);
        };
    });
});