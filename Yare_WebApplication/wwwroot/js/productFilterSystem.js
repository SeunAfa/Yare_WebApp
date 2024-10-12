//Show & Hide Filter System
const productCount_Container = document.querySelector(".productCount-Container");
const filter_Btn = document.querySelector(".filter-Btn");
const activeFilter_Btn = document.querySelector(".fileterHeader-controlsContainer");
const filterProp_Container = document.querySelector(".filetersProperties-Container");
const productsList_filterContainer = document.querySelector("#productsList-filterContainer ");
const close_Btn = document.querySelector(".close-Btn");

//Show filters
filter_Btn.addEventListener("click", () => {

    gsap.fromTo([filterProp_Container, activeFilter_Btn, productCount_Container], {
        x: 2000
    }, {
        x: 0,
        duration: 0.25,
        onStart: function () {
            productsList_filterContainer.style.display = "block";
            filterProp_Container.style.display = "block";
            activeFilter_Btn.style.display = "flex";
        },
    });

    filter_Btn.style.display = "none";

});

//Hide filters
close_Btn.addEventListener("click", () => {

    gsap.fromTo([filterProp_Container, activeFilter_Btn, filter_Btn, productCount_Container], {
        x: 2000
    }, {
        x: 0,
        duration: 0.25,
        onStart: function () {
            productsList_filterContainer.style.display = "none";
            filterProp_Container.style.display = "none";
            activeFilter_Btn.style.display = "none";
        },
    });

    filter_Btn.style.display = "block";

});

//Toggle property short or full list 
function showHide_filterProp(seeMore_Btn, seeLess_Btn, fullList_filterProperty) {

    function showFullList() {
        const fullListElement = document.querySelector(fullList_filterProperty);
        const seeMoreElement = document.querySelector(seeMore_Btn);
        const seeLessElement = document.querySelector(seeLess_Btn);

        if (fullListElement) fullListElement.style.display = "flex";
        if (seeMoreElement) seeMoreElement.style.display = "none";
        if (seeLessElement) seeLessElement.style.display = "flex";
    }

    function showShortList() {
        const fullListElement = document.querySelector(fullList_filterProperty);
        const seeMoreElement = document.querySelector(seeMore_Btn);
        const seeLessElement = document.querySelector(seeLess_Btn);

        if (fullListElement) fullListElement.style.display = "none";
        if (seeMoreElement) seeMoreElement.style.display = "flex";
        if (seeLessElement) seeLessElement.style.display = "none";
    }

    const seeMoreElement = document.querySelector(seeMore_Btn);
    const seeLessElement = document.querySelector(seeLess_Btn);

    if (seeMoreElement) seeMoreElement.addEventListener("click", showFullList);
    if (seeLessElement) seeLessElement.addEventListener("click", showShortList);
}

const Brands = showHide_filterProp(".seeMore-brand-Btn", ".seeLess-brand-Btn", "#fullList-brandsWrapper");
const WatchDiameter = showHide_filterProp(".seeMore-watchDiameter-Btn", ".seeLess-watchDiameter-Btn", "#fullList-watchDiameterWrapper");
const WatchMovement = showHide_filterProp(".seeMore-watchMovement-Btn", ".seeLess-watchMovement-Btn", "#fullList-watchMovementWrapper");
const ByGemstone = showHide_filterProp(".seeMore-byGemstone-Btn", ".seeLess-byGemstone-Btn", "#fullList-byGemstoneWrapper");
const WatchCaseShape = showHide_filterProp(".seeMore-watchCaseShape-Btn", ".seeLess-watchCaseShape-Btn", "#fullList-watchCaseShapeWrapper");
const WatchStrapType = showHide_filterProp(".seeMore-watchStrapType-Btn", ".seeLess-watchStrapType-Btn", "#fullList-watchStrapTypeWrapper");
const ByOccassion = showHide_filterProp(".seeMore-byOccassion-Btn", ".seeLess-byOccassion-Btn", "#fullList-byOccassionWrapper");
const JewelleryCategory = showHide_filterProp(".seeMore-jewelleryCategory-Btn", ".seeLess-jewelleryCategory-Btn", "#fullList-jewelleryCategoryWrapper");
const AccessoryCategory = showHide_filterProp(".seeMore-accessoryCategory-Btn", ".seeLess-accessoryCategory-Btn", "#fullList-accessoryCategoryWrapper");


document.addEventListener("DOMContentLoaded", function () {
    const genderFilter_Btn = document.querySelectorAll(".genderFilter-Btn");
    const brandCheckboxes = document.querySelectorAll(".brandCheckbox");
    const metalItem_Btns = document.querySelectorAll(".metalItem-Btn");
    const metalItem_Img = document.querySelectorAll(".metalItem-Img");
    const watchDiameterFilter_Btn = document.querySelectorAll(".watchDiameterFilter-Btn");
    const watchMovementCheckbox = document.querySelectorAll(".watchMovementCheckbox");
    const waterResistantCheckbox = document.querySelectorAll(".waterResistantCheckbox");
    const watchCaseShapeCheckbox = document.querySelectorAll(".watchCaseShapeCheckbox");
    const watchStrapTypeCheckbox = document.querySelectorAll(".watchStrapTypeCheckbox");
    const productByOccassionCheckbox = document.querySelectorAll(".productbyOccassionCheckbox");
    const accessoryCategoryCheckbox = document.querySelectorAll(".accessoryCategoryCheckbox");
    const jewelleryCategoryCheckbox = document.querySelectorAll(".jewelleryCategoryCheckbox");
    const byGemstoneCheckbox = document.querySelectorAll(".byGemstoneCheckbox");
    const productCategoryCheckboxes = document.querySelectorAll('.productCategoryCheckbox');
    const priceRangeInput = document.getElementById("priceRange");
    const priceRangeLabel = document.getElementById("priceRangeLabel");
    const noFilterResulst_Container = document.querySelector(".noFilterResulst-Container");

    const products = document.querySelectorAll(".productCard-Container");

    //Property filter counts
    let genderCount = 0;
    let brandCount = 0;
    let metalCount = 0;
    let watchDiameterCount = 0;
    let watchMovementCount = 0;
    let waterResistantCount = 0;
    let watchCaseShapeCount = 0;
    let watchStrapTypeCount = 0;
    let productByOccassionCount = 0;
    let priceCount = 0;
    let accessoryCategoryCount = 0;
    let jewelleryCategoryCount = 0;
    let byGemstoneCount = 0;
    let productCategoryCount = 0;

    //Show Applied Filters Count
    function appliedFilters_totalCount() {//+ byGemstoneCount
        let appliedFilters_Count = genderCount + brandCount + metalCount + watchDiameterCount + watchMovementCount + waterResistantCount + watchCaseShapeCount + watchStrapTypeCount + productByOccassionCount + priceCount + accessoryCategoryCount + jewelleryCategoryCount + byGemstoneCount + productCategoryCount;

        if (appliedFilters_Count > 0) {
            document.querySelector(".appliedFilters-Text").textContent = appliedFilters_Count;
            document.querySelector(".appliedFilters-Text").style.display = "inline";
            console.log("appliedFilters_Count", appliedFilters_Count);
        } else {
            document.querySelector(".appliedFilters-Text").style.display = "none";
        }
    }

    appliedFilters_totalCount();

    //Clear all filters applied
    function clearAllFilters() {
        // Reset filter counts
        genderCount = 0;
        brandCount = 0;
        metalCount = 0;
        watchDiameterCount = 0;
        watchMovementCount = 0;
        waterResistantCount = 0;
        watchCaseShapeCount = 0;
        watchStrapTypeCount = 0;
        productByOccassionCount = 0;
        accessoryCategoryCount = 0;
        jewelleryCategoryCount = 0;
        byGemstoneCount = 0;
        productCategoryCount = 0;

        // Remove active states from filter buttons and checkboxes
        genderFilter_Btn.forEach((item) => {
            item.classList.remove("genderFilter-btnActive");
        });
        brandCheckboxes.forEach((checkbox) => {
            checkbox.checked = false;
        });
        metalItem_Btns.forEach((item) => {
            item.classList.remove("metalItem-BtnActive");
            item.querySelector(".metalItem-Img").classList.remove("metalItem-imgActive");
        });
        watchDiameterFilter_Btn.forEach((item) => {
            item.classList.remove("watchDiameterFilter-btnActive");
        });
        watchMovementCheckbox.forEach((checkbox) => {
            checkbox.checked = false;
        });
        waterResistantCheckbox.forEach((checkbox) => {
            checkbox.checked = false;
        });
        watchCaseShapeCheckbox.forEach((checkbox) => {
            checkbox.checked = false;
        });
        watchStrapTypeCheckbox.forEach((checkbox) => {
            checkbox.checked = false;
        });
        productByOccassionCheckbox.forEach((checkbox) => {
            checkbox.checked = false;
        });
        accessoryCategoryCheckbox.forEach((checkbox) => {
            checkbox.checked = false;
        });
        jewelleryCategoryCheckbox.forEach((checkbox) => {
            checkbox.checked = false;
        });
        byGemstoneCheckbox.forEach((checkbox) => {
            checkbox.checked = false;
        });
        productCategoryCheckboxes.forEach((checkbox) => {
            checkbox.checked = false;
        });

        resetPriceRangeInput();


        // Show all products
        products.forEach((product) => {
            product.style.display = "block";
        });

        // Hide no results container
        noFilterResulst_Container.style.display = "none";

        // Update product count
        document.querySelector(".productCount-Initial").style.display = "inline";
        document.querySelector(".productCount-Results").style.display = "none";
        document.querySelector(".productCount-Initial").innerHTML = products.length;

        // Update applied filters count
        appliedFilters_totalCount();
  
    }

    //Reset price range slider
    function resetPriceRangeInput() {
        priceRangeInput.value = priceRangeInput.max;
        priceRangeLabel.textContent = formatPrice(priceRangeInput.value);
        priceCount = 0;
    }

    // Call clearAllFilters function when needed
    const clearFiltersBtn = document.querySelector(".clearAll-Btn");
    clearFiltersBtn.addEventListener("click", clearAllFilters);

    // Gender filter
    genderFilter_Btn.forEach((item) => {
        item.addEventListener("click", () => {
            const selectedGender = item.getAttribute("data-value");
            const isActive = item.classList.contains("genderFilter-btnActive");

            if (isActive) {
                item.classList.remove("genderFilter-btnActive");
                genderCount = 0;
            } else {
                genderFilter_Btn.forEach((btn) => {
                    btn.classList.remove("genderFilter-btnActive");
                });
                item.classList.add("genderFilter-btnActive");
                genderCount = 1;
            }
            console.log("gender Count", genderCount);
            appliedFilters_totalCount();

            filterProducts(
                getSelectedGender(), getSelectedBrands(), getSelectedMetals(),
                getSelectedWatchDiameters(), getSelectedWatchMovement(), getSelectedWaterResistant(),
                getSelectedWatchCaseShape(), getSelectedWatchStrapType(), getSelectedProductByOccassion(),
                getSelectedPriceRange(), getSelectedAccessoryCategory(), getSelectedJewelleryCategory(),
                getSelectedByGemstone(), getSelectedProductCategory()
            );

        });
    });

    // Brand filter
    brandCheckboxes.forEach((checkbox) => {
        checkbox.addEventListener("change", () => {

            if (checkbox.checked) {
                brandCount++;
            } else {
                brandCount--;
            }

            console.log("Brand count:", brandCount);
            appliedFilters_totalCount();


            filterProducts(
                getSelectedGender(), getSelectedBrands(), getSelectedMetals(),
                getSelectedWatchDiameters(), getSelectedWatchMovement(), getSelectedWaterResistant(),
                getSelectedWatchCaseShape(), getSelectedWatchStrapType(), getSelectedProductByOccassion(),
                getSelectedPriceRange(), getSelectedAccessoryCategory(), getSelectedJewelleryCategory(),
                getSelectedByGemstone(), getSelectedProductCategory()
            );
        });
    });

    // Metal filter
    metalItem_Btns.forEach((item) => {
        item.addEventListener("click", () => {
            const selectedMetal = item.getAttribute("data-value");
            const isActive = item.classList.contains("metalItem-BtnActive");

            if (isActive) {
                item.classList.remove("metalItem-BtnActive");
                item.querySelector(".metalItem-Img").classList.remove("metalItem-imgActive");
                metalCount--;
            } else {
                item.classList.add("metalItem-BtnActive");
                item.querySelector(".metalItem-Img").classList.add("metalItem-imgActive");
                metalCount++;
            }
            console.log("Metal count:", metalCount);
            appliedFilters_totalCount();


            filterProducts(
                getSelectedGender(), getSelectedBrands(), getSelectedMetals(),
                getSelectedWatchDiameters(), getSelectedWatchMovement(), getSelectedWaterResistant(),
                getSelectedWatchCaseShape(), getSelectedWatchStrapType(), getSelectedProductByOccassion(),
                getSelectedPriceRange(), getSelectedAccessoryCategory(), getSelectedJewelleryCategory(),
                getSelectedByGemstone(), getSelectedProductCategory()
            );
        });
    });

    // Watch Diameter filter
    watchDiameterFilter_Btn.forEach((item) => {
        item.addEventListener("click", () => {
            const selectedWatchDiameter = item.getAttribute("data-value");
            const isActive = item.classList.contains("watchDiameterFilter-btnActive");

            if (isActive) {
                item.classList.remove("watchDiameterFilter-btnActive");
                watchDiameterCount--;
            } else {
                item.classList.add("watchDiameterFilter-btnActive");
                watchDiameterCount++;
            }

            console.log("watch Diameter Count:", watchDiameterCount);
            appliedFilters_totalCount();


            filterProducts(
                getSelectedGender(), getSelectedBrands(), getSelectedMetals(),
                getSelectedWatchDiameters(), getSelectedWatchMovement(), getSelectedWaterResistant(),
                getSelectedWatchCaseShape(), getSelectedWatchStrapType(), getSelectedProductByOccassion(),
                getSelectedPriceRange(), getSelectedAccessoryCategory(), getSelectedJewelleryCategory(),
                getSelectedByGemstone(), getSelectedProductCategory()
            );
        });
    });

    // Watch Movement filter
    watchMovementCheckbox.forEach((checkbox) => {
        checkbox.addEventListener("change", () => {

            if (checkbox.checked) {
                watchMovementCount++;
            } else {
                watchMovementCount--;
            }

            console.log("watch Movement Count:", watchMovementCount);
            appliedFilters_totalCount();


            filterProducts(
                getSelectedGender(), getSelectedBrands(), getSelectedMetals(),
                getSelectedWatchDiameters(), getSelectedWatchMovement(), getSelectedWaterResistant(),
                getSelectedWatchCaseShape(), getSelectedWatchStrapType(), getSelectedProductByOccassion(),
                getSelectedPriceRange(), getSelectedAccessoryCategory(), getSelectedJewelleryCategory(),
                getSelectedByGemstone(), getSelectedProductCategory()
            );

        });
    });

    // Water Resistant filter
    waterResistantCheckbox.forEach((checkbox) => {
        checkbox.addEventListener("change", () => {

            if (checkbox.checked) {
                waterResistantCount++;
            } else {
                waterResistantCount--;
            }

            console.log("water Resistant Count:", waterResistantCount);
            appliedFilters_totalCount();


            filterProducts(
                getSelectedGender(), getSelectedBrands(), getSelectedMetals(),
                getSelectedWatchDiameters(), getSelectedWatchMovement(), getSelectedWaterResistant(),
                getSelectedWatchCaseShape(), getSelectedWatchStrapType(), getSelectedProductByOccassion(),
                getSelectedPriceRange(), getSelectedAccessoryCategory(), getSelectedJewelleryCategory(),
                getSelectedByGemstone(), getSelectedProductCategory()
            );
        });
    });

    // By gemstone filter
    byGemstoneCheckbox.forEach((checkbox) => {
        checkbox.addEventListener("change", () => {

            if (checkbox.checked) {
                byGemstoneCount++;
            } else {
                byGemstoneCount--;
            }

            console.log("by Gemstone Count:", byGemstoneCount);
            appliedFilters_totalCount();

            filterProducts(
                getSelectedGender(), getSelectedBrands(), getSelectedMetals(),
                getSelectedWatchDiameters(), getSelectedWatchMovement(), getSelectedWaterResistant(),
                getSelectedWatchCaseShape(), getSelectedWatchStrapType(), getSelectedProductByOccassion(),
                getSelectedPriceRange(), getSelectedAccessoryCategory(), getSelectedJewelleryCategory(),
                getSelectedByGemstone(), getSelectedProductCategory()
            );

        });
    });

    //Watch case Shape filter
    watchCaseShapeCheckbox.forEach((checkbox) => {
        checkbox.addEventListener("change", () => {

            if (checkbox.checked) {
                watchCaseShapeCount++;
            } else {
                watchCaseShapeCount--;
            }

            console.log("watch Case Shape Count:", watchCaseShapeCount);
            appliedFilters_totalCount();


            filterProducts(
                getSelectedGender(), getSelectedBrands(), getSelectedMetals(),
                getSelectedWatchDiameters(), getSelectedWatchMovement(), getSelectedWaterResistant(),
                getSelectedWatchCaseShape(), getSelectedWatchStrapType(), getSelectedProductByOccassion(),
                getSelectedPriceRange(), getSelectedAccessoryCategory(), getSelectedJewelleryCategory(),
                getSelectedByGemstone(), getSelectedProductCategory()
            );
        });
    });

    //Watch strap Type filter
    watchStrapTypeCheckbox.forEach((checkbox) => {
        checkbox.addEventListener("change", () => {

            if (checkbox.checked) {
                watchStrapTypeCount++;
            } else {
                watchStrapTypeCount--;
            }

            console.log("watch Strap Type Count:", watchStrapTypeCount);
            appliedFilters_totalCount();


            filterProducts(
                getSelectedGender(), getSelectedBrands(), getSelectedMetals(),
                getSelectedWatchDiameters(), getSelectedWatchMovement(), getSelectedWaterResistant(),
                getSelectedWatchCaseShape(), getSelectedWatchStrapType(), getSelectedProductByOccassion(),
                getSelectedPriceRange(), getSelectedAccessoryCategory(), getSelectedJewelleryCategory(),
                getSelectedByGemstone(), getSelectedProductCategory()
            );
        });
    });

    //product By Occassion filter
    productByOccassionCheckbox.forEach((checkbox) => {
        checkbox.addEventListener("change", () => {

            if (checkbox.checked) {
                productByOccassionCount++;
            } else {
                productByOccassionCount--;
            }

            console.log("product By Occassion Count:", productByOccassionCount);
            appliedFilters_totalCount();


            filterProducts(
                getSelectedGender(), getSelectedBrands(), getSelectedMetals(),
                getSelectedWatchDiameters(), getSelectedWatchMovement(), getSelectedWaterResistant(),
                getSelectedWatchCaseShape(), getSelectedWatchStrapType(), getSelectedProductByOccassion(),
                getSelectedPriceRange(), getSelectedAccessoryCategory(), getSelectedJewelleryCategory(),
                getSelectedByGemstone(), getSelectedProductCategory()
            );
        });
    });

    //Price filter
    priceRangeInput.addEventListener("input", () => {

        priceCount = 1;

        appliedFilters_totalCount();
        filterProducts(
            getSelectedGender(), getSelectedBrands(), getSelectedMetals(),
            getSelectedWatchDiameters(), getSelectedWatchMovement(), getSelectedWaterResistant(),
            getSelectedWatchCaseShape(), getSelectedWatchStrapType(), getSelectedProductByOccassion(),
            getSelectedPriceRange(), getSelectedAccessoryCategory(), getSelectedJewelleryCategory(),
            getSelectedByGemstone(), getSelectedProductCategory()
        );

    });

    //Accessory Category filter
    accessoryCategoryCheckbox.forEach((checkbox) => {
        checkbox.addEventListener("change", () => {

            if (checkbox.checked) {
                accessoryCategoryCount++;
            } else {
                accessoryCategoryCount--;
            }

            console.log("Accessory Category Count:", accessoryCategoryCount);
            appliedFilters_totalCount();


            filterProducts(
                getSelectedGender(), getSelectedBrands(), getSelectedMetals(),
                getSelectedWatchDiameters(), getSelectedWatchMovement(), getSelectedWaterResistant(),
                getSelectedWatchCaseShape(), getSelectedWatchStrapType(), getSelectedProductByOccassion(),
                getSelectedPriceRange(), getSelectedAccessoryCategory(), getSelectedJewelleryCategory(),
                getSelectedByGemstone(), getSelectedProductCategory()
            );
        });
    });

    //Jewellery Category filter
    jewelleryCategoryCheckbox.forEach((checkbox) => {
        checkbox.addEventListener("change", () => {

            if (checkbox.checked) {
                jewelleryCategoryCount++;
            } else {
                jewelleryCategoryCount--;
            }

            console.log("Jewellery Category Count:", jewelleryCategoryCount);
            appliedFilters_totalCount();


            filterProducts(
                getSelectedGender(), getSelectedBrands(), getSelectedMetals(),
                getSelectedWatchDiameters(), getSelectedWatchMovement(), getSelectedWaterResistant(),
                getSelectedWatchCaseShape(), getSelectedWatchStrapType(), getSelectedProductByOccassion(),
                getSelectedPriceRange(), getSelectedAccessoryCategory(), getSelectedJewelleryCategory(),
                getSelectedByGemstone(), getSelectedProductCategory()
            );
        });
    });

    //Product Category filter
    productCategoryCheckboxes.forEach((checkbox) => {
        checkbox.addEventListener("change", () => {

            if (checkbox.checked) {
                productCategoryCount++;
            } else {
                productCategoryCount--;
            }

            console.log("Product Category Count:", productCategoryCount);
            appliedFilters_totalCount();


            filterProducts(
                getSelectedGender(), getSelectedBrands(), getSelectedMetals(),
                getSelectedWatchDiameters(), getSelectedWatchMovement(), getSelectedWaterResistant(),
                getSelectedWatchCaseShape(), getSelectedWatchStrapType(), getSelectedProductByOccassion(),
                getSelectedPriceRange(), getSelectedAccessoryCategory(), getSelectedJewelleryCategory(),
                getSelectedByGemstone(), getSelectedProductCategory()
            );
        });
    });

    function filterProducts(selectedGender, selectedBrands, selectedMetals, selectedWatchDiameters, selectedWatchMovement,
        selectedWaterResistant, selectedWatchCaseShape, selectedWatchStrapType, selectedProductByOccassion, selectedPrice,
        selectedAccessoryCategory, selectedJewelleryCategory, selectedByGemstone, selectedProductCategory) {

        let foundCount = 0;
        let filtersApplied = false;

        // Check if any filter has been applied
        if (genderCount !== 0 ||
            (selectedBrands.length !== 0 && !selectedBrands.includes("All")) ||
            (selectedMetals.length !== 0 && !selectedMetals.includes("All")) ||
            (selectedWatchDiameters.length !== 0 && !selectedWatchDiameters.includes("All")) ||
            (selectedWatchMovement.length !== 0 && !selectedWatchMovement.includes("All")) ||
            (selectedWaterResistant.length !== 0 && !selectedWaterResistant.includes("All")) ||
            (selectedWatchCaseShape.length !== 0 && !selectedWatchCaseShape.includes("All")) ||
            (selectedWatchStrapType.length !== 0 && !selectedWatchStrapType.includes("All")) ||
            (selectedProductByOccassion.length !== 0 && !selectedProductByOccassion.includes("All")) ||
            (selectedAccessoryCategory.length !== 0 && !selectedAccessoryCategory.includes("All")) ||
            (selectedJewelleryCategory.length !== 0 && !selectedJewelleryCategory.includes("All")) ||
            (selectedByGemstone.length !== 0 && !selectedByGemstone.includes("All")) ||
            (selectedProductCategory.length !== 0 && !selectedProductCategory.includes("All")) ||
            selectedPrice !== parseFloat(priceRangeInput.max)) {
            filtersApplied = true;
        }

        //for each product get corrosponding mathching applied filters
        products.forEach((product) => {
            const productGender = product.getAttribute("data-gender");
            const productBrand = product.getAttribute("data-brand");
            const productMetal = product.getAttribute("data-metal");
            const productWatchDiameter = product.getAttribute("data-watchDiameter");
            const productWatchMovement = product.getAttribute("data-watchMovement");
            const productWaterResistant = product.getAttribute("data-waterResistant");
            const productWatchCaseShape = product.getAttribute("data-watchCaseShape");
            const productWatchStrapType = product.getAttribute("data-watchStrapType");
            const productByOccassion = product.getAttribute("data-byOccassion");
            const productPrice = parseFloat(product.getAttribute("data-price"));
            const productAccessoryCategory = product.getAttribute("data-accessoryCategory");
            const productJewelleryCategory = product.getAttribute("data-jewelleryCategory");
            const productByGemstone = product.getAttribute("data-byGemstone");
            const productProductCategory = product.getAttribute("data-productCategory");

            const genderMatch = genderCount === 0 || productGender === selectedGender;
            const brandMatch = selectedBrands.includes("All") || selectedBrands.includes(productBrand);
            const metalMatch = selectedMetals.includes("All") || selectedMetals.includes(productMetal);
            const watchDiameterMatch = selectedWatchDiameters.includes("All") || selectedWatchDiameters.includes(productWatchDiameter);
            const watchMovementMatch = selectedWatchMovement.includes("All") || selectedWatchMovement.includes(productWatchMovement);
            const waterResistantMatch = selectedWaterResistant.includes("All") || selectedWaterResistant.includes(productWaterResistant);
            const watchCaseShapeMatch = selectedWatchCaseShape.includes("All") || selectedWatchCaseShape.includes(productWatchCaseShape);
            const watchStrapTypeMatch = selectedWatchStrapType.includes("All") || selectedWatchStrapType.includes(productWatchStrapType);
            const productByOccassionMatch = selectedProductByOccassion.includes("All") || selectedProductByOccassion.includes(productByOccassion);
            const priceMatch = selectedPrice === parseFloat(priceRangeInput.max) || productPrice <= selectedPrice;
            const productAccessoryCategoryMatch = selectedAccessoryCategory.includes("All") || selectedAccessoryCategory.includes(productAccessoryCategory);
            const productJewelleryCategoryMatch = selectedJewelleryCategory.includes("All") || selectedJewelleryCategory.includes(productJewelleryCategory);
            const productByGemstoneMatch = selectedByGemstone.includes("All") || selectedByGemstone.includes(productByGemstone);
            const productProductCategoryMatch = selectedProductCategory.includes("All") || selectedProductCategory.includes(productProductCategory);

            if (genderMatch && brandMatch && metalMatch && watchDiameterMatch && watchMovementMatch && waterResistantMatch && watchCaseShapeMatch
                && watchStrapTypeMatch && productByOccassionMatch && priceMatch && productAccessoryCategoryMatch && productJewelleryCategoryMatch &&
                productByGemstoneMatch && productProductCategoryMatch) {
                product.style.display = "block";
                foundCount++;
            } else {
                product.style.display = "none";
            }
        });

        // Set the display of the noFilterResulst_Container based on foundCount
        if (foundCount > 0) {
            noFilterResulst_Container.style.display = "none";
        } else {
            noFilterResulst_Container.style.display = "block";
        }

        // Log the counts to the console
        console.log("Found", foundCount, "products");

        // Set count to zero if no matches are found
        document.querySelector(".productCount-Results").innerHTML = foundCount > 0 ? foundCount : 0;

        // Hide productCount-Initial if filters are applied
        if (filtersApplied) {
            document.querySelector(".productCount-Initial").style.display = "none";
            document.querySelector(".productCount-Results").style.display = "inline";
        } else {
            document.querySelector(".productCount-Initial").style.display = "inline";
            document.querySelector(".productCount-Results").style.display = "none";
        }

        return foundCount;
    }

    // Get selected gender
    function getSelectedGender() {
        const activeGenderButton = document.querySelector(".genderFilter-btnActive");
        return activeGenderButton ? activeGenderButton.getAttribute("data-value") : "All";
    }

    // Get selected brands
    function getSelectedBrands() {
        const selectedBrands = [];
        brandCheckboxes.forEach((checkbox) => {
            if (checkbox.checked) {
                selectedBrands.push(checkbox.getAttribute("data-value"));
            }
        });
        return selectedBrands.length > 0 ? selectedBrands : ["All"];
    }

    // Get selected metals
    function getSelectedMetals() {
        const selectedMetals = [];
        metalItem_Btns.forEach((btn) => {
            if (btn.classList.contains("metalItem-BtnActive")) {
                selectedMetals.push(btn.getAttribute("data-value"));
            }
        });
        return selectedMetals.length > 0 ? selectedMetals : ["All"];
    }

    // Get selected watch diameters
    function getSelectedWatchDiameters() {
        const selectedWatchDiameters = [];
        watchDiameterFilter_Btn.forEach((btn) => {
            if (btn.classList.contains("watchDiameterFilter-btnActive")) {
                selectedWatchDiameters.push(btn.getAttribute("data-value"));
            }
        });
        return selectedWatchDiameters.length > 0 ? selectedWatchDiameters : ["All"];
    }

    // Get selected watch movement
    function getSelectedWatchMovement() {
        const selectedWatchMovement = [];
        watchMovementCheckbox.forEach((checkbox) => {
            if (checkbox.checked) {
                selectedWatchMovement.push(checkbox.getAttribute("data-value"));
            }
        });
        return selectedWatchMovement.length > 0 ? selectedWatchMovement : ["All"];
    }

    // Get selected water resistant
    function getSelectedWaterResistant() {
        const selectedWaterResistant = [];
        waterResistantCheckbox.forEach((checkbox) => {
            if (checkbox.checked) {
                selectedWaterResistant.push(checkbox.getAttribute("data-value"));
            }
        });
        return selectedWaterResistant.length > 0 ? selectedWaterResistant : ["All"];
    }

    // Get selected by watch case shape
    function getSelectedWatchCaseShape() {
        const selectedWatchCaseShape = [];
        watchCaseShapeCheckbox.forEach((checkbox) => {
            if (checkbox.checked) {
                selectedWatchCaseShape.push(checkbox.getAttribute("data-value"));
            }
        });
        return selectedWatchCaseShape.length > 0 ? selectedWatchCaseShape : ["All"];
    }

    // Get selected by watch strap
    function getSelectedWatchStrapType() {
        const selectedWatchStrapType = [];
        watchStrapTypeCheckbox.forEach((checkbox) => {
            if (checkbox.checked) {
                selectedWatchStrapType.push(checkbox.getAttribute("data-value"));
            }
        });
        return selectedWatchStrapType.length > 0 ? selectedWatchStrapType : ["All"];
    }

    // Get selected by occassion
    function getSelectedProductByOccassion() {
        const selectedProductByOccassion = [];
        productByOccassionCheckbox.forEach((checkbox) => {
            if (checkbox.checked) {
                selectedProductByOccassion.push(checkbox.getAttribute("data-value"));
            }
        });
        return selectedProductByOccassion.length > 0 ? selectedProductByOccassion : ["All"];
    }

    // Price format 
    function formatPrice(value) {
        return "£" + parseFloat(value).toLocaleString("en-GB", { minimumFractionDigits: 2, maximumFractionDigits: 2 });
    }

    // Get selected price
    function getSelectedPriceRange() {

        const selectedPrice = parseFloat(priceRangeInput.value);

        // Function to format the price value

        // Function to update the price range label
        function updatePriceLabel(value) {
            priceRangeLabel.textContent = formatPrice(value);
        }

        updatePriceLabel(selectedPrice);

        return parseFloat(priceRangeInput.value);
    }

    // Get selected accessory category
    function getSelectedAccessoryCategory() {
        const selectedAccessoryCategory = [];
        accessoryCategoryCheckbox.forEach((checkbox) => {
            if (checkbox.checked) {
                selectedAccessoryCategory.push(checkbox.getAttribute("data-value"));
            }
        });
        return selectedAccessoryCategory.length > 0 ? selectedAccessoryCategory : ["All"];
    }

    // Get selected jewellery category
    function getSelectedJewelleryCategory() {
        const selectedJewelleryCategory = [];
        jewelleryCategoryCheckbox.forEach((checkbox) => {
            if (checkbox.checked) {
                selectedJewelleryCategory.push(checkbox.getAttribute("data-value"));
            }
        });
        return selectedJewelleryCategory.length > 0 ? selectedJewelleryCategory : ["All"];
    }

    // Get selected gemstone 
    function getSelectedByGemstone() {
        const selectedByGemstone = [];
        byGemstoneCheckbox.forEach((checkbox) => {
            if (checkbox.checked) {
                selectedByGemstone.push(checkbox.getAttribute("data-value"));
            }
        });
        return selectedByGemstone.length > 0 ? selectedByGemstone : ["All"];
    }

    // Get selected product category
    function getSelectedProductCategory() {
        const selectedProductCategory = [];
        productCategoryCheckboxes.forEach((checkbox) => {
            if (checkbox.checked) {
                selectedProductCategory.push(checkbox.getAttribute("data-value"));
            }
        });
        return selectedProductCategory.length > 0 ? selectedProductCategory : ["All"];
    }

});

const productCategoryCheckboxes = document.querySelectorAll('.productCategoryCheckbox');
const jewelleryCategoryFilterContainer = document.querySelector('.jewelleryCategoryFilter-Container');
const byGemstoneFilterContainer = document.querySelector('.byGemstoneFilter-Container');
const accessoryCategoryFilterContainer = document.querySelector('.accessoryCategoryFilter-Container');
const byOccassionFilterContainers = document.querySelectorAll('.byOccassionFilter-Container');
const watchBrandFilterContainer = document.querySelector('.watchBrandFilter-Container');
const metalFilterContainer = document.querySelector('.metalFilter-Container');
const watchDiameterFilterContainer = document.querySelector('.watchDiameterFilter-Container');
const watchMovementFilterContainer = document.querySelector('.watchMovementFilter-Container');
const waterResistantFilterContainer = document.querySelector('.waterResistantFilter-Container');
const watchCaseShapeFilterContainer = document.querySelector('.watchCaseShapeFilter-Container');
const watchStrapTypeFilterContainer = document.querySelector('.watchStrapTypeFilter-Container');

//Hide & Show filters for product category selected for certain paths
function hideWatchFilterContainers() {
    watchBrandFilterContainer.style.display = "none";
    watchDiameterFilterContainer.style.display = "none";
    metalFilterContainer.style.display = "none";
    watchMovementFilterContainer.style.display = "none";
    waterResistantFilterContainer.style.display = "none";
    watchCaseShapeFilterContainer.style.display = "none";
    watchStrapTypeFilterContainer.style.display = "none";
}

function showWatchFilterContainers() {
    watchBrandFilterContainer.style.display = "block";
    watchDiameterFilterContainer.style.display = "block";
    metalFilterContainer.style.display = "block";
    watchMovementFilterContainer.style.display = "block";
    waterResistantFilterContainer.style.display = "block";
    watchCaseShapeFilterContainer.style.display = "block";
    watchStrapTypeFilterContainer.style.display = "block";
}

function hideJewelleryFilterContainers() {
    jewelleryCategoryFilterContainer.style.display = "none";
    byGemstoneFilterContainer.style.display = "none";
}

function showJewelleryFilterContainers() {
    jewelleryCategoryFilterContainer.style.display = "block";
    byGemstoneFilterContainer.style.display = "block";
}

function hideAccessoryFilterContainers() {
    accessoryCategoryFilterContainer.style.display = "none";
}

function showAccessoryFilterContainers() {
    accessoryCategoryFilterContainer.style.display = "block";
}

function updateOccasionFilters() {
    const isWatchChecked = Array.from(productCategoryCheckboxes).some(checkbox => checkbox.classList.contains('productCategoryCheckbox-Watch') && checkbox.checked);
    const isJewelleryChecked = Array.from(productCategoryCheckboxes).some(checkbox => checkbox.classList.contains('productCategoryCheckbox-Jewellery') && checkbox.checked);

    byOccassionFilterContainers.forEach(container => {
        if (isWatchChecked && isJewelleryChecked) {
            if (container.classList.contains('byOccassionFilter-Jewellery-Container')) {
                container.style.display = "block";
            } else {
                container.style.display = "none";
            }
        } else if (isWatchChecked) {
            if (container.classList.contains('byOccassionFilter-Watches-Container')) {
                container.style.display = "block";
            } else {
                container.style.display = "none";
            }
        } else if (isJewelleryChecked) {
            if (container.classList.contains('byOccassionFilter-Jewellery-Container')) {
                container.style.display = "block";
            } else {
                container.style.display = "none";
            }
        } else {
            container.style.display = "none";
        }
    });
}

function checkedFilter_HideFilterProp() {
    const currentPath = window.location.pathname;

    // Hide filter containers if the path matches
    if (currentPath.includes("Collections/Index/OurTopTenCollection")) {
        hideWatchFilterContainers();
        hideJewelleryFilterContainers();
        hideAccessoryFilterContainers();

        document.querySelector(".clearAll-Btn").addEventListener("click", () => {
            hideWatchFilterContainers();
            hideJewelleryFilterContainers();
            hideAccessoryFilterContainers();

            // Hide byOccassionFilter containers
            document.querySelectorAll('.byOccassionFilter-Container').forEach(container => {
                container.style.display = "none";
            });
            document.querySelectorAll('.byOccassionFilter-Jewellery-Container').forEach(container => {
                container.style.display = "none";
            });
            document.querySelectorAll('.byOccassionFilter-Watches-Container').forEach(container => {
                container.style.display = "none";
            });
        });

    } else if (currentPath.includes("Collections/Index/ExclusiveBrandsCollection")) {
        hideWatchFilterContainers();
        hideJewelleryFilterContainers();
        hideAccessoryFilterContainers();

        document.querySelector(".clearAll-Btn").addEventListener("click", () => {
            hideWatchFilterContainers();
            hideJewelleryFilterContainers();
            hideAccessoryFilterContainers();

            // Hide byOccassionFilter containers
            document.querySelectorAll('.byOccassionFilter-Container').forEach(container => {
                container.style.display = "none";
            });
            document.querySelectorAll('.byOccassionFilter-Jewellery-Container').forEach(container => {
                container.style.display = "none";
            });
            document.querySelectorAll('.byOccassionFilter-Watches-Container').forEach(container => {
                container.style.display = "none";
            });
        });

    } else if (currentPath.includes("Collections/Index/LimitedEditionCollection")) {
        hideWatchFilterContainers();
        hideJewelleryFilterContainers();
        hideAccessoryFilterContainers();

        document.querySelector(".clearAll-Btn").addEventListener("click", () => {
            hideWatchFilterContainers();
            hideJewelleryFilterContainers();
            hideAccessoryFilterContainers();

            // Hide byOccassionFilter containers
            document.querySelectorAll('.byOccassionFilter-Container').forEach(container => {
                container.style.display = "none";
            });
            document.querySelectorAll('.byOccassionFilter-Jewellery-Container').forEach(container => {
                container.style.display = "none";
            });
            document.querySelectorAll('.byOccassionFilter-Watches-Container').forEach(container => {
                container.style.display = "none";
            });
        });

    } else if (currentPath.includes("Collections/Index/ByOccasionCollection")) {
        hideWatchFilterContainers();
        hideJewelleryFilterContainers();
        hideAccessoryFilterContainers();

        document.querySelector(".clearAll-Btn").addEventListener("click", () => {
            hideWatchFilterContainers();
            hideJewelleryFilterContainers();
            hideAccessoryFilterContainers();

            // Hide byOccassionFilter containers
            document.querySelectorAll('.byOccassionFilter-Container').forEach(container => {
                container.style.display = "none";
            });
            document.querySelectorAll('.byOccassionFilter-Jewellery-Container').forEach(container => {
                container.style.display = "none";
            });
            document.querySelectorAll('.byOccassionFilter-Watches-Container').forEach(container => {
                container.style.display = "none";
            });
        });


    } else if (currentPath.includes("Collections/Index/BestSellersCollection")) {
        hideWatchFilterContainers();
        hideJewelleryFilterContainers();
        hideAccessoryFilterContainers();

        document.querySelector(".clearAll-Btn").addEventListener("click", () => {
            hideWatchFilterContainers();
            hideJewelleryFilterContainers();
            hideAccessoryFilterContainers();

            // Hide byOccassionFilter containers
            document.querySelectorAll('.byOccassionFilter-Container').forEach(container => {
                container.style.display = "none";
            });
            document.querySelectorAll('.byOccassionFilter-Jewellery-Container').forEach(container => {
                container.style.display = "none";
            });
            document.querySelectorAll('.byOccassionFilter-Watches-Container').forEach(container => {
                container.style.display = "none";
            });
        });

    }
    //Show filter based off Product category checked
    productCategoryCheckboxes.forEach(checkbox => {
        checkbox.addEventListener('change', function () {
            if (checkbox.checked) {
                if (checkbox.classList.contains("productCategoryCheckbox-Watch")) {
                    //alert("Watch product checked");
                    showWatchFilterContainers();
                }

                if (checkbox.classList.contains("productCategoryCheckbox-Jewellery")) {
                    //alert("Jewellery product checked");
                    showJewelleryFilterContainers();
                }

                if (checkbox.classList.contains("productCategoryCheckbox-Accessory")) {
                    //alert("Accessory product checked");
                    showAccessoryFilterContainers();
                }
            } else {
                if (checkbox.classList.contains("productCategoryCheckbox-Watch")) {
                    //alert("Watch product unchecked");
                    hideWatchFilterContainers();
                }

                if (checkbox.classList.contains("productCategoryCheckbox-Jewellery")) {
                    //alert("Jewellery product unchecked");
                    hideJewelleryFilterContainers();
                }

                if (checkbox.classList.contains("productCategoryCheckbox-Accessory")) {
                    //alert("Accessory product unchecked");
                    hideAccessoryFilterContainers();
                }
            }

            updateOccasionFilters();
        });
    });
}

checkedFilter_HideFilterProp();


