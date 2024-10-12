///////////////////////////////////
// HomePg Search Component
document.addEventListener("DOMContentLoaded", () => {
    const searchInput = $("#searchInput");
    const yareSearch_subMenu_contentContainer = $("#yareSearch-subMenu-topTenCollectionContainer");
    const searchLiveResultsContainer = $("#tab-searchLiveResultsContainer");
    const searchCloseIcon = $(".search-close-icon");

    function performSearch(searchTerm) {
        const searchString = searchTerm.trim();

        if (searchString !== "") {
            // If there is a search string, hide search content and show live search results
            yareSearch_subMenu_contentContainer.hide();
            searchLiveResultsContainer.show();

            $.ajax({
                url: filterUrl,
                data: { searchString: searchString },
                method: "GET",
                success: function (data) {
                    searchLiveResultsContainer.html(data);
                }
            });

        } else {
            // If the search string is empty, show watchListContainer and hide watchLiveResultsContainer
            yareSearch_subMenu_contentContainer.show();
            searchLiveResultsContainer.hide();
        }
    }

    searchInput.on("input", function () {
        performSearch($(this).val());
    });

    // Enter key press on the search input
    searchInput.on("keydown", function (e) {
        if (e.key === "Enter") {
            e.preventDefault(); // Prevent the default form submission
            performSearch($(this).val()); // Trigger the search function
        }
    });

    searchCloseIcon.on("click", () => {
        searchInput.val("");
        yareSearch_subMenu_contentContainer.show();
        searchLiveResultsContainer.hide();
    });

    /////////////////////////////////////////////////////
    // Suggested Search Pills Component
    $(".suggestedSearch-pills").on('click', function () {
        const originalValue = $(this).text().trim();
        const searchTerm = originalValue.toLowerCase() === 'watches' ? 'watch' : originalValue;

        // Set the search input to the original term
        searchInput.val(originalValue);
        searchContainer.classList.remove("searchContainer");
        searchContainer.classList.add("searchContainer-active");
        // Perform the search with the translated term
        performSearch(searchTerm); // Pass the translated term to the search function
    });

});

////////////////////////////////////////////
// Search Input Component
const searchInput = document.getElementById("searchInput");
const searchContainer = document.getElementById("searchContainer");
const searchCloseIcon = document.querySelector(".search-close-icon");

searchCloseIcon.addEventListener("click", () => {
    searchInput.value = "";
    searchContainer.classList.add("searchContainer");
    searchContainer.classList.remove("searchContainer-active");
});

searchInput.addEventListener("input", () => {
    searchContainer.classList.remove("searchContainer");
    searchContainer.classList.add("searchContainer-active");
});

searchInput.addEventListener("blur", () => {
    searchContainer.classList.add("searchContainer");
    searchContainer.classList.remove("searchContainer-active");
});