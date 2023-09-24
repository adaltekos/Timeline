const imageContainers = document.querySelectorAll('.photo-box');
const expandedView = document.querySelector('.expanded-view');
const expandedImage = expandedView.querySelector('img');
const expandedVideo = expandedView.querySelector('video');
const closeBtn = expandedView.querySelector('.close-button');
const prevBtn = expandedView.querySelector('.prev-button');
const nextBtn = expandedView.querySelector('.next-button');
let currentIndex = 0;

// Function to open the expanded view
function openExpandedView(index) {
    currentIndex = index;
    if (imageContainers[index].querySelector('img').getAttribute("type").startsWith('image/')) {
        expandedImage.src = imageContainers[index].querySelector('a').href;
        expandedImage.style.display = 'block'; // Show the image element
        expandedVideo.style.display = 'none'; // Hide the video element
    }
    else if (imageContainers[index].querySelector('img').getAttribute("type").startsWith('video/')) {
        expandedVideo.src = imageContainers[index].querySelector('a').href;
        expandedVideo.style.display = 'block'; // Show the video element
        expandedImage.style.display = 'none'; // Hide the image element
    }
    document.body.style.overflowY = 'hidden';
    expandedView.style.display = 'block';
    expandedView.focus();
}

// Function to close the expanded view
function closeExpandedView() {
    document.body.style.overflowY = 'auto';
    expandedView.style.display = 'none';
    if (expandedVideo && !expandedVideo.paused) {
        expandedVideo.pause();
    }
}

// Function to navigate to the previous image
function prevImage(event) {  
    currentIndex = (currentIndex - 1 + imageContainers.length) % imageContainers.length;
    openExpandedView(currentIndex);
    if (expandedVideo && !expandedVideo.paused) {
        expandedVideo.pause();
    }
}

// Function to navigate to the next image
function nextImage() {
    currentIndex = (currentIndex + 1) % imageContainers.length;
    openExpandedView(currentIndex);
    if (expandedVideo && !expandedVideo.paused) {
        expandedVideo.pause();
    }
}

// Event listeners for image container clicks (excluding checkboxes)
document.querySelectorAll('.photo-box').forEach(function (container, index) {
    container.addEventListener('click', function (event) {
        if (!event.target.classList.contains('photo-checkbox')) {
            // If the clicked element is not the checkbox, open the expanded view
            openExpandedView(index);
        }
    });
});

// Event listener for closing the expanded view
closeBtn.addEventListener('click', closeExpandedView);

// Event listener for navigating to the previous image
prevBtn.addEventListener('click', prevImage);

// Event listener for navigating to the next image
nextBtn.addEventListener('click', nextImage);

// Navigation to next and previous images
expandedView.addEventListener('keydown', (event) => {
    console.log(event.target)
    if (event.key === 'ArrowLeft') {
        currentIndex = (currentIndex - 1 + imageContainers.length) % imageContainers.length;
        openExpandedView(currentIndex);
    } else if (event.key === 'ArrowRight') {
        currentIndex = (currentIndex + 1) % imageContainers.length;
        openExpandedView(currentIndex);
    }
});