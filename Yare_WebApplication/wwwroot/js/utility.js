//////////////////////////
//Product Card Component
const productCards = document.querySelectorAll(".productCard-Container");

productCards.forEach((item, index) => {
    const productCard_Img = item.querySelector(".productCard-imgs");
    const originalSrc = productCard_Img.getAttribute("src");
    const hoverImageUrl = productCard_Img.dataset.hoverImageUrl;

    item.addEventListener("mouseenter", () => {
        if (productCard_Img) {
            productCard_Img.classList.add("productCard-imgs_hover");
            if (hoverImageUrl) {
                productCard_Img.src = hoverImageUrl;
            }
        }
    });

    item.addEventListener("mouseleave", () => {
        if (productCard_Img) {
            productCard_Img.classList.remove("productCard-imgs_hover");
            productCard_Img.src = originalSrc;
        }
    });
});

/////////////////////////////////
//Product Row Sliders Component
function set_Slider(prevButton, nextButton, slidesContainer, slide) {
    const slidePrevElems = document.querySelectorAll(prevButton);
    const slideNextElems = document.querySelectorAll(nextButton);
    const slidesContainers = document.querySelectorAll(slidesContainer);
    const slideElems = document.querySelectorAll(slide);

    slidePrevElems.forEach((slidePrev, index) => {
        const slideNext = slideNextElems[index];
        const slides = slidesContainers[index];
        const individualSlides = slides.querySelectorAll(slide);

        slidePrev.style.cssText = "display:none;";
        let currentIndex = 0;

        function showSlide(index) {
            currentIndex = index;

            if (currentIndex === 0) {
                slidePrev.style.cssText = "display:none;";
            } else {
                slidePrev.style.cssText = "display:block;";
            }

            slides.style.transform = `translateX(-${currentIndex * individualSlides[0].offsetWidth}px)`;
        }

        function prevSlide(event) {
            event.preventDefault();
            currentIndex = (currentIndex - 1 + individualSlides.length) % individualSlides.length;
            showSlide(currentIndex);
        }

        function nextSlide(event) {
            event.preventDefault();
            currentIndex = (currentIndex + 1) % individualSlides.length;
            showSlide(currentIndex);
        }

        function resetSlider() {
            currentIndex = 0;
            showSlide(currentIndex);
        }

        slidePrev.addEventListener("click", prevSlide);
        slideNext.addEventListener("click", nextSlide);

        // Initialize showing the first slide
        showSlide(currentIndex);

        return {
            resetSlider: resetSlider
        };
    });
}

const collectionTabSlider = set_Slider(".yareCollections-slide-prev", ".yareCollections-slide-next", ".yareCollections-slides", ".yareCollections-slide");
const hmPg_jewelleryCategory = set_Slider(".jewelleryCategory-slide-prev", ".jewelleryCategory-slide-next", ".jewelleryCategory-slides", ".jewelleryCategory-slide");
const hmPg_productWatches = set_Slider(".watchProducts-productCard-slide-prev", ".watchProducts-productCard-slide-next", ".watchProducts-productCard-slides", ".watchProducts-productCard-slide");
const hmPg_jewelleryProducts = set_Slider(".jewelleryProduct-productCard-slide-productCard-slide-prev", ".jewelleryProduct-productCard-slide-productCard-slide-next", ".jewelleryProduct-productCard-slides", ".jewelleryProduct-productCard-slide");
const search_bestSellingProducts = set_Slider(".searchProduct-productCard-slide-productCard-slide-prev", ".searchProduct-productCard-slide-productCard-slide-next", ".searchProduct-productCard-slides", ".searchProduct-productCard-slide");

//////////////////////////////
//Back To Top Btn Component
const backToTopBtn = document.querySelector(".backTo-topBtn");

if (backToTopBtn) {
    
    backToTopBtn.addEventListener("click", () => {
        window.scrollTo({ top: 0, behavior: "smooth" });
    });
}

window.addEventListener("scroll", () => {
    if (document.body.scrollTop > 450 || document.documentElement.scrollTop > 450) {
        //console.log("down");
        backToTopBtn.classList.add("backTo-topBtn-Show");
    } else {
        //console.log("up");
        backToTopBtn.classList.remove("backTo-topBtn-Show");
    }
});



