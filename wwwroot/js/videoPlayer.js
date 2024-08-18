// Array of YouTube video IDs
const videoIds = ['xfuvw7RV4bM', 'onWYtOguHC0', '_vGNNVMiwn4', 'rvYaUOmV1eg', 
    'pq7QP0qEGaU', 'Wm0JwtPC6hs', 'M2fZNxVJeUY', '5FmkfwSENVM', '60uUnwOJCzU',
    'W7NcG6y3Vns', 'ZsCePvyIAYs'];

let currentIndex = 0; // Track the current index of the displayed thumbnail
let thumbnailsToShow = 4; // Number of thumbnails to show at a time
let currentlySelectedVideo = null;



// Function to create thumbnail elements
function createThumbnails() {
    const thumbnailContainer = document.getElementById('thumbnail-carousel');
    thumbnailContainer.innerHTML = ''; // Clear existing thumbnails

    // Calculate the end index based on currentIndex and thumbnailsToShow
    const endIndex = Math.min(currentIndex + thumbnailsToShow, videoIds.length);

    for (let i = currentIndex; i < endIndex; i++) {
        const thumbnail = document.createElement('img');
        thumbnail.src = `https://img.youtube.com/vi/${videoIds[i]}/0.jpg`; // Thumbnail URL
        thumbnail.classList.add('thumbnail');
        thumbnail.addEventListener('click', () => {
            changeVideo(videoIds[i]);
            currentlySelectedVideo = videoIds[i]
        });
        thumbnailContainer.appendChild(thumbnail);
        if (videoIds[i] == currentlySelectedVideo) {
            thumbnail.classList.add('active');
        }
    }
}

// Function to handle next button click
document.getElementById('nextButton').addEventListener('click', () => {
    currentIndex = (currentIndex + thumbnailsToShow) % videoIds.length;
    createThumbnails();
});

// Function to handle previous button click
document.getElementById('prevButton').addEventListener('click', () => {
    currentIndex = (currentIndex - thumbnailsToShow + videoIds.length) % videoIds.length;
    createThumbnails();
});

// Function to change the video in the player
function changeVideo(videoId) {
    const videoPlayer = document.getElementById('video-player');
    videoPlayer.src = `https://www.youtube.com/embed/${videoId}?autoplay=0`;
    const thumbnails = document.querySelectorAll('.thumbnail');
    thumbnails.forEach(thumbnail => {
        thumbnail.classList.remove('active');
        if (thumbnail.src.includes(videoId)) {
            thumbnail.classList.add('active');
        }
    });
}

function checkWindowSize() {
    if (window.innerWidth <= 768) {
        thumbnailsToShow = 2;
        createThumbnails();
    } else {
        thumbnailsToShow = 4;
        createThumbnails();
    }
}
checkWindowSize();

// Call function to create thumbnails
createThumbnails();

window.addEventListener('resize', checkWindowSize);