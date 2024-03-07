// CONNECT

// 金流資訊（固定）
let flowObj = {
    "MerchantId": "990007054",
    "TerminalId": "917390001",
    "MerchantName": "社團法人中國真佛宗密教總會",
    "RequestUrl": "https://test.payware.com.tw/wpss/authpay.aspx",
    "ReturnURL": "https://tbsva.webshopping.vip/api/PaymentDonate/Return",
    "Encoding": "utf-8",
    "GoBackURL": "https://tbsva.webshopping.vip",
    "ReceiveURL": "https://tbsva.webshopping.vip/api/PaymentDonate/Receive",
    "DeadlineDate": "",
    "RequiredConfirm": "1",
    "deferred": 7,
    "validateKey": "validateKey"
};
// 送出訂單
function sendOrder(mchantId, termId, mchantName, rquetURL, rturnURL, cardHolder, payType, odrNo, amt, pdt, odrDesc, enConding, mobile, telNum, addr, mail, memId, goBackURL, receURL, dLineDate, rquiredCfm, deferred, carrier, invoName, validateKey) {
    let formData = $('.sendOrderForm');

    formData.find('input[name="MerchantId"]').val(mchantId);
    formData.find('input[name="TerminalId"]').val(termId);
    formData.find('input[name="MerchantName"]').val(mchantName);
    formData.find('input[name="RequestUrl"]').val(rquetURL);
    formData.find('input[name="ReturnURL"]').val(rturnURL);
    formData.find('input[name="CardHolder"]').val(cardHolder);
    formData.find('input[name="PayType"]').val(payType);
    formData.find('input[name="OrderNo"]').val(odrNo);
    formData.find('input[name="Amount"]').val(amt);
    formData.find('input[name="Product"]').val(pdt);
    formData.find('input[name="OrderDesc"]').val(odrDesc);
    formData.find('input[name="Encoding"]').val(enConding);
    formData.find('input[name="Mobile"]').val(mobile);
    formData.find('input[name="TelNumber"]').val(telNum);
    formData.find('input[name="Address"]').val(addr);
    formData.find('input[name="Email"]').val(mail);
    formData.find('input[name="memberId"]').val(memId);
    formData.find('input[name="GoBackURL"]').val(goBackURL);
    formData.find('input[name="ReceiveURL"]').val(receURL);
    formData.find('input[name="DeadlineDate"]').val(dLineDate);
    formData.find('input[name="RequiredConfirm"]').val(rquiredCfm);
    formData.find('input[name="deferred"]').val(deferred);
    formData.find('input[name="Carrier"]').val(carrier);
    formData.find('input[name="InvoiceName"]').val(invoName);
    formData.find('input[name="validateKey"]').val(validateKey);

    formData.submit();
};
$(() => {
    if (true) {
        let infos = requestAll(location.href);
        if (infos !== "") {
            
            sendOrder(flowObj.MerchantId, flowObj.TerminalId, flowObj.MerchantName, flowObj.RequestUrl, flowObj.ReturnURL, infos.buyername, infos.paytype, infos.orderid, infos.amount, "結緣捐款", infos.remark, flowObj.Encoding, infos.mobilephone, infos.buyerphone, infos.address1, infos.buyeremail, "", flowObj.GoBackURL, flowObj.ReceiveURL, flowObj.DeadlineDate, flowObj.RequiredConfirm, flowObj.deferred, "", infos.receipttitle, flowObj.validateKey);
        } else {

        };
    };
});