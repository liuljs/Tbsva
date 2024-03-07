// 宣告要帶入的欄位
let titlez = $('.titlez'), positionz = $('.positionz'), addressz = $('.addressz'), linkz = $('.linkz');
let classifyz = $('.classifyz');

let btnResetz = $('.btnResetz'), btnAddz = $('.btnAddz'), btnReturnz = $('.btnReturnz');
let enabled = 1, iconz = 0, topz = 0; // 狀態預設為顯示 小圖示預設為不顯示
let long, lat; // 緯度、經度
CONNECT = 'taiwanDojo';
// 驗證
function dataUpdateCheck(aId, title, classify, position, link) {
    if (aId.trim() === '') {
        title.focus();
        $(document).scrollTop(0); // 置頂
        return check = false, errorText = '請確認必填欄位皆有填寫！';
    }
    if (title.val().trim() === '' || title.val().trim().length > 40) {
        title.focus();
        $(document).scrollTop(0); // 置頂
        return check = false, errorText = '請確認名稱欄位有確實填寫、格式是否正確（長度不得超過 40 個字元）！';
    }
    if (classify.val().trim() === 'preset') {
        classify.focus();
        $(document).scrollTop(0); // 置頂
        return check = false, errorText = '請確認類別欄位有確實填寫！';
    }
    if (position.val().trim() === '' || position.val().trim().length > 40) {
        position.focus();
        $(document).scrollTop(0); // 置頂
        return check = false, errorText = '請確認經緯度欄位有確實填寫、格式是否正確（例如：23.96558325419615,120.69546737472919）！';
    }
    if (link.val().trim() !== "" && link.val().trim().indexOf('goo.gl/maps/') === -1) {
        link.focus();
        $(document).scrollTop(0); // 置頂
        return check = false, errorText = '請確認連結欄位格式是否正確（Google 地圖分享連結 https://goo.gl/maps/...）！';
    }
    else {
        return check = true, errorText = "";
    }
};
$().ready(function () {
    // 類別
    let dataObj = {
        "Methods": "GET",
        "APIs": URL,
        "CONNECTs": `${CONNECT}/category`,
        "QUERYs": "",
        "Counts": listSize,
        "Sends": "",
    };
    getPageDatas(dataObj).then(res => {
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
        // 頁面載入完成 
        loadingCompleted();
    }, rej => {
        if (rej == "NOTFOUND") {
            html = `<option value="preset" selected>選擇分類</option>`;
            classifyz.html(html);
        };
    });
    // 返回
    btnReturnz.on('click', backToPrevious);
    // Reset 重置
    btnResetz.on('click', function (e) {
        e.preventDefault()
        if (confirm("您確定要將目前編輯的內容全部清空嗎？")) {
            // 置頂
            // topz.prop('checked', false), topz.val('0');
            // 清空標題
            titlez.val('');
            // 重置類別
            classifyz.val('preset');
            // 清空經緯度
            positionz.val('');
            // twzipcode reset
            addressz.find('.twZipCodes').twzipcode('reset');
            // 清空詳細地址
            addressz.find('.inputz').val('');
            // 清空地圖
            linkz.val('');
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
    // Add 新增
    btnAddz.on('click', function (e) {
        e.preventDefault();
        // 驗證
        dataUpdateCheck(idz, titlez, classifyz, positionz, linkz);
        if (check == true) {
            // 取得經緯度
            let posArr = positionz.val().split(',');
            long = posArr[0], lat = posArr[1];
            // Dojo Address
            addrArr = [];
            addressz.find('.twZipCodes').twzipcode('get', function (county, district, zipcode) {
                if (county !== "") {
                    addrArr.push(county);
                    if (district !== "") {
                        addrArr.unshift(zipcode);
                        addrArr.push(district);
                    };
                };
                return addrArr;
            });
            if (addressz.find('.inputz').val() !== "") {
                addrArr.push(addressz.find('.inputz').val().trim());
            };
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
            // 小圖示
            if (quill.getLength() > 1) {
                iconz = 1;
            } else {
                iconz = 0;
            }
            // 將要新增的內容傳送至後台資料庫中儲存
            let dataObj = new FormData();
            dataObj.append('first', topz);
            dataObj.append('name', titlez.val().trim());
            dataObj.append('categoryId', classifyz.val().trim());
            dataObj.append('longitude', long);
            dataObj.append('latitude', lat);
            dataObj.append('address', JSON.stringify(addrArr));
            dataObj.append('video', linkz.val().trim());
            dataObj.append('enabled', enabled);
            dataObj.append('icon', iconz);
            dataObj.append('content', JSON.stringify(quill.getContents()));
            dataObj.append('cNameArr', JSON.stringify(fNameArr));
            if (confirm("您確定要新增嗎？")) {
                // 傳送
                let xhr = new XMLHttpRequest();
                xhr.onload = function () {
                    if (xhr.status == 200 || xhr.status == 201) {
                        alert("新增成功！");
                        location.href = './dojo_03.html';
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