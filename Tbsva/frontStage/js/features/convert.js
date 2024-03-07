// Convert
let namez = $('.namez'), genderz = $('.genderz'), enNamez = $('.enNamez'), convertz = $('.convertz'), birthz = $('.birthz'), areaz = $('.areaz'), phonez = $('.phonez'), cellz = $('.cellz'), mailz = $('.mailz'), receiptz = $('.receiptz'), sendz = $('.sendz');
let residenceAddrz = $('.residenceAddrz'), contactAddrz = $('.contactAddrz'), sendAddrz = $('.sendAddrz'), cloneResidencez = $('.cloneResidencez'), clonez = $('.clonez'), residenceArr = new Array(), contactArr = new Array(), sendArr = new Array();
let btnResetz = $('.btnResetz'), btnSendz = $('.btnSendz');

let enabled = 0, auditz = 0;
// Rituals Video
let movieNumz = $('.movieNumz'), moviePlayz = $('.moviePlayz'), ritualItemz = $('.ritualz').find('.items');

CONNECT = 'convertForms';
// 驗證欄位
function dataUpdateCheck(name, enName, convert, birth, area, residence, contact, phone, mail, send) {
    if (name.val().trim() === '' || name.val().trim().length > 30) {
        name.focus();
        return check = false, errorText = '請確認中文姓名是否確實填寫、格式是否正確（長度不得超過 30 個字元）！';
    }
    if (enName.val().trim().length > 50) {
        enName.focus();
        return check = false, errorText = '請確認英文姓名格式是否正確（長度不得超過 50 個字元）！';
    }
    if (convert.val().trim() === 'preset') {
        convert.focus();
        return check = false, errorText = '請確認皈依者身分是否確實選擇！';
    }
    if (birth.val().trim() === '') {
        birth.focus();
        return check = false, errorText = '請確認出生日期是否確實選擇！';
    }
    if (area.val().trim() === '0') {
        area.focus();
        return check = false, errorText = '請確認隸屬地區是否確實選擇！';
    }
    if (residence.find('.twZipCodes').twzipcode('get', 'county,district') === '' || residence.find('.inputz').val() === '') {
        residence.find('.inputz').focus();
        return check = false, errorText = '請確認戶籍地址是否確實選取、詳細地址是否確實填寫！';
    }
    if (contact.find('.twZipCodes').twzipcode('get', 'county,district') === '' || contact.find('.inputz').val() === '') {
        contact.find('.inputz').focus();
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
    if (Number(send.find('.radioz:checked').val()) === 1) {
        if (send.find('.twZipCodes').twzipcode('get', 'county,district') === '' || send.find('.inputz').val() === '') {
            send.find('.inputz').focus();
            return check = false, errorText = '請確認寄送地址是否確實選取、詳細地址是否確實填寫！';
        } else {
            return check = true, errorText = "";
        };
    }
    else {
        return check = true, errorText = "";
    };

};
$(() => {
    // Convert Forms
    cloneResidencez.on('change', function () {
        if ($(this).prop('checked') == true) {
            let detailz = residenceAddrz.find('.inputz').val();
            if (residenceAddrz.find('.twZipCodes.disabled').length == 0) {
                residenceAddrz.find('.twZipCodes').twzipcode('get', function (county, district) {
                    if (county !== "" && district !== "") {
                        contactAddrz.find('.twZipCodes').twzipcode('set', {
                            'county': county,
                            'district': district
                        });

                    };
                    if (detailz !== "") {
                        contactAddrz.find('.inputz').val(detailz);
                    }
                });
            } else {
                contactAddrz.find('.inputz').val(detailz);
            }
        } else {
            contactAddrz.find('.twZipCodes').twzipcode('reset');
            contactAddrz.find('.inputz').val('');
        };
    });
    //
    clonez.on('change', function () {
        if ($(this).prop('checked') == true) {
            let detailz = contactAddrz.find('.inputz').val();
            if (contactAddrz.find('.twZipCodes.disabled').length == 0) {
                contactAddrz.find('.twZipCodes').twzipcode('get', function (county, district) {
                    if (county !== "" && district !== "") {
                        sendAddrz.find('.twZipCodes').twzipcode('set', {
                            'county': county,
                            'district': district
                        });
                    };
                    if (detailz !== "") {
                        sendAddrz.find('.inputz').val(detailz);
                    };
                });
            } else {
                sendAddrz.find('.inputz').val(detailz);
            }
        } else {
            sendAddrz.find('.twZipCodes').twzipcode('reset');
            sendAddrz.find('.inputz').val('');
        };
    });
    // sends
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
    btnSendz.on('click', function () {
        // Cell Phone 
        if (cellz.val() !== "") {
            if (CellRegExp.test(cellz.val()) === false) {
                cellz.focus();
                alert('請確認手機號碼格式是否正確！');
            }
        };
        dataUpdateCheck(namez, enNamez, convertz, birthz, areaz, residenceAddrz, contactAddrz, phonez, mailz, sendz);
        if (check == true) {
            // Residence Address
            residenceArr = [];
            residenceAddrz.find('.twZipCodes').twzipcode('get', function (county, district, zipcode) {
                if (county !== "") {
                    residenceArr.push(county);
                    if (district !== "") {
                        residenceArr.unshift(zipcode);
                        residenceArr.push(district);
                    };
                };
                return residenceArr;
            });
            if (residenceAddrz.find('.inputz').val() !== "") {
                residenceArr.push(residenceAddrz.find('.inputz').val().trim());
            };
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
            sendArr = [];
            if (sendz.find('.radioz:checked').val() == 1) {
                // Send Address
                sendAddrz.find('.twZipCodes').twzipcode('get', function (county, district, zipcode) {
                    if (county !== "") {
                        sendArr.push(county);
                        if (district !== "") {
                            sendArr.unshift(zipcode);
                            sendArr.push(district);
                        };
                    };
                    return sendArr;
                });
                if (sendAddrz.find('.inputz').val() !== "") {
                    sendArr.push(sendAddrz.find('.inputz').val().trim());
                };
            } else {
                sendArr = [];
            };

            let dataObj = {
                "namez": namez.val().trim(),
                "gender": genderz.find('.radioz:checked').val().trim(),
                "enNamez": enNamez.val().trim(),
                "convertz": convertz.val().trim(),
                "birthz": birthz.val().trim(),
                "affiliatedAreaz": areaz.val().trim(),
                "residenceAddress": JSON.stringify(residenceArr),
                "contactAddress": JSON.stringify(contactArr),
                "contactNumber": phonez.val().trim(),
                "moblieNumber": cellz.val().trim(),
                "email": mailz.val().trim(),
                "sendCertificate": sendz.find('.radioz:checked').val().trim(),
                "sendAddress": JSON.stringify(sendArr),
                "enabled": enabled,
                "audit": auditz
            };
            if (confirm("您確定要送出嗎？")) {
                let xhr = new XMLHttpRequest();
                xhr.onload = function () {
                    if (xhr.status == 200 || xhr.status == 201) {
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
            };
        } else {
            alert(errorText);
        };
    });
    // Rituals Video
    ritualItemz.on('click', function () {
        let num = $(this).find('.numz').html();
        if (num !== movieNumz.html()) {
            movieNumz.html(num);
            if ($(this).hasClass('active')) {

            } else {
                $(this).addClass('active').siblings().removeClass('active');
                let videoSrc = $(this).data('srcz');
                moviePlayz.find('.iframes').attr('src', videoSrc);

                $(window).scrollTop($('.videos').offset().top);
            }
        } else { }
    });
});