// Embla Carousels concat AJAX
let carouselz = $('.carouselz'), CONNECT01 = 'IndexSlideshow', items;
// Auto
const autoplay = (embla, interval) => {
    let timer = 0;

    const play = () => {
        stop();
        requestAnimationFrame(() => (timer = window.setTimeout(next, interval)));
    };

    const stop = () => {
        window.clearTimeout(timer);
        timer = 0;
    };

    const next = () => {
        if (embla.canScrollNext()) {
            embla.scrollNext();
        } else {
            embla.scrollTo(0);
        }
        play();
    };

    return { play, stop };
};
const listenForPrevBtnClick = (btn, embla, autoplayer) => {
    const scrollPrev = () => {
        autoplayer.stop();
        embla.scrollPrev();
    };
    btn.addEventListener("click", scrollPrev, false);
};
const listenForNextBtnClick = (btn, embla, autoplayer) => {
    const scrollNext = () => {
        autoplayer.stop();
        embla.scrollNext();
    };
    btn.addEventListener("click", scrollNext, false);
};
const disablePrevNextBtns = (prevBtn, nextBtn, embla) => {
    return () => {
        if (embla.canScrollPrev()) prevBtn.classList.remove("disabled");
        else prevBtn.classList.add("disabled");

        if (embla.canScrollNext()) nextBtn.classList.remove("disabled");
        else nextBtn.classList.add("disabled");
    };
};
// // Arrow
// const setupPrevNextBtns = (prevBtn, nextBtn, embla) => {
//     prevBtn.addEventListener('click', embla.scrollPrev, false);
//     nextBtn.addEventListener('click', embla.scrollNext, false);
// };
// Dot
// const setupDotBtns = (dotsArray, embla) => {
//     dotsArray.forEach((dotNode, i) => {
//         dotNode.addEventListener("click", () => embla.scrollTo(i), false);
//     });
// };
// const generateDotBtns = (dots, embla) => {
//     const template = `<button class="embla__dot" type="button"></button>`;
//     dots.innerHTML = embla.scrollSnapList().reduce(acc => acc + template, "");
//     return [].slice.call(dots.querySelectorAll(".embla__dot"));
// };
// const selectDotBtn = (dotsArray, embla) => () => {
//     const previous = embla.previousScrollSnap();
//     const selected = embla.selectedScrollSnap();
//     dotsArray[previous].classList.remove("is-selected");
//     dotsArray[selected].classList.add("is-selected");
// };

// Embla Carousel

window.onload = function () {
    // Carousels
    let casObj = {
        "Methods": "GET",
        "APIs": URL,
        "CONNECTs": `${CONNECT01}`,
        "QUERYs": "",
        "Counts": "",
        "Sends": "",
    };
    getPageDatas(casObj).then(res => {
        // DOSOMETHING
        if (res !== null) {
            html = "";
            for (let i = 0; i < res.length; i++) {
                items = `
                    <a ${res[i].hyperlink !== "" ? `href="${res[i].hyperlink}"` : ""} target="_blank" class="embla__slide items">
                        <img class="fulls" src="${res[i].imageURL01}?carousel${paramRandoms()}">
                        <img class="tablets" src="${res[i].imageURL02 !== null ? res[i].imageURL02 : res[i].imageURL01}?carousel${paramRandoms()}">
                        <img class="smPhones" src="${res[i].imageURL03 !== null ? res[i].imageURL03 : res[i].imageURL01}?carousel${paramRandoms()}">
                    </a>
                `;
                html += items;
            };
            carouselz.html(html);

            const emblaNode = document.querySelector(".embla");
            const prevBtn = emblaNode.querySelector(".embla__button--prev");
            const nextBtn = emblaNode.querySelector(".embla__button--next");
            // const dots = document.querySelector(".embla__dots");
            var options = { loop: false }
            var embla = EmblaCarousel(emblaNode, options);
            // const dotsArray = generateDotBtns(dots, embla);
            const autoplayer = autoplay(embla, 6000);
            // const setSelectedDotBtn = selectDotBtn(dotsArray, embla);
            const disablePrevAndNextBtns = disablePrevNextBtns(prevBtn, nextBtn, embla);

            // setupPrevNextBtns(prevBtn, nextBtn, embla);
            // setupDotBtns(dotsArray, embla);
            listenForPrevBtnClick(prevBtn, embla, autoplayer);
            listenForNextBtnClick(nextBtn, embla, autoplayer);

            // embla.on("select", setSelectedDotBtn);
            embla.on("select", disablePrevAndNextBtns);

            // embla.on("init", setSelectedDotBtn);
            embla.on("init", autoplayer.play);
            embla.on("init", disablePrevAndNextBtns);

            embla.on("pointerDown", autoplayer.stop);
            embla.on("pointerUp", autoplayer.play);

        } else { };
    }, rej => {
        if (rej == "NOTFOUND") { };
    });
};