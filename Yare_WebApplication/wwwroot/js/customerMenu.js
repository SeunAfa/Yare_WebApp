//////////////////////////////
//Show Tab SubMenu Component
const header = document.querySelector("header");
const mainContent = document.querySelector(".main-content");
const subMenu_Links = document.querySelectorAll(".subMenu-Links");
const subMenu_TabsContainer = document.querySelectorAll(".subMenu-TabsContainer");
const subMenu_Tabs = document.getElementById("subMenu-Tabs");
const subMenuTabs_Content = document.querySelectorAll(".subMenuTabsContent");
const searchBtn = document.getElementById("searchBtn");

const yareSearch_subMenu_contentContainer = $("#yareSearch-subMenu-topTenCollectionContainer");
const searchLiveResultsContainer = $("#tab-searchLiveResultsContainer");

let isAnimating = false;
let animationTimeout;

const showSubMenu_Tab = (e) => {
    const clicked = e.target.closest(".subMenu-Links");

    if (!clicked) {
        // Interrupt ongoing animation
        if (isAnimating) {
            clearTimeout(animationTimeout);
            isAnimating = false;
        }

        mainContent.classList.remove("main-contentActive");
        subMenu_Links.forEach(l => l.classList.remove("subMenu-linksActive"));
        subMenu_Tabs.classList.remove("subMenu-Tabs_Active");
        subMenu_Tabs.classList.remove("search-subMenu-Tabs_Active");
        subMenu_TabsContainer.forEach(l => l.classList.remove("subMenu-TabsContainer_Active"));
        subMenuTabs_Content.forEach(content => content.classList.remove("subMenuTabsContent_Active"));
        yareSearch_subMenu_contentContainer.show();
        searchLiveResultsContainer.hide();
        searchInput.value = "";

        return;
    } else {
        // Check if the clicked tab is already active
        if (clicked.classList.contains("subMenu-linksActive")) {
            return; // Do nothing if the tab is already active
        }

        // Remove active class from all links and tab containers
        subMenu_Links.forEach(l => l.classList.remove("subMenu-linksActive"));
        subMenu_TabsContainer.forEach(l => l.classList.remove("subMenu-TabsContainer_Active"));
        subMenuTabs_Content.forEach(content => content.classList.remove("subMenuTabsContent_Active"));

        // Add active class to the clicked link
        clicked.classList.add("subMenu-linksActive");

        // Add active class to the corresponding tab container
        const activeTabContainer = document.querySelector(`.subMenu-TabsContainer-${clicked.dataset.tab}`);
        activeTabContainer.classList.add("subMenu-TabsContainer_Active");

        // Add Activate Content
        mainContent.classList.add("main-contentActive");

        // Only add subMenu-Tabs_Active if it is not already active
        if (!subMenu_Tabs.classList.contains("subMenu-Tabs_Active")) {
            subMenu_Tabs.classList.add("subMenu-Tabs_Active");
        }

        // Handle the special case for the search button
        if (clicked === searchBtn) {
            subMenu_Tabs.classList.remove("subMenu-Tabs_Active");
            subMenu_Tabs.classList.add("search-subMenu-Tabs_Active");
        } else {
            subMenu_Tabs.classList.remove("search-subMenu-Tabs_Active");
        }

        // Animation and content activation
        animationTimeout = setTimeout(() => {
            subMenuTabs_Content.forEach(content => content.classList.add("subMenuTabsContent_Active"));
            isAnimating = false;

            const yareSearch_scrollContainer = document.getElementById("yareSearch-subMenu-contentContainer");
            yareSearch_scrollContainer.scrollTop = 0;
        }, 350);

        isAnimating = true;
    }
};

const menuSub_btnContainer = document.querySelector("#subMenu-Navigation");
menuSub_btnContainer.addEventListener("click", showSubMenu_Tab);

/////////////////////////////////////////
//Scroll Nav To Top Component
document.querySelectorAll('.subMenu-Links').forEach(function (button) {
    button.addEventListener('click', function () {
        hideShoppingCartShowSubMenu();
        // Scroll #subMenu-Navigation to the top of the document
        const subMenuNavigation = document.getElementById("subMenu-Navigation");
        subMenuNavigation.scrollIntoView({ behavior: "smooth" });
    });
});


/////////////////////////////////////////////
// Hide Shoppingcart Show SubMenu Component
function hideShoppingCartShowSubMenu() {
    const element = document.querySelector('#shoppingBag-Container');
    const property = 'display';
    const value = 'block';

    const style = window.getComputedStyle(element);

    // Check if the property exists and has the specified value
    if (style.getPropertyValue(property) === value) {
        //console.log(`The element has ${property}: ${value}`);
        menuSub_btnContainer.addEventListener("click", showSubMenu_Tab);

        gsap.fromTo("#shoppingBag-Container", {
            x: 0
        }, {
            x: 2000,
            duration: 0.60,
            onComplete: function () {

                shoppingBag_Container.style.display = "none";

                const subMenuNavigation = document.getElementById("subMenu-Navigation");
                subMenuNavigation.scrollIntoView({ behavior: "smooth" });
            },
        });

        detailsPgAddToCartBtnScrollTriggers();
    } else {
        //console.log(`The element does not have ${property}: ${value}`);
    }
}

//////////////////////////////
//Hide SubMenu Tab Component
const hideSubMenu_fromMainContent = () => {
    if (!isAnimating) {
        const yareSearch_subMenu_contentContainer = $("#yareSearch-subMenu-contentContainer");
        const searchLiveResultsContainer = $("#tab-searchLiveResultsContainer");

        // Remove Active Content
        mainContent.classList.remove("main-contentActive");
        subMenu_Links.forEach(l => l.classList.remove("subMenu-linksActive"));
        subMenu_TabsContainer.forEach(l => l.classList.remove("subMenu-TabsContainer_Active"));
        subMenu_Tabs.classList.remove("subMenu-Tabs_Active");
        subMenu_Tabs.classList.remove("search-subMenu-Tabs_Active");

        subMenuTabs_Content.forEach(content => content.classList.remove("subMenuTabsContent_Active"));

        yareSearch_subMenu_contentContainer.show();
        searchLiveResultsContainer.hide();
        searchInput.value = "";

        resetSlider();
    }
};

header.addEventListener("mouseleave", hideSubMenu_fromMainContent);

/////////////////////////////////////
// Close Tab SubMenu Component
const closeBtn_subMenuTabsContainer = document.querySelectorAll(".tabClose_Btn");

closeBtn_subMenuTabsContainer.forEach(l => {

    l.addEventListener("click", () => {
        const yareSearch_subMenu_contentContainer = $("#yareSearch-subMenu-contentContainer");
        const searchLiveResultsContainer = $("#tab-searchLiveResultsContainer");

        // Remove Active Content
        mainContent.classList.remove("main-contentActive");
        subMenu_Links.forEach(l => l.classList.remove("subMenu-linksActive"));
        subMenu_TabsContainer.forEach(l => l.classList.remove("subMenu-TabsContainer_Active"));
        subMenu_Tabs.classList.remove("subMenu-Tabs_Active");
        subMenu_Tabs.classList.remove("search-subMenu-Tabs_Active");
        subMenuTabs_Content.forEach(content => content.classList.remove("subMenuTabsContent_Active"));

        yareSearch_subMenu_contentContainer.show();
        searchLiveResultsContainer.hide();
        searchInput.value = "";

        resetSlider();

    });

});

///////////////////////////////////
// Reset Sliders Component
function resetSlider() {

    currentIndex_CollectionSlide = 0;
    showCollectionSlide(currentIndex_CollectionSlide);

}

/////////////////////////////////////////
//RecomendedCollection Slider Component
const yareCollections_slidePrev = document.querySelector(".yareCollections-slide-prev");
const yareCollections_slideNext = document.querySelector(".yareCollections-slide-next");

yareCollections_slidePrev.addEventListener("click", () => {
    prevCollectionSlide();
});

yareCollections_slideNext.addEventListener("click", () => {
    nextCollectionSlide();
});

yareCollections_slidePrev.style.cssText = "display:none;";

let currentIndex_CollectionSlide = 0;

function showCollectionSlide(index) {
    const slides = document.querySelector('.yareCollections-slides');
    const slideWidth = document.querySelector('.yareCollections-slide').offsetWidth;
    currentIndex_CollectionSlide = index;

    if (currentIndex_CollectionSlide === 0) {
        yareCollections_slidePrev.style.cssText = "display:none;";

    } else if (currentIndex_CollectionSlide === 1) {
        yareCollections_slidePrev.style.cssText = "display:block;";
    }

    slides.style.transform = `translateX(-${currentIndex_CollectionSlide * slideWidth}px)`;
}

function prevCollectionSlide() {
    currentIndex_CollectionSlide = (currentIndex_CollectionSlide - 1 + document.querySelectorAll('.yareCollections-slide').length) % document.querySelectorAll('.yareCollections-slide').length;
    showCollectionSlide(currentIndex_CollectionSlide);
}

function nextCollectionSlide() {
    currentIndex_CollectionSlide = (currentIndex_CollectionSlide + 1) % document.querySelectorAll('.yareCollections-slide').length;
    showCollectionSlide(currentIndex_CollectionSlide);
}


///////////////////////////////////////////////////////
//RecomendedCollection slider item hover animation
const yareCollections_eachItem = document.querySelectorAll(".yareCollections-eachItem-Container");
const yareCollections_eachItemTitle = document.querySelectorAll(".yareCollections-eachItem-Title");
const yareCollections_eachItemLink = document.querySelectorAll(".yareCollections-eachItem-Link");
const yareCollections_eachItemImg = document.querySelectorAll(".yareCollections-eachItem-Img");

yareCollections_eachItem.forEach((item, index) => {
    item.addEventListener("mouseover", () => {
        yareCollections_eachItemTitle[index].classList.add("yareCollections-eachItem-Title-Active");
        yareCollections_eachItemLink[index].classList.add("yareCollections-eachItem-Link-Active");
        yareCollections_eachItemImg[index].classList.add("yareCollections-eachItem-Img-Active");
    });

    item.addEventListener("mouseleave", () => {
        yareCollections_eachItemTitle[index].classList.remove("yareCollections-eachItem-Title-Active");
        yareCollections_eachItemLink[index].classList.remove("yareCollections-eachItem-Link-Active");
        yareCollections_eachItemImg[index].classList.remove("yareCollections-eachItem-Img-Active");
    });
});

///////////////////////////////////////////
////Jewellery Hover Component
const yareJewellery_eachImgRightContainers = document.querySelectorAll(".yareJewellery-eachImgRight-imgContainer");
const yareJewellery_eachImgs = document.querySelectorAll(".yareJewellery-eachImg");

yareJewellery_eachImgRightContainers.forEach((item, index) => {
    item.addEventListener("mouseover", () => {
        yareJewellery_eachImgs[index].classList.add("yareJewellery-eachImg-Active");
    });

    item.addEventListener("mouseleave", () => {
        yareJewellery_eachImgs[index].classList.remove("yareJewellery-eachImg-Active");
    });
});

