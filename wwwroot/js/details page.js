$(document).ready(function () {
    $("#carousels").owlCarousel({
        loop: true,
        margin: 10,
        nav: true,
        responsive: {
            0: {
                items: 1
            },
            600: {
                items: 2
            },
            1000: {
                items: 3
            }
        }
    });
});

function selectOption(targetId, value) {
    document.getElementById(targetId + 'Dropdown').innerText = value;
}