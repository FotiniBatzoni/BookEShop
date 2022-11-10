var dataTable;

$(document).ready(function () {
    loadDataTable();
})

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            "url":"/Admin/Product/GetAll"
        },
        "columns": [
            { "data": "title", "width": "20%" },
            { "data": "isbn", "width": "20%" },
            { "data": "price", "width": "20%" },
            { "data": "author", "width": "20%" },
            { "data": "category.name", "width": "20%" },
        ]
    });
}