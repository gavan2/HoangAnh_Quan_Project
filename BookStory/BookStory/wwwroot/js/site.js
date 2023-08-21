// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
var connection = new signalR.HubConnectionBuilder()
    .withUrl("/signalRServer")
    .build();


connection.on("LoadBook", function (id, id1) {
    if (window.location.pathname.toLowerCase() === '/book/detail') {
        reloadPartialContent('/Book/Detail?id=' + id, function () {
            // Không cần chuyển hướng ở đây
        });
    }
});

connection.start().catch(function (err) {
    return console.error(err.toString());
});
function reloadPartialContent(url, callback) {
    $.ajax({
        type: "GET",
        url: url,
        success: function (data) {
            // Update the specific part of the page with the reloaded content
            $('#truyen').html(data);
            callback(); // Gọi hàm callback khi tải lại đã hoàn thành
        },
        error: function (error) {
            console.error(error);
        }
    });
}