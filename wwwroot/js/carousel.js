window.onload = function () {
    let slides = document.getElementsByClassName('carousel-item');
    let currentIndex = 0;
    let intervalDuration = 9000;

    function addActive(slide) {
        slide.classList.add('active');
    }

    function removeActive(slide) {
        slide.classList.remove('active');
    }

    function showNextSlide() {
        removeActive(slides[currentIndex]);
        currentIndex = (currentIndex + 1) % slides.length;
        addActive(slides[currentIndex]);
        setTimeout(showNextSlide, intervalDuration); // Call itself recursively after intervalDuration
    }

    addActive(slides[currentIndex]);
    setTimeout(showNextSlide, intervalDuration); // Start the carousel after intervalDuration
};