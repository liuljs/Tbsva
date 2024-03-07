// 後台主選單
function createMenus(arr) {
    let categories = $('.categories');
    let auth01 = "", auth02 = "", auth03 = "", auth04 = "", auth05 = "", auth06 = "", auth07 = "";
    let indexAuth = "", founderAuth = "", aboutAuth = "", missionAuth = "", supportAuth = "", eventAuth = "", recruitAuth = "";
    let donateAuth = "", recordAuth = "";
    let backAuth = "", memberAuth = "", managerAuth = "", permiGroupAuth = "";
    for (let i = 0; i < arr.length; i++) {
        if (arr[i].category == categories.eq(i).data('numz')) {
            // 編輯類別
            if (arr[i].category == 'editCategory') {
                let authStatus = false;
                for (let j = 0; j < arr[i].auth.length; j++) {
                    // 是否有權限（只要有一個，就代表擁有編輯類別的權限）
                    if (arr[i].auth[j].view == '1') {
                        authStatus = true
                    };
                    // 首頁
                    if (arr[i].auth[j].belong == 'front') {
                        //
                        if (arr[i].auth[j].view == '1') {
                            auth01 = '';
                            indexAuth += `
                                <li data-numz="${arr[i].auth[j].authId}">
                                    <a href="./${arr[i].auth[j].path}">
                                        <span class="sub-item">${arr[i].auth[j].name}</span>
                                    </a>
                                </li>
                            `;
                            auth01 = `
                                <li class="nav-item">
                                    <a data-toggle="collapse" href="#front">
                                        <i class="fad fa-laptop-house"></i>
                                        <p>首頁</p>
                                        <span class="caret"></span>
                                    </a>
                                    <div class="collapse" id="front">
                                        <ul class="nav nav-collapse">
                                            ${indexAuth}
                                        </ul>
                                    </div>
                                </li>
                            `;
                        } else { auth01 = ''; };
                    };
                    // 創始人
                    if (arr[i].auth[j].belong == 'founder') {
                        //
                        if (arr[i].auth[j].view == '1') {
                            auth02 = '';
                            founderAuth += `
                                <li data-numz="${arr[i].auth[j].authId}">
                                    <a href="./${arr[i].auth[j].path}">
                                        <span class="sub-item">${arr[i].auth[j].name}</span>
                                    </a>
                                </li>
                            `;
                            auth02 = `
                                <li class="nav-item">
                                    <a data-toggle="collapse" href="#founders">
                                        <i class="fad fa-user-crown"></i>
                                        <p>蓮生活佛</p>
                                        <span class="caret"></span>
                                    </a>
                                    <div class="collapse" id="founders">
                                        <ul class="nav nav-collapse">
                                            ${founderAuth}
                                        </ul>
                                    </div>
                                </li>
                            `;
                        } else { auth02 = ''; };
                    };
                    // 關於
                    if (arr[i].auth[j].belong == 'about') {
                        //
                        if (arr[i].auth[j].view == '1') {
                            auth03 = '';
                            if (arr[i].auth[j].authId == 'e07') {
                                aboutAuth += `
                                    <li data-numz="${arr[i].auth[j].authId}" class="submenu">
                                        <a data-toggle="collapse" href="#supervisors">
                                            <p class="sub-item">${arr[i].auth[j].name}</p>
                                            <span class="caret"></span>
                                        </a>
                                        <div class="collapse" id="supervisors">
                                            <ul class="nav nav-collapse subnav">
                                                <li data-numz="${arr[i].auth[j].authId}">
                                                    <a href="./supervisor_01.html">
                                                        <span class="sub-item">目錄類別</span>
                                                    </a>
                                                </li>
                                                <li data-numz="${arr[i].auth[j].authId}">
                                                    <a href="./${arr[i].auth[j].path}">
                                                        <span class="sub-item">成員列表</span>
                                                    </a>
                                                </li>
                                            </ul>
                                        </div>
                                    </li>
                                `;
                            } else {
                                aboutAuth += `
                                    <li data-numz="${arr[i].auth[j].authId}">
                                        <a href="./${arr[i].auth[j].path}">
                                            <span class="sub-item">${arr[i].auth[j].name}</span>
                                        </a>
                                    </li>
                                `;
                            };
                            auth03 = `
                                <li class="nav-item">
                                    <a data-toggle="collapse" href="#abouts">
                                        <i class="fad fa-chalkboard-teacher"></i>
                                        <p>密教總會</p>
                                        <span class="caret"></span>
                                    </a>
                                    <div class="collapse" id="abouts">
                                        <ul class="nav nav-collapse">
                                            ${aboutAuth}
                                        </ul>
                                    </div>
                                </li>
                            `;
                        } else { auth03 = ''; };
                    };
                    // 密總任務
                    if (arr[i].auth[j].belong == 'mission') {
                        //
                        if (arr[i].auth[j].view == '1') {
                            auth04 = '';
                            if (arr[i].auth[j].authId == 'e08') {
                                missionAuth += `
                                    <li data-numz="${arr[i].auth[j].authId}" class="submenu">
                                        <a data-toggle="collapse" href="#dojos">
                                            <p class="sub-item">${arr[i].auth[j].name}</p>
                                            <span class="caret"></span>
                                        </a>
                                        <div class="collapse" id="dojos">
                                            <ul class="nav nav-collapse subnav">
                                                <li data-numz="${arr[i].auth[j].authId}">
                                                    <a href="./dojo_01.html">
                                                        <span class="sub-item">目錄類別</span>
                                                    </a>
                                                </li>
                                                <li data-numz="${arr[i].auth[j].authId}">
                                                    <a href="./${arr[i].auth[j].path}">
                                                        <span class="sub-item">道場列表</span>
                                                    </a>
                                                </li>
                                            </ul>
                                        </div>
                                    </li>
                                `;
                            } else if (arr[i].auth[j].authId == 'e09') {
                                missionAuth += `
                                    <li data-numz="${arr[i].auth[j].authId}" class="submenu">
                                        <a data-toggle="collapse" href="#missionary">
                                            <p class="sub-item">${arr[i].auth[j].name}</p>
                                            <span class="caret"></span>
                                        </a>
                                        <div class="collapse" id="missionary">
                                            <ul class="nav nav-collapse subnav">
                                                <li data-numz="${arr[i].auth[j].authId}">
                                                    <a href="./${arr[i].auth[j].path}">
                                                        <span class="sub-item">上師名單</span>
                                                    </a>
                                                </li>
                                                <li data-numz="${arr[i].auth[j].authId}">
                                                    <a href="./missionary_04.html">
                                                        <span class="sub-item">法師名單</span>
                                                    </a>
                                                </li>
                                                <li data-numz="${arr[i].auth[j].authId}">
                                                    <a href="./missionary_05.html">
                                                        <span class="sub-item">教授師名單</span>
                                                    </a>
                                                </li>
                                                <li data-numz="${arr[i].auth[j].authId}">
                                                    <a href="./missionary_06.html">
                                                        <span class="sub-item">講師名單</span>
                                                    </a>
                                                </li>
                                                <li data-numz="${arr[i].auth[j].authId}">
                                                    <a href="./missionary_07.html">
                                                        <span class="sub-item">助教名單</span>
                                                    </a>
                                                </li>
                                            </ul>
                                        </div>
                                    </li>
                                `;
                            } else {
                                missionAuth += `
                                    <li data-numz="${arr[i].auth[j].authId}">
                                        <a href="./${arr[i].auth[j].path}">
                                            <span class="sub-item">${arr[i].auth[j].name}</span>
                                        </a>
                                    </li>
                                `;
                            };
                            auth04 = `
                                <li class="nav-item">
                                    <a data-toggle="collapse" href="#missions">
                                        <i class="fa-solid fa-list-check"></i>
                                        <p>密總任務</p>
                                        <span class="caret"></span>
                                    </a>
                                    <div class="collapse" id="missions">
                                        <ul class="nav nav-collapse">
                                        ${missionAuth}
                                        </ul>
                                    </div>
                                </li>
                            `;
                        } else { auth04 = ''; };
                    };
                    // 護持捐款
                    if (arr[i].auth[j].belong == 'support') {
                        //
                        if (arr[i].auth[j].view == '1') {
                            auth05 = '';
                            supportAuth += `
                                <li data-numz="${arr[i].auth[j].authId}">
                                    <a href="./${arr[i].auth[j].path}">
                                        <span class="sub-item">${arr[i].auth[j].name}</span>
                                    </a>
                                </li>
                            `;
                            auth05 = `
                                <li class="nav-item">
                                    <a data-toggle="collapse" href="#supports">
                                        <i class="fa-duotone fa-hand-holding-seedling"></i>
                                        <p>護持捐款</p>
                                        <span class="caret"></span>
                                    </a>
                                    <div class="collapse" id="supports">
                                        <ul class="nav nav-collapse">
                                            ${supportAuth}
                                        </ul>
                                    </div>
                                </li>
                            `;
                        } else { auth05 = ''; };
                    };
                    // 密總動態
                    if (arr[i].auth[j].belong == 'event') {
                        //
                        if (arr[i].auth[j].view == '1') {
                            auth06 = '';
                            if (arr[i].auth[j].authId == 'e19') {
                                eventAuth += `
                                    <li data-numz="${arr[i].auth[j].authId}" class="submenu">
                                        <a data-toggle="collapse" href="#activities">
                                            <p class="sub-item">${arr[i].auth[j].name}</p>
                                            <span class="caret"></span>
                                        </a>
                                        <div class="collapse" id="activities">
                                            <ul class="nav nav-collapse subnav">
                                                <li  data-numz="${arr[i].auth[j].authId}">
                                                    <a href="./activity_01.html">
                                                        <span class="sub-item">目錄類別</span>
                                                    </a>
                                                </li>
                                                <li  data-numz="${arr[i].auth[j].authId}">
                                                    <a href="./${arr[i].auth[j].path}">
                                                        <span class="sub-item">活動列表</span>
                                                    </a>
                                                </li>
                                            </ul>
                                        </div>
                                    </li>
                                `;
                            } else {
                                eventAuth += `
                                    <li data-numz="${arr[i].auth[j].authId}">
                                        <a href="./${arr[i].auth[j].path}">
                                            <span class="sub-item">${arr[i].auth[j].name}</span>
                                        </a>
                                    </li>
                                `;
                            }
                            auth06 = `
                                <li class="nav-item">
                                    <a data-toggle="collapse" href="#events">
                                        <i class="fad fa-kite"></i>
                                        <p>密總動態</p>
                                        <span class="caret"></span>
                                    </a>
                                    <div class="collapse" id="events">
                                        <ul class="nav nav-collapse">
                                           ${eventAuth}
                                        </ul>
                                    </div>
                                </li>
                            `;
                        } else { auth06 = ''; };
                    };
                    // 招募義工
                    if (arr[i].auth[j].belong == 'recruit') {
                        //
                        if (arr[i].auth[j].view == '1') {
                            auth07 = '';
                            recruitAuth += `
                                <li data-numz="${arr[i].auth[j].authId}">
                                    <a href="./${arr[i].auth[j].path}">
                                        <span class="sub-item">${arr[i].auth[j].name}</span>
                                    </a>
                                </li>
                            `;
                            auth07 = `
                                <li class="nav-item">
                                    <a data-toggle="collapse" href="#recruits">
                                        <i class="fad fa-american-sign-language-interpreting"></i>
                                        <p>招募義工</p>
                                        <span class="caret"></span>
                                    </a>
                                    <div class="collapse" id="recruits">
                                        <ul class="nav nav-collapse">
                                            ${recruitAuth}
                                        </ul>
                                    </div>
                                </li>
                            `;
                        } else { auth07 = ''; };
                    };
                };
                if (authStatus == true) {
                    //
                    categories.eq(i).html(`
                        <li class="nav-section">
                            <span class="sidebar-mini-icon">
                                <i class="fa fa-ellipsis-h"></i>
                            </span>
                            <h4 class="text-section">
                                <i class="fad fa-crop"></i>
                                ${arr[i].name}
                            </h4>
                        </li>
                        ${auth01 + auth02 + auth03 + auth04 + auth05 + auth06 + auth07}
                    `);
                } else {
                    // 並未擁有任何一個權限
                    categories.eq(i).html('');
                };
            };
            // 財務類別
            if (arr[i].category == 'financialCategory') {
                let authStatus = false;
                for (let j = 0; j < arr[i].auth.length; j++) {
                    // 是否有權限（只要有一個，就代表擁有編輯類別的權限）
                    if (arr[i].auth[j].view == '1') {
                        authStatus = true
                    };
                    // 捐款管理
                    if (arr[i].auth[j].belong == 'donate') {
                        //
                        if (arr[i].auth[j].view == '1') {
                            auth01 = '';
                            donateAuth += `
                                <li data-numz="${arr[i].auth[j].authId}">
                                    <a href="./${arr[i].auth[j].path}">
                                        <span class="sub-item">${arr[i].auth[j].name}</span>
                                    </a>
                                </li>
                            `;
                            auth01 = `
                                <li class="nav-item">
                                    <a data-toggle="collapse" href="#donates">
                                        <i class="fa-duotone fa-money-check-dollar-pen"></i>
                                        <p>捐款管理</p>
                                        <span class="caret"></span>
                                    </a>
                                    <div class="collapse" id="donates">
                                        <ul class="nav nav-collapse">
                                            ${donateAuth}
                                        </ul>
                                    </div>
                                </li>
                            `;
                        } else { auth01 = ''; };
                    };
                    // 訂單管理
                    if (arr[i].auth[j].belong == 'record') {
                        //
                        if (arr[i].auth[j].view == '1' && arr[i].auth[j].edit == '1') {
                            auth02 = '';
                            recordAuth += `
                                <li data-numz="${arr[i].auth[j].authId}">
                                    <a href="./${arr[i].auth[j].path}">
                                        <span class="sub-item">${arr[i].auth[j].name}</span>
                                    </a>
                                </li>
                            `;
                            auth02 = `
                                <li class="nav-item">
                                    <a data-toggle="collapse" href="#records">
                                        <i class="fad fa-cabinet-filing"></i>
                                        <p>訂單管理</p>
                                        <span class="caret"></span>
                                    </a>
                                    <div class="collapse" id="records">
                                        <ul class="nav nav-collapse">
                                            ${recordAuth}
                                        </ul>
                                    </div>
                                </li>
                            `;
                        } else {
                            auth02 = '';
                        };

                    };
                };
                if (authStatus == true) {
                    //
                    categories.eq(i).html(`
                        <li class="nav-section">
                            <span class="sidebar-mini-icon">
                                <i class="fa fa-ellipsis-h"></i>
                            </span>
                            <h4 class="text-section">
                                <i class="fad fa-coin"></i>
                                ${arr[i].name}
                            </h4>
                        </li>
                        ${auth01 + auth02}
                    `);
                } else {
                    // 並未擁有任何一個權限
                    categories.eq(i).html('');
                };
            };
            // 編輯類別
            if (arr[i].category == 'managerCategory') {
                let authStatus = false;
                for (let j = 0; j < arr[i].auth.length; j++) {
                    // 是否有權限（只要有一個，就代表擁有編輯類別的權限）
                    if (arr[i].auth[j].view == '1') {
                        authStatus = true
                    };
                    // 後台管理
                    if (arr[i].auth[j].belong == 'back') {
                        //
                        if (arr[i].auth[j].view == '1') {
                            auth01 = '';
                            backAuth += `
                                <li data-numz="${arr[i].auth[j].authId}">
                                    <a href="./${arr[i].auth[j].path}">
                                        <span class="sub-item">${arr[i].auth[j].name}</span>
                                    </a>
                                </li>
                            `;
                            auth01 = `
                                <li class="nav-item">
                                    <a data-toggle="collapse" href="#back">
                                        <i class="fad fa-info-square"></i>
                                        <p>後台管理</p>
                                        <span class="caret"></span>
                                    </a>
                                    <div class="collapse" id="back">
                                        <ul class="nav nav-collapse">
                                            ${backAuth}
                                        </ul>
                                    </div>
                                </li>
                            `;
                        } else { auth01 = ''; };
                    };
                    // 會員管理
                    if (arr[i].auth[j].belong == 'member') {
                        //
                        if (arr[i].auth[j].view == '1') {
                            auth02 = '';
                            memberAuth += `
                                <li data-numz="${arr[i].auth[j].authId}">
                                    <a href="./${arr[i].auth[j].path}">
                                        <span class="sub-item">${arr[i].auth[j].name}</span>
                                    </a>
                                </li>
                            `;
                            auth02 = `
                                <li class="nav-item">
                                    <a data-toggle="collapse" href="#members">
                                        <i class="fa-duotone fa-circle-user"></i>
                                        <p>會員管理</p>
                                        <span class="caret"></span>
                                    </a>
                                    <div class="collapse" id="members">
                                        <ul class="nav nav-collapse">
                                            ${memberAuth}
                                        </ul>
                                    </div>
                                </li>
                            `;
                        } else { auth02 = ''; };
                    };
                    // 管理者管理
                    if (arr[i].auth[j].belong == 'manager') {
                        //
                        if (arr[i].auth[j].view == '1') {
                            auth03 = '';
                            managerAuth += `
                                <li data-numz="${arr[i].auth[j].authId}">
                                    <a href="./${arr[i].auth[j].path}">
                                        <span class="sub-item">${arr[i].auth[j].name}</span>
                                    </a>
                                </li>
                            `;
                            auth03 = `
                                <li class="nav-item">
                                    <a data-toggle="collapse" href="#managers">
                                        <i class="far fa-user-tie"></i>
                                        <p>管理者管理</p>
                                        <span class="caret"></span>
                                    </a>
                                    <div class="collapse" id="managers">
                                        <ul class="nav nav-collapse">
                                        ${managerAuth}
                                        </ul>
                                    </div>
                                </li>
                            `;
                        } else { auth03 = ''; };
                    };
                    // 權限群組管理
                    if (arr[i].auth[j].belong == 'authGroup') {
                        //
                        if (arr[i].auth[j].view == '1') {
                            auth04 = '';
                            if (arr[i].auth[j].authId == 'm05') {
                                permiGroupAuth = `
                                    <li data-numz="${arr[i].auth[j].authId}">
                                        <a href="./${arr[i].auth[j].path}">
                                            <span class="sub-item">權限群組設定</span>
                                        </a>
                                    </li>
                                    <li data-numz="${arr[i].auth[j].authId}">
                                        <a href="./authGroup_04.html">
                                            <span class="sub-item">權限群組授予</span>
                                        </a>
                                    </li>
                                `;
                            }
                            auth04 = `
                                <li class="nav-item">
                                    <a data-toggle="collapse" href="#permiGroups">
                                        <i class="fad fa-layer-group"></i>
                                            <p>權限群組管理</p>
                                        <span class="caret"></span>
                                    </a>
                                    <div class="collapse" id="permiGroups">
                                        <ul class="nav nav-collapse">
                                            ${permiGroupAuth}
                                        </ul>
                                    </div>
                                </li>
                            `;
                        } else { auth04 = ''; };
                    };
                };
                if (authStatus == true) {
                    //
                    categories.eq(i).html(`
                        <li class="nav-section">
                            <span class="sidebar-mini-icon">
                                <i class="fa fa-ellipsis-h"></i>
                            </span>
                            <h4 class="text-section">
                                <i class="fad fa-tasks-alt"></i>
                                ${arr[i].name}
                            </h4>
                        </li>
                        ${auth01 + auth02 + auth03 + auth04}
                    `);
                } else {
                    // 並未擁有任何一個權限
                    categories.eq(i).html('');
                };
            };
        };
    };
};
// 確認是否有登入
getLoginInFo()
    .then(res => {
        // 登入成功，將資訊寫入 LocalStorage
        writeData(res);
        // 權限
        let authDatas = JSON.parse(authz);
        // 選單
        createMenus(authDatas);
        // 標示目前的位置
        let active = window.location.pathname.split('/').pop();
        $(`a[href="${window.location.origin + window.location.pathname}"], a[href="${window.location.pathname}"], a[href="./${active}"]`).each((idx, obj) => {
            let active = $(obj).parents('li');
            if (active.hasClass('nav-item')) {
                active.addClass('active');
                active.find('> .collapse').addClass('show');
                active.find('> a[data-toggle="collapse"]').attr('aria-expanded', true);
            } else {
                active.addClass("active");
            };
        });
        // 確認權限
        isControlOrNot(authDatas);
    })
    .catch(rej => {
        // 未登入
        if (rej === "NotSignIn") {
            // alert("錯誤訊息 " + xhr.status + "：您的連線異常，請重新登入！");
            getLogout();
        };
    });
// 登出按鈕
let btnLogoutz = $('.btnLogoutz');
// 更改密碼 Change Password
let old_psw = $('.oldPswz'), newPswz = $('.newPswz'), cfmPswz = $('.cfmPswz');
let btnOpenz = $('.btnOpenz');
$().ready(function () {
    // 更改密碼 Change Password
    btnOpenz.on('click', function () {
        // Confirm
        $('.btnChangePswz').unbind('click').on('click', function (e) {
            e.preventDefault(), e.stopPropagation();
            changePsw(old_psw, newPswz, cfmPswz);
        });
        // Cancel
        $('.btnCancelz, .close').unbind('click').on('click', function (e) {
            e.preventDefault(), e.stopPropagation();
            let trz = $(this).parents('.modal-content');
            $.map(trz.find('.editInput'), function (item, index) {
                if (item.value !== "") {
                    return check = false
                } else {
                    return check = true
                };
            });
            if (check == true) {
                // 欄位未填任何內容，直接關閉
                trz.find('.closez').trigger('click');
            } else {
                if (confirm("您尚未儲存內容，是否直接關閉？")) {
                    // 清空欄位的值
                    trz.find('.editInput').val('');
                    // 關閉
                    trz.find('.closez').trigger('click');
                };
            };
        });
    });
    // 登出 Logout
    btnLogoutz.on('click', getLogout);
});