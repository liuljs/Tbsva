// Donate
let namez = $('.namez'), genderz = $('.genderz'), areaz = $('.areaz'), phonez = $('.phonez'), cellz = $('.cellz'), mailz = $('.mailz'), notez = $('.notez'), payz = $('.payz'), receiptz = $('.receiptz'), sendz = $('.sendz'), readz = $('.readz'), agreez = $('.agreez');

let donation = $('.donation'), relation = $('.relation'), totals = 0, donateArr = new Array(), enterz;
let topz = $('.topz'), cardz = $('.cardz'), card, listz = $('.listz'), editorz = $('#editorz');
let contactAddrz = $('.contactAddrz'), sendAddrz = $('.sendAddrz'), clonez = $('.clonez'), contactArr = new Array(), sendArr = new Array(), addrz, addrArr = new Array();
let sumz = $('.sumz'), btnResetz = $('.btnResetz'), btnSendz = $('.btnSendz');
let minVal = 0, maxVal;
let donateType; // 1 一般捐款 || 2 結緣捐贈
CONNECT = 'Donate', TABCONNECT = 'donateRelatedItem';

// 計算捐款總額
function calcDonateTotals() {
    $('.donation').find('.itemz').each(function () {
        if ($(this).find('.checkz').val() == 1) {
            if (Number($(this).find('.pricez').data('pricez')) > 0) {
                // 固定捐款金額 
                let vals = Number($(this).find('.pricez').data('pricez'));
                totals += vals;
            } else {
                // 流動捐款金額
                let vals = Number($(this).find('.pricez').val());
                totals += vals;
            };
        };
    });
    // 總額
    sumz.html(thousands(totals)); // 顯示
    sumz.attr('data-sumz', totals); // 計算
    totals = 0; // 重置
};
// 計算結緣小計
function calcRelatedSubTotals(qty, price) {
    if (qty > 0) {
        return qty * price;
    } else {
        return 0;
    }
};
// 計算結緣總額
function calcRelatedTotals() {
    $('.relation').find('.itemz').each(function () {
        if ($(this).find('.checkz').val() == 1) {
            let vals = Number($(this).find('.subTotalz').attr('data-subz'));
            totals += vals;
        };
    });
    // 總額
    sumz.html(thousands(totals)); // 顯示
    sumz.attr('data-sumz', totals); // 計算
    totals = 0; // 重置
};
// 驗證欄位
function dataUpdateCheck(name, area, address, phone, mail, receipt, send, amount, read) {
    if (name.val().trim() === '') {
        name.focus();
        return check = false, errorText = '請確認捐款姓名是否確實填寫，或格式是否正確！';
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
        return check = false, errorText = '請確認聯絡電話是否確實填寫，或格式是否正確！';
    }
    if (mail.val().trim() === '' || EmailRegExp.test(mail.val()) === false) {
        mail.focus();
        return check = false, errorText = '請確認電子信箱是否確實填寫，或格式是否正確！';
    }
    if (Number(receipt.find('.radioz:checked').val()) === 1) {
        if (receipt.find('.inputz').val() === '') {
            receipt.find('.inputz').focus();
            return check = false, errorText = '請確認收據抬頭是否確實填寫，或格式是否正確！';
        }
    }
    if (Number(send.find('.radioz:checked').val()) === 1) {
        if (send.find('.twZipCodes').twzipcode('get', 'county,district') === '' || send.find('.inputz').val() === '') {
            send.find('.inputz').focus();
            return check = false, errorText = '請確認寄送地址是否確實選取、詳細地址是否確實填寫！';
        } else {
            return check = true, errorText = "";
        };
    }
    if (Number(amount.attr('data-sumz')) === 0) {
        $(window).scrollTop(donation.offset().top - 100);
        return check = false, errorText = '請確認有選擇至少一項捐款項目（可複選）！';
    }
    if (Number(amount.attr('data-sumz')) < 100) {
        $(window).scrollTop(donation.offset().top - 100);
        return check = false, errorText = '感謝您的善心，由於線上、轉帳手續費等問題，捐款總金額需大於新台幣 100 元！';
    }
    if (Number(read.find('.checkz').val()) === 0) {
        return check = false, errorText = '請確實閱讀並且同意個人資料蒐集、處理、利用同意書的內容！';
    }
    else {
        return check = true, errorText = "";
    };

};
//
function process(prim, data) {
    // 捐款項目 || 結緣項目
    if (prim == '1') {
        html = "";
        for (let i = 0; i < data.length; i++) {
            if (data[i].amount == 0) {
                enterz = `
                    <input type="input" data-pricez="${data[i].amount}" class="generals input inputz numberz pricez" value="${data[i].amount}">
                `;
            } else {
                enterz = `
                    <div data-pricez="${data[i].amount}" class="generals prices pricez">${thousands(data[i].amount)}</div>
                `;
            };
            html = `
                <div data-numz="${data[i].donateRelatedItemId}" class="items itemz border">
                    <div class="parts border">
                        <div class="field">
                            <label class="checkOptions">
                                <div class="checkboxes">
                                    <input type="checkbox" class="checkbox checkz" value="0">
                                    <span></span>
                                </div>
                                <p class="mainTitle">${data[i].title}</p>
                            </label>
                        </div>
                    </div>
                    <div class="parts border">
                        <div class="field">
                            <div class="inputs units">
                                <div class="unit">
                                    ${enterz}
                                    <span>元</span>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            `;
            if (data[i].secondary == '1') {
                donation.eq(0).append(html);
            } else if (data[i].secondary == '2') {
                donation.eq(1).append(html);
            } else if (data[i].secondary == '3') {
                donation.eq(2).append(html);
            } else if (data[i].secondary == '4') {
                donation.eq(3).append(html);
            };
        };
    } else {
        card = '', html = '';
        for (let i = 0; i < data.length; i++) {
            // 是否開啟置頂項目
            if (data[i].first == 1) {
                topz.addClass('active');
                //
                topz.find('.itemz').attr('data-numz', data[0].donateRelatedItemId);
                topz.find('.imagez').attr('src', `${data[0].imageURL}?${paramRandoms()}`);
                topz.find('.titlez').html(data[0].title);
                topz.find('.contentz').html(data[0].content2);
                topz.find('.getz').html(data[0].content1);
                topz.find('.qtyz').attr('data-pricez', data[0].amount);
            } else {
                // 顯示渲染
                card += `
                    <div data-numz="${data[i].donateRelatedItemId}" class="items itemz swiper-slide hovers border">
                        <div class="parts border">
                            <div class="images">
                                <img class="cover" src="${data[i].imageURL}?${paramRandoms()}">
                                <img src="./img/elements/samples/base800x640.jpg">
                            </div>
                            <div class="btnMore btnMorez">
                                <span>點擊更多資訊</span>
                            </div>
                        </div>
                        <div class="parts border">
                            <div class="heads">
                                <p class="mainTitle abridged1" title="${data[i].title}">${data[i].title}</p>
                            </div>
                            <div class="gets border">
                                <div class="names">
                                    <p class="mainTitle">取得辦法</p>
                                </div>
                                <div class="texts">
                                    <p class="text abridged2" title="${data[i].content1}">
                                        ${data[i].content1}
                                    </p>
                                </div>
                            </div>
                            <div class="btns">
                                <form class="forms">
                                    <div class="form">
                                        <div class="fields">
                                            <div class="field">
                                                <div class="qties hovers">
                                                    <div data-pricez="${data[i].amount}" class="qty qtyz">
                                                        <div data-numz="minus" class="controls controlz btnMinus">
                                                            <i class="fal fa-minus"></i>
                                                        </div>
                                                        <div class="inputs">
                                                            <input type="text" class="generals input inputz numberz" value="0">
                                                        </div>
                                                        <div data-numz="plus" class="controls controlz btnPlus">
                                                            <i class="fal fa-plus"></i>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </form>
                                <div class="btnSecond btnAdd btnAddz hovers">
                                    <span>加入結緣清單</span>
                                    <span class="circle"><i class="fad fa-praying-hands"></i></span>
                                </div>
                            </div>
                            <div class="decRows">
                                <span>結緣品</span>
                            </div>
                        </div>
                    </div>
                `;
            };
            // 列表渲染
            html += `
                <div data-numz="${data[i].donateRelatedItemId}" class="items itemz border">
                    <div class="parts border">
                        <div class="field">
                            <label class="checkOptions">
                                <div class="checkboxes">
                                    <input type="checkbox" class="checkbox checkz" value="0">
                                    <span></span>
                                </div>
                                <p class="mainTitle titlez" title="${data[i].title}">${data[i].title}</p>
                            </label>
                        </div>
                    </div>
                    <div class="parts border">
                        <div class="field">
                            <div class="qties hovers">
                                <div data-pricez="${data[i].amount}" class="qty qtyz">
                                    <div data-numz="minus" class="controls controlz btnMinus">
                                        <i class="fal fa-minus"></i>
                                    </div>
                                    <div class="inputs">
                                        <input type="text" class="generals input inputz numberz" value="0">
                                        <input type="text" class="input nones">
                                    </div>
                                    <div data-numz="plus" class="controls controlz btnPlus">
                                        <i class="fal fa-plus"></i>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="subTotals units">
                            <div class="unit">
                                <p data-subz="" class="prices subTotal subTotalz">0</p>
                                <span>元</span>
                            </div>
                        </div>
                    </div>
                </div>
            `;
        };
        //
        cardz.html(card);
        //
        listz.html(html);
        //
        var swiper = new Swiper(".swiperz", {
            slidesPerView: 1,
            spaceBetween: 30,
            loopFillGroupWithBlank: true,
            navigation: {
                nextEl: ".next",
                prevEl: ".prev"
            },
            breakpoints: {
                720: {
                    slidesPerView: 2,
                    spaceBetween: 20
                },
                1085: {
                    slidesPerView: 3,
                    spaceBetween: 15
                },
                1200: {
                    slidesPerView: 2,
                    spaceBetween: 20
                },
                1440: {
                    slidesPerView: 3,
                    spaceBetween: 15
                }
            }
        });
    };
    let checkOption = $('.checkOptions .checkz');
    // checkOption.val('0'); // 預設為 false
    checkOption.on('change', function () {
        if ($(this).prop('checked') == true) {
            $(this).val("1");
        } else {
            $(this).val("0");
        }
    });
    // subTotal
    // 數量增減
    $('.qtyz').find('.inputz').on('change', function (e) {
        e.preventDefault();
    })
    $('.qtyz').find('.inputz').on('input', function () {
        let trz = $(this).parents('.itemz');
        let pricez = trz.find('.qtyz').data('pricez');
        let num = trz.data('numz'), newVal;
        // 最大數量
        // maxVal = trz.parents('tr').data('qtyz');
        maxVal = 100;
        if ($(this).val() == "" || $(this).val() == 0) {
            $(this).val(minVal);
            newVal = minVal;
        } else if ($(this).val() > maxVal) {
            alert(`不好意思，每一個結緣捐贈，每次最大的捐贈數量為 ${maxVal} 份。`);
            $(this).val(maxVal);
            newVal = maxVal;
        } else {
            newVal = $(this).val();
        };
        trz.find('.subTotalz').html(thousands(calcRelatedSubTotals(newVal, pricez)));
        trz.find('.subTotalz').attr('data-subz', calcRelatedSubTotals(newVal, pricez));
        if (trz.find('.checkz').val() == 1) {
            calcRelatedTotals();
        }
    });
    // 數量控制項
    $('.qtyz').find('.controlz').on('click', function () {
        let trz = $(this).parents('.itemz');
        let pricez = trz.find('.qtyz').data('pricez');
        let vals = trz.find('.inputz'), newVal;
        // 最大數量
        // maxVal = trz.data('qtyz');
        maxVal = 100;
        if ($(this).data('numz') == "minus") {
            if (vals.val() > minVal) {
                newVal = parseFloat(vals.val()) - 1;
            } else {
                newVal = minVal;
            };
        } else {
            if (vals.val() < maxVal) {
                newVal = parseFloat(vals.val()) + 1;
            } else {
                newVal = maxVal;
                alert(`不好意思，每一個結緣捐贈，每次最大的捐贈數量為 ${maxVal} 份。`);
            };
        };
        vals.val(newVal);
        trz.find('.subTotalz').html(thousands(calcRelatedSubTotals(newVal, pricez)));
        trz.find('.subTotalz').attr('data-subz', calcRelatedSubTotals(newVal, pricez));
        if (trz.find('.checkz').val() == 1) {
            calcRelatedTotals();
        };
    });
    // calcDonateTotal
    $('.donation').find('.itemz').find('.inputz').on('change', function () {
        let vals = Number($(this).val());
        if ($(this).parents('.itemz').find('.checkz').val() == 1) {
            calcDonateTotals();
        };
    });
    $('.donation').find('.itemz').find('.checkz').on('change', function () {
        calcDonateTotals();
    });
    // calcRelateTotal
    $('.relation').find('.itemz').find('.checkz').on('change', function () {
        calcRelatedTotals();
    });
    // lightBox
    $('.btnMorez').on('click', function () {
        let numz = $(this).parents('.itemz').data('numz');
        let targets = $(this).parents('.mains').find('.lightBoxz');
        if (targets.hasClass('active')) {
        } else {
            targets.addClass('active');
            //
            stays = $(window).scrollTop();
            $('.wrap').css({ 'top': -stays });
            $(window).scrollTop(stays);
            $('.wrap').addClass('active');
            //
            let dataObj = {
                "Methods": "GET",
                "APIs": URL,
                "CONNECTs": TABCONNECT,
                "QUERYs": `${TABCONNECT}/${numz}`,
                "Sends": "",
                "Counts": ""
            };
            getPageDatas(dataObj).then(res => {
                if (res !== null) {
                    //
                    targets.find('.titlez').html(res.title);
                    targets.find('.imagez').attr('src', res.imageURL);
                    // 如果有內容
                    if (res.content3 !== "" && res.content3 !== null) {
                        //
                        html = "";
                        // Content
                        html = quill.setContents(JSON.parse(res.content3).ops);
                        editorz.html(quill.root.innerHTML);
                    } else { }

                } else {

                };
            }, rej => {
                if (rej == "NOTFOUND") { };
            });
            //
            targets.find('.btnClose').on('click', function () {
                if (targets.hasClass('active')) {
                    $('.wrap').removeClass('active');
                    $('.wrap').css({ 'top': 0 });
                    $(window).scrollTop(stays);

                    targets.removeClass('active');

                    setTimeout(() => {
                        // 清空
                        targets.find('.titlez').html('');
                        targets.find('.imagez').attr('src', '');
                        editorz.html('');
                    }, 500);
                };
            });
        };
    });
    // 加入清單
    $('.btnAddz').on('click', function () {
        //
        let trz = $(this).parents('.itemz');
        let numz = trz.data('numz');
        // 數量
        let qtyz = Number(trz.find('.qty .inputz').val());
        let pricez = Number(trz.find('.qty').data('pricez'));
        if (qtyz > 0) {
            let selfs = relation.find(`.itemz[data-numz="${numz}"]`);
            if (confirm('確定要加入結緣清單嗎？')) {
                if (Number(selfs.find('.qty .inputz').val()) == 0) {
                    qtyz = qtyz;
                } else {
                    qtyz += Number(selfs.find('.qty .inputz').val());
                }
                selfs.find('.qty .inputz').val(qtyz);
                selfs.find('.subTotalz').html(thousands(calcRelatedSubTotals(qtyz, pricez)));
                selfs.find('.subTotalz').attr('data-subz', calcRelatedSubTotals(qtyz, pricez));

                if (Number(selfs.find('.checkz').val()) == 0) {
                    selfs.find('.checkz').trigger('click');
                } else {
                    if (selfs.find('.checkz').val() == 1) {
                        calcRelatedTotals();
                    }
                };
                trz.find('.qty .inputz').val('0');
            }
        } else {
            alert('請填寫您要結緣捐贈的數量！');
        }

    });
};
// NOTFOUND
function fails() { };
$(() => {
    // 捐款項目 || 結緣項目
    let dataObj = {
        "Methods": "GET",
        "APIs": URL,
        "CONNECTs": TABCONNECT,
        "QUERYs": "",
        "Sends": "",
        "Counts": ""
    };
    if ($('.donatePage').data('numz') == '1') {
        dataObj.QUERYs = `${TABCONNECT}?primary=1`;
    } else {
        dataObj.QUERYs = `${TABCONNECT}?primary=2`;
    };
    getPageDatas(dataObj).then(res => {
        if (res !== null) {
            process($('.donatePage').data('numz'), res);
        } else {
            fails();
        };
    }, rej => {
        if (rej == "NOTFOUND") { };
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
    // 
    btnResetz.on('click', function (e) {
        e.preventDefault()
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

            // subTotal amount
            $('.subTotalz').html(totals);
            $('.subTotalz').attr('data-sumz', totals); // 計算

            // total amount
            sumz.html(totals); // 顯示
            sumz.attr('data-sumz', totals); // 計算

            $(document).scrollTop(formz.offset().top - 60);
        };
    })
    //
    btnSendz.on('click', function () {
        if ($(this).data('numz') == "donate") {
            donateType = 1; // 一般捐款
            // Donate Project
            donateArr = [];
            donation.find('.itemz').each(function () {
                if (Number($(this).find('.checkz').val()) === 1) {
                    if ($(this).find('.pricez').data('pricez') == 0 && $(this).find('.pricez').val() == 0) {
                        $(this).find('.pricez').focus();
                        alert(`請確實填寫${$(this).find('.mainTitle').html()}的捐款金額！`);
                    } else {
                        let donate = {
                            "title": $(this).find('.mainTitle').html(),
                            "amount": $(this).find('.pricez').data('pricez') > 0 ? Number($(this).find('.pricez').data('pricez')) : Number($(this).find('.pricez').val()),
                            "qty": 1
                        };
                        donateArr.push(donate);
                    };
                };
            });
        } else {
            donateType = 2; // 結緣捐贈
            // Related Project
            donateArr = [];
            relation.find('.itemz').each(function () {
                if (Number($(this).find('.checkz').val()) === 1) {
                    if ($(this).find('.qty .inputz').val() == 0) {
                        $(this).find('.qty .inputz').focus();
                        alert(`請確實填寫${$(this).find('.titlez').html()}的結緣數量！`);
                    } else {
                        let donate = {
                            "title": $(this).find('.titlez').html(),
                            "amount": Number($(this).find('.subTotalz').attr('data-subz')),
                            "qty": $(this).find('.qty .inputz').val()
                        };
                        donateArr.push(donate);
                    };
                };
            });
        };
        // Cell Phone 
        if (cellz.val() !== "") {
            if (CellRegExp.test(cellz.val()) === false) {
                cellz.focus();
                alert('請確認手機號碼格式是否正確！');
            }
        };
        // Receipt
        if (receiptz.find('.radioz:checked').val() == 0) {
            receiptz.find('.inputz').val('');
        };
        dataUpdateCheck(namez, areaz, contactAddrz, phonez, mailz, receiptz, sendz, sumz, readz);
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
                "donateType": donateType,
                "buyerName": namez.val().trim(),
                "buyerSex": genderz.find('.radioz:checked').val().trim(),
                "affiliatedArea": areaz.val().trim(),
                "address1": JSON.stringify(contactArr),
                "buyerPhone": phonez.val().trim(),
                "mobilePhone": cellz.val().trim(),
                "buyerEmail": mailz.val().trim(),
                "donateRelatedItemRecord": JSON.stringify(donateArr),
                "remark": notez.val().trim(),
                "payType": payz.find('.radioz:checked').val().trim(),
                "needReceipt": receiptz.find('.radioz:checked').val().trim(),
                "receiptTitle": receiptz.find('.inputz').val().trim(),
                "receiptPostMethod": sendz.find('.radioz:checked').val().trim(),
                "address3": JSON.stringify(sendArr),
                "amount": Number(sumz.attr('data-sumz')),
                "needAnonymous": agreez.find('.radioz:checked').val().trim()
            };
            if (confirm('您確定要進行捐款嗎（點選確認後，將移至繳款網址）？')) {
                let xhr = new XMLHttpRequest();
                xhr.onload = () => {
                    if (xhr.status == 200 || xhr.status == 201) {
                        let callBackData = JSON.parse(xhr.responseText);
                        if (callBackData.payType == '83') {
                            // 僅記錄
                            alert('感謝您的善心（本選項僅紀錄捐款資訊，並非線上直接繳款，需自行利用 ATM、EATM、網路銀行等方式匯款。）！')
                        } else {
                            //
                            if (callBackData.address1 != "" && callBackData.address1 != "[]") {
                                addrz = "";
                                addrArr = JSON.parse(callBackData.address1);
                                for (let i = 0; i < addrArr.length; i++) {
                                    addrz += addrArr[i];
                                };
                            } else {
                                addrArr = [];
                                addrz = "";
                            };
                            // 跳轉快刷網址
                            let winRef = window.open("", "_blank");
                            winRef.location = `./connects.html?orderId=${callBackData.orderId}&buyerName=${encodeURI(callBackData.buyerName)}&buyerPhone=${callBackData.buyerPhone}&mobilePhone=${callBackData.mobilePhone}&buyerEmail=${callBackData.buyerEmail}&address1=${encodeURI(addrz)}&payType=${callBackData.payType}&amount=${callBackData.amount}&receiptTitle=${encodeURI(callBackData.receiptTitle)}&remark=${encodeURI(callBackData.remark)}`; // 另開
                        };
                    } else {
                        alert('伺服器發生未預期的問題，請重新整理後再進行操作！');
                        location.reload();
                    };
                };
                xhr.open('POST', `${URL}${CONNECT}`, true);
                xhr.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded');
                xhr.send($.param(dataObj));
            } else { };
        } else {
            alert(errorText);
        };
    });
});