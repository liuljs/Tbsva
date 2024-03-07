// Applies
let namez = $('.namez'), genderz = $('.genderz'), birthz = $('.birthz'), areaz = $('.areaz'), phonez = $('.phonez'), cellz = $('.cellz'), mailz = $('.mailz'), notez = $('.notez'), payz = $('.payz'), receiptz = $('.receiptz'), sendz = $('.sendz'), readz = $('.readz');
let contactAddrz = $('.contactAddrz'), contactArr = new Array();
let btnResetz = $('.btnResetz'), btnSendz = $('.btnSendz');
let enabled = 0, audit = 0;
CONNECT = 'addTbsva';
// 驗證欄位
function dataUpdateCheck(name, birth, area, address, phone, mail, read) {
    if (name.val().trim() === '' || name.val().trim().length > 30) {
        name.focus();
        return check = false, errorText = '請確認會員姓名是否確實填寫、格式是否正確（長度不得超過 30 個字元）！';
    }
    if (birth.val().trim() === '') {
        birth.focus();
        return check = false, errorText = '請確認出生日期是否確實選擇！';
    }
    if (area.val().trim() === '0') {
        area.focus();
        return check = false, errorText = '請確認隸屬地區是否確實選擇！';
    }
    if (address.find('.twZipCodes').twzipcode('get', 'county,district') === '' || address.find('.inputz').val() === '') {
        address.find('.inputz').focus();
        return check = false, errorText = '請確認聯絡地址是否確實選取、詳細地址是否確實填寫！';
    }
    if (phone.val().trim() === '' || PhoneRegExp.test(phone.val()) === false) {
        phone.focus();
        return check = false, errorText = '請確認聯絡電話是否確實填寫、格式是否正確！';
    }
    if (mail.val().trim() === '' || EmailRegExp.test(mail.val()) === false) {
        mail.focus();
        return check = false, errorText = '請確認電子信箱是否確實填寫、格式是否正確！';
    }
    if (Number(read.find('.checkz').val()) === 0) {
        return check = false, errorText = '請確實閱讀並且同意個人資料蒐集、處理、利用同意書的內容！';
    }
    else {
        return check = true, errorText = "";
    };

};
$(() => {
    //
    btnResetz.on('click', function (e) {
        e.preventDefault();
        let formz = $(this).parents('form');
        if (confirm('您確定要將已填寫的資料清空嗎？')) {
            // twzipcode reset
            $('.twZipCodes').twzipcode('reset');

            // form reset
            formz[0].reset(), totals = 0; // 總數重置;
            // checkbox status
            if (formz.find('.checkz').prop('checked') == false) {
                formz.find('.checkz').val('0');
            };

            $(document).scrollTop(formz.offset().top - 60);
        };
    });
    //
    btnSendz.on('click', function () {
        // Cell Phone 
        if (cellz.val() !== "") {
            if (CellRegExp.test(cellz.val()) === false) {
                cellz.focus();
                alert('請確認手機號碼格式是否正確！');
            };
        };
        dataUpdateCheck(namez, birthz, areaz, contactAddrz, phonez, mailz, readz);
        if (check == true) {
            // Contact Address
            contactArr = [];
            contactAddrz.find('.twZipCodes').twzipcode('get', function (county, district, zipcode) {
                if (county !== "") {
                    contactArr.push(county);
                    if (district !== "") {
                        contactArr.unshift(zipcode);
                        contactArr.push(district);
                    };
                };
                return contactArr;
            });
            if (contactAddrz.find('.inputz').val() !== "") {
                contactArr.push(contactAddrz.find('.inputz').val().trim());
            };
            let dataObj = {
                "namez": namez.val().trim(),
                "gender": genderz.find('.radioz:checked').val().trim(),
                "birthz": birthz.val().trim(),
                "affiliatedAreaz": areaz.val().trim(),
                "contactAddress": JSON.stringify(contactArr),
                "contactNumber": phonez.val().trim(),
                "moblieNumber": cellz.val().trim(),
                "email": mailz.val().trim(),
                "contentz": notez.val(),
                "enabled": enabled,
                "audit": audit
            };
            if (confirm("您確定要送出嗎？")) {
                let xhr = new XMLHttpRequest();
                xhr.onload = function () {
                    if (xhr.status == 200 || xhr.status == 204) {
                        alert("送出成功！");
                        location.reload();
                    } else {
                        alert("錯誤訊息 " + xhr.status + "：您的連線異常，請重新登入！");
                        location.reload();
                    };
                };
                xhr.open('POST', `${URL}${CONNECT}`, true);
                xhr.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded');
                xhr.send($.param(dataObj));
            }
        } else {
            alert(errorText);
        };
    });
});