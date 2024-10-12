/////////////////////////////////////
//// Dropdown Component
document.addEventListener('DOMContentLoaded', function () {
    var btn = document.querySelector('.productManagment-Btn');
    var container = document.querySelector('.productManagment-Container');

    btn.addEventListener('click', function () {
        container.classList.toggle('open');
    });
});

//////////////////////////////////////////
// Live Search Component
document.addEventListener("DOMContentLoaded", function () {
    const searchInput = $("#searchInput");
    const productListContainer = $("#productListContainer");
    const searchLiveResultsContainer = $("#searchLiveResultsContainer");
    const closeBtnSearchBar = $(".search-close-icon");

    const urls = [
        { area: "Admin", controller: "Product", action: "Filter" },
        { area: "Admin", controller: "Watch", action: "Filter" },
        { area: "Admin", controller: "Jewellery", action: "Filter" },
        { area: "Admin", controller: "Accessory", action: "Filter" },
        { area: "Admin", controller: "Collection", action: "Filter" },
    ];

    // Extract controller name from the URL
    const currentController = window.location.pathname.split("/").pop();

    // Find the index of the current controller in the urls array
    const currentIndex = urls.findIndex(url => url.controller.toLowerCase() === currentController.toLowerCase());

    function performSearch(index) {
        const searchString = searchInput.val();

        if (searchString.trim() !== "") {
            // If there is a search string, hide productListContainer and show searchLiveResultsContainer
            productListContainer.hide();
            searchLiveResultsContainer.show();

            const urlObj = urls[index]; // Get the URL object based on the index
            const url = `/${urlObj.area}/${urlObj.controller}/${urlObj.action}`;
            console.log("URL:", url); // Log URL for debugging

            $.ajax({
                url: url,
                data: { searchString: searchString },
                method: "GET",
                success: function (data) {
                    searchLiveResultsContainer.html(data);
                }
            });
        } else {
            // If the search string is empty, show productListContainer and hide searchLiveResultsContainer
            productListContainer.show();
            searchLiveResultsContainer.hide();
        }
    }

    searchInput.on("input", function () {
        performSearch(currentIndex); // Call performSearch with the current index
    });

    // Listen for the Enter key press on the search input
    searchInput.on("keydown", function (e) {
        if (e.key === "Enter") {
            e.preventDefault(); // Prevent the default form submission
            performSearch(currentIndex); // Trigger the search function with the current index
        }
    });

    // Event listener for the close button
    closeBtnSearchBar.on("click", function () {
        // Clear the search input value
        searchInput.val("");
        // Trigger search to show original productListContainer if search value is empty
        if (searchInput.val().trim() === "") {
            performSearch(currentIndex); // Call performSearch with the current index
        }
    });
});

//////////////////////////////////////////
// Search Input Component
const searchInput = document.getElementById("searchInput");
const searchContainer = document.getElementById("searchContainer");
const searchCloseIcon = document.querySelector(".search-close-icon");

if (searchCloseIcon) {
    searchCloseIcon.addEventListener("click", () => {
        if (searchInput) {
            searchInput.value = "";
        }
    });
}

if (searchInput && searchContainer) {
    searchInput.addEventListener("input", () => {
        searchContainer.classList.remove("searchContainer");
        searchContainer.classList.add("searchContainer-active");
    });

    searchInput.addEventListener("blur", () => {
        searchContainer.classList.add("searchContainer");
        searchContainer.classList.remove("searchContainer-active");
    });
}

//////////////////////////////////////
// Product Number Automate Component
function generateRandomProductNumber(length) {
    const letters = 'ABCDEFGHIJKLMNOPQRSTUVWXYZ';
    const digits = '0123456789';
    let result = '';

    // Add at least 3 random digits
    for (let i = 0; i < 3; i++) {
        result += digits.charAt(Math.floor(Math.random() * digits.length));
    }

    // Fill the remaining characters with random letters and digits
    const characters = letters + digits;
    for (let i = 3; i < length; i++) {
        result += characters.charAt(Math.floor(Math.random() * characters.length));
    }

    // Shuffle the result to ensure value is ramdon
    result = result.split('').sort(() => 0.5 - Math.random()).join('');
    return result;
}

// Display Product Number On Load 
window.onload = function () {
    const productNumberField = document.querySelector('.ProductNumber');
    const randomProductNumber = generateRandomProductNumber(8);

    // Check if the element exists before setting its value
    if (productNumberField) {
        const randomProductNumber = generateRandomProductNumber(8);
        productNumberField.value = randomProductNumber;
    }
}


