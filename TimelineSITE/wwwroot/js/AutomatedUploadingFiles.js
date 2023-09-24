document.addEventListener("DOMContentLoaded", function () {
    const fileInput = document.getElementById("fileInput");
    const albumIdInput = document.getElementById("albumIdInput");
    const uploadStatus = document.getElementById("uploadStatus");
    const uploadCounter = document.getElementById("uploadCounter");
    const uploadedCount = document.getElementById("uploadedCount");
    const totalCount = document.getElementById("totalCount");
    
    fileInput.addEventListener("change", async function () {
        if (fileInput.files.length > 0) {
            const files = fileInput.files;
            const albumId = albumIdInput.value;
            const antiForgeryToken = getAntiForgeryValue();

            const totalFileCount = files.length;
            totalCount.textContent = totalFileCount;

            uploadCounter.style.display = "block";
            let uploadedFileCount = 0;

            for (const file of files) {
                const formData = new FormData();
                formData.append("UploadedFile", file);
                formData.append("AlbumId", albumId);
                formData.append("__RequestVerificationToken", antiForgeryToken);

                try {
                    const response = await fetch('/Albums/PhotoAlbum?handler=UploadFile', {
                        method: "POST",
                        body: formData,
                    });

                    if (response.ok) {
                        // Handle a successful upload (HTTP status 200-299)
                        uploadedFileCount++;
                        uploadedCount.textContent = uploadedFileCount;
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
            }
            if (totalFileCount === uploadedFileCount) {
                location.reload();
            }
        }
    });
});

function getAntiForgeryValue() {
    const antiForgeryTokenInput = document.querySelector('input[name="__RequestVerificationToken"]');

    if (antiForgeryTokenInput) {
        return antiForgeryTokenInput.value;
    }

    return null;
}
