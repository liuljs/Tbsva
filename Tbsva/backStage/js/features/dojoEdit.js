// 宣告要帶入的欄位
let sortz = $('.sortz'), titlez = $('.titlez'), classifyz = $('.classifyz'), positionz = $('.positionz'), addressz = $('.addressz'), linkz = $('.linkz');
let statusz = $('.statusz'), topz = 0, iconz;
let long, lat; // 緯度、經度

let orgTop, orgSort, orgTitle, orgClassify, orgPosition, orgAddress, orgLink, orgStatus, orgContent;
let btnReturnz = $('.btnReturnz'), btnResetz = $('.btnResetz'), btnSavez = $('.btnSavez');

CONNECT = "taiwanDojo";
// 接收資料，做渲染、處理
function process(data) {
    // 原資料
    orgTop = data.first, orgSort = data.sort, orgTitle = data.name, orgClassify = data.categoryId, orgPosition = `${data.longitude},${data.latitude}`, orgAddress = data.address, orgLink = data.video, orgStatus = data.enabled, orgContent = data.content;
    // 置頂狀態
    // topz.val(data.first);
    // if (topz.val() == "1") {
    //     topz.prop('checked', true);
    // } else {
    //     topz.prop('checked', false);
    // };
    // 排序
    sortz.val(data.sort);
    // 標題
    titlez.val(data.name);
    // 類別
    classifyz.val(data.categoryId);
    // 經緯度
    positionz.val(`${data.longitude},${data.latitude}`);
    // 地址
    if (data.address !== "" && data.address !== "[]") {
        let addrArr = JSON.parse(data.address);
        addressz.find('.twZipCodes').twzipcode('set', {
            'zipcode': addrArr[0],
            'county': addrArr[1],
            'district': addrArr[2]
        });
        addressz.find('.inputz').val(addrArr[3]);
    };
    // 地圖連結
    linkz.val(data.video);
    // 顯示狀態
    statusz.val(data.enabled);
    if (statusz.val() == "1") {
        statusz.prop('checked', true);
    } else {
        statusz.prop('checked', false);
    };
    // 將取得的內容渲染至編輯頁面上
    quill.setContents(JSON.parse(data.content).ops);
    // 頁面載入完成 
    loadingCompleted();
};
// 驗證
function dataUpdateCheck(aId, sort, title, classify, position, link) {
    if (aId.trim() === '') {
        title.focus();
        $(document).scrollTop(0); // 置頂
        return check = false, errorText = '請確認必填欄位皆有填寫！';
    }
    if (sort.val().trim() === '') {
        sort.focus();
        $(document).scrollTop(0); // 置頂
        return check = false, errorText = '請確認排序欄位有確實填寫！';
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
    // 從 localStorage 取編號，用於呼叫要修改的訊息
    let numz = localStorage.getItem('dojNum');
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
    // topz.on('change', function () {
    //     if ($(this).prop('checked') == true) {
    //         $(this).val('1');
    //     } else {
    //         $(this).val('0');
    //     };
    // });
    // 顯示狀態控制
    statusz.on('change', function () {
        if ($(this).prop('checked') == true) {
            $(this).val("1");
        } else {
            $(this).val("0");
        }
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
                dataObj.append('taiwanDojoId', numz)
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

                        } else {
                            alert(callBackData.Content);
                        }
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
            // topz.val(orgTop);
            // if (topz.val() == "1") {
            //     topz.prop('checked', true);
            // } else {
            //     topz.prop('checked', false);
            // };
            // 排序
            sortz.val(orgSort);
            // 原標題
            titlez.val(orgTitle);
            // 原類別
            classifyz.val(orgClassify);
            // 原經緯度
            positionz.val(orgPosition);
            // 地址
            if (orgAddress.address !== "" && orgAddress.address !== "[]") {
                let addrArr = JSON.parse(orgAddress);
                addressz.find('.twZipCodes').twzipcode('set', {
                    'zipcode': addrArr[0],
                    'county': addrArr[1],
                    'district': addrArr[2]
                });
                addressz.find('.inputz').val(addrArr[3]);
            };
            // 地圖連結
            linkz.val(orgLink);
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
        dataUpdateCheck(idz, sortz, titlez, classifyz, positionz, linkz);
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
            };
            // 小圖示
            if (quill.getLength() > 1) {
                iconz = 1;
            } else {
                iconz = 0;
            }
            // 將要新增的消息內容傳送至後台資料庫中儲存
            let dataObj = new FormData();
            dataObj.append('first', topz);
            dataObj.append('sort', sortz.val().trim());
            dataObj.append('name', titlez.val().trim());
            dataObj.append('categoryId', classifyz.val().trim());
            dataObj.append('longitude', long);
            dataObj.append('latitude', lat);
            dataObj.append('address', JSON.stringify(addrArr));
            dataObj.append('video', linkz.val().trim());
            dataObj.append('enabled', statusz.val().trim());
            dataObj.append('icon', iconz);
            dataObj.append('content', JSON.stringify(quill.getContents()));
            dataObj.append('cNameArr', JSON.stringify(fNameArr));
            if (confirm("您確定要儲存修改嗎？")) {
                // 傳送
                let xhr = new XMLHttpRequest();
                xhr.onload = function () {
                    if (xhr.status == 200 || xhr.status == 204) {
                        alert("修改成功！");
                        location.href = './dojo_03.html';
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