const photoCheckboxes = document.querySelectorAll('.photo-checkbox');
const selectAllCheckbox = document.getElementById('selectAll');
const deleteButton = document.getElementById('deleteAll');
const uploadStatus = document.getElementById("uploadStatus");

selectAllCheckbox.addEventListener('change', function () {
    const isChecked = selectAllCheckbox.checked;
    // Loop through all photo checkboxes and update their checked state
    photoCheckboxes.forEach(function (checkbox) {
        checkbox.checked = isChecked;
    });
    // Set the opacity of "Select All" based on its checked state

});

photoCheckboxes.forEach(function (checkbox) {
    checkbox.addEventListener('change', function () {
        // Update the "Select All" checkbox state based on individual checkboxes
        selectAllCheckbox.checked = Array.from(photoCheckboxes).every(function (cb) {
            return cb.checked;
        });
        // Set the opacity of all individual checkboxes based on any checkbox being selected
    });
});

function getSelectedCheckboxes() {
    const selectedCheckboxes = [];
    const checkboxes = document.querySelectorAll('.photo-checkbox');

    checkboxes.forEach(function (checkbox) {
        if (checkbox.checked) {
            const checkboxId = checkbox.getAttribute('data-id'); // Use a data attribute to uniquely identify the checkbox
            selectedCheckboxes.push(checkboxId);
        }
    });

    return selectedCheckboxes;
}

deleteButton.addEventListener('click', async function () {
    const selectedCheckboxes = getSelectedCheckboxes();
    const antiForgeryToken = getAntiForgeryValue();
    const albumId = albumIdInput.value;

    const formData = new FormData();
    formData.append("AlbumId", albumId);
    formData.append("__RequestVerificationToken", antiForgeryToken);
    formData.append("SelectedCheckboxes", JSON.stringify(selectedCheckboxes));

    // Make a POST request with the selected checkboxes data
    try {
        const response = await fetch('/Albums/PhotoAlbum?handler=DeleteFile', {
            method: 'POST',
            body: formData,
        });
        if (response.ok) {
            // Handle a successful upload (HTTP status 200-299)
            const successMessage = await response.text();
            /*uploadStatus.innerHTML = `<p>${successMessage}</p>`;*/
        } else {
            // Handle errors (HTTP status outside 200-299)
            const error = await response.text();
            uploadStatus.innerHTML += `<p>Error: ${error}</p>`;
        }
    } catch (error) {
        // Handle network or other errors
        console.error('An error occurred:', error);
    }
});

function getAntiForgeryValue() {
    const antiForgeryTokenInput = document.querySelector('input[name="__RequestVerificationToken"]');

    if (antiForgeryTokenInput) {
        return antiForgeryTokenInput.value;
    }

    return null;
}
