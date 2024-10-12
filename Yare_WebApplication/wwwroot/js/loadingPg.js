//1
const customerLayout = (() => {
    const loadPgContainer = document.querySelector(".loadPg-Container");
    const loadingPgSVG = document.querySelector(".loadingPg-SVG");
    const digitalStoreFrontContainer = document.getElementById("digitalStoreFront-Container");
    const deviceNotSupportedContainer = document.querySelector(".deviceNotSupported-Container");

    if (loadPgContainer && loadingPgSVG && digitalStoreFrontContainer && deviceNotSupportedContainer) {
        let resizeTimeout;

        const resetSVGAnimation = () => {
            loadingPgSVG.classList.remove("loadingPg-finishSVG", "loadingPg-startSVG");
            void loadingPgSVG.offsetHeight;  // Force reflow (retrigger animation)
            loadingPgSVG.classList.add("loadingPg-startSVG");
        };

        const showLoadingPage = () => {
            resetSVGAnimation();  // Reset and reapply the start class
            loadPgContainer.style.display = "flex"; 
            loadPgContainer.style.opacity = 1;
            loadPgContainer.style.transition = "none";  
            digitalStoreFrontContainer.style.display = "none"; 
            deviceNotSupportedContainer.style.display = "none";  
            deviceNotSupportedContainer.style.opacity = 0;  
        };

        const handleResize = () => {
            showLoadingPage();
            clearTimeout(resizeTimeout);
            resizeTimeout = setTimeout(() => {
                window.location.reload();
            }, 500);
        };

        const handleLoad = () => {
            loadingPgSVG.classList.remove("loadingPg-startSVG");
            loadingPgSVG.classList.add("loadingPg-finishSVG");

            loadingPgSVG.addEventListener("animationend", () => {
                loadPgContainer.style.transition = "opacity 1s";
                loadPgContainer.style.opacity = 0;

                loadPgContainer.addEventListener("transitionend", () => {
                    loadPgContainer.style.display = "none";

                    if (window.matchMedia('(max-width: 575.98px)').matches ||
                        window.matchMedia('(min-width: 576px) and (max-width: 767.98px)').matches ||
                        window.matchMedia('(min-width: 768px) and (max-width: 991.98px)').matches) {

                        digitalStoreFrontContainer.style.display = "none";
                        deviceNotSupportedContainer.style.display = "flex";
                        deviceNotSupportedContainer.style.opacity = 0;
                        deviceNotSupportedContainer.style.transition = "opacity 1s";
                        requestAnimationFrame(() => {
                            deviceNotSupportedContainer.style.opacity = 1;
                        });

                    } else if (window.matchMedia('(min-width: 992px) and (max-width: 1199.98px)').matches ||
                        window.matchMedia('(min-width: 1200px) and (max-width: 1399.98px)').matches ||
                        window.matchMedia('(min-width: 1400px)').matches) {

                        deviceNotSupportedContainer.style.display = "none";
                        digitalStoreFrontContainer.style.display = "block";
                        digitalStoreFrontContainer.style.opacity = 0;
                        digitalStoreFrontContainer.style.transition = "opacity 1s";
                        requestAnimationFrame(() => {
                            digitalStoreFrontContainer.style.opacity = 1;
                        });

                        executeExternal_animationEffect();
                    }
                }, { once: true });
            }, { once: true });
        };

        const digitalStoreLoadPg = (event) => {
            if (event.type === 'resize') {
                handleResize();
            } else if (event.type === 'load') {
                handleLoad();
            } else if (event.type === 'beforeunload') {
                showLoadingPage();  // Show the loading page immediately before unload
            }
        };

        // Adding event listeners
        window.addEventListener('resize', digitalStoreLoadPg);
        window.addEventListener('load', digitalStoreLoadPg);
        window.addEventListener('beforeunload', digitalStoreLoadPg);

        // Execute function for on pages
        function executeExternal_animationEffect() {
            if (typeof detailsPgZoomScrollTriggers === 'function') {
                detailsPgZoomScrollTriggers();
            }
            if (typeof detailsPgAddToCartBtnScrollTriggers === 'function') {
                detailsPgAddToCartBtnScrollTriggers();
            }
            if (typeof homePgScrollEffect === 'function') {
                homePgScrollEffect();
            }
            if (typeof collectionPgScrollEffect === 'function') {
                collectionPgScrollEffect();
            }
        }
    }
});

customerLayout();

//2
const adminLayout = (() => {
    const loadPgContainer = document.querySelector(".loadPg-Container");
    const loadingPgSVG = document.querySelector(".loadingPg-SVG");
    const digitalStoreFrontContainer = document.querySelector("#admin-mainView");
    const deviceNotSupportedContainer = document.querySelector(".admin-deviceNotSupported-Container");

    if (loadPgContainer && loadingPgSVG && digitalStoreFrontContainer && deviceNotSupportedContainer) {
        let resizeTimeout;

        const resetSVGAnimation = () => {
            loadingPgSVG.classList.remove("loadingPg-finishSVG", "loadingPg-startSVG");
            void loadingPgSVG.offsetHeight;  // Force reflow (retrigger animation)
            loadingPgSVG.classList.add("loadingPg-startSVG");
        };

        const showLoadingPage = () => {
            resetSVGAnimation();  
            loadPgContainer.style.display = "flex";  
            loadPgContainer.style.opacity = 1;
            loadPgContainer.style.transition = "none"; 
            digitalStoreFrontContainer.style.display = "none";  
            deviceNotSupportedContainer.style.display = "none"; 
            deviceNotSupportedContainer.style.opacity = 0; 
        };

        const handleResize = () => {
            showLoadingPage();
            clearTimeout(resizeTimeout);
            resizeTimeout = setTimeout(() => {
                window.location.reload();
            }, 500);
        };

        const handleLoad = () => {
            loadingPgSVG.classList.remove("loadingPg-startSVG");
            loadingPgSVG.classList.add("loadingPg-finishSVG");

            loadingPgSVG.addEventListener("animationend", () => {
                loadPgContainer.style.transition = "opacity 1s";
                loadPgContainer.style.opacity = 0;

                loadPgContainer.addEventListener("transitionend", () => {
                    loadPgContainer.style.display = "none";

                    if (window.matchMedia('(max-width: 575.98px)').matches ||
                        window.matchMedia('(min-width: 576px) and (max-width: 767.98px)').matches ||
                        window.matchMedia('(min-width: 768px) and (max-width: 991.98px)').matches) {

                        digitalStoreFrontContainer.style.display = "none";
                        deviceNotSupportedContainer.style.display = "flex";
                        deviceNotSupportedContainer.style.opacity = 0;
                        deviceNotSupportedContainer.style.transition = "opacity 1s";
                        requestAnimationFrame(() => {
                            deviceNotSupportedContainer.style.opacity = 1;
                        });

                    } else if (window.matchMedia('(min-width: 992px) and (max-width: 1199.98px)').matches ||
                        window.matchMedia('(min-width: 1200px) and (max-width: 1399.98px)').matches ||
                        window.matchMedia('(min-width: 1400px)').matches) {

                        deviceNotSupportedContainer.style.display = "none";
                        digitalStoreFrontContainer.style.display = "flex";
                        digitalStoreFrontContainer.style.opacity = 0;
                        digitalStoreFrontContainer.style.transition = "opacity 1s";
                        requestAnimationFrame(() => {
                            digitalStoreFrontContainer.style.opacity = 1;
                        });

                        executeExternal_animationEffect();
                    }
                }, { once: true });
            }, { once: true });
        };

        const digitalStoreLoadPg = (event) => {
            if (event.type === 'resize') {
                handleResize();
            } else if (event.type === 'load') {
                handleLoad();
            } else if (event.type === 'beforeunload') {
                showLoadingPage();  // Show the loading page immediately before unload
            }
        };

        // Adding event listeners
        window.addEventListener('resize', digitalStoreLoadPg);
        window.addEventListener('load', digitalStoreLoadPg);
        window.addEventListener('beforeunload', digitalStoreLoadPg);

        // Execute function for on pages
        function executeExternal_animationEffect() {
            if (typeof detailsPgZoomScrollTriggers === 'function') {
                detailsPgZoomScrollTriggers();
            }
            if (typeof detailsPgAddToCartBtnScrollTriggers === 'function') {
                detailsPgAddToCartBtnScrollTriggers();
            }
            if (typeof homePgScrollEffect === 'function') {
                homePgScrollEffect();
            }
            if (typeof collectionPgScrollEffect === 'function') {
                collectionPgScrollEffect();
            }
        }
    }
});

adminLayout();

//3
const identityLayout = (() => {
    const loadPgContainer = document.querySelector(".loadPg-Container");
    const loadingPgSVG = document.querySelector(".loadingPg-SVG");
    const digitalStoreFrontContainer = document.querySelector(".identity-mainView");
    const deviceNotSupportedContainer = document.querySelector(".identity-deviceNotSupported-Container");

    if (loadPgContainer && loadingPgSVG && digitalStoreFrontContainer && deviceNotSupportedContainer) {
        let resizeTimeout;

        const resetSVGAnimation = () => {
            loadingPgSVG.classList.remove("loadingPg-finishSVG", "loadingPg-startSVG");
            void loadingPgSVG.offsetHeight;  // Force reflow (retrigger animation)
            loadingPgSVG.classList.add("loadingPg-startSVG");
        };

        const showLoadingPage = () => {
            resetSVGAnimation();  // Reset and reapply the start class
            loadPgContainer.style.display = "flex";
            loadPgContainer.style.opacity = 1; 
            loadPgContainer.style.transition = "none"; 
            digitalStoreFrontContainer.style.display = "none"; 
            deviceNotSupportedContainer.style.display = "none"; 
            deviceNotSupportedContainer.style.opacity = 0;  
        };

        const handleResize = () => {
            showLoadingPage();
            clearTimeout(resizeTimeout);
            resizeTimeout = setTimeout(() => {
                window.location.reload();
            }, 500);
        };

        const handleLoad = () => {
            loadingPgSVG.classList.remove("loadingPg-startSVG");
            loadingPgSVG.classList.add("loadingPg-finishSVG");

            loadingPgSVG.addEventListener("animationend", () => {
                loadPgContainer.style.transition = "opacity 1s";
                loadPgContainer.style.opacity = 0;

                loadPgContainer.addEventListener("transitionend", () => {
                    loadPgContainer.style.display = "none";

                    // Ensure digitalStoreFrontContainer is visible when loading is complete
                    if (window.matchMedia('(max-width: 575.98px)').matches ||
                        window.matchMedia('(min-width: 576px) and (max-width: 767.98px)').matches ||
                        window.matchMedia('(min-width: 768px) and (max-width: 991.98px)').matches) {

                        digitalStoreFrontContainer.style.display = "none";
                        deviceNotSupportedContainer.style.display = "flex";
                        deviceNotSupportedContainer.style.opacity = 0;
                        deviceNotSupportedContainer.style.transition = "opacity 1s";
                        requestAnimationFrame(() => {
                            deviceNotSupportedContainer.style.opacity = 1;
                        });

                    } else if (window.matchMedia('(min-width: 992px) and (max-width: 1199.98px)').matches ||
                        window.matchMedia('(min-width: 1200px) and (max-width: 1399.98px)').matches ||
                        window.matchMedia('(min-width: 1400px)').matches) {

                        deviceNotSupportedContainer.style.display = "none";
                        digitalStoreFrontContainer.style.display = "flex";  // Ensure main view is shown
                        digitalStoreFrontContainer.style.opacity = 0;
                        digitalStoreFrontContainer.style.transition = "opacity 1s";
                        requestAnimationFrame(() => {
                            digitalStoreFrontContainer.style.opacity = 1;
                        });

                        executeExternal_animationEffect();
                    }
                }, { once: true });
            }, { once: true });
        };

        const digitalStoreLoadPg = (event) => {
            if (event.type === 'resize') {
                handleResize();
            } else if (event.type === 'load') {
                handleLoad();
            } else if (event.type === 'beforeunload') {
                showLoadingPage();  // Show the loading page immediately before unload
            }
        };

        // Adding event listeners
        window.addEventListener('resize', digitalStoreLoadPg);
        window.addEventListener('load', digitalStoreLoadPg);
        window.addEventListener('beforeunload', digitalStoreLoadPg);

        // Execute function for on pages
        function executeExternal_animationEffect() {
            if (typeof detailsPgZoomScrollTriggers === 'function') {
                detailsPgZoomScrollTriggers();
            }
            if (typeof detailsPgAddToCartBtnScrollTriggers === 'function') {
                detailsPgAddToCartBtnScrollTriggers();
            }
            if (typeof homePgScrollEffect === 'function') {
                homePgScrollEffect();
            }
            if (typeof collectionPgScrollEffect === 'function') {
                collectionPgScrollEffect();
            }
        }
    }
});

identityLayout();
