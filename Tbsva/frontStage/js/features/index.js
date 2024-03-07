// INDEX
let newsz = $('.newsz'), activityz = $('.activityz'), topMoviez = $('.topMoviez'), moviez = $(".moviez"), annMeritz = $('.annMeritz');
let display = 4, CONNECT02 = 'news', CONNECT03 = 'activity', CONNECT04 = 'video', vidCategory = 2, CONNECT05 = 'video2';
let btnPrvz = $('.btnPrvz');

$(() => {
    // News
    let newsObj = {
        "Methods": "GET",
        "APIs": URL,
        "CONNECTs": CONNECT02,
        "QUERYs": `${CONNECT02}?count=${display}&page=${current}`,
        "Counts": display,
        "Sends": "",
    };
    getPageDatas(newsObj).then(res => {
        // DOSOMETHING
        if (res !== null) {
            html = "";
            for (let i = 0; i < res.length; i++) {
                html += `
                    <a href="./news.html?id=${res[i].newsId}" class="items itemz">
                        <div class="parts">
                            <div class="images">
                                <img src="./img/elements/samples/sample500x500.jpg">
                                <img class="cover imagez" src="${res[i].cover}?news${paramRandoms()}">
                            </div>
                        </div>
                        <div class="parts">
                            <div class="dates">
                                <div class="decCols"></div>
                                <p class="date datez">${dateChangeSlash(res[i].createDate)}</p>
                                <div class="decRows"></div>
                            </div>
                            <div class="texts">
                                <p class="mainTitle abridged2 titlez" title="${res[i].Title}">${res[i].name}</p>
                            </div>
                        </div>
                    </a>
                `;
            };
            newsz.html(html);
        } else { };
    }, rej => {
        if (rej == "NOTFOUND") { };
    });
    // Activity
    let actObj = {
        "Methods": "GET",
        "APIs": URL,
        "CONNECTs": CONNECT03,
        "QUERYs": `${CONNECT03}?count=${display}&page=${current}`,
        "Counts": display,
        "Sends": "",
    };
    getPageDatas(actObj).then(res => {
        // DOSOMETHING
        if (res !== null) {
            html = "";
            for (let i = 0; i < res.length; i++) {
                html += `
                    <a href="./activity.html?id=${res[i].activityId}" class="items itemz">
                        <div class="parts">
                            <div class="images">
                                <img src="./img/elements/samples/sample540x450.jpg">
                                <img class="cover imagez" src="${res[i].cover}?activity${paramRandoms()}">
                            </div>
                        </div>
                        <div class="parts">
                            <div class="dates">
                                <div class="categories">
                                    <p class="category categoryz">${res[i].categoryName}</p>
                                    <div class="decRows"></div>
                                </div>
                                <p class="date datez">${dateChangeSlash(res[i].createDate)}</p>
                            </div>
                            <div class="texts">
                                <p class="mainTitle abridged2 titlez" title="${res[i].name}">${res[i].name}</p>
                            </div>
                        </div>
                    </a>
                `;
            };
            activityz.html(html);
        } else { };
    }, rej => {
        if (rej == "NOTFOUND") { };
    });
    // Movie
    let movObj = {
        "Methods": "GET",
        "APIs": URL,
        "CONNECTs": CONNECT04,
        "QUERYs": `${CONNECT04}?videoCategory=${vidCategory}&count=${display}&page=${current}`,
        "Counts": display,
        "Sends": "",
    };
    getPageDatas(movObj).then(res => {
        // DOSOMETHING
        if (res !== null) {
            html = "";
            // 首筆
            topMoviez.attr('data-numz', res[0].videoId);
            // topMoviez.find('.iframez').attr('src', JSON.parse(res[0].video));
            topMoviez.find('.iframez').attr('src', res[0].video);
            topMoviez.find('.titlez').html(res[0].name);
            topMoviez.find('.titlez').attr('title', res[0].name);
            topMoviez.find('.categoryz').html(movCategoryDisplay(res[0].videoCategory));
            topMoviez.find('.datez').html(dateChangeSlash(res[0].createDate));
            //
            for (let i = 0; i < res.length; i++) {
                html += `
                    <a data-numz="${res[i].videoId}" data-srcz="${res[i].video}" class="items itemz">
                        <div class="parts">
                            <div class="images">
                                <img src="./img/elements/samples/sample640x480.jpg">
                                <img class="cover imagez" src="${res[i].imageURL}?video${paramRandoms()}">
                            </div>
                        </div>
                        <div class="parts">
                            <div class="texts">
                                <p class="mainTitle abridged2 titlez" title="${res[i].name}">${res[i].name}</p>
                            </div>
                            <div>
                                <div class="categories">
                                    <p class="category categoryz">${movCategoryDisplay(res[i].videoCategory)}</p>
                                    <div class="decRows"></div>
                                </div>
                                <div class="dates">
                                    <p class="date datez">${dateChangeSlash(res[i].createDate)}</p>
                                </div>
                            </div>
                        </div>
                    </a>
                `;
            };
            moviez.html(html);
            //
            let topMovNumz = topMoviez.attr('data-numz');
            moviez.find('.itemz').eq(0).addClass('active')
            // 顯示點擊的影片
            moviez.find('.itemz').on('click', function () {
                let numz = $(this).data('numz'), srcz = $(this).data('srcz');
                let vidTitle = $(this).find('.titlez').html(), vidCategory = $(this).find('.categoryz').html(), vidDate = $(this).find('.datez').html();
                if (numz !== topMovNumz) {
                    topMoviez.attr('data-numz', numz);
                    topMovNumz = numz;
                    if ($(this).hasClass('active')) {
                    } else {
                        $(this).addClass('active').siblings().removeClass('active');
                        topMoviez.find('.iframez').attr('src', srcz);
                        topMoviez.find('.titlez').html(vidTitle);
                        topMoviez.find('.categoryz').html(vidCategory);
                        topMoviez.find('.datez').html(vidDate);

                        $(window).scrollTop(topMoviez.offset().top - 80);
                    }
                } else { }
            });
        } else { };
    }, rej => {
        if (rej == "NOTFOUND") { };
    });
    // Merit
    let merObj = {
        "Methods": "GET",
        "APIs": URL,
        "CONNECTs": `${CONNECT05}?first=1`, // first 只於前台首頁顯示一筆
        "QUERYs": ``,
        "Counts": "",
        "Sends": "",
    };
    getPageDatas(merObj).then(res => {
        // DOSOMETHING
        if (res !== null) {
            //
            annMeritz.find('.imagez').attr('src', `${res[0].imageURL}?${paramRandoms()}`);
            // annMeritz.find('.iframez').attr('src', JSON.parse(res[0].video));
            annMeritz.find('.iframez').attr('src', res[0].video);
            annMeritz.find('.titlez').html(res[0].name);
            annMeritz.find('.datez').html(dateChangeSlash(res[0].createDate));
            annMeritz.find('.contentz').html(JSON.parse(res[0].content));
            // 
            btnPrvz.on('click', function () {
                let targets = $(this).parents('.mains').find('.lightBoxz'), imgParents = $(this).parents('.parts');
                let titles = imgParents.find('.legends').html() !== undefined ? imgParents.find('.legends').html() : "年度功德主方案";
                if (targets.hasClass('active')) {
                } else {
                    targets.addClass('active');
                    //
                    stays = $(window).scrollTop();
                    $('.wrap').css({ 'top': -stays });
                    $(window).scrollTop(stays);
                    $('.wrap').addClass('active');
                    //
                    if (titles !== undefined && titles !== "") {
                        targets.find('.titlez').html(titles);
                    } else {
                        targets.find('.titlez').html('');
                    };
                    targets.find('.imagez').addClass('cover').attr('src', `${res[0].imageURL}?${paramRandoms()}`);
                    targets.find('.btnClosez').on('click', function () {
                        if (targets.hasClass('active')) {
                            $('.wrap').removeClass('active');
                            $('.wrap').css({ 'top': 0 });
                            $(window).scrollTop(stays);
                            targets.find('.imagez').removeClass('active');
                            targets.removeClass('active');
                        }
                    });
                };
            });
        } else { };
    }, rej => {
        if (rej == "NOTFOUND") { };
    });
});

