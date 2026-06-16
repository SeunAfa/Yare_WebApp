// Hero slider
function createSlider(slidesSelector, btnLeftId, btnRightId, dotsContainerId) {
    const slides = document.querySelectorAll(slidesSelector);
    const btnLeft = document.getElementById(btnLeftId);
    const btnRight = document.getElementById(btnRightId);
    const dotContainer = document.getElementById(dotsContainerId);
    if (!slides.length || !btnLeft || !btnRight) return;

    let cur = 0;
    const max = slides.length;

    if (dotContainer) {
        slides.forEach((_, i) => {
            const btn = document.createElement('button');
            btn.className = 'dots__dot-hmPg' + (i === 0 ? ' dots__dot-hmPg--active' : '');
            btn.dataset.slide = i;
            dotContainer.appendChild(btn);
        });
        dotContainer.addEventListener('click', e => {
            if (e.target.classList.contains('dots__dot-hmPg')) {
                goTo(parseInt(e.target.dataset.slide));
            }
        });
    }

    function goTo(n) {
        cur = (n + max) % max;
        slides.forEach((s, i) => s.style.transform = `translateX(${100 * (i - cur)}%)`);
        if (dotContainer) {
            dotContainer.querySelectorAll('.dots__dot-hmPg').forEach(d => d.classList.remove('dots__dot-hmPg--active'));
            const active = dotContainer.querySelector(`[data-slide="${cur}"]`);
            if (active) active.classList.add('dots__dot-hmPg--active');
        }
    }

    // Position slides instantly on first paint (no slide-through animation)
    slides.forEach(s => { s.style.transition = 'none'; });
    goTo(0);
    requestAnimationFrame(() => requestAnimationFrame(() => {
        slides.forEach(s => { s.style.transition = ''; });
    }));

    // Auto-advance every 5s; pause while hovering the slider
    let timer = setInterval(() => goTo(cur + 1), 5000);
    function resetTimer() { clearInterval(timer); timer = setInterval(() => goTo(cur + 1), 5000); }

    const container = document.getElementById('slider-hmPg-Container');
    if (container) {
        container.addEventListener('mouseenter', () => clearInterval(timer));
        container.addEventListener('mouseleave', resetTimer);
    }

    btnLeft.addEventListener('click', () => { goTo(cur - 1); resetTimer(); });
    btnRight.addEventListener('click', () => { goTo(cur + 1); resetTimer(); });
    document.addEventListener('keydown', e => {
        if (e.key === 'ArrowLeft') { goTo(cur - 1); resetTimer(); }
        if (e.key === 'ArrowRight') { goTo(cur + 1); resetTimer(); }
    });
}

// Generic product/jewellery category slider
function createProductSlider(slidesId, prevId, nextId) {
    const slidesEl = document.getElementById(slidesId);
    const prev = document.getElementById(prevId);
    const next = document.getElementById(nextId);
    if (!slidesEl || !prev || !next) return;

    const slides = slidesEl.querySelectorAll('.productCard-slide, .jewelleryCategory-slide');
    if (!slides.length) return;
    let cur = 0;
    const max = slides.length;

    function goTo(n) {
        cur = (n + max) % max;
        slidesEl.style.transform = `translateX(-${cur * 100}%)`;
    }

    prev.addEventListener('click', () => goTo(cur - 1));
    next.addEventListener('click', () => goTo(cur + 1));
}

// Product details: slide the sticky bar in when the header info is out of view, out when back in.
function initProductStickyBar() {
    const bar = document.getElementById('pd-sticky-bar');
    const sentinel = document.getElementById('pd-header-sentinel');
    if (!bar || !sentinel) return;
    // Avoid double-binding on re-render
    if (bar._observerBound) return;
    bar._observerBound = true;

    const io = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            // Show bar when the sentinel is NOT intersecting (i.e. scrolled past the product header)
            if (entry.isIntersecting) bar.classList.remove('is-visible');
            else                       bar.classList.add('is-visible');
        });
    }, { threshold: 0, rootMargin: '-60px 0px 0px 0px' });
    io.observe(sentinel);
}

function initHomePage() {
    // Hero slider
    createSlider('.slide-hmPg', 'sliderBtnLeft', 'sliderBtnRight', 'sliderDots');
    // Watch product slider
    createProductSlider('watchSlides', 'watchSlidePrev', 'watchSlideNext');
    // Jewellery product slider
    createProductSlider('jewellerySlides', 'jewellerySlidePrev', 'jewellerySlideNext');
    // Jewellery category slider
    createProductSlider('jewCatSlides', 'jewCatPrev', 'jewCatNext');
}
