// Add active class to navbar menu item
$(function () {
  $('.navbar a[href^="' + location.pathname + '"]').closest('li').addClass('active');
});
