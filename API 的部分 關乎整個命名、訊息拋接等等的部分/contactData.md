# 網站後台串接文件

Web Backend API Concatenate Doc.

Operation v1.0

<p align="right">- 2021/10/19</p>

---

## 共用

### - 文件版次

| 版次 | 日期 | 更新說明 |
| :---: | :---: | :--- |
| V 1.0 | 2021/10/19 | 初版 -   |

### - API 串接概述

1. API 設計規範採用 RESTful API。
2. 完整的 API 端口網址 = 網址 URL + /api + /vx（x 版本編號）+ 端口。
3. HTTP Method（全大寫），RESTful API 使用了 HTTP Method 來當作「動詞」。

    ```action  
        GET     : 讀取
        POST    : 新增
        PUT     : 修改（必需修改整份文件）
        PATCH   : 修改（只需修改其中某幾個欄位）
        DELETE  : 刪除
    ```

4. API 拋接資料，資料格式依傳輸資訊的內容類型而訂定。

    單一資料格式採用表單或 JSON 格式，複數（包含多媒體）格式採用 form-data 格式。

5. API 回傳資料，根據 HTTP Status Code 來訂定狀態及回應。

    <font color="#8B0003">註：如果前端的請求成功（正常），而後端（資料庫）目前並無資料，須回傳一個回應（空值、空字串、空陣列或空物件）。</font>

6. API 回傳狀態

    常用類型 - 200 系列為請求成功、400 系列為用戶端錯誤、500 系列為伺服端錯誤。

7. 回傳格式物件
    * 請採用小駝峰式命名（lowerCamelCase）。
    * 若 value 值無值可帶入，依照屬性帶入對應資料。
      * 字串 帶入空字串；
      * 物件 帶入空物件；
      * 數值 帶入 -1；
    * 回傳物件格式

    ```example
        {
            "code" : 狀態代碼,      // 狀態代碼 | 限制：-  | 必填 {string}
            "message" : 狀態訊息,   // 狀態訊息 | 限制：-  | 必填 {string}
            "detail" : 狀態說明,    // 狀態說明 | 限制：-  | 選填 {string}      請求成功（無內容可空值）；請求失敗請回傳原因；
            "data" : 回傳內容,      // 回傳內容 | 限制：-  | 選填 {JSON}{array} 無內容可空值
            "timestamp" : 回傳時間  // 回傳時間 | 限制：-  | 必填 {date}        毫秒
        }
    ```

8. 請求物件格式
    * 請採用小駝峰式命名（lowerCamelCase）。
    * 若 value 值無值可帶入，依照屬性帶入對應資料。
      * 字串 帶入空字串；
      * 物件 帶入空物件；
      * 數值 帶入 -1；
    * 請求物件格式 <font size="2" color="#8B0003">（註：限制指的是欄位輸入的長度限制）</font>。

    ```example
        {
            "data" : 傳送內容,      // 傳送內容 | 限制：-  | 選填 {JSON}{array}
            "timestamp" : 請求時間  // 請求時間 | 限制：-  | 必填 {date}
        }
    ```

9. 完整 API 串接規範，可先看 APIConcat Doc 文件。

### - 共用設定

1. API 通用網址（Base URL）

    > `https://payware.com.tw/api/v1/ （範例）`

2. 前後端皆須做防呆及驗證機制 <font size="2" color="#8B0003">（註：以統一的錯誤說明作為提示）</font>。
3. 後台 API 端口統一前綴為加上 /admin 作為區分；前後台相通不須加上（取得無須授權；新增、刪除則須授權）。
4. API 請求成功（200、201）：新增成功，回傳物件（ ID、狀態、時間）；更新成功，不回傳物件。
5. API 請求失敗（非預期）通用回傳狀態
    | code | message | detail |
    | :--- | :--- | :--- |
    | 404（失敗） | errorToNotFound | 伺服器無法正常提供訊息，或是伺服器無法回應且不知原因。 |
    | 500（失敗） | errorToServer   | 伺服器內部發生未預期的錯誤。 |

## API 功能規劃

### 登入功能

#### - 後台登入

* Route：admin/login
* Method：POST
* Input（請求）：

    ```example
        {
            "data" : {
                "account" : "admin",      // 管理者帳號 | 限制：32 | 必填 {string}  英或數 5 個字（含）以上
                "password" : "test1234",  // 管理者密碼 | 限制：32 | 必填 {string}  需含英數字 8 個字（含）以上        
            },
            "timestamp" : 123456789  // 請求時間 | 限制：-  | 必填 {date}  毫秒
        }
    ```

* Situation（情況）：
    | code | message | detail |
    | :--- | :--- | :--- |
    | 200（成功） | successToGetContent | "登入成功！" |
    | 400（失敗） | errorToBadRequest | "您的帳號或密碼欄位格式輸入錯誤！" |
    | 400（失敗） | errorToNotFilled | "您的帳號欄位尚未填寫！" |
    | 400（失敗） | errorToNotFilled | "您的密碼欄位尚未填寫！" |
    | 403（失敗） | errorToForbidden | "此帳號已被停用！" |
* Response（回應）：

    ```example
        請求成功（200）
        {
            "code" : "200",                     // 狀態代碼 | 限制：-  | 必填 {string}
            "message" : "successToGetContent",  // 狀態訊息 | 限制：-  | 必填 {string}
            "detail" : "登入成功！",　           // 狀態說明 | 限制：-  | 選填 {string}
            "data" : "",                        // 回傳內容 | 限制：-  | 選填 {JSON}{array}  無內容可空值
            "timestamp" : 123456789,            // 回傳時間 | 限制：-  | 必填 {date}         毫秒
        }
        註：後端回傳憑證至前端 Cookie（HttpOnly）
    ```

    ```example
        請求失敗（）
        {
            "code" : "400",                        // 狀態代碼 | 限制：-  | 必填 {string}
            "message" : "errorToBadRequest",       // 狀態訊息 | 限制：-  | 必填 {string}
            "detail" : "您的帳號或密碼格式錯誤！",  // 狀態說明 | 限制：-  | 選填 {string}
            "data" : "",                           // 回傳內容 | 限制：-  | 選填 {JSON}{array}  無內容可空值
            "timestamp" : 123456789,               // 回傳時間 | 限制：-  | 必填 {date}         毫秒
        }
    ```

#### - 取得登入者資訊

* Route：admin/loginInfo
* Method：POST
* Input（請求）：N/A
* Situation（情況）：
    | code | message | detail |
    | :--- | :--- | :--- |
    | 200（成功） | successToGetContent | "" |
    | 401（失敗） | errorToUnauthorized | "用戶端沒有認證憑據，伺服器拒絕回應、存取。" |
* Response（回應）：

    ```example
        請求成功（200）
        {
            "code" : "200",                     // 狀態代碼 | 限制：-  | 必填 {string}
            "message" : "successToGetContent",  // 狀態訊息 | 限制：-  | 必填 {string}
            "detail" : "",                      // 狀態說明 | 限制：-  | 選填 {string}
            "data" : {                          // 回傳內容 | 限制：-  | 選填 {JSON}{array}  無內容可空值
                "id" : 1,                                              // 流水號編號 | 限制：-  | 必填 {int}     流水號，資料庫排序、索引用
                "managerId" : "f84a4543-98c8-422c-8590-753e719e16c0",  // 管理者編號 | 限制：-  | 必填 {string}  管理者類別另設 ID 唯一索引用
                "name" : "Payware",                                    // 管理者名稱 | 限制：-  | 必填 {string} 
                "authority" : [                                        // 管理者權限 | 限制：-  | 必填 {array}   功能使用權限（N 無權；Y 有權），view：檢視；delete：刪除；edit：編輯； 
                    {"code": "MM" , "view" : "Y" , "delete" : "Y" , "edit" : "Y"},  // MM：會員管理；
                    {"code": "OM" , "view" : "Y" , "delete" : "Y" , "edit" : "Y"},  // OM：訂單管理；
                    {"code": "PM" , "view" : "Y" , "delete" : "Y" , "edit" : "Y"},  // PM：商品管理；
                    {"code": "SM" , "view" : "Y" , "delete" : "Y" , "edit" : "Y"}   // SM：管理者管理；
                ],
                "enabled" : 1,                                         // 管理者狀態 | 限制：-  | 必填 {int}     啟用狀態（0 關閉；1 開啟）
                "loginDate" : "2021-05-05 14:07",                      // 登入時間點 | 限制：-  | 必填 {date}
            },                               
            "timestamp" : 123456789,            // 回傳時間 | 限制：-  | 必填 {date}        毫秒
        }
    ```

    ```example
        請求失敗（）
        {
            "code" : "401",                                         // 狀態代碼 | 限制：-  | 必填 {string}
            "message" : "errorToUnauthorized",                      // 狀態訊息 | 限制：-  | 必填 {string}
            "detail" : "用戶端沒有認證憑據，伺服器拒絕回應、存取。",  // 狀態說明 | 限制：-  | 選填 {string}
            "data" : "",                                            // 回傳內容 | 限制：-  | 選填 {JSON}{array}  無內容可空值
            "timestamp" : 123456789,                                // 回傳時間 | 限制：-  | 必填 {date}         毫秒
        }
    ```

#### - 後台登出

* Route：admin/logout
* Method：POST
* Input（請求）：N/A
* Situation（情況）：
    | code | message | detail |
    | :--- | :--- | :--- |
    | 200（成功） | successButNoContent | "登出成功！" |
* Response（回應）：

     ```example
        請求成功（200）
        {
            "code" : "200",                     // 狀態代碼 | 限制：-  | 必填 {string}
            "message" : "successButNoContent",  // 狀態訊息 | 限制：-  | 必填 {string}
            "detail" : "登出成功！",             // 狀態說明 | 限制：-  | 選填 {string}
            "data" : "",                        // 回傳內容 | 限制：-  | 選填 {JSON}{array}  無內容可空值       
            "timestamp" : 123456789,            // 回傳時間 | 限制：-  | 必填 {date}         毫秒
        }
        註：清除 Local Storage 和 Cookie 的憑證。
    ```

    ```example
        請求失敗（）
        {
            "code" : "401",                                         // 狀態代碼 | 限制：-  | 必填 {string}
            "message" : "errorToUnauthorized",                      // 狀態訊息 | 限制：-  | 必填 {string}
            "detail" : "用戶端沒有認證憑據，伺服器拒絕回應、存取。",  // 狀態說明 | 限制：-  | 選填 {string}
            "data" : "",                                            // 回傳內容 | 限制：-  | 選填 {JSON}{array}  無內容可空值
            "timestamp" : 123456789,                                // 回傳時間 | 限制：-  | 必填 {date}         毫秒
        }
    ```

### 基本資訊

#### - 取得基本資訊

* Route：admin/information
* Method：POST
* Input（請求）：N/A
* Situation（情況）：
    | code | message | detail |
    | :--- | :--- | :--- |
    | 200（成功） | successToGetContent | "" |
    | 401（失敗） | errorToUnauthorized | "用戶端沒有認證憑據，伺服器拒絕回應、存取。" |
    | 404（失敗） | errorToNotFound | "請求失敗，請求的資訊、資源不存在。" |
* Response（回應）：

    ```example
        請求成功（200）
        {
            "code" : "200",                     // 狀態代碼 | 限制：-  | 必填 {string}
            "message" : "successToGetContent",  // 狀態訊息 | 限制：-  | 必填 {string}
            "detail" : "",                      // 狀態說明 | 限制：-  | 選填 {string}
            "data" : {                          // 回傳內容 | 限制：-  | 選填 {JSON}{array}  無內容可空值
                "id" : 1,                                                  // 流水號編號 | 限制：-  | 必填 {int}   流水號，資料庫排序、索引用
                "informationId" : "f84a4543-98c8-422c-8590-753e719e16c0",  // 商家編號 | 限制：-  | 必填 {string}  商家類別另設 ID 唯一索引用
                "merchantId" : "990000008",                                // 商家代碼 | 限制：-  | 必填 {string}      
                "terminalId" : "900080001",                                // 端末代號 | 限制：-  | 必填 {string}
                "uniformId" : "12345678",                                  // 統一編號 | 限制：-  | 選填 {string}
                "name" : "派維爾科技有限公司",                              // 公司名稱 | 限制：-  | 必填 {string}
                "principal" : "王小名",                                    // 負責人   | 限制：-  | 必填 {string}
                "address" : "高雄市鹽埕區必忠街247號",                      // 公司地址 | 限制：-  | 必填 {string}
                "telePhone" : "07-5616462",                                // 公司電話 | 限制：-  | 必填 {string}
                "cellPhone" : "0900-123456",                               // 手機號碼 | 限制：-  | 選填 {string}
                "email" : "payware.it@payware.com.tw",                     // 公司信箱 | 限制：-  | 必填 {string}
                "createDate" : "2021-10-22 16:29",                         // 建立日期 | 限制：-  | 必填 {date}                 
                "updateDate" : "2021-10-22 16:29",                         // 更新日期 | 限制：-  | 必填 {date}                 
            },                               
            "timestamp" : 123456789,            // 回傳時間 | 限制：-  | 必填 {date}         毫秒
        }
    ```

    ```example
        請求失敗（）
        {
            "code" : "401",                                         // 狀態代碼 | 限制：-  | 必填 {string}
            "message" : "errorToUnauthorized  ",                    // 狀態訊息 | 限制：-  | 必填 {string}
            "detail" : "用戶端沒有認證憑據，伺服器拒絕回應、存取。",  // 狀態說明 | 限制：-  | 必填 {string}
            "data" : "",                                            // 回傳內容 | 限制：-  | 選填 {JSON}{array}  無內容可空值
            "timestamp" : 123456789,                                // 回傳時間 | 限制：-  | 必填 {date}         毫秒
        }
    ```

#### - 更新基本資訊

* Route：admin/information
* Method：PUT
* Input（請求）：

    ```example
        {
            "data" : {
                "managerId" : "f84a4543-98c8-422c-8590-753e719e16c0",       // 管理者編號 | 限制：-  | 必填 {string}  管理者類別另設 ID 唯一索引用，目前登入的管理者編號
                "informationId" : "f84a4543-98c8-422c-8590-753e719e16c0",   // 商家編號 | 限制：-  | 必填 {string}    商家類別另設 ID 唯一索引用
                "principal" : "王小名",                                     // 負責人   | 限制：15 | 必填 {string}
                "address" : "高雄市鹽埕區必忠街247號",                       // 公司地址 | 限制：80 | 必填 {string}
                "telePhone" : "07-5616462",                                 // 公司電話 | 限制：15 | 必填 {string}    需符合電話格式
                "cellPhone" : "0900-123456",                                // 手機號碼 | 限制：15 | 選填 {string}    需符合手機格式
                "email" : "payware.it@payware.com.tw",                      // 公司信箱 | 限制：80 | 必填 {string}    需符合信箱格式
                "updateDate" : "2021-10-22 16:29",                          // 更新日期 | 限制：-  | 必填 {date}  
            },
            "timestamp" : 123456789  // 呼叫時間 | 限制：-  | 必填 {date}  毫秒
        }
    ```

* Situation（情況）：
    | code | message | detail |
    | :--- | :--- | :--- |
    | 200（成功） | successButNoContent | "修改成功！" |
    | 400（失敗） | errorToNotFilled | "負責人欄位尚未填寫！" |
    | 400（失敗） | errorToBadRequest | "負責人欄位長度輸入超過限制（15個字）！" |
    | 400（失敗） | errorToNotFilled | "公司電話欄位尚未填寫！" |
    | 400（失敗） | errorToBadRequest | "公司電話欄位格式輸入錯誤（請符合電話格式）！" |
    | 400（失敗） | errorToBadRequest | "手機欄位格式輸入錯誤（請符合手機格式）！" |
    | 400（失敗） | errorToNotFilled | "公司地址欄位尚未填寫！" |
    | 400（失敗） | errorToBadRequest | "公司地址欄位長度輸入超過限制（80個字）！" |
    | 400（失敗） | errorToNotFilled | "公司信箱尚未填寫！" |
    | 400（失敗） | errorToBadRequest | "公司信箱欄位格式輸入錯誤（請符合信箱格式）！" |
    | 400（失敗） | errorToBadRequest | "公司信箱欄位長度輸入超過限制（80個字）！" |
    | 401（失敗） | errorToUnauthorized | "用戶端沒有認證憑據，伺服器拒絕回應、存取。" |

* Response（回應）：

    ```example
        請求成功（200）
        {
            "code" : "200",                     // 狀態代碼 | 限制：-  | 必填 {string}
            "message" : "successToGetContent",  // 狀態訊息 | 限制：-  | 必填 {string}
            "detail" : "修改資訊成功！",         // 狀態說明 | 限制：-  | 選填 {string}
            "data" : "",                        // 回傳內容 | 限制：-  | 選填 {JSON}{array}  無內容可空值                               
            "timestamp" : 123456789,            // 回傳時間 | 限制：-  | 必填 {date}         毫秒
        }
    ```

    ```example
        請求失敗（）
        {
            "code" : "401",                                        // 狀態代碼 | 限制：-  | 必填 {string}
            "message" : "errorToUnauthorized ",                   // 狀態訊息 | 限制：-  | 必填 {string}
            "detail" : "用戶端沒有認證憑據，伺服器拒絕回應、存取。",  // 狀態說明 | 限制：-  | 必填 {string}
            "data" : "",                                           // 回傳內容 | 限制：-  | 選填 {JSON}{array}  無內容可空值
            "timestamp" : 123456789,                               // 回傳時間 | 限制：-  | 必填 {date}         毫秒
        }
    ```

### 輪播管理

#### - 取得輪播資訊（全部）

* Route：/indexSlideshow/{slideId}
* Method：GET
* Input（請求）：
  
    ```example
        {
            "data" : {
                "slideId" : "f84a4543-98c8-422c-8590-753e719e16c0",  // 輪播圖片編號 | 限制：-  | 選填 {string}  輪播圖片類別另設 ID 唯一索引用    
            },
            "timestamp" : 123456789 // 請求時間 | 限制：-  | 必填 {date}  毫秒
        }
        註：不傳送 slideId 則回傳全部輪播圖片。
    ```

* Situation（情況）：
    | code | message | detail |
    | :--- | :--- | :--- |
    | 200（成功） | successToGetContent | "" |
    | 404（失敗） | errorToNotFound | "請求失敗，請求的資訊、資源不存在。" |
* Response（回應）：

    ```example
        請求成功（200）
        {
            "code" : "200",                     // 狀態代碼 | 限制：-  | 必填 {string}
            "message" : "successToGetContent",  // 狀態訊息 | 限制：-  | 必填 {string}
            "detail" : "",                      // 狀態說明 | 限制：-  | 選填 {string}
            "data" : {                          // 回傳內容 | 限制：-  | 選填 {JSON}{array}  無內容可空值
                "id" : 1,                                            // 流水號編號 | 限制：-  | 必填 {int}       流水號，資料庫排序、索引用
                "slideId" : "f84a4543-98c8-422c-8590-753e719e16c0",  // 輪播編號 | 限制：-  | 必填 {string}      輪播圖片類別另設 ID 唯一索引用
                "name" : "123.png",                                  // 輪播圖片名稱 | 限制：-  | 必填 {string}
                "imageURL" : "img/banners/123.png",                  // 輪播圖片位置 | 限制：-  | 必填 {string}  傳送格式 = img / 存放圖片的資料夾 / 個別資料夾 / 圖片名稱.圖片格式
                "hyperlink" : "https://tcayla.webshopping.vip",      // 超連結址 | 限制：-  | 選填 {string}
                "first" : 0,                                         // 圖片置頂 | 限制：-  | 必填 {int}}        置頂狀態（0 關閉；1 開啟）
                "sort" : 0,                                          // 排序     | 限制：-  | 必填 {int}         排序，預設為流水號編號（從編號 1 號開始；0 為最大）                                  
                "createDate" : "2021-10-22 16:29",                   // 建立日期 | 限制：-  | 必填 {date}
                "updateDate" : "2021-10-22 16:29",                   // 更新日期 | 限制：-  | 必填 {date}                                 
            },                               
            "timestamp" : 123456789,            // 回傳時間 | 限制：-  | 必填 {date}         毫秒
        }
        註：排序功能，除非有使用要求，否則一般不作使用。
    ```

    ```example
        請求失敗（）
        {
            "code" : "404",                                 // 狀態代碼 | 限制：-  | 必填 {string}
            "message" : "errorToNotFound",                  // 狀態訊息 | 限制：-  | 必填 {string}
            "detail" : "請求失敗，請求的資訊、資源不存在。",  // 狀態說明 | 限制：-  | 必填 {string}
            "data" : "",                                    // 回傳內容 | 限制：-  | 選填 {JSON}{array}  無內容可空值
            "timestamp" : 123456789,                        // 回傳時間 | 限制：-  | 必填 {date}         毫秒
        }
    ```

#### - 新增輪播資訊

* Route：/indexSlideshow
* Method：POST
* Input（請求）：
  
    ```example
        {
            "data" : {
                "file" : file,                                   // 圖片檔案 | 限制：-  | 必填 {file}    要上傳的圖片檔案
                "hyperlink" : "https://tcayla.webshopping.vip",  // 超連結址 | 限制：-  | 選填 {string}  
                "first" : 0,                                     // 圖片置頂 | 限制：-  | 必填 {int}     預設置頂狀態為 0（0 關閉；1 開啟）
            },
            "timestamp" : 123456789  // 請求時間 | 限制：-  | 必填 {date}  毫秒
        }
        註：排序以預設流水號方式由後端直接產生，前端新增不另帶
    ```

* Situation（情況）：
    | code | message | detail |
    | :--- | :--- | :--- |
    | 201（成功） | successToCreated | "新增成功！" |
    | 400（失敗） | errorToNotFilled | "尚未選擇您要上傳的輪播圖片！" |
    | 400（失敗） | errorToBadRequest | "請上傳正確的圖片檔案格式（格式：JPG、PNG 等圖片格式）！" |
    | 400（失敗） | errorToNotFilled | "置頂欄位尚未填寫！" |
    | 401（失敗） | errorToUnauthorized | "用戶端沒有認證憑據，伺服器拒絕回應、存取。" |
* Response（回應）：

    ```example
        請求成功（201）
        {
            "code" : "201",                  // 狀態代碼 | 限制：-  | 必填 {string}
            "message" : "successToCreated",  // 狀態訊息 | 限制：-  | 必填 {string}
            "detail" : "新增成功！",          // 狀態說明 | 限制：-  | 選填 {string}
            "data" : {                       // 回傳內容 | 限制：-  | 選填 {JSON}{array}  無內容可空值
                "id" : 1,                                            // 編號流水號 | 限制：-  | 必填 {int}   流水號，資料庫排序、索引用
                "slideId" : "f84a4543-98c8-422c-8590-753e719e16c0",  // 輪播編號 | 限制：-  | 必填 {string}  輪播圖片類別另設 ID 唯一索引用
                "first" : 0,                                         // 圖片置頂 | 限制：-  | 必填 {int}     預設置頂狀態為 0（0 關閉；1 開啟）
                "sort" : 0,                                          // 排序     | 限制：-  | 必填 {int}     排序，預設為流水號編號（從編號 1 號開始；0 為最大）                                  
                "createDate" : "2021-10-22 16:29",                   // 建立日期 | 限制：-  | 必填 {date}                 
            },                               
            "timestamp" : 123456789,         // 回傳時間 | 限制：-  | 必填 {date}         毫秒
        }
    ```

    ```example
        請求失敗（）
        {
            "code" : "400",                           // 狀態代碼 | 限制：-  | 必填 {string}
            "message" : "errorToNotFilled",           // 狀態訊息 | 限制：-  | 必填 {string}
            "detail" : "尚未選擇您要上傳的輪播圖片！",  // 狀態說明 | 限制：-  | 必填 {string}
            "data" : "",                              // 回傳內容 | 限制：-  | 選填 {JSON}{array}  無內容可空值
            "timestamp" : 123456789,                  // 回傳時間 | 限制：-  | 必填 {date}         毫秒
        }
    ```

#### - 更新輪播資訊

* Route：/indexSlideshow/{slideId}
* Method：PUT
* Input（請求）：
  
    ```example
        {
            "data" : {
                "slideId" : "f84a4543-98c8-422c-8590-753e719e16c0",  // 輪播編號 | 限制：-  | 必填 {string}  輪播圖片類別另設 ID 唯一索引用
                "hyperlink" : "https://tcayla.webshopping.vip",      // 超連結址 | 限制：-  | 選填 {string}
                "first" : 0,                                         // 圖片置頂 | 限制：-  | 必填 {int}     預設置頂狀態為 0（0 關閉；1 開啟）
                "sort" : 0,                                          // 排序     | 限制：-  | 必填 {int}     排序，預設為流水號編號（從編號 1 號開始；0 為最大）                                  
                "updateDate" : "2021-10-22 16:29",                   // 更新日期 | 限制：-  | 必填 {date}                                 
            },
            "timestamp" : 123456789  // 請求時間 | 限制：-  | 必填 {date}  毫秒
        }
        註：排序功能，除非有使用要求，否則一般不作使用（直接由前端帶預設值，不由使用者設置）。
    ```

* Situation（情況）：
    | code | message | detail |
    | :--- | :--- | :--- |
    | 200（成功） | successButNoContent | "修改成功！" |
    | 400（失敗） | errorToNotFilled | "置頂欄位尚未填寫！" |
    | 400（失敗） | errorToNotFilled | "排序欄位尚未填寫！" |
    | 401（失敗） | errorToUnauthorized | "用戶端沒有認證憑據，伺服器拒絕回應、存取。" |
    | 404（失敗） | errorToNotFound | "請求失敗，請求的資訊、資源不存在。" |
* Response（回應）：

    ```example
        請求成功（200）
        {
            "code" : "200",                     // 狀態代碼 | 限制：-  | 必填 {string}
            "message" : "successButNoContent",  // 狀態訊息 | 限制：-  | 必填 {string}
            "detail" : "修改成功！",             // 狀態說明 | 限制：-  | 選填 {string}
            "data" : ""                         // 回傳內容 | 限制：-  | 選填 {JSON}{array}  無內容可空值                           
            "timestamp" : 123456789,            // 回傳時間 | 限制：-  | 必填 {date}         毫秒
        }
    ```

    ```example
        請求失敗（）
        {
            "code" : "401",                                        // 狀態代碼 | 限制：-  | 必填 {string}
            "message" : "errorToUnauthorized",                     // 狀態訊息 | 限制：-  | 必填 {string}
            "detail" : "用戶端沒有認證憑據，伺服器拒絕回應、存取。",  // 狀態說明 | 限制：-  | 必填 {string}
            "data" : "",                                           // 回傳內容 | 限制：-  | 選填 {JSON}{array}  無內容可空值
            "timestamp" : 123456789,                               // 回傳時間 | 限制：-  | 必填 {date}         毫秒
        }
    ```

#### - 刪除輪播資訊

* Route：/indexSlideshow/{slideId}
* Method：DELETE
* Input（請求）：
  
    ```example
        {
            "data" : {
                "slideId" : "f84a4543-98c8-422c-8590-753e719e16c0",  // 輪播編號 | 限制：-  | 必填 {string}  輪播圖片類別另設 ID 唯一索引用                               
            },
            "timestamp" : 123456789  // 請求時間 | 限制：-  | 必填 {date}  毫秒
        }
    ```

* Situation（情況）：
    | code | message | detail |
    | :--- | :--- | :--- |
    | 200（成功） | successButNoContent | "刪除成功！" |
    | 401（失敗） | errorToUnauthorized | "用戶端沒有認證憑據，伺服器拒絕回應、存取。" |
    | 404（失敗） | errorToNotFound | "請求失敗，請求的資訊、資源不存在。" |
* Response（回應）：

    ```example
        請求成功（200）
        {
            "code" : "200",                     // 狀態代碼 | 限制：-  | 必填 {string}
            "message" : "successButNoContent",  // 狀態訊息 | 限制：-  | 必填 {string}
            "detail" : "刪除成功！",             // 狀態說明 | 限制：-  | 選填 {string}
            "data" : ""                         // 回傳內容 | 限制：-  | 選填 {JSON}{array}  無內容可空值                           
            "timestamp" : 123456789,            // 回傳時間 | 限制：-  | 必填 {date}         毫秒
        }
    ```

    ```example
        請求失敗（）
        {
            "code" : "401",                                        // 狀態代碼 | 限制：-  | 必填 {string}
            "message" : "errorToUnauthorized",                     // 狀態訊息 | 限制：-  | 必填 {string}
            "detail" : "用戶端沒有認證憑據，伺服器拒絕回應、存取。",  // 狀態說明 | 限制：-  | 必填 {string}
            "data" : "",                                           // 回傳內容 | 限制：-  | 選填 {JSON}{array}  無內容可空值
            "timestamp" : 123456789,                               // 回傳時間 | 限制：-  | 必填 {date}         毫秒
        }
    ```

### 編輯管理

#### - 編輯器說明

編輯管理，採用文字編輯器「Quill」做內容的編輯，在後台使用者可自行做內容的版型編輯。
此編輯器以類似 JSON 格式的方式做儲存。
對圖片儲存方式：在前端上傳圖片時，會直接將圖片上傳至後端，後端將圖片儲存至伺服器，再將圖片儲存位置的「絕對或相對路經」傳回前端。

註：目前內容只以單一則的形式做取得、新增（修改）。

#### - 關於我們

* Route：/aboutUs/{aboutUsId}

#### - 隱私權政策

* Route：/privacy/{privacyId}

#### - 服務條款

* Route：/terms/{termsId}

#### - 取得內容資訊

* Route：aboutUs/{aboutUsId}（以關於我們作範例）
* Method：GET
* Input（請求）：
  
    ```example
        {
            "data" : {
                "aboutUsId" : "f84a4543-98c8-422c-8590-753e719e16c0",  // 內容編號 | 限制：-  | 選填 {string}  內容類別另設 ID 唯一索引用    
            },
            "timestamp" : 123456789  // 請求時間 | 限制：-  | 必填 {date}  毫秒
        }
        註：不傳送 aboutUsId 則回傳全部內容。
    ```

* Situation（情況）：
    | code | message | detail |
    | :--- | :--- | :--- |
    | 200（成功） | successToGetContent | "" |
    | 404（失敗） | errorToNotFound | "請求失敗，請求的資訊、資源不存在。" |
* Response（回應）：

    ```example
        請求成功（200）
        {
            "code" : "200",                     // 狀態代碼 | 限制：-  | 必填 {string}
            "message" : "successToGetContent",  // 狀態訊息 | 限制：-  | 必填 {string}
            "detail" : "",                      // 狀態說明 | 限制：-  | 選填 {string}
            "data" : {                          // 回傳內容 | 限制：-  | 選填 {JSON}{array}  無內容可空值
                "id" : 1,                                              // 流水號編號 | 限制：-  | 必填 {int}   流水號，資料庫排序、索引用
                "aboutUsId" : "f84a4543-98c8-422c-8590-753e719e16c0",  // 內容編號 | 限制：-  | 必填 {string}  內容類別另設 ID 唯一索引用
                "content" : {...},                                     // 關於我們 | 限制：-  | 必填 {JSON}    Delta 物件，類似 JSON 格式的物件
                "createDate" : "2021-10-22 16:29",                     // 建立日期 | 限制：-  | 必填 {date}
                "updateDate" : "2021-10-22 16:29",                     // 更新日期 | 限制：-  | 必填 {date}                                 
            },                               
            "timestamp" : 123456789,            // 回傳時間 | 限制：-  | 必填 {date}         毫秒
        }
    ```

    ```example
        請求失敗（）
        {
            "code" : "404",                                 // 狀態代碼 | 限制：-  | 必填 {string}
            "message" : "errorToNotFound",                  // 狀態訊息 | 限制：-  | 必填 {string}
            "detail" : "請求失敗，請求的資訊、資源不存在。",  // 狀態說明 | 限制：-  | 必填 {string}
            "data" : "",                                    // 回傳內容 | 限制：-  | 選填 {JSON}{array}  無內容可空值
            "timestamp" : 123456789,                        // 回傳時間 | 限制：-  | 必填 {date}         毫秒
        }
    ```

#### - 新增（修改）內容資訊

* Route：aboutUs/{aboutUsId}（以關於我們作範例）
* Method：PUT
* Input（請求）：

    ```example
        {
            "data" : {
                "aboutUsId" : "f84a4543-98c8-422c-8590-753e719e16c0",  // 內容編號 | 限制：-  | 必填 {string}  內容類別另設 ID 唯一索引用
                "content" : {...},                                     // 關於我們 | 限制：-  | 必填 {JSON}    Delta 物件，類似 JSON 格式的物件
                "cNameArr" : [...],                                    // 圖片集合 | 限制：-  | 必填 {array}   編輯器中插入的圖片名稱集合，用於刪除被取代的圖片；無值（無圖片）則空陣列。
                "updateDate" : "2021-10-22 16:29",                     // 更新日期 | 限制：-  | 必填 {date}                               
            },
            "timestamp" : 123456789  // 請求時間 | 限制：-  | 必填 {date}  毫秒
        }
    ```

* Situation（情況）：
    | code | message | detail |
    | :--- | :--- | :--- |
    | 200（成功） | successButNoContent | "修改成功！" |
    | 400（失敗） | errorToNotFilled | "內容欄位尚未填寫！" |
    | 401（失敗） | errorToUnauthorized | "用戶端沒有認證憑據，伺服器拒絕回應、存取。" |
* Response（回應）：

    ```example
        請求成功（200）
        {
            "code" : "200",                     // 狀態代碼 | 限制：-  | 必填 {string}
            "message" : "successButNoContent",  // 狀態訊息 | 限制：-  | 必填 {string}
            "detail" : "新增成功！",             // 狀態說明 | 限制：-  | 選填 {string}
            "data" : ""                         // 回傳內容 | 限制：-  | 選填 {JSON}{array}  無內容可空值                           
            "timestamp" : 123456789,            // 回傳時間 | 限制：-  | 必填 {date}         毫秒
        }
    ```

    ```example
        請求失敗（）
        {
            "code" : "401",                                        // 狀態代碼 | 限制：-  | 必填 {string}
            "message" : "errorToUnauthorized",                     // 狀態訊息 | 限制：-  | 必填 {string}
            "detail" : "用戶端沒有認證憑據，伺服器拒絕回應、存取。",  // 狀態說明 | 限制：-  | 必填 {string}
            "data" : "",                                           // 回傳內容 | 限制：-  | 選填 {JSON}{array}  無內容可空值
            "timestamp" : 123456789,                               // 回傳時間 | 限制：-  | 必填 {date}         毫秒
        }
    ```

#### - 新增（修改）內容圖片儲存方式

* Route：aboutUs/image（以關於我們作範例）
* Method：POST
* Input（請求）：

    ```example
        {
            "data" : {
                "file" : file,  // 圖片檔案 | 限制：-  | 必填 {file}  要上傳的圖片檔案
            },
            "timestamp" : 123456789  // 請求時間 | 限制：-  | 必填 {date}  秒
        }
    ```

* Situation（情況）：
    | code | message | detail |
    | :--- | :--- | :--- |
    | 201（成功） | successToCreated | "新增成功！" |
    | 400（失敗） | errorToBadRequest | "請上傳正確的圖片檔案格式（格式：JPG、PNG 等圖片格式）！" |
    | 401（失敗） | errorToUnauthorized | "用戶端沒有認證憑據，伺服器拒絕回應、存取。" |
* Response（回應）：

    ```example
        請求成功（201）
        {
            "code" : "201",                  // 狀態代碼 | 限制：-  | 必填 {string}
            "message" : "successToCreated",  // 狀態訊息 | 限制：-  | 必填 {string}
            "detail" : "新增成功！",          // 狀態說明 | 限制：-  | 選填 {string}
            "data" : {                       // 回傳內容 | 限制：-  | 選填 {JSON}{array}  無內容可空值 
                "id" : 1,                                            // 流水號編號 | 限制：-  | 必填 {int}   流水號，資料庫排序、索引用
                "imageId" : "f84a4543-98c8-422c-8590-753e719e16c0",  // 圖片編號 | 限制：-  | 必填 {string}  圖片類別另設 ID 唯一索引用
                "imageURL" : "img/banners/123.png",                  // 圖片位置 | 限制：-  | 必填 {JSON}    傳送格式 = img / 存放圖片的資料夾 / 個別資料夾 / 圖片名稱.圖片格式
            },                          
            "timestamp" : 123456789,         // 回傳時間 | 限制：-  | 必填 {date}         毫秒
        }
    ```

    ```example
        請求失敗（）
        {
            "code" : "400",                                                     // 狀態代碼 | 限制：-  | 必填 {string}
            "message" : "errorToBadRequest",                                    // 狀態訊息 | 限制：-  | 必填 {string}
            "detail" : "請上傳正確的圖片檔案格式（格式：JPG、PNG 等圖片格式）！",  // 狀態說明 | 限制：-  | 必填 {string}
            "data" : "",                                                        // 回傳內容 | 限制：-  | 選填 {JSON}{array}  無內容可空值
            "timestamp" : 123456789,                                            // 回傳時間 | 限制：-  | 必填 {date}         毫秒
        }
    ```

### 消息管理（無目錄）

#### - 取得消息資訊

* Route：/news/{newsId}
* Method：GET
* Input（請求）：
  
    ```example
        {
            "data" : {
                "newsId" : "f84a4543-98c8-422c-8590-753e719e16c0",  // 消息編號 | 限制：-  | 選填 {string}  消息類別另設 ID 唯一索引用    
            },
            "timestamp" : 123456789  // 請求時間 | 限制：-  | 必填 {date}  毫秒
        }
        註：不傳送 newsId 則回傳全部消息。
        註：取得全部消息不傳送消息內容欄位，傳送消息簡述欄位。
    ```

* Situation（情況）：
    | code | message | detail |
    | :--- | :--- | :--- |
    | 200（成功） | successToGetContent | "" |
    | 404（失敗） | errorToNotFound | "請求失敗，請求的資訊、資源不存在。" |
* Response（回應）：

    ```example
        請求成功（200）
        {
            "code" : "200",                     // 狀態代碼 | 限制：-  | 必填 {string}
            "message" : "successToGetContent",  // 狀態訊息 | 限制：-  | 必填 {string}
            "detail" : "",                      // 狀態說明 | 限制：-  | 選填 {string}
            "data" : {                          // 回傳內容 | 限制：-  | 選填 {JSON}{array}  無內容可空值
                "id" : 1,                                           // 流水號編號 | 限制：-  | 必填 {int}    流水號，資料庫排序、索引用
                "newsId" : "f84a4543-98c8-422c-8590-753e719e16c0",  // 消息編號 | 限制：-  | 必填 {string}   消息類別另設 ID 唯一索引用
                "imageURL" : "img/news/20211102/123.png",           // 縮圖位置 | 限制：-  | 必填 {JSON}     傳送格式 = img / 存放圖片的資料夾 / 個別資料夾 / 圖片名稱.圖片格式 
                "name" : "123.png",                                 // 消息標題 | 限制：40 | 必填 {string}
                "brief" : "...",                                    // 消息簡述 | 限制：100 | 必填 {string}   
                "content" : {...},                                  // 消息內容 | 限制：-  | 必填 {JSON}     Delta 物件，類似 JSON 格式的物件（註：取得全部不傳送此欄位或內容為 null。）
                "first" : 0,                                        // 消息置頂 | 限制：-  | 必填 {int}}     置頂狀態（0 關閉；1 開啟）
                "sort" : 0,                                         // 排序     | 限制：-  | 必填 {int}      排序，預設為流水號編號（從編號 1 號開始；0 為最大）                                  
                "enabled" : 1,                                      // 消息狀態 | 限制：-  | 必填 {int}      啟用狀態（0 關閉；1 開啟）
                "startDate" : "2021-10-22 16:29",                   // 上架日期 | 限制：-  | 選填 {date}     設定上架日期，上架前啟用狀態為關閉（如果設定為開啟則改為關閉）
                "endDate" : " "2021-10-22 16:29",                   // 下架日期 | 限制：-  | 選填 {date}     設定下架日期，下架前啟用狀態為開啟（如果更新為關閉則清除下架日期改為空值）
                "createDate" : "2021-10-22 16:29",                  // 建立日期 | 限制：-  | 必填 {date}
                "updateDate" : "2021-10-22 16:29",                  // 更新日期 | 限制：-  | 必填 {date}                                 
            },                               
            "timestamp" : 123456789,            // 回傳時間 | 限制：-  | 必填 {date}         毫秒
        }
        註：消息縮圖位置與消息內容中的圖片位置應相同。
        註：排序功能，除非有使用要求，否則一般不作使用（直接由前端帶預設值，不由使用者設置）。
    ```

    ```example
        請求失敗（）
        {
            "code" : "404",                                 // 狀態代碼 | 限制：-  | 必填 {string}
            "message" : "errorToNotFound",                  // 狀態訊息 | 限制：-  | 必填 {string}
            "detail" : "請求失敗，請求的資訊、資源不存在。",  // 狀態說明 | 限制：-  | 必填 {string}
            "data" : "",                                    // 回傳內容 | 限制：-  | 選填 {JSON}{array}  無內容可空值
            "timestamp" : 123456789,                        // 回傳時間 | 限制：-  | 必填 {date}         毫秒
        }
    ```

#### - 取得篩選消息資訊（分頁器）

* Route：/news/query?{count=""}&{page=""}
* Method：GET
* Input（請求）：
  
    ```example
        {
            "data" : {
                "count" : "10",                                         // 渲染筆數 | 限制：-  | 選填 {int}     每頁一次渲染的消息筆數    
                "page" : "1",                                           // 當前頁碼 | 限制：-  | 選填 {int}     目前的頁碼        
            },
            "timestamp" : 123456789  // 請求時間 | 限制：-  | 必填 {date}  毫秒
        }
        註：條件須都帶上，可不填值（不填值則回傳全部）。
    ```

* Situation（情況）：
    | code | message | detail |
    | :--- | :--- | :--- |
    | 200（成功） | successToGetContent | "" |
    | 404（失敗） | errorToNotFound | "請求失敗，請求的資訊、資源不存在。" |
* Response（回應）：

    ```example
        請求成功（200）
        {
            "code" : "200",                     // 狀態代碼 | 限制：-  | 必填 {string}
            "message" : "successToGetContent",  // 狀態訊息 | 限制：-  | 必填 {string}
            "detail" : "",                      // 狀態說明 | 限制：-  | 選填 {string}
            "data" : {                          // 回傳內容 | 限制：-  | 選填 {JSON}{array}  無內容可空值
                "id" : 1,                                           // 流水號編號 | 限制：-  | 必填 {int}    流水號，資料庫排序、索引用
                "newsId" : "f84a4543-98c8-422c-8590-753e719e16c0",  // 消息編號 | 限制：-  | 必填 {string}   消息類別另設 ID 唯一索引用
                "imageURL" : "img/news/20211102/123.png",           // 縮圖位置 | 限制：-  | 必填 {JSON}     傳送格式 = img / 存放圖片的資料夾 / 個別資料夾 / 圖片名稱.圖片格式 
                "name" : "123.png",                                 // 消息標題 | 限制：40 | 必填 {string}
                "brief" : "...",                                    // 消息簡述 | 限制：100 | 必填 {string}   
                "content" : {...},                                  // 消息內容 | 限制：-  | 必填 {JSON}     Delta 物件，類似 JSON 格式的物件（註：取得全部不傳送此欄位或內容為 null。）
                "first" : 0,                                        // 消息置頂 | 限制：-  | 必填 {int}}     置頂狀態（0 關閉；1 開啟）
                "sort" : 0,                                         // 排序     | 限制：-  | 必填 {int}      排序，預設為流水號編號（從編號 1 號開始；0 為最大）                                  
                "enabled" : 1,                                      // 消息狀態 | 限制：-  | 必填 {int}      啟用狀態（0 關閉；1 開啟）
                "startDate" : "2021-10-22 16:29",                   // 上架日期 | 限制：-  | 選填 {date}     設定上架日期，上架前啟用狀態為關閉（如果設定為開啟則改為關閉）
                "endDate" : " "2021-10-22 16:29",                   // 下架日期 | 限制：-  | 選填 {date}     設定下架日期，下架前啟用狀態為開啟（如果更新為關閉則清除下架日期改為空值）
                "createDate" : "2021-10-22 16:29",                  // 建立日期 | 限制：-  | 必填 {date}
                "updateDate" : "2021-10-22 16:29",                  // 更新日期 | 限制：-  | 必填 {date}                                 
            },                               
            "timestamp" : 123456789,            // 回傳時間 | 限制：-  | 必填 {date}         毫秒
        }
        註：消息縮圖位置與消息內容中的圖片位置應相同。
        註：排序功能，除非有使用要求，否則一般不作使用（直接由前端帶預設值，不由使用者設置）。
    ```

    ```example
        請求失敗（）
        {
            "code" : "404",                                 // 狀態代碼 | 限制：-  | 必填 {string}
            "message" : "errorToNotFound",                  // 狀態訊息 | 限制：-  | 必填 {string}
            "detail" : "請求失敗，請求的資訊、資源不存在。",  // 狀態說明 | 限制：-  | 必填 {string}
            "data" : "",                                    // 回傳內容 | 限制：-  | 選填 {JSON}{array}  無內容可空值
            "timestamp" : 123456789,                        // 回傳時間 | 限制：-  | 必填 {date}         毫秒
        }
    ```

#### - 新增消息資訊

* Route：/news
* Method：POST
* Input（請求）：
  
    ```example
        {
            "data" : {
                "file" : file,                      // 消息縮圖 | 限制：-  | 必填 {file}    要上傳的圖片檔案
                "name" : "123.png",                 // 消息標題 | 限制：40 | 必填 {string}
                "brief" : "...",                    // 消息簡述 | 限制：100 | 必填 {string}                   
                "content" : {...},                  // 消息內容 | 限制：-  | 必填 {JSON}    Delta 物件，類似 JSON 格式的物件
                "cNameArr" : [...],                 // 圖片集合 | 限制：-  | 必填 {array}   編輯器中插入的圖片名稱集合，用於刪除被取代的圖片；無值（無圖片）則空陣列。
                "first" : 0,                        // 消息置頂 | 限制：-  | 必填 {int}}    置頂狀態（0 關閉；1 開啟）
                "enabled" : 1,                      // 消息狀態 | 限制：-  | 必填 {int}     啟用狀態（0 關閉；1 開啟）
                "startDate" : "2021-10-22 16:29",   // 上架日期 | 限制：-  | 選填 {date}    設定上架日期，上架前啟用狀態為關閉（如果設定為開啟則改為關閉）
                "endDate" : " "2021-10-22 16:29",   // 下架日期 | 限制：-  | 選填 {date}    設定下架日期，下架前啟用狀態為開啟（如果更新為關閉則清除下架日期改為空值）
                "createDate" : "2021-10-22 16:29",  // 建立日期 | 限制：-  | 必填 {date}
            },
            "timestamp" : 123456789  // 請求時間 | 限制：-  | 必填 {date}  毫秒
        }
        註：排序以預設流水號方式由後端直接產生，前端新增不另帶
    ```

* Situation（情況）：
    | code | message | detail |
    | :--- | :--- | :--- |
    | 201（成功） | successToCreated | "新增成功！" |
    | 400（失敗） | errorToNotFilled | "尚未選擇您要上傳的消息縮圖！" |
    | 400（失敗） | errorToBadRequest | "請上傳正確的圖片檔案格式（格式：JPG、PNG 等圖片格式）！" |
    | 400（失敗） | errorToNotFilled | "標題欄位尚未填寫！" |
    | 400（失敗） | errorToBadRequest | "標題欄位長度輸入超過限制（40個字）！" |
    | 400（失敗） | errorToNotFilled | "簡述欄位尚未填寫！" |
    | 400（失敗） | errorToBadRequest | "簡述欄位長度輸入超過限制（100個字）！" |
    | 400（失敗） | errorToNotFilled | "內容欄位尚未填寫！" |
    | 400（失敗） | errorToNotFilled | "置頂欄位尚未填寫！" |
    | 400（失敗） | errorToNotFilled | "狀態欄位尚未填寫！" |
    | 400（失敗） | errorToBadRequest | "上架日期欄位格式輸入錯誤（不得設定過去時間）！" |
    | 400（失敗） | errorToBadRequest | "上架日期欄位格式輸入錯誤（上架時間不得大於下架時間）！" |
    | 400（失敗） | errorToBadRequest | "下架日期欄位格式輸入錯誤（不得設定過去時間）！" |
    | 400（失敗） | errorToBadRequest | "下架日期欄位格式輸入錯誤（下架時間不得小於上架時間）！" |
    | 401（失敗） | errorToUnauthorized | "用戶端沒有認證憑據，伺服器拒絕回應、存取。" |
* Response（回應）：

    ```example
        請求成功（201）
        {
            "code" : "201",                  // 狀態代碼 | 限制：-  | 必填 {string}
            "message" : "successToCreated",  // 狀態訊息 | 限制：-  | 必填 {string}
            "detail" : "新增成功！",          // 狀態說明 | 限制：-  | 選填 {string}
            "data" : {                       // 回傳內容 | 限制：-  | 選填 {JSON}{array}  無內容可空值
                "id" : 1,                                           // 編號流水號 | 限制：-  | 必填 {int}   流水號，資料庫排序、索引用
                "newsId" : "f84a4543-98c8-422c-8590-753e719e16c0",  // 輪播編號 | 限制：-  | 必填 {string}  消息類別另設 ID 唯一索引用
                "first" : 0,                                        // 消息置頂 | 限制：-  | 必填 {int}}    置頂狀態（0 關閉；1 開啟）
                "sort" : 0,                                         // 排序     | 限制：-  | 必填 {int}     排序，預設為流水號編號（從編號 1 號開始；0 為最大）                                  
                "enabled" : 1,                                      // 消息狀態 | 限制：-  | 必填 {int}     啟用狀態（0 關閉；1 開啟）
                "startDate" : "2021-10-22 16:29",                   // 上架日期 | 限制：-  | 選填 {date}    設定上架日期，上架前啟用狀態為關閉（如果設定為開啟則改為關閉）
                "endDate" : " "2021-10-22 16:29",                   // 下架日期 | 限制：-  | 選填 {date}    設定下架日期，下架前啟用狀態為開啟（如果更新為關閉則清除下架日期改為空值）
                "createDate" : "2021-10-22 16:29",                  // 建立日期 | 限制：-  | 必填 {date}              
            },                               
            "timestamp" : 123456789,         // 回傳時間 | 限制：-  | 必填 {date}         毫秒
        }
    ```

    ```example
        請求失敗（）
        {
            "code" : "400",                           // 狀態代碼 | 限制：-  | 必填 {string}
            "message" : "errorToNotFilled",           // 狀態訊息 | 限制：-  | 必填 {string}
            "detail" : "尚未選擇您要上傳的輪播圖片！",  // 狀態說明 | 限制：-  | 必填 {string}
            "data" : "",                              // 回傳內容 | 限制：-  | 選填 {JSON}{array}  無內容可空值
            "timestamp" : 123456789,                  // 回傳時間 | 限制：-  | 必填 {date}         毫秒
        }
    ```

#### - 更新消息資訊

* Route：/news/{newsId}
* Method：PUT
* Input（請求）：
  
    ```example
        {
            "data" : {
                "newsId" : "f84a4543-98c8-422c-8590-753e719e16c0",  // 消息編號 | 限制：-  | 必填 {string}  消息類別另設 ID 唯一索引用
                "file" : file,                                      // 消息縮圖 | 限制：-  | 選填 {file}    要上傳的圖片檔案（如果傳送的是空值則為不修改；如果傳送新的檔案則修改；）
                "name" : "123.png",                                 // 消息標題 | 限制：40 | 必填 {string}
                "brief" : "...",                                    // 消息簡述 | 限制：100 | 必填 {string}                   
                "content" : {...},                                  // 消息內容 | 限制：-  | 必填 {JSON}    Delta 物件，類似 JSON 格式的物件
                "cNameArr" : [...],                                 // 圖片集合 | 限制：-  | 必填 {array}   編輯器中插入的圖片名稱集合，用於刪除被取代的圖片；無值（無圖片）則空陣列。
                "first" : 0,                                        // 消息置頂 | 限制：-  | 必填 {int}}    置頂狀態（0 關閉；1 開啟）
                "sort" : 0,                                         // 排序     | 限制：-  | 必填 {int}     排序，預設為流水號編號（從編號 1 號開始；0 為最大）                                                  
                "enabled" : 1,                                      // 消息狀態 | 限制：-  | 必填 {int}     啟用狀態（0 關閉；1 開啟）
                "startDate" : "2021-10-22 16:29",                   // 上架日期 | 限制：-  | 選填 {date}    設定上架日期，上架前啟用狀態為關閉（如果設定為開啟則改為關閉）
                "endDate" : " "2021-10-22 16:29",                   // 下架日期 | 限制：-  | 選填 {date}    設定下架日期，下架前啟用狀態為開啟（如果更新為關閉則清除下架日期改為空值）
                "updateDate" : "2021-10-22 16:29",                  // 更新日期 | 限制：-  | 必填 {date}                                 
            },
            "timestamp" : 123456789  // 請求時間 | 限制：-  | 必填 {date}  毫秒
        }
        註：排序功能，除非有使用要求，否則一般不作使用（直接由前端帶預設值，不由使用者設置）。
    ```

* Situation（情況）：
    | code | message | detail |
    | :--- | :--- | :--- |
    | 200（成功） | successButNoContent | "修改成功！" |
    | 400（失敗） | errorToBadRequest | "請上傳正確的圖片檔案格式（格式：JPG、PNG 等圖片格式）！" |
    | 400（失敗） | errorToNotFilled | "標題欄位尚未填寫！" |
    | 400（失敗） | errorToBadRequest | "標題欄位長度輸入超過限制（40個字）！" |
    | 400（失敗） | errorToNotFilled | "簡述欄位尚未填寫！" |
    | 400（失敗） | errorToBadRequest | "簡述欄位長度輸入超過限制（100個字）！" |
    | 400（失敗） | errorToNotFilled | "內容欄位尚未填寫！" |
    | 400（失敗） | errorToNotFilled | "置頂欄位尚未填寫！" |
    | 400（失敗） | errorToNotFilled | "狀態欄位尚未填寫！" |
    | 400（失敗） | errorToBadRequest | "上架日期欄位格式輸入錯誤（不得設定過去時間）！" |
    | 400（失敗） | errorToBadRequest | "上架日期欄位格式輸入錯誤（上架時間不得大於下架時間）！" |
    | 400（失敗） | errorToBadRequest | "下架日期欄位格式輸入錯誤（不得設定過去時間）！" |
    | 400（失敗） | errorToBadRequest | "下架日期欄位格式輸入錯誤（下架時間不得小於上架時間）！" |
    | 401（失敗） | errorToUnauthorized | "用戶端沒有認證憑據，伺服器拒絕回應、存取。" |
    | 404（失敗） | errorToNotFound | "請求失敗，請求的資訊、資源不存在。" |
* Response（回應）：

    ```example
        請求成功（200）
        {
            "code" : "200",                     // 狀態代碼 | 限制：-  | 必填 {string}
            "message" : "successButNoContent",  // 狀態訊息 | 限制：-  | 必填 {string}
            "detail" : "修改成功！",             // 狀態說明 | 限制：-  | 選填 {string}
            "data" : ""                         // 回傳內容 | 限制：-  | 選填 {JSON}{array}  無內容可空值                           
            "timestamp" : 123456789,            // 回傳時間 | 限制：-  | 必填 {date}         毫秒
        }
    ```

    ```example
        請求失敗（）
        {
            "code" : "401",                                        // 狀態代碼 | 限制：-  | 必填 {string}
            "message" : "errorToUnauthorized",                     // 狀態訊息 | 限制：-  | 必填 {string}
            "detail" : "用戶端沒有認證憑據，伺服器拒絕回應、存取。",  // 狀態說明 | 限制：-  | 必填 {string}
            "data" : "",                                           // 回傳內容 | 限制：-  | 選填 {JSON}{array}  無內容可空值
            "timestamp" : 123456789,                               // 回傳時間 | 限制：-  | 必填 {date}         毫秒
        }
    ```

#### - 新增（修改）消息圖片儲存方式

* Route：news/image
* Method：POST
* Input（請求）：

    ```example
        {
            "data" : {
                "file" : file,  // 圖片檔案 | 限制：-  | 必填 {file}  要上傳的圖片檔案
            },
            "timestamp" : 123456789  // 請求時間 | 限制：-  | 必填 {date}  秒
        }
    ```

* Situation（情況）：
    | code | message | detail |
    | :--- | :--- | :--- |
    | 201（成功） | successToCreated | "新增成功！" |
    | 400（失敗） | errorToBadRequest | "請上傳正確的圖片檔案格式（格式：JPG、PNG 等圖片格式）！" |
    | 401（失敗） | errorToUnauthorized | "用戶端沒有認證憑據，伺服器拒絕回應、存取。" |
* Response（回應）：

    ```example
        請求成功（201）
        {
            "code" : "201",                  // 狀態代碼 | 限制：-  | 必填 {string}
            "message" : "successToCreated",  // 狀態訊息 | 限制：-  | 必填 {string}
            "detail" : "新增成功！",          // 狀態說明 | 限制：-  | 選填 {string}
            "data" : {                       // 回傳內容 | 限制：-  | 選填 {JSON}{array}  無內容可空值 
                "id" : 1,                                            // 流水號編號 | 限制：-  | 必填 {int}   流水號，資料庫排序、索引用
                "imageId" : "f84a4543-98c8-422c-8590-753e719e16c0",  // 圖片編號 | 限制：-  | 必填 {string}  圖片類別另設 ID 唯一索引用
                "imageURL" : "img/banners/123.png",                  // 圖片位置 | 限制：-  | 必填 {JSON}    傳送格式 = img / 存放圖片的資料夾 / 個別資料夾 / 圖片名稱.圖片格式
            },                          
            "timestamp" : 123456789,         // 回傳時間 | 限制：-  | 必填 {date}         毫秒
        }
    ```

    ```example
        請求失敗（）
        {
            "code" : "400",                                                     // 狀態代碼 | 限制：-  | 必填 {string}
            "message" : "errorToBadRequest",                                    // 狀態訊息 | 限制：-  | 必填 {string}
            "detail" : "請上傳正確的圖片檔案格式（格式：JPG、PNG 等圖片格式）！",  // 狀態說明 | 限制：-  | 必填 {string}
            "data" : "",                                                        // 回傳內容 | 限制：-  | 選填 {JSON}{array}  無內容可空值
            "timestamp" : 123456789,                                            // 回傳時間 | 限制：-  | 必填 {date}         毫秒
        }

#### - 刪除消息資訊

* Route：/news/{newsId}
* Method：DELETE
* Input（請求）：
  
    ```example
        {
            "data" : {
                "newsId" : "f84a4543-98c8-422c-8590-753e719e16c0",  // 消息編號 | 限制：-  | 必填 {string}  消息類別另設 ID 唯一索引用                               
            },
            "timestamp" : 123456789  // 請求時間 | 限制：-  | 必填 {date}  毫秒
        }
    ```

* Situation（情況）：
    | code | message | detail |
    | :--- | :--- | :--- |
    | 200（成功） | successButNoContent | "刪除成功！" |
    | 401（失敗） | errorToUnauthorized | "用戶端沒有認證憑據，伺服器拒絕回應、存取。" |
    | 404（失敗） | errorToNotFound | "請求失敗，請求的資訊、資源不存在。" |
* Response（回應）：

    ```example
        請求成功（200）
        {
            "code" : "200",                     // 狀態代碼 | 限制：-  | 必填 {string}
            "message" : "successButNoContent",  // 狀態訊息 | 限制：-  | 必填 {string}
            "detail" : "刪除成功！",             // 狀態說明 | 限制：-  | 選填 {string}
            "data" : ""                         // 回傳內容 | 限制：-  | 選填 {JSON}{array}  無內容可空值                           
            "timestamp" : 123456789,            // 回傳時間 | 限制：-  | 必填 {date}         毫秒
        }
    ```

    ```example
        請求失敗（）
        {
            "code" : "401",                                        // 狀態代碼 | 限制：-  | 必填 {string}
            "message" : "errorToUnauthorized",                     // 狀態訊息 | 限制：-  | 必填 {string}
            "detail" : "用戶端沒有認證憑據，伺服器拒絕回應、存取。",  // 狀態說明 | 限制：-  | 必填 {string}
            "data" : "",                                           // 回傳內容 | 限制：-  | 選填 {JSON}{array}  無內容可空值
            "timestamp" : 123456789,                               // 回傳時間 | 限制：-  | 必填 {date}         毫秒
        }
    ```

### 活動管理（有目錄）

#### - 活動目錄

##### 取得活動目錄資訊

* Route：/activity/category/{categoryId}
* Method：GET
* Input（請求）：
  
    ```example
        {
            "data" : {
                "categoryId" : "f84a4543-98c8-422c-8590-753e719e16c0",  // 目錄編號 | 限制：-  | 選填 {string}  目錄類別另設 ID 唯一索引用    
            },
            "timestamp" : 123456789  // 請求時間 | 限制：-  | 必填 {date}  毫秒
        }
        註：不傳送 categoryId 則回傳全部目錄。
        註：預設一個「所有目錄」且不得刪除。 
    ```

* Situation（情況）：
    | code | message | detail |
    | :--- | :--- | :--- |
    | 200（成功） | successToGetContent | "" |
    | 404（失敗） | errorToNotFound | "請求失敗，請求的資訊、資源不存在。" |
* Response（回應）：

    ```example
        請求成功（200）
        {
            "code" : "200",                     // 狀態代碼 | 限制：-  | 必填 {string}
            "message" : "successToGetContent",  // 狀態訊息 | 限制：-  | 必填 {string}
            "detail" : "",                      // 狀態說明 | 限制：-  | 選填 {string}
            "data" : {                          // 回傳內容 | 限制：-  | 選填 {JSON}{array}  無內容可空值
                "id" : 1,                                               // 流水號編號 | 限制：-  | 必填 {int}   流水號，資料庫排序、索引用
                "categoryId" : "f84a4543-98c8-422c-8590-753e719e16c0",  // 目錄編號 | 限制：-  | 必填 {string}  目錄類別另設 ID 唯一索引用
                "name" : "活動目錄",                                    // 目錄名稱 | 限制：20 | 必填 {string}
                "berif" : "...",                                        // 目錄簡述 | 限制：40 | 選填 {string}  目錄的說明簡述，可為空值或 null
                "sort" : 0,                                             // 目錄排序 | 限制：-  | 必填 {int}     排序，預設可為流水號編號（從編號 1 號開始；設為 0 即為為置頂）                
                "enabled" : 1,                                          // 目錄狀態 | 限制：-  | 必填 {int}     啟用狀態（0 關閉；1 開啟）
                "createDate" : "2021-10-22 16:29",                      // 建立日期 | 限制：-  | 必填 {date}
                "updateDate" : "2021-10-22 16:29",                      // 更新日期 | 限制：-  | 必填 {date}                                 
            },                               
            "timestamp" : 123456789,            // 回傳時間 | 限制：-  | 必填 {date}         毫秒
        }
    ```

    ```example
        請求失敗（）
        {
            "code" : "404",                                 // 狀態代碼 | 限制：-  | 必填 {string}
            "message" : "errorToNotFound",                  // 狀態訊息 | 限制：-  | 必填 {string}
            "detail" : "請求失敗，請求的資訊、資源不存在。",  // 狀態說明 | 限制：-  | 必填 {string}
            "data" : "",                                    // 回傳內容 | 限制：-  | 選填 {JSON}{array}  無內容可空值
            "timestamp" : 123456789,                        // 回傳時間 | 限制：-  | 必填 {date}         毫秒
        }
    ```

##### 新增活動目錄資訊

* Route：/activity/category
* Method：POST
* Input（請求）：
  
    ```example
        {
            "data" : {
                "name" : "活動目錄",                // 目錄名稱 | 限制：20 | 必填 {string}
                "berif" : "...",                    // 目錄簡述 | 限制：40 | 選填 {string}  目錄的說明簡述，可為空值或 null
                "sort" : 0,                         // 目錄排序 | 限制：-  | 必填 {int}     排序，預設可為流水號編號（從編號 1 號開始；設為 0 即為為置頂）                
                "enabled" : 1,                      // 目錄狀態 | 限制：-  | 必填 {int}     啟用狀態（0 關閉；1 開啟）
                "createDate" : "2021-10-22 16:29",  // 建立日期 | 限制：-  | 必填 {date}
            },
            "timestamp" : 123456789  // 請求時間 | 限制：-  | 必填 {date}  毫秒
        }
    ```

* Situation（情況）：
    | code | message | detail |
    | :--- | :--- | :--- |
    | 201（成功） | successToCreated | "新增成功！" |
    | 400（失敗） | errorToNotFilled | "名稱欄位尚未填寫！" |
    | 400（失敗） | errorToBadRequest | "名稱欄位長度輸入超過限制（20個字）。" |
    | 400（失敗） | errorToBadRequest | "簡述欄位長度輸入超過限制（40個字）。" |
    | 400（失敗） | errorToNotFilled | "排序欄位尚未填寫！" |
    | 400（失敗） | errorToNotFilled | "狀態欄位尚未填寫！" |
    | 401（失敗） | errorToUnauthorized | "用戶端沒有認證憑據，伺服器拒絕回應、存取。" |
* Response（回應）：

    ```example
        請求成功（201）
        {
            "code" : "201",                  // 狀態代碼 | 限制：-  | 必填 {string}
            "message" : "successToCreated",  // 狀態訊息 | 限制：-  | 必填 {string}
            "detail" : "新增成功！",          // 狀態說明 | 限制：-  | 選填 {string}
            "data" : {                       // 回傳內容 | 限制：-  | 選填 {JSON}{array}  無內容可空值
                "id" : 1,                                               // 編號流水號 | 限制：-  | 必填 {int}   流水號，資料庫排序、索引用
                "categoryId" : "f84a4543-98c8-422c-8590-753e719e16c0",  // 目錄編號 | 限制：-  | 必填 {string}  消息類別另設 ID 唯一索引用
                "sort" : 0,                                             // 目錄排序 | 限制：-  | 選填 {int}     排序，預設可為流水號編號（從編號 1 號開始；設為 0 即為為置頂）                
                "enabled" : 1,                                          // 消息狀態 | 限制：-  | 必填 {int}     啟用狀態（0 關閉；1 開啟）
                "createDate" : "2021-10-22 16:29",                      // 建立日期 | 限制：-  | 必填 {date}              
            },                               
            "timestamp" : 123456789,         // 回傳時間 | 限制：-  | 必填 {date}         毫秒
        }
    ```

    ```example
        請求失敗（）
        {
            "code" : "400",                  // 狀態代碼 | 限制：-  | 必填 {string}
            "message" : "errorToNotFilled",  // 狀態訊息 | 限制：-  | 必填 {string}
            "detail" : "名稱欄位尚未填寫！",  // 狀態說明 | 限制：-  | 必填 {string}
            "data" : "",                     // 回傳內容 | 限制：-  | 選填 {JSON}{array}  無內容可空值
            "timestamp" : 123456789,         // 回傳時間 | 限制：-  | 必填 {date}         毫秒
        }
    ```

##### 更新活動目錄資訊

* Route：/activity/category/{categoryId}
* Method：PUT
* Input（請求）：
  
    ```example
        {
            "data" : {
                "categoryId" : "f84a4543-98c8-422c-8590-753e719e16c0",  // 目錄編號 | 限制：-  | 必填 {string}  目錄類別另設 ID 唯一索引用
                "name" : "活動目錄",                                    // 目錄名稱 | 限制：20 | 必填 {string}
                "berif" : "...",                                        // 目錄簡述 | 限制：40 | 選填 {string}  目錄的說明簡述，可為空值或 null
                "sort" : 0,                                             // 目錄排序 | 限制：-  | 必填 {int}     排序，預設可為流水號編號（從編號 1 號開始；設為 0 即為為置頂）                
                "enabled" : 1,                                          // 目錄狀態 | 限制：-  | 必填 {int}     啟用狀態（0 關閉；1 開啟）
                "updateDate" : "2021-10-22 16:29",                      // 更新日期 | 限制：-  | 必填 {date}                                 
            },
            "timestamp" : 123456789  // 請求時間 | 限制：-  | 必填 {date}  毫秒
        }
    ```

* Situation（情況）：
    | code | message | detail |
    | :--- | :--- | :--- |
    | 200（成功） | successButNoContent | "修改成功！" |
    | 400（失敗） | errorToNotFilled | "名稱欄位尚未填寫！" |
    | 400（失敗） | errorToBadRequest | "名稱欄位長度輸入超過限制（20個字）。" |
    | 400（失敗） | errorToBadRequest | "簡述欄位長度輸入超過限制（40個字）。" |
    | 400（失敗） | errorToNotFilled | "排序欄位尚未填寫！" |
    | 400（失敗） | errorToNotFilled | "狀態欄位尚未填寫！" |
    | 401（失敗） | errorToUnauthorized | "用戶端沒有認證憑據，伺服器拒絕回應、存取。" |
    | 404（失敗） | errorToNotFound | "請求失敗，請求的資訊、資源不存在。" |
* Response（回應）：

    ```example
        請求成功（200）
        {
            "code" : "200",                     // 狀態代碼 | 限制：-  | 必填 {string}
            "message" : "successButNoContent",  // 狀態訊息 | 限制：-  | 必填 {string}
            "detail" : "修改成功！",             // 狀態說明 | 限制：-  | 選填 {string}
            "data" : ""                         // 回傳內容 | 限制：-  | 選填 {JSON}{array}  無內容可空值                           
            "timestamp" : 123456789,            // 回傳時間 | 限制：-  | 必填 {date}         毫秒
        }
    ```

    ```example
        請求失敗（）
        {
            "code" : "401",                                        // 狀態代碼 | 限制：-  | 必填 {string}
            "message" : "errorToUnauthorized",                     // 狀態訊息 | 限制：-  | 必填 {string}
            "detail" : "用戶端沒有認證憑據，伺服器拒絕回應、存取。",  // 狀態說明 | 限制：-  | 必填 {string}
            "data" : "",                                           // 回傳內容 | 限制：-  | 選填 {JSON}{array}  無內容可空值
            "timestamp" : 123456789,                               // 回傳時間 | 限制：-  | 必填 {date}         毫秒
        }
    ```

##### 刪除活動目錄資訊

* Route：/activity/category/{categoryId}
* Method：DELETE
* Input（請求）：
  
    ```example
        {
            "data" : {
                "categoryId" : "f84a4543-98c8-422c-8590-753e719e16c0",  // 目錄編號 | 限制：-  | 必填 {string}  目錄類別另設 ID 唯一索引用                               
            },
            "timestamp" : 123456789  // 請求時間 | 限制：-  | 必填 {date}  毫秒
        }
        註：可刪除已有活動的目錄（活動的目錄更新為「所有目錄」）。
    ```

* Situation（情況）：
    | code | message | detail |
    | :--- | :--- | :--- |
    | 200（成功） | successButNoContent | "刪除成功！" |
    | 401（失敗） | errorToUnauthorized | "用戶端沒有認證憑據，伺服器拒絕回應、存取。" |
    | 404（失敗） | errorToNotFound | "請求失敗，請求的資訊、資源不存在。" |
* Response（回應）：

    ```example
        請求成功（200）
        {
            "code" : "200",                     // 狀態代碼 | 限制：-  | 必填 {string}
            "message" : "successButNoContent",  // 狀態訊息 | 限制：-  | 必填 {string}
            "detail" : "刪除成功！",             // 狀態說明 | 限制：-  | 選填 {string}
            "data" : ""                         // 回傳內容 | 限制：-  | 選填 {JSON}{array}  無內容可空值                           
            "timestamp" : 123456789,            // 回傳時間 | 限制：-  | 必填 {date}         毫秒
        }
    ```

    ```example
        請求失敗（）
        {
            "code" : "401",                                        // 狀態代碼 | 限制：-  | 必填 {string}
            "message" : "errorToUnauthorized",                     // 狀態訊息 | 限制：-  | 必填 {string}
            "detail" : "用戶端沒有認證憑據，伺服器拒絕回應、存取。",  // 狀態說明 | 限制：-  | 必填 {string}
            "data" : "",                                           // 回傳內容 | 限制：-  | 選填 {JSON}{array}  無內容可空值
            "timestamp" : 123456789,                               // 回傳時間 | 限制：-  | 必填 {date}         毫秒
        }
    ```

#### - 活動內容

##### 取得活動資訊

* Route：/activity/{activityId}
* Method：GET
* Input（請求）：
  
    ```example
        {
            "data" : {
                "activityId" : "f84a4543-98c8-422c-8590-753e719e16c0",  // 活動編號 | 限制：-  | 選填 {string}  活動類別另設 ID 唯一索引用    
            },
            "timestamp" : 123456789  // 請求時間 | 限制：-  | 必填 {date}  毫秒
        }
        註：不傳送 activityId 則回傳全部活動。
        註：取得全部活動不傳送活動內容欄位，傳送活動簡述欄位。
    ```

* Situation（情況）：
    | code | message | detail |
    | :--- | :--- | :--- |
    | 200（成功） | successToGetContent | "" |
    | 404（失敗） | errorToNotFound | "請求失敗，請求的資訊、資源不存在。" |
* Response（回應）：

    ```example
        請求成功（200）
        {
            "code" : "200",                     // 狀態代碼 | 限制：-  | 必填 {string}
            "message" : "successToGetContent",  // 狀態訊息 | 限制：-  | 必填 {string}
            "detail" : "",                      // 狀態說明 | 限制：-  | 選填 {string}
            "data" : {                          // 回傳內容 | 限制：-  | 選填 {JSON}{array}  無內容可空值
                "id" : 1,                                               // 流水號編號 | 限制：-  | 必填 {int}    流水號，資料庫排序、索引用
                "activityId" : "f84a4543-98c8-422c-8590-753e719e16c0",  // 活動編號 | 限制：-  | 必填 {string}   活動類別另設 ID 唯一索引用
                "categoryId" : "f84a4543-98c8-422c-8590-753e719e16c0",  // 目錄編號 | 限制：-  | 必填 {string}   活動隸屬目錄 ID
                "categoryName" : "活動目錄",                            // 目錄名稱 | 限制：-  | 必填 {string}   活動隸屬目錄名稱             
                "imageURL" : "img/activity/20211102/123.png",           // 縮圖位置 | 限制：-  | 必填 {JSON}     傳送格式 = img / 存放圖片的資料夾 / 個別資料夾 / 圖片名稱.圖片格式 
                "name" : "...",                                         // 活動標題 | 限制：40 | 必填 {string}
                "video" : "https://youtube.com.tw/123",                 // 影片網址 | 限制：-  | 選填 {string}   活動影片網址，以 YOUTUBE 影片崁入式                   
                "brief" : "...",                                        // 活動簡述 | 限制：100 | 必填 {string}
                "content" : {...},                                      // 活動內容 | 限制：-  | 必填 {JSON}     Delta 物件，類似 JSON 格式的物件（註：取得全部不傳送此欄位或內容為 null。）
                "first" : 0,                                            // 活動置頂 | 限制：-  | 必填 {int}}     置頂狀態（0 關閉；1 開啟）
                "sort" : 0,                                             // 排序     | 限制：-  | 必填 {int}      排序，預設為流水號編號（從編號 1 號開始；0 為最大）                                  
                "enabled" : 1,                                          // 活動狀態 | 限制：-  | 必填 {int}      啟用狀態（0 關閉；1 開啟）
                "startDate" : "2021-10-22 16:29",                       // 上架日期 | 限制：-  | 選填 {date}     設定上架日期，上架前啟用狀態為關閉（如果設定為開啟則改為關閉）
                "endDate" : " "2021-10-22 16:29",                       // 下架日期 | 限制：-  | 選填 {date}     設定下架日期，下架前啟用狀態為開啟（如果更新為關閉則清除下架日期改為空值）
                "createDate" : "2021-10-22 16:29",                      // 建立日期 | 限制：-  | 必填 {date}
                "updateDate" : "2021-10-22 16:29",                      // 更新日期 | 限制：-  | 必填 {date}                                 
            },                               
            "timestamp" : 123456789,            // 回傳時間 | 限制：-  | 必填 {date}         毫秒
        }
        註：活動縮圖位置與活動內容中的圖片位置應相同。
        註：排序功能，除非有使用要求，否則一般不作使用（直接由前端帶預設值，不由使用者設置）。
    ```

    ```example
        請求失敗（）
        {
            "code" : "404",                                 // 狀態代碼 | 限制：-  | 必填 {string}
            "message" : "errorToNotFound",                  // 狀態訊息 | 限制：-  | 必填 {string}
            "detail" : "請求失敗，請求的資訊、資源不存在。",  // 狀態說明 | 限制：-  | 必填 {string}
            "data" : "",                                    // 回傳內容 | 限制：-  | 選填 {JSON}{array}  無內容可空值
            "timestamp" : 123456789,                        // 回傳時間 | 限制：-  | 必填 {date}         毫秒
        }
    ```

##### 取得篩選活動資訊（分頁器）

* Route：/activity/query?{categoryId=""}&{count=""}&{page=""}
* Method：GET
* Input（請求）：
  
    ```example
        {
            "data" : {
                "categoryId" : "f84a4543-98c8-422c-8590-753e719e16c0",  // 目錄編號 | 限制：-  | 選填 {string}  活動隸屬目錄 ID
                "count" : "10",                                         // 渲染筆數 | 限制：-  | 選填 {int}     每頁一次渲染的消息筆數    
                "page" : "1",                                           // 當前頁碼 | 限制：-  | 選填 {int}     目前的頁碼        
            },
            "timestamp" : 123456789  // 請求時間 | 限制：-  | 必填 {date}  毫秒
        }
        註：條件須都帶上，可不填值（不填值則回傳全部）。
    ```

* Situation（情況）：
    | code | message | detail |
    | :--- | :--- | :--- |
    | 200（成功） | successToGetContent | "" |
    | 404（失敗） | errorToNotFound | "請求失敗，請求的資訊、資源不存在。" |
* Response（回應）：

    ```example
        請求成功（200）
        {
            "code" : "200",                     // 狀態代碼 | 限制：-  | 必填 {string}
            "message" : "successToGetContent",  // 狀態訊息 | 限制：-  | 必填 {string}
            "detail" : "",                      // 狀態說明 | 限制：-  | 選填 {string}
            "data" : {                          // 回傳內容 | 限制：-  | 選填 {JSON}{array}  無內容可空值
                "id" : 1,                                               // 流水號編號 | 限制：-  | 必填 {int}    流水號，資料庫排序、索引用
                "activityId" : "f84a4543-98c8-422c-8590-753e719e16c0",  // 活動編號 | 限制：-  | 必填 {string}   活動類別另設 ID 唯一索引用
                "categoryId" : "f84a4543-98c8-422c-8590-753e719e16c0",  // 目錄編號 | 限制：-  | 必填 {string}   活動隸屬目錄 ID
                "categoryName" : "活動目錄",                            // 目錄名稱 | 限制：-  | 必填 {string}   活動隸屬目錄名稱             
                "imageURL" : "img/activity/20211102/123.png",           // 縮圖位置 | 限制：-  | 必填 {JSON}     傳送格式 = img / 存放圖片的資料夾 / 個別資料夾 / 圖片名稱.圖片格式 
                "name" : "...",                                         // 活動標題 | 限制：40 | 必填 {string}
                "video" : "https://youtube.com.tw/123",                 // 影片網址 | 限制：-  | 選填 {string}   活動影片網址，以 YOUTUBE 影片崁入式                   
                "brief" : "...",                                        // 活動簡述 | 限制：100 | 必填 {string}
                "content" : {...},                                      // 活動內容 | 限制：-  | 必填 {JSON}     Delta 物件，類似 JSON 格式的物件（註：取得全部不傳送此欄位或內容為 null。）
                "first" : 0,                                            // 活動置頂 | 限制：-  | 必填 {int}}     置頂狀態（0 關閉；1 開啟）
                "sort" : 0,                                             // 排序     | 限制：-  | 必填 {int}      排序，預設為流水號編號（從編號 1 號開始；0 為最大）                                  
                "enabled" : 1,                                          // 活動狀態 | 限制：-  | 必填 {int}      啟用狀態（0 關閉；1 開啟）
                "startDate" : "2021-10-22 16:29",                       // 上架日期 | 限制：-  | 選填 {date}     設定上架日期，上架前啟用狀態為關閉（如果設定為開啟則改為關閉）
                "endDate" : " "2021-10-22 16:29",                       // 下架日期 | 限制：-  | 選填 {date}     設定下架日期，下架前啟用狀態為開啟（如果更新為關閉則清除下架日期改為空值）
                "createDate" : "2021-10-22 16:29",                      // 建立日期 | 限制：-  | 必填 {date}
                "updateDate" : "2021-10-22 16:29",                      // 更新日期 | 限制：-  | 必填 {date}                                 
            },                               
            "timestamp" : 123456789,            // 回傳時間 | 限制：-  | 必填 {date}         毫秒
        }
        註：活動縮圖位置與活動內容中的圖片位置應相同。
        註：排序功能，除非有使用要求，否則一般不作使用（直接由前端帶預設值，不由使用者設置）。
    ```

    ```example
        請求失敗（）
        {
            "code" : "404",                                 // 狀態代碼 | 限制：-  | 必填 {string}
            "message" : "errorToNotFound",                  // 狀態訊息 | 限制：-  | 必填 {string}
            "detail" : "請求失敗，請求的資訊、資源不存在。",  // 狀態說明 | 限制：-  | 必填 {string}
            "data" : "",                                    // 回傳內容 | 限制：-  | 選填 {JSON}{array}  無內容可空值
            "timestamp" : 123456789,                        // 回傳時間 | 限制：-  | 必填 {date}         毫秒
        }
    ```

##### 新增活動資訊

* Route：/activity
* Method：POST
* Input（請求）：
  
    ```example
        {
            "data" : {
                "file" : file,                                          // 活動縮圖 | 限制：-  | 必填 {file}     要上傳的圖片檔案
                "name" : "123.png",                                     // 活動標題 | 限制：40 | 必填 {string}
                "categoryId" : "f84a4543-98c8-422c-8590-753e719e16c0",  // 目錄編號 | 限制：-  | 必填 {string}   活動隸屬目錄 ID             
                "brief" : "...",                                        // 活動簡述 | 限制：100 | 必填 {string}                   
                "video" : "https://youtube.com.tw/123",                 // 影片網址 | 限制：-  | 選填 {string}   活動影片網址，以 YOUTUBE 影片崁入式                   
                "content" : {...},                                      // 活動內容 | 限制：-  | 必填 {JSON}     Delta 物件，類似 JSON 格式的物件
                "cNameArr" : [...],                                     // 圖片集合 | 限制：-  | 必填 {array}    編輯器中插入的圖片名稱集合，用於刪除被取代的圖片；無值（無圖片）則空陣列。
                "first" : 0,                                            // 活動置頂 | 限制：-  | 必填 {int}}     置頂狀態（0 關閉；1 開啟）
                "enabled" : 1,                                          // 活動狀態 | 限制：-  | 必填 {int}      啟用狀態（0 關閉；1 開啟）
                "startDate" : "2021-10-22 16:29",                       // 上架日期 | 限制：-  | 選填 {date}     設定上架日期，上架前啟用狀態為關閉（如果設定為開啟則改為關閉）
                "endDate" : " "2021-10-22 16:29",                       // 下架日期 | 限制：-  | 選填 {date}     設定下架日期，下架前啟用狀態為開啟（如果更新為關閉則清除下架日期改為空值）
                "createDate" : "2021-10-22 16:29",                      // 建立日期 | 限制：-  | 必填 {date}
                "updateDate" : "2021-10-22 16:29",                      // 更新日期 | 限制：-  | 必填 {date}  
            },
            "timestamp" : 123456789  // 請求時間 | 限制：-  | 必填 {date}  毫秒
        }
        註：排序以預設流水號方式由後端直接產生，前端新增不另帶
    ```

* Situation（情況）：
    | code | message | detail |
    | :--- | :--- | :--- |
    | 201（成功） | successToCreated | "新增成功！" |
    | 400（失敗） | errorToNotFilled | "尚未選擇您要上傳的消息縮圖！" |
    | 400（失敗） | errorToBadRequest | "請上傳正確的圖片檔案格式（格式：JPG、PNG 等圖片格式）！" |
    | 400（失敗） | errorToNotFilled | "標題欄位尚未填寫！" |
    | 400（失敗） | errorToBadRequest | "標題欄位長度輸入超過限制（40個字）！" |
    | 400（失敗） | errorToNotFilled | "目錄欄位尚未填寫！" |
    | 400（失敗） | errorToNotFilled | "簡述欄位尚未填寫！" |
    | 400（失敗） | errorToBadRequest | "簡述欄位長度輸入超過限制（100個字）！" |
    | 400（失敗） | errorToNotFilled | "內容欄位尚未填寫！" |
    | 400（失敗） | errorToNotFilled | "置頂欄位尚未填寫！" |
    | 400（失敗） | errorToNotFilled | "狀態欄位尚未填寫！" |
    | 400（失敗） | errorToBadRequest | "上架日期欄位格式輸入錯誤（不得設定過去時間）！" |
    | 400（失敗） | errorToBadRequest | "上架日期欄位格式輸入錯誤（上架時間不得大於下架時間）！" |
    | 400（失敗） | errorToBadRequest | "下架日期欄位格式輸入錯誤（不得設定過去時間）！" |
    | 400（失敗） | errorToBadRequest | "下架日期欄位格式輸入錯誤（下架時間不得小於上架時間）！" |
    | 401（失敗） | errorToUnauthorized | "用戶端沒有認證憑據，伺服器拒絕回應、存取。" |
* Response（回應）：

    ```example
        請求成功（201）
        {
            "code" : "201",                  // 狀態代碼 | 限制：-  | 必填 {string}
            "message" : "successToCreated",  // 狀態訊息 | 限制：-  | 必填 {string}
            "detail" : "新增成功！",          // 狀態說明 | 限制：-  | 選填 {string}
            "data" : {                       // 回傳內容 | 限制：-  | 選填 {JSON}{array}  無內容可空值
                "id" : 1,                                               // 編號流水號 | 限制：-  | 必填 {int}   流水號，資料庫排序、索引用
                "activityId" : "f84a4543-98c8-422c-8590-753e719e16c0",  // 活動編號 | 限制：-  | 必填 {string}  活動類別另設 ID 唯一索引用
                "categoryId" : "f84a4543-98c8-422c-8590-753e719e16c0",  // 目錄編號 | 限制：-  | 必填 {string}  活動隸屬目錄 ID             
                "first" : 0,                                            // 消息置頂 | 限制：-  | 必填 {int}}    置頂狀態（0 關閉；1 開啟）
                "sort" : 0,                                             // 排序     | 限制：-  | 必填 {int}     排序，預設為流水號編號（從編號 1 號開始；0 為最大）                                  
                "enabled" : 1,                                          // 消息狀態 | 限制：-  | 必填 {int}     啟用狀態（0 關閉；1 開啟）
                "startDate" : "2021-10-22 16:29",                       // 上架日期 | 限制：-  | 選填 {date}    設定上架日期，上架前啟用狀態為關閉（如果設定為開啟則改為關閉）
                "endDate" : " "2021-10-22 16:29",                       // 下架日期 | 限制：-  | 選填 {date}    設定下架日期，下架前啟用狀態為開啟（如果更新為關閉則清除下架日期改為空值）
                "createDate" : "2021-10-22 16:29",                      // 建立日期 | 限制：-  | 必填 {date}              
            },                               
            "timestamp" : 123456789,         // 回傳時間 | 限制：-  | 必填 {date}         毫秒
        }
    ```

    ```example
        請求失敗（）
        {
            "code" : "400",                           // 狀態代碼 | 限制：-  | 必填 {string}
            "message" : "errorToNotFilled",           // 狀態訊息 | 限制：-  | 必填 {string}
            "detail" : "尚未選擇您要上傳的輪播圖片！",  // 狀態說明 | 限制：-  | 必填 {string}
            "data" : "",                              // 回傳內容 | 限制：-  | 選填 {JSON}{array}  無內容可空值
            "timestamp" : 123456789,                  // 回傳時間 | 限制：-  | 必填 {date}         毫秒
        }
    ```

##### 更新活動資訊

* Route：/activity/{activityId}
* Method：PUT
* Input（請求）：
  
    ```example
        {
            "data" : {
                "activityId" : "f84a4543-98c8-422c-8590-753e719e16c0",  // 活動編號 | 限制：-  | 必填 {string}   活動類別另設 ID 唯一索引用
                "categoryId" : "f84a4543-98c8-422c-8590-753e719e16c0",  // 目錄編號 | 限制：-  | 必填 {string}   活動隸屬目錄 ID
                "file" : file,                                          // 活動縮圖 | 限制：-  | 選填 {file}     要上傳的圖片檔案（如果傳送的是空值則為不修改；如果傳送新的檔案則修改；）           
                "name" : "...",                                         // 活動標題 | 限制：40 | 必填 {string}
                "video" : "https://youtube.com.tw/123",                 // 影片網址 | 限制：-  | 選填 {string}   活動影片網址，以 YOUTUBE 影片崁入式                   
                "brief" : "...",                                        // 活動簡述 | 限制：100 | 必填 {string}
                "content" : {...},                                      // 活動內容 | 限制：-  | 必填 {JSON}     Delta 物件，類似 JSON 格式的物件（註：取得全部不傳送此欄位或內容為 null。）
                "first" : 0,                                            // 活動置頂 | 限制：-  | 必填 {int}}     置頂狀態（0 關閉；1 開啟）
                "sort" : 0,                                             // 排序     | 限制：-  | 必填 {int}      排序，預設為流水號編號（從編號 1 號開始；0 為最大）                                  
                "enabled" : 1,                                          // 活動狀態 | 限制：-  | 必填 {int}      啟用狀態（0 關閉；1 開啟）
                "startDate" : "2021-10-22 16:29",                       // 上架日期 | 限制：-  | 選填 {date}     設定上架日期，上架前啟用狀態為關閉（如果設定為開啟則改為關閉）
                "endDate" : " "2021-10-22 16:29",                       // 下架日期 | 限制：-  | 選填 {date}     設定下架日期，下架前啟用狀態為開啟（如果更新為關閉則清除下架日期改為空值）
                "updateDate" : "2021-10-22 16:29",                      // 更新日期 | 限制：-  | 必填 {date}                                 
            },
            "timestamp" : 123456789  // 請求時間 | 限制：-  | 必填 {date}  毫秒
        }
        註：排序功能，除非有使用要求，否則一般不作使用（直接由前端帶預設值，不由使用者設置）。
    ```

* Situation（情況）：
    | code | message | detail |
    | :--- | :--- | :--- |
    | 200（成功） | successButNoContent | "修改成功！" |
    | 400（失敗） | errorToBadRequest | "請上傳正確的圖片檔案格式（格式：JPG、PNG 等圖片格式）！" |
    | 400（失敗） | errorToNotFilled | "標題欄位尚未填寫！" |
    | 400（失敗） | errorToBadRequest | "標題欄位長度輸入超過限制（40個字）！" |
    | 400（失敗） | errorToNotFilled | "目錄欄位尚未填寫！" |
    | 400（失敗） | errorToNotFilled | "簡述欄位尚未填寫！" |
    | 400（失敗） | errorToBadRequest | "簡述欄位長度輸入超過限制（100個字）！" |
    | 400（失敗） | errorToNotFilled | "內容欄位尚未填寫！" |
    | 400（失敗） | errorToNotFilled | "置頂欄位尚未填寫！" |
    | 400（失敗） | errorToNotFilled | "狀態欄位尚未填寫！" |
    | 400（失敗） | errorToBadRequest | "上架日期欄位格式輸入錯誤（不得設定過去時間）！" |
    | 400（失敗） | errorToBadRequest | "上架日期欄位格式輸入錯誤（上架時間不得大於下架時間）！" |
    | 400（失敗） | errorToBadRequest | "下架日期欄位格式輸入錯誤（不得設定過去時間）！" |
    | 400（失敗） | errorToBadRequest | "上架日期欄位格式輸入錯誤（下架時間不得小於上架時間）！" |
    | 401（失敗） | errorToUnauthorized | "用戶端沒有認證憑據，伺服器拒絕回應、存取。" |
    | 404（失敗） | errorToNotFound | "請求失敗，請求的資訊、資源不存在。" |
* Response（回應）：

    ```example
        請求成功（200）
        {
            "code" : "200",                     // 狀態代碼 | 限制：-  | 必填 {string}
            "message" : "successButNoContent",  // 狀態訊息 | 限制：-  | 必填 {string}
            "detail" : "修改成功！",             // 狀態說明 | 限制：-  | 選填 {string}
            "data" : ""                         // 回傳內容 | 限制：-  | 選填 {JSON}{array}  無內容可空值                           
            "timestamp" : 123456789,            // 回傳時間 | 限制：-  | 必填 {date}         毫秒
        }
    ```

    ```example
        請求失敗（）
        {
            "code" : "401",                                        // 狀態代碼 | 限制：-  | 必填 {string}
            "message" : "errorToUnauthorized",                     // 狀態訊息 | 限制：-  | 必填 {string}
            "detail" : "用戶端沒有認證憑據，伺服器拒絕回應、存取。",  // 狀態說明 | 限制：-  | 必填 {string}
            "data" : "",                                           // 回傳內容 | 限制：-  | 選填 {JSON}{array}  無內容可空值
            "timestamp" : 123456789,                               // 回傳時間 | 限制：-  | 必填 {date}         毫秒
        }
    ```

##### 新增（修改）活動圖片儲存方式

* Route：activity/image
* Method：POST
* Input（請求）：

    ```example
        {
            "data" : {
                "file" : file,  // 圖片檔案 | 限制：-  | 必填 {file}  要上傳的圖片檔案
            },
            "timestamp" : 123456789  // 請求時間 | 限制：-  | 必填 {date}  秒
        }
    ```

* Situation（情況）：
    | code | message | detail |
    | :--- | :--- | :--- |
    | 201（成功） | successToCreated | "新增成功！" |
    | 400（失敗） | errorToBadRequest | "請上傳正確的圖片檔案格式（格式：JPG、PNG 等圖片格式）！" |
    | 401（失敗） | errorToUnauthorized | "用戶端沒有認證憑據，伺服器拒絕回應、存取。" |
* Response（回應）：

    ```example
        請求成功（201）
        {
            "code" : "201",                  // 狀態代碼 | 限制：-  | 必填 {string}
            "message" : "successToCreated",  // 狀態訊息 | 限制：-  | 必填 {string}
            "detail" : "新增成功！",          // 狀態說明 | 限制：-  | 選填 {string}
            "data" : {                       // 回傳內容 | 限制：-  | 選填 {JSON}{array}  無內容可空值 
                "id" : 1,                                            // 流水號編號 | 限制：-  | 必填 {int}   流水號，資料庫排序、索引用
                "imageId" : "f84a4543-98c8-422c-8590-753e719e16c0",  // 圖片編號 | 限制：-  | 必填 {string}  圖片類別另設 ID 唯一索引用
                "imageURL" : "img/banners/123.png",                  // 圖片位置 | 限制：-  | 必填 {JSON}    傳送格式 = img / 存放圖片的資料夾 / 個別資料夾 / 圖片名稱.圖片格式
            },                          
            "timestamp" : 123456789,         // 回傳時間 | 限制：-  | 必填 {date}         毫秒
        }
    ```

    ```example
        請求失敗（）
        {
            "code" : "400",                                                     // 狀態代碼 | 限制：-  | 必填 {string}
            "message" : "errorToBadRequest",                                    // 狀態訊息 | 限制：-  | 必填 {string}
            "detail" : "請上傳正確的圖片檔案格式（格式：JPG、PNG 等圖片格式）！",  // 狀態說明 | 限制：-  | 必填 {string}
            "data" : "",                                                        // 回傳內容 | 限制：-  | 選填 {JSON}{array}  無內容可空值
            "timestamp" : 123456789,                                            // 回傳時間 | 限制：-  | 必填 {date}         毫秒
        }

##### 刪除活動資訊

* Route：/activity/{activityId}
* Method：DELETE
* Input（請求）：
  
    ```example
        {
            "data" : {
                "activityId" : "f84a4543-98c8-422c-8590-753e719e16c0",  // 活動編號 | 限制：-  | 必填 {string}  活動類別另設 ID 唯一索引用                               
            },
            "timestamp" : 123456789  // 請求時間 | 限制：-  | 必填 {date}  毫秒
        }
    ```

* Situation（情況）：
    | code | message | detail |
    | :--- | :--- | :--- |
    | 200（成功） | successButNoContent | "刪除成功！" |
    | 401（失敗） | errorToUnauthorized | "用戶端沒有認證憑據，伺服器拒絕回應、存取。" |
    | 404（失敗） | errorToNotFound | "請求失敗，請求的資訊、資源不存在。" |
* Response（回應）：

    ```example
        請求成功（200）
        {
            "code" : "200",                     // 狀態代碼 | 限制：-  | 必填 {string}
            "message" : "successButNoContent",  // 狀態訊息 | 限制：-  | 必填 {string}
            "detail" : "刪除成功！",             // 狀態說明 | 限制：-  | 選填 {string}
            "data" : ""                         // 回傳內容 | 限制：-  | 選填 {JSON}{array}  無內容可空值                           
            "timestamp" : 123456789,            // 回傳時間 | 限制：-  | 必填 {date}         毫秒
        }
    ```

    ```example
        請求失敗（）
        {
            "code" : "401",                                        // 狀態代碼 | 限制：-  | 必填 {string}
            "message" : "errorToUnauthorized",                     // 狀態訊息 | 限制：-  | 必填 {string}
            "detail" : "用戶端沒有認證憑據，伺服器拒絕回應、存取。",  // 狀態說明 | 限制：-  | 必填 {string}
            "data" : "",                                           // 回傳內容 | 限制：-  | 選填 {JSON}{array}  無內容可空值
            "timestamp" : 123456789,                               // 回傳時間 | 限制：-  | 必填 {date}         毫秒
        }
    ```

### 文章管理（有目錄）

#### - 文章目錄

##### 取得文章目錄資訊

* Route：/article/category/{categoryId}
* Method：GET
* Input（請求）：
  
    ```example
        {
            "data" : {
                "categoryId" : "f84a4543-98c8-422c-8590-753e719e16c0",  // 目錄編號 | 限制：-  | 選填 {string}  目錄類別另設 ID 唯一索引用    
            },
            "timestamp" : 123456789  // 請求時間 | 限制：-  | 必填 {date}  毫秒
        }
        註：不傳送 categoryId 則回傳全部目錄。
        註：預設一個「所有目錄」且不得刪除。 
    ```

* Situation（情況）：
    | code | message | detail |
    | :--- | :--- | :--- |
    | 200（成功） | successToGetContent | "" |
    | 404（失敗） | errorToNotFound | "請求失敗，請求的資訊、資源不存在。" |
* Response（回應）：

    ```example
        請求成功（200）
        {
            "code" : "200",                     // 狀態代碼 | 限制：-  | 必填 {string}
            "message" : "successToGetContent",  // 狀態訊息 | 限制：-  | 必填 {string}
            "detail" : "",                      // 狀態說明 | 限制：-  | 選填 {string}
            "data" : {                          // 回傳內容 | 限制：-  | 選填 {JSON}{array}  無內容可空值
                "id" : 1,                                               // 流水號編號 | 限制：-  | 必填 {int}   流水號，資料庫排序、索引用
                "categoryId" : "f84a4543-98c8-422c-8590-753e719e16c0",  // 目錄編號 | 限制：-  | 必填 {string}  目錄類別另設 ID 唯一索引用
                "name" : "文章目錄",                                    // 目錄名稱 | 限制：20 | 必填 {string}
                "berif" : "...",                                        // 目錄簡述 | 限制：40 | 選填 {string}  目錄的說明簡述，可為空值或 null
                "sort" : 0,                                             // 目錄排序 | 限制：-  | 必填 {int}     排序，預設可為流水號編號（從編號 1 號開始；設為 0 即為為置頂）                
                "enabled" : 1,                                          // 目錄狀態 | 限制：-  | 必填 {int}     啟用狀態（0 關閉；1 開啟）
                "createDate" : "2021-10-22 16:29",                      // 建立日期 | 限制：-  | 必填 {date}
                "updateDate" : "2021-10-22 16:29",                      // 更新日期 | 限制：-  | 必填 {date}                                 
            },                               
            "timestamp" : 123456789,            // 回傳時間 | 限制：-  | 必填 {date}         毫秒
        }
    ```

    ```example
        請求失敗（）
        {
            "code" : "404",                                 // 狀態代碼 | 限制：-  | 必填 {string}
            "message" : "errorToNotFound",                  // 狀態訊息 | 限制：-  | 必填 {string}
            "detail" : "請求失敗，請求的資訊、資源不存在。",  // 狀態說明 | 限制：-  | 必填 {string}
            "data" : "",                                    // 回傳內容 | 限制：-  | 選填 {JSON}{array}  無內容可空值
            "timestamp" : 123456789,                        // 回傳時間 | 限制：-  | 必填 {date}         毫秒
        }
    ```

##### 新增文章目錄資訊

* Route：/article/category
* Method：POST
* Input（請求）：
  
    ```example
        {
            "data" : {
                "name" : "文章目錄",                // 目錄名稱 | 限制：20 | 必填 {string}
                "berif" : "...",                    // 目錄簡述 | 限制：40 | 選填 {string}  目錄的說明簡述，可為空值或 null
                "sort" : 0,                         // 目錄排序 | 限制：-  | 必填 {int}     排序，預設可為流水號編號（從編號 1 號開始；設為 0 即為為置頂）                
                "enabled" : 1,                      // 目錄狀態 | 限制：-  | 必填 {int}     啟用狀態（0 關閉；1 開啟）
                "createDate" : "2021-10-22 16:29",  // 建立日期 | 限制：-  | 必填 {date}
            },
            "timestamp" : 123456789  // 請求時間 | 限制：-  | 必填 {date}  毫秒
        }
    ```

* Situation（情況）：
    | code | message | detail |
    | :--- | :--- | :--- |
    | 201（成功） | successToCreated | "新增成功！" |
    | 400（失敗） | errorToNotFilled | "名稱欄位尚未填寫！" |
    | 400（失敗） | errorToBadRequest | "名稱欄位長度輸入超過限制（20個字）。" |
    | 400（失敗） | errorToBadRequest | "簡述欄位長度輸入超過限制（40個字）。" |
    | 400（失敗） | errorToNotFilled | "排序欄位尚未填寫！" |
    | 400（失敗） | errorToNotFilled | "狀態欄位尚未填寫！" |
    | 401（失敗） | errorToUnauthorized | "用戶端沒有認證憑據，伺服器拒絕回應、存取。" |
* Response（回應）：

    ```example
        請求成功（201）
        {
            "code" : "201",                  // 狀態代碼 | 限制：-  | 必填 {string}
            "message" : "successToCreated",  // 狀態訊息 | 限制：-  | 必填 {string}
            "detail" : "新增成功！",          // 狀態說明 | 限制：-  | 選填 {string}
            "data" : {                       // 回傳內容 | 限制：-  | 選填 {JSON}{array}  無內容可空值
                "id" : 1,                                               // 編號流水號 | 限制：-  | 必填 {int}   流水號，資料庫排序、索引用
                "categoryId" : "f84a4543-98c8-422c-8590-753e719e16c0",  // 目錄編號 | 限制：-  | 必填 {string}  消息類別另設 ID 唯一索引用
                "sort" : 0,                                             // 目錄排序 | 限制：-  | 選填 {int}     排序，預設可為流水號編號（從編號 1 號開始；設為 0 即為為置頂）                
                "enabled" : 1,                                          // 消息狀態 | 限制：-  | 必填 {int}     啟用狀態（0 關閉；1 開啟）
                "createDate" : "2021-10-22 16:29",                      // 建立日期 | 限制：-  | 必填 {date}              
            },                               
            "timestamp" : 123456789,         // 回傳時間 | 限制：-  | 必填 {date}         毫秒
        }
    ```

    ```example
        請求失敗（）
        {
            "code" : "400",                  // 狀態代碼 | 限制：-  | 必填 {string}
            "message" : "errorToNotFilled",  // 狀態訊息 | 限制：-  | 必填 {string}
            "detail" : "名稱欄位尚未填寫！",  // 狀態說明 | 限制：-  | 必填 {string}
            "data" : "",                     // 回傳內容 | 限制：-  | 選填 {JSON}{array}  無內容可空值
            "timestamp" : 123456789,         // 回傳時間 | 限制：-  | 必填 {date}         毫秒
        }
    ```

##### 更新文章目錄資訊

* Route：/article/category/{categoryId}
* Method：PUT
* Input（請求）：
  
    ```example
        {
            "data" : {
                "categoryId" : "f84a4543-98c8-422c-8590-753e719e16c0",  // 目錄編號 | 限制：-  | 必填 {string}  目錄類別另設 ID 唯一索引用
                "name" : "活動目錄",                                    // 目錄名稱 | 限制：20 | 必填 {string}
                "berif" : "...",                                        // 目錄簡述 | 限制：40 | 選填 {string}  目錄的說明簡述，可為空值或 null
                "sort" : 0,                                             // 目錄排序 | 限制：-  | 必填 {int}     排序，預設可為流水號編號（從編號 1 號開始；設為 0 即為為置頂）                
                "enabled" : 1,                                          // 目錄狀態 | 限制：-  | 必填 {int}     啟用狀態（0 關閉；1 開啟）
                "updateDate" : "2021-10-22 16:29",                      // 更新日期 | 限制：-  | 必填 {date}                                 
            },
            "timestamp" : 123456789  // 請求時間 | 限制：-  | 必填 {date}  毫秒
        }
    ```

* Situation（情況）：
    | code | message | detail |
    | :--- | :--- | :--- |
    | 200（成功） | successButNoContent | "修改成功！" |
    | 400（失敗） | errorToNotFilled | "名稱欄位尚未填寫！" |
    | 400（失敗） | errorToBadRequest | "名稱欄位長度輸入超過限制（20個字）。" |
    | 400（失敗） | errorToBadRequest | "簡述欄位長度輸入超過限制（40個字）。" |
    | 400（失敗） | errorToNotFilled | "排序欄位尚未填寫！" |
    | 400（失敗） | errorToNotFilled | "狀態欄位尚未填寫！" |
    | 401（失敗） | errorToUnauthorized | "用戶端沒有認證憑據，伺服器拒絕回應、存取。" |
    | 404（失敗） | errorToNotFound | "請求失敗，請求的資訊、資源不存在。" |
* Response（回應）：

    ```example
        請求成功（200）
        {
            "code" : "200",                     // 狀態代碼 | 限制：-  | 必填 {string}
            "message" : "successButNoContent",  // 狀態訊息 | 限制：-  | 必填 {string}
            "detail" : "修改成功！",             // 狀態說明 | 限制：-  | 選填 {string}
            "data" : ""                         // 回傳內容 | 限制：-  | 選填 {JSON}{array}  無內容可空值                           
            "timestamp" : 123456789,            // 回傳時間 | 限制：-  | 必填 {date}         毫秒
        }
    ```

    ```example
        請求失敗（）
        {
            "code" : "401",                                        // 狀態代碼 | 限制：-  | 必填 {string}
            "message" : "errorToUnauthorized",                     // 狀態訊息 | 限制：-  | 必填 {string}
            "detail" : "用戶端沒有認證憑據，伺服器拒絕回應、存取。",  // 狀態說明 | 限制：-  | 必填 {string}
            "data" : "",                                           // 回傳內容 | 限制：-  | 選填 {JSON}{array}  無內容可空值
            "timestamp" : 123456789,                               // 回傳時間 | 限制：-  | 必填 {date}         毫秒
        }
    ```

##### 刪除文章目錄資訊

* Route：/article/category/{categoryId}
* Method：DELETE
* Input（請求）：
  
    ```example
        {
            "data" : {
                "categoryId" : "f84a4543-98c8-422c-8590-753e719e16c0",  // 目錄編號 | 限制：-  | 必填 {string}  目錄類別另設 ID 唯一索引用                               
            },
            "timestamp" : 123456789  // 請求時間 | 限制：-  | 必填 {date}  毫秒
        }
        註：可刪除已有文章的目錄（文章的目錄更新為「所有目錄」）。
    ```

* Situation（情況）：
    | code | message | detail |
    | :--- | :--- | :--- |
    | 200（成功） | successButNoContent | "刪除成功！" |
    | 401（失敗） | errorToUnauthorized | "用戶端沒有認證憑據，伺服器拒絕回應、存取。" |
    | 404（失敗） | errorToNotFound | "請求失敗，請求的資訊、資源不存在。" |
* Response（回應）：

    ```example
        請求成功（200）
        {
            "code" : "200",                     // 狀態代碼 | 限制：-  | 必填 {string}
            "message" : "successButNoContent",  // 狀態訊息 | 限制：-  | 必填 {string}
            "detail" : "刪除成功！",             // 狀態說明 | 限制：-  | 選填 {string}
            "data" : ""                         // 回傳內容 | 限制：-  | 選填 {JSON}{array}  無內容可空值                           
            "timestamp" : 123456789,            // 回傳時間 | 限制：-  | 必填 {date}         毫秒
        }
    ```

    ```example
        請求失敗（）
        {
            "code" : "401",                                        // 狀態代碼 | 限制：-  | 必填 {string}
            "message" : "errorToUnauthorized",                     // 狀態訊息 | 限制：-  | 必填 {string}
            "detail" : "用戶端沒有認證憑據，伺服器拒絕回應、存取。",  // 狀態說明 | 限制：-  | 必填 {string}
            "data" : "",                                           // 回傳內容 | 限制：-  | 選填 {JSON}{array}  無內容可空值
            "timestamp" : 123456789,                               // 回傳時間 | 限制：-  | 必填 {date}         毫秒
        }
    ```

#### - 文章內容

##### 取得文章資訊

* Route：/article/{articleId}
* Method：GET
* Input（請求）：
  
    ```example
        {
            "data" : {
                "articleId" : "f84a4543-98c8-422c-8590-753e719e16c0",  // 文章編號 | 限制：-  | 選填 {string}  文章類別另設 ID 唯一索引用    
            },
            "timestamp" : 123456789  // 請求時間 | 限制：-  | 必填 {date}  毫秒
        }
        註：不傳送 articleId 則回傳全部文章。
        註：取得全部文章不傳送文章內容欄位，傳送文章簡述欄位。
    ```

* Situation（情況）：
    | code | message | detail |
    | :--- | :--- | :--- |
    | 200（成功） | successToGetContent | "" |
    | 404（失敗） | errorToNotFound | "請求失敗，請求的資訊、資源不存在。" |
* Response（回應）：

    ```example
        請求成功（200）
        {
            "code" : "200",                     // 狀態代碼 | 限制：-  | 必填 {string}
            "message" : "successToGetContent",  // 狀態訊息 | 限制：-  | 必填 {string}
            "detail" : "",                      // 狀態說明 | 限制：-  | 選填 {string}
            "data" : {                          // 回傳內容 | 限制：-  | 選填 {JSON}{array}  無內容可空值
                "id" : 1,                                               // 流水號編號 | 限制：-  | 必填 {int}    流水號，資料庫排序、索引用
                "articleId" : "f84a4543-98c8-422c-8590-753e719e16c0",   // 文章編號 | 限制：-  | 必填 {string}   文章類別另設 ID 唯一索引用
                "categoryId" : "f84a4543-98c8-422c-8590-753e719e16c0",  // 目錄編號 | 限制：-  | 必填 {string}   文章隸屬目錄 ID
                "categoryName" : "文章目錄",                            // 目錄名稱 | 限制：-  | 必填 {string}   文章隸屬目錄名稱             
                "imageURL" : "img/activity/20211102/123.png",           // 縮圖位置 | 限制：-  | 必填 {JSON}     傳送格式 = img / 存放圖片的資料夾 / 個別資料夾 / 圖片名稱.圖片格式 
                "name" : "...",                                         // 文章標題 | 限制：40 | 必填 {string}
                "brief" : "...",                                        // 文章簡述 | 限制：100 | 必填 {string}
                "content" : {...},                                      // 文章內容 | 限制：-  | 必填 {JSON}     Delta 物件，類似 JSON 格式的物件（註：取得全部不傳送此欄位或內容為 null。）
                "first" : 0,                                            // 文章置頂 | 限制：-  | 必填 {int}}     置頂狀態（0 關閉；1 開啟）
                "sort" : 0,                                             // 排序     | 限制：-  | 必填 {int}      排序，預設為流水號編號（從編號 1 號開始；0 為最大）                                  
                "enabled" : 1,                                          // 文章狀態 | 限制：-  | 必填 {int}      啟用狀態（0 關閉；1 開啟）
                "startDate" : "2021-10-22 16:29",                       // 上架日期 | 限制：-  | 選填 {date}     設定上架日期，上架前啟用狀態為關閉（如果設定為開啟則改為關閉）
                "endDate" : " "2021-10-22 16:29",                       // 下架日期 | 限制：-  | 選填 {date}     設定下架日期，下架前啟用狀態為開啟（如果更新為關閉則清除下架日期改為空值）
                "createDate" : "2021-10-22 16:29",                      // 建立日期 | 限制：-  | 必填 {date}
                "updateDate" : "2021-10-22 16:29",                      // 更新日期 | 限制：-  | 必填 {date}                                 
            },                               
            "timestamp" : 123456789,            // 回傳時間 | 限制：-  | 必填 {date}         毫秒
        }
        註：文章縮圖位置與文章內容中的圖片位置應相同。
        註：排序功能，除非有使用要求，否則一般不作使用（直接由前端帶預設值，不由使用者設置）。
    ```

    ```example
        請求失敗（）
        {
            "code" : "404",                                 // 狀態代碼 | 限制：-  | 必填 {string}
            "message" : "errorToNotFound",                  // 狀態訊息 | 限制：-  | 必填 {string}
            "detail" : "請求失敗，請求的資訊、資源不存在。",  // 狀態說明 | 限制：-  | 必填 {string}
            "data" : "",                                    // 回傳內容 | 限制：-  | 選填 {JSON}{array}  無內容可空值
            "timestamp" : 123456789,                        // 回傳時間 | 限制：-  | 必填 {date}         毫秒
        }
    ```

##### 取得篩選文章資訊（分頁器）

* Route：/article/query?{categoryId=""}&{count=""}&{page=""}
* Method：GET
* Input（請求）：
  
    ```example
        {
            "data" : {
                "categoryId" : "f84a4543-98c8-422c-8590-753e719e16c0",  // 目錄編號 | 限制：-  | 選填 {string}  文章隸屬目錄 ID
                "count" : "10",                                         // 渲染筆數 | 限制：-  | 選填 {int}     每頁一次渲染的消息筆數    
                "page" : "1",                                           // 當前頁碼 | 限制：-  | 選填 {int}     目前的頁碼        
            },
            "timestamp" : 123456789  // 請求時間 | 限制：-  | 必填 {date}  毫秒
        }
        註：條件須都帶上，可不填值（不填值則回傳全部）。
    ```

* Situation（情況）：
    | code | message | detail |
    | :--- | :--- | :--- |
    | 200（成功） | successToGetContent | "" |
    | 404（失敗） | errorToNotFound | "請求失敗，請求的資訊、資源不存在。" |
* Response（回應）：

    ```example
        請求成功（200）
        {
            "code" : "200",                     // 狀態代碼 | 限制：-  | 必填 {string}
            "message" : "successToGetContent",  // 狀態訊息 | 限制：-  | 必填 {string}
            "detail" : "",                      // 狀態說明 | 限制：-  | 選填 {string}
            "data" : {                          // 回傳內容 | 限制：-  | 選填 {JSON}{array}  無內容可空值
                "id" : 1,                                               // 流水號編號 | 限制：-  | 必填 {int}    流水號，資料庫排序、索引用
                "articleId" : "f84a4543-98c8-422c-8590-753e719e16c0",   // 文章編號 | 限制：-  | 必填 {string}   文章類別另設 ID 唯一索引用
                "categoryId" : "f84a4543-98c8-422c-8590-753e719e16c0",  // 目錄編號 | 限制：-  | 必填 {string}   文章隸屬目錄 ID
                "categoryName" : "文章目錄",                            // 目錄名稱 | 限制：-  | 必填 {string}   文章隸屬目錄名稱             
                "imageURL" : "img/activity/20211102/123.png",           // 縮圖位置 | 限制：-  | 必填 {JSON}     傳送格式 = img / 存放圖片的資料夾 / 個別資料夾 / 圖片名稱.圖片格式 
                "name" : "...",                                         // 文章標題 | 限制：40 | 必填 {string}
                "brief" : "...",                                        // 文章簡述 | 限制：100 | 必填 {string}
                "content" : {...},                                      // 文章內容 | 限制：-  | 必填 {JSON}     Delta 物件，類似 JSON 格式的物件（註：取得全部不傳送此欄位或內容為 null。）
                "first" : 0,                                            // 文章置頂 | 限制：-  | 必填 {int}}     置頂狀態（0 關閉；1 開啟）
                "sort" : 0,                                             // 排序     | 限制：-  | 必填 {int}      排序，預設為流水號編號（從編號 1 號開始；0 為最大）                                  
                "enabled" : 1,                                          // 文章狀態 | 限制：-  | 必填 {int}      啟用狀態（0 關閉；1 開啟）
                "startDate" : "2021-10-22 16:29",                       // 上架日期 | 限制：-  | 選填 {date}     設定上架日期，上架前啟用狀態為關閉（如果設定為開啟則改為關閉）
                "endDate" : " "2021-10-22 16:29",                       // 下架日期 | 限制：-  | 選填 {date}     設定下架日期，下架前啟用狀態為開啟（如果更新為關閉則清除下架日期改為空值）
                "createDate" : "2021-10-22 16:29",                      // 建立日期 | 限制：-  | 必填 {date}
                "updateDate" : "2021-10-22 16:29",                      // 更新日期 | 限制：-  | 必填 {date}                                 
            },                               
            "timestamp" : 123456789,            // 回傳時間 | 限制：-  | 必填 {date}         毫秒
        }
        註：文章縮圖位置與文章內容中的圖片位置應相同。
        註：排序功能，除非有使用要求，否則一般不作使用（直接由前端帶預設值，不由使用者設置）。
    ```

    ```example
        請求失敗（）
        {
            "code" : "404",                                 // 狀態代碼 | 限制：-  | 必填 {string}
            "message" : "errorToNotFound",                  // 狀態訊息 | 限制：-  | 必填 {string}
            "detail" : "請求失敗，請求的資訊、資源不存在。",  // 狀態說明 | 限制：-  | 必填 {string}
            "data" : "",                                    // 回傳內容 | 限制：-  | 選填 {JSON}{array}  無內容可空值
            "timestamp" : 123456789,                        // 回傳時間 | 限制：-  | 必填 {date}         毫秒
        }
    ```

>到這

##### 新增文章資訊

* Route：/activity
* Method：POST
* Input（請求）：
  
    ```example
        {
            "data" : {
                "file" : file,                                          // 活動縮圖 | 限制：-  | 必填 {file}     要上傳的圖片檔案
                "name" : "123.png",                                     // 活動標題 | 限制：40 | 必填 {string}
                "categoryId" : "f84a4543-98c8-422c-8590-753e719e16c0",  // 目錄編號 | 限制：-  | 必填 {string}   活動隸屬目錄 ID             
                "brief" : "...",                                        // 活動簡述 | 限制：100 | 必填 {string}                   
                "video" : "https://youtube.com.tw/123",                 // 影片網址 | 限制：-  | 選填 {string}   活動影片網址，以 YOUTUBE 影片崁入式                   
                "content" : {...},                                      // 活動內容 | 限制：-  | 必填 {JSON}     Delta 物件，類似 JSON 格式的物件
                "cNameArr" : [...],                                     // 圖片集合 | 限制：-  | 必填 {array}    編輯器中插入的圖片名稱集合，用於刪除被取代的圖片；無值（無圖片）則空陣列。
                "first" : 0,                                            // 活動置頂 | 限制：-  | 必填 {int}}     置頂狀態（0 關閉；1 開啟）
                "enabled" : 1,                                          // 活動狀態 | 限制：-  | 必填 {int}      啟用狀態（0 關閉；1 開啟）
                "startDate" : "2021-10-22 16:29",                       // 上架日期 | 限制：-  | 選填 {date}     設定上架日期，上架前啟用狀態為關閉（如果設定為開啟則改為關閉）
                "endDate" : " "2021-10-22 16:29",                       // 下架日期 | 限制：-  | 選填 {date}     設定下架日期，下架前啟用狀態為開啟（如果更新為關閉則清除下架日期改為空值）
                "createDate" : "2021-10-22 16:29",                      // 建立日期 | 限制：-  | 必填 {date}
                "updateDate" : "2021-10-22 16:29",                      // 更新日期 | 限制：-  | 必填 {date}  
            },
            "timestamp" : 123456789  // 請求時間 | 限制：-  | 必填 {date}  毫秒
        }
        註：排序以預設流水號方式由後端直接產生，前端新增不另帶
    ```

* Situation（情況）：
    | code | message | detail |
    | :--- | :--- | :--- |
    | 201（成功） | successToCreated | "新增成功！" |
    | 400（失敗） | errorToNotFilled | "尚未選擇您要上傳的消息縮圖！" |
    | 400（失敗） | errorToBadRequest | "請上傳正確的圖片檔案格式（格式：JPG、PNG 等圖片格式）！" |
    | 400（失敗） | errorToNotFilled | "標題欄位尚未填寫！" |
    | 400（失敗） | errorToBadRequest | "標題欄位長度輸入超過限制（40個字）！" |
    | 400（失敗） | errorToNotFilled | "目錄欄位尚未填寫！" |
    | 400（失敗） | errorToNotFilled | "簡述欄位尚未填寫！" |
    | 400（失敗） | errorToBadRequest | "簡述欄位長度輸入超過限制（100個字）！" |
    | 400（失敗） | errorToNotFilled | "內容欄位尚未填寫！" |
    | 400（失敗） | errorToNotFilled | "置頂欄位尚未填寫！" |
    | 400（失敗） | errorToNotFilled | "狀態欄位尚未填寫！" |
    | 400（失敗） | errorToBadRequest | "上架日期欄位格式輸入錯誤（不得設定過去時間）！" |
    | 400（失敗） | errorToBadRequest | "上架日期欄位格式輸入錯誤（上架時間不得大於下架時間）！" |
    | 400（失敗） | errorToBadRequest | "下架日期欄位格式輸入錯誤（不得設定過去時間）！" |
    | 400（失敗） | errorToBadRequest | "下架日期欄位格式輸入錯誤（下架時間不得小於上架時間）！" |
    | 401（失敗） | errorToUnauthorized | "用戶端沒有認證憑據，伺服器拒絕回應、存取。" |
* Response（回應）：

    ```example
        請求成功（201）
        {
            "code" : "201",                  // 狀態代碼 | 限制：-  | 必填 {string}
            "message" : "successToCreated",  // 狀態訊息 | 限制：-  | 必填 {string}
            "detail" : "新增成功！",          // 狀態說明 | 限制：-  | 選填 {string}
            "data" : {                       // 回傳內容 | 限制：-  | 選填 {JSON}{array}  無內容可空值
                "id" : 1,                                           // 編號流水號 | 限制：-  | 必填 {int}   流水號，資料庫排序、索引用
                "newsId" : "f84a4543-98c8-422c-8590-753e719e16c0",  // 消息編號 | 限制：-  | 必填 {string}  消息類別另設 ID 唯一索引用
                "first" : 0,                                        // 消息置頂 | 限制：-  | 必填 {int}}    置頂狀態（0 關閉；1 開啟）
                "sort" : 0,                                         // 排序     | 限制：-  | 必填 {int}     排序，預設為流水號編號（從編號 1 號開始；0 為最大）                                  
                "enabled" : 1,                                      // 消息狀態 | 限制：-  | 必填 {int}     啟用狀態（0 關閉；1 開啟）
                "startDate" : "2021-10-22 16:29",                   // 上架日期 | 限制：-  | 選填 {date}    設定上架日期，上架前啟用狀態為關閉（如果設定為開啟則改為關閉）
                "endDate" : " "2021-10-22 16:29",                   // 下架日期 | 限制：-  | 選填 {date}    設定下架日期，下架前啟用狀態為開啟（如果更新為關閉則清除下架日期改為空值）
                "createDate" : "2021-10-22 16:29",                  // 建立日期 | 限制：-  | 必填 {date}              
            },                               
            "timestamp" : 123456789,         // 回傳時間 | 限制：-  | 必填 {date}         毫秒
        }
    ```

    ```example
        請求失敗（）
        {
            "code" : "400",                           // 狀態代碼 | 限制：-  | 必填 {string}
            "message" : "errorToNotFilled",           // 狀態訊息 | 限制：-  | 必填 {string}
            "detail" : "尚未選擇您要上傳的輪播圖片！",  // 狀態說明 | 限制：-  | 必填 {string}
            "data" : "",                              // 回傳內容 | 限制：-  | 選填 {JSON}{array}  無內容可空值
            "timestamp" : 123456789,                  // 回傳時間 | 限制：-  | 必填 {date}         毫秒
        }
    ```

##### 更新文章資訊

* Route：/activity/{activityId}
* Method：PUT
* Input（請求）：
  
    ```example
        {
            "data" : {
                "activityId" : "f84a4543-98c8-422c-8590-753e719e16c0",  // 活動編號 | 限制：-  | 必填 {string}   活動類別另設 ID 唯一索引用
                "categoryId" : "f84a4543-98c8-422c-8590-753e719e16c0",  // 目錄編號 | 限制：-  | 必填 {string}   活動隸屬目錄 ID
                "file" : file,                                          // 活動縮圖 | 限制：-  | 選填 {file}     要上傳的圖片檔案（如果傳送的是空值則為不修改；如果傳送新的檔案則修改；）           
                "name" : "...",                                         // 活動標題 | 限制：40 | 必填 {string}
                "video" : "https://youtube.com.tw/123",                 // 影片網址 | 限制：-  | 選填 {string}   活動影片網址，以 YOUTUBE 影片崁入式                   
                "brief" : "...",                                        // 活動簡述 | 限制：100 | 必填 {string}
                "content" : {...},                                      // 活動內容 | 限制：-  | 必填 {JSON}     Delta 物件，類似 JSON 格式的物件（註：取得全部不傳送此欄位或內容為 null。）
                "first" : 0,                                            // 活動置頂 | 限制：-  | 必填 {int}}     置頂狀態（0 關閉；1 開啟）
                "sort" : 0,                                             // 排序     | 限制：-  | 必填 {int}      排序，預設為流水號編號（從編號 1 號開始；0 為最大）                                  
                "enabled" : 1,                                          // 活動狀態 | 限制：-  | 必填 {int}      啟用狀態（0 關閉；1 開啟）
                "startDate" : "2021-10-22 16:29",                       // 上架日期 | 限制：-  | 選填 {date}     設定上架日期，上架前啟用狀態為關閉（如果設定為開啟則改為關閉）
                "endDate" : " "2021-10-22 16:29",                       // 下架日期 | 限制：-  | 選填 {date}     設定下架日期，下架前啟用狀態為開啟（如果更新為關閉則清除下架日期改為空值）
                "updateDate" : "2021-10-22 16:29",                      // 更新日期 | 限制：-  | 必填 {date}                                 
            },
            "timestamp" : 123456789  // 請求時間 | 限制：-  | 必填 {date}  毫秒
        }
        註：排序功能，除非有使用要求，否則一般不作使用（直接由前端帶預設值，不由使用者設置）。
    ```

* Situation（情況）：
    | code | message | detail |
    | :--- | :--- | :--- |
    | 200（成功） | successButNoContent | "修改成功！" |
    | 400（失敗） | errorToBadRequest | "請上傳正確的圖片檔案格式（格式：JPG、PNG 等圖片格式）！" |
    | 400（失敗） | errorToNotFilled | "標題欄位尚未填寫！" |
    | 400（失敗） | errorToBadRequest | "標題欄位長度輸入超過限制（40個字）！" |
    | 400（失敗） | errorToNotFilled | "目錄欄位尚未填寫！" |
    | 400（失敗） | errorToNotFilled | "簡述欄位尚未填寫！" |
    | 400（失敗） | errorToBadRequest | "簡述欄位長度輸入超過限制（100個字）！" |
    | 400（失敗） | errorToNotFilled | "內容欄位尚未填寫！" |
    | 400（失敗） | errorToNotFilled | "置頂欄位尚未填寫！" |
    | 400（失敗） | errorToNotFilled | "狀態欄位尚未填寫！" |
    | 400（失敗） | errorToBadRequest | "上架日期欄位格式輸入錯誤（不得設定過去時間）！" |
    | 400（失敗） | errorToBadRequest | "上架日期欄位格式輸入錯誤（上架時間不得大於下架時間）！" |
    | 400（失敗） | errorToBadRequest | "下架日期欄位格式輸入錯誤（不得設定過去時間）！" |
    | 400（失敗） | errorToBadRequest | "上架日期欄位格式輸入錯誤（下架時間不得小於上架時間）！" |
    | 401（失敗） | errorToUnauthorized | "用戶端沒有認證憑據，伺服器拒絕回應、存取。" |
    | 404（失敗） | errorToNotFound | "請求失敗，請求的資訊、資源不存在。" |
* Response（回應）：

    ```example
        請求成功（200）
        {
            "code" : "200",                     // 狀態代碼 | 限制：-  | 必填 {string}
            "message" : "successButNoContent",  // 狀態訊息 | 限制：-  | 必填 {string}
            "detail" : "修改成功！",             // 狀態說明 | 限制：-  | 選填 {string}
            "data" : ""                         // 回傳內容 | 限制：-  | 選填 {JSON}{array}  無內容可空值                           
            "timestamp" : 123456789,            // 回傳時間 | 限制：-  | 必填 {date}         毫秒
        }
    ```

    ```example
        請求失敗（）
        {
            "code" : "401",                                        // 狀態代碼 | 限制：-  | 必填 {string}
            "message" : "errorToUnauthorized",                     // 狀態訊息 | 限制：-  | 必填 {string}
            "detail" : "用戶端沒有認證憑據，伺服器拒絕回應、存取。",  // 狀態說明 | 限制：-  | 必填 {string}
            "data" : "",                                           // 回傳內容 | 限制：-  | 選填 {JSON}{array}  無內容可空值
            "timestamp" : 123456789,                               // 回傳時間 | 限制：-  | 必填 {date}         毫秒
        }
    ```

##### 新增（修改）文章圖片儲存方式

* Route：activity/image
* Method：POST
* Input（請求）：

    ```example
        {
            "data" : {
                "file" : file,  // 圖片檔案 | 限制：-  | 必填 {file}  要上傳的圖片檔案
            },
            "timestamp" : 123456789  // 請求時間 | 限制：-  | 必填 {date}  秒
        }
    ```

* Situation（情況）：
    | code | message | detail |
    | :--- | :--- | :--- |
    | 201（成功） | successToCreated | "新增成功！" |
    | 400（失敗） | errorToBadRequest | "請上傳正確的圖片檔案格式（格式：JPG、PNG 等圖片格式）！" |
    | 401（失敗） | errorToUnauthorized | "用戶端沒有認證憑據，伺服器拒絕回應、存取。" |
* Response（回應）：

    ```example
        請求成功（201）
        {
            "code" : "201",                  // 狀態代碼 | 限制：-  | 必填 {string}
            "message" : "successToCreated",  // 狀態訊息 | 限制：-  | 必填 {string}
            "detail" : "新增成功！",          // 狀態說明 | 限制：-  | 選填 {string}
            "data" : {                       // 回傳內容 | 限制：-  | 選填 {JSON}{array}  無內容可空值 
                "id" : 1,                                            // 流水號編號 | 限制：-  | 必填 {int}   流水號，資料庫排序、索引用
                "imageId" : "f84a4543-98c8-422c-8590-753e719e16c0",  // 圖片編號 | 限制：-  | 必填 {string}  圖片類別另設 ID 唯一索引用
                "imageURL" : "img/banners/123.png",                  // 圖片位置 | 限制：-  | 必填 {JSON}    傳送格式 = img / 存放圖片的資料夾 / 個別資料夾 / 圖片名稱.圖片格式
            },                          
            "timestamp" : 123456789,         // 回傳時間 | 限制：-  | 必填 {date}         毫秒
        }
    ```

    ```example
        請求失敗（）
        {
            "code" : "400",                                                     // 狀態代碼 | 限制：-  | 必填 {string}
            "message" : "errorToBadRequest",                                    // 狀態訊息 | 限制：-  | 必填 {string}
            "detail" : "請上傳正確的圖片檔案格式（格式：JPG、PNG 等圖片格式）！",  // 狀態說明 | 限制：-  | 必填 {string}
            "data" : "",                                                        // 回傳內容 | 限制：-  | 選填 {JSON}{array}  無內容可空值
            "timestamp" : 123456789,                                            // 回傳時間 | 限制：-  | 必填 {date}         毫秒
        }

##### 刪除文章資訊

* Route：/activity/{activityId}
* Method：DELETE
* Input（請求）：
  
    ```example
        {
            "data" : {
                "activityId" : "f84a4543-98c8-422c-8590-753e719e16c0",  // 活動編號 | 限制：-  | 必填 {string}  活動類別另設 ID 唯一索引用                               
            },
            "timestamp" : 123456789  // 請求時間 | 限制：-  | 必填 {date}  毫秒
        }
    ```

* Situation（情況）：
    | code | message | detail |
    | :--- | :--- | :--- |
    | 200（成功） | successButNoContent | "刪除成功！" |
    | 401（失敗） | errorToUnauthorized | "用戶端沒有認證憑據，伺服器拒絕回應、存取。" |
    | 404（失敗） | errorToNotFound | "請求失敗，請求的資訊、資源不存在。" |
* Response（回應）：

    ```example
        請求成功（200）
        {
            "code" : "200",                     // 狀態代碼 | 限制：-  | 必填 {string}
            "message" : "successButNoContent",  // 狀態訊息 | 限制：-  | 必填 {string}
            "detail" : "刪除成功！",             // 狀態說明 | 限制：-  | 選填 {string}
            "data" : ""                         // 回傳內容 | 限制：-  | 選填 {JSON}{array}  無內容可空值                           
            "timestamp" : 123456789,            // 回傳時間 | 限制：-  | 必填 {date}         毫秒
        }
    ```

    ```example
        請求失敗（）
        {
            "code" : "401",                                        // 狀態代碼 | 限制：-  | 必填 {string}
            "message" : "errorToUnauthorized",                     // 狀態訊息 | 限制：-  | 必填 {string}
            "detail" : "用戶端沒有認證憑據，伺服器拒絕回應、存取。",  // 狀態說明 | 限制：-  | 必填 {string}
            "data" : "",                                           // 回傳內容 | 限制：-  | 選填 {JSON}{array}  無內容可空值
            "timestamp" : 123456789,                               // 回傳時間 | 限制：-  | 必填 {date}         毫秒
        }
    ```
