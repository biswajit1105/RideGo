let searchForm = document.querySelector('.search-form');

document.querySelector('#search-btn').onclick = () => {
    navbar.classList.remove('active');
    loginView.classList.remove('active');
    cartView.classList.remove('active');
    searchForm.classList.toggle('active');
}

let cartView = document.querySelector('.shopping-cart');

document.querySelector('#cart-btn').onclick = () => {
    navbar.classList.remove('active');
    loginView.classList.remove('active');
    searchForm.classList.remove('active');
    cartView.classList.toggle('active');
}

let loginView = document.querySelector('.login-form');

document.querySelector('#login-btn').onclick = () => {
    navbar.classList.remove('active');
    cartView.classList.remove('active');
    searchForm.classList.remove('active');
    loginView.classList.toggle('active');
}


let navbar = document.querySelector('.navbar');

document.querySelector('#menu-btn').onclick = () => {
    loginView.classList.remove('active');
    cartView.classList.remove('active');
    searchForm.classList.remove('active');
    navbar.classList.toggle('active');
}

window.onscroll = () => {
    navbar.classList.remove('active');
    loginView.classList.remove('active');
    cartView.classList.remove('active');
    searchForm.classList.remove('active');
}

var swiper = new Swiper(".product-slider", {
    loop: true,
    spaceBetween: 20,
    autoplay: {
        delay: 7500,
        disableOnInteraction: false,
    },
    centeredSlides: true,
    breakpoints: {
        0: {
            slidesPerView: 1,
        },
        885: {
            slidesPerView: 2,
        },
        1189: {
            slidesPerView: 3,
        },
    },
});

var swiper = new Swiper(".review-slider", {
    loop: true,
    spaceBetween: 20,
    autoplay: {
        delay: 7500,
        disableOnInteraction: false,
    },
    centeredSlides: true,
    breakpoints: {
        0: {
            slidesPerView: 1,
        },
        885: {
            slidesPerView: 2,
        },
        1189: {
            slidesPerView: 3,
        },
    },
});

$(function () {
    var placeHolderElement = $('#PlaceHolderNew');
    $('a[data-toggle="ajax-modal-new"]').click(function (event) {
        loginView.classList.remove('active');
        var url = $(this).data('url');
        var decodedUrl = decodeURIComponent(url);
        $.get(decodedUrl).done(function (data) {
            placeHolderElement.html(data);
            placeHolderElement.find('.modal').modal({
                backdrop: false
            }).modal('show');
        });
    });
});

function closeAndReturn() {
    loginView.classList.remove('active');
    $('#NewUserClose').modal('hide');
    $('#ForgotModal').modal('hide');
    location.reload(true);
}

document.addEventListener('DOMContentLoaded', function () {
    let loginView = document.querySelector('.login-form');

    // Check if TempData["AlreadyRegister"] is not null
    if ('@TempData["AlreadyRegister"]' !== '') {
        loginView.classList.toggle('active');
    }
    else {
        loginView.classList.remove('active');
    }
});

$(function () {
    var placeHolderElement = $('#PlaceHolderforgot');
    $('a[data-toggle="ajax-modal-forget"]').click(function (event) {
        loginView.classList.remove('active');
        var url = $(this).data('url');
        var decodedUrl = decodeURIComponent(url);
        $.get(decodedUrl).done(function (data) {
            placeHolderElement.html(data);
            placeHolderElement.find('.modal').modal({
                backdrop: false
            }).modal('show');
        });
    });
});

$(document).ready(function () {
    $('.password-toggle-btn').on('click', function () {
        var passwordInput = $(this).prev('.password-input');
        if (passwordInput.attr('type') === 'password') {
            passwordInput.attr('type', 'text');
        } else {
            passwordInput.attr('type', 'password');
        }
    });
});