// 宣告要帶入的欄位
let companyNamez = $('.companyNamez'), UIDz = $('.UIDz'), principalz = $('.principalz'), telz = $('.telz'), cellz = $('.cellz'), addressz = $('.addressz'), mailz = $('.mailz'), merchantz = $('.merchantz'), terminalz = $('.terminalz');

$().ready(function () {
    let dataObj = {
        "Methods": "POST",
        "APIs": URL,
        "CONNECTs": 'Company/Get',
        "QUERYs": "",
        "Sends": "",
        "Counts": ""
    };
    getPageDatas(dataObj).then(res => {
        if (res !== null) {
            // 將取得的資料帶入在面中（動態產生）
            companyNamez.text(res[0].NAME);
            UIDz.text(res[0].UID);
            principalz.text(res[0].PRINCIPAL);
            telz.text(res[0].TEL);
            cellz.text(res[0].CELL_PHONE);
            addressz.text(res[0].ADDRESS);
            mailz.text(res[0].EMAIL);
            merchantz.text(res[0].MERCHANT_ID);
            terminalz.text(res[0].TERMINAL_ID);
        };
        // 資料載入完成後
        loadingCompleted();
    }, rej => {
        if (rej == "NOTFOUND") { };
    });
});