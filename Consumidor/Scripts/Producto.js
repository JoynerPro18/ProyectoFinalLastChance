var tableHdr = null;
var IdRecord = "";

$(document).ready(function () {
    loadData();

    $('#btnnuevo').on('click', function (e) {
        e.preventDefault();
        NewRecord();
    });

    $('#btnguardar').on('click', function (e) {
        e.preventDefault();
        Guardar();
    });

    $('#dt-records').on('click', 'button.btn-edit', function (e) {
        var _this = $(this).parents('tr');
        var data = tableHdr.row(_this).data();
        loadDtl(data);
        IdRecord = data.IdProducto;
    });

    $('#dt-records').on('click', 'button.btn-delete', function (e) {
        var _this = $(this).parents('tr');
        var data = tableHdr.row(_this).data();
        IdRecord = data.IdProducto;
        if (confirm('¿Seguro de eliminar el registro?')) {
            Eliminar();
        }
    });

});

function loadData() {
    tableHdr = $('#dt-records').DataTable({
        responsive: true,
        destroy: true,
        ajax: "/Producto/Lista",
        order: [],
        columns: [
            { "data": "IdProducto" },
            { "data": "Nombre" },
            { "data": "Precio" },
            { "data": "Categoria" }
        ],
        // ... (Keep the rest of the DataTable configuration)
    });
}

function NewRecord() {
    $(".modal-header h3").text("Crear Producto");

    $('#txtIdProducto').val('');
    $('#txtNombreProducto').val('');
    $('#txtPrecioProducto').val('');
    $('#txtCategoriaProducto').val('');

    $('#modal-record').modal('toggle');
}

function loadDtl(data) {
    $(".modal-header h3").text("Editar Producto");

    $('#txtIdProducto').val(data.IdProducto).prop('disabled', true);
    $('#txtNombreProducto').val(data.Nombre);
    $("#txtPrecioProducto").val(data.Precio);
    $("#txtCategoriaProducto").val(data.Categoria);

    $('#modal-record').modal('toggle');
}

function Guardar() {
    var record = "'IdProducto':'" + $.trim($('#txtIdProducto').val()) + "'";
    record += ",'Nombre':'" + $.trim($('#txtNombreProducto').val()) + "'";
    record += ",'Precio':'" + $.trim($('#txtPrecioProducto').val()) + "'";
    record += ",'Categoria':'" + $.trim($('#txtCategoriaProducto').val()) + "'";
    console.log(record);

    $.ajax({
        type: 'POST',
        url: '/Producto/Guardar',
        data: eval('({' + record + '})'),
        success: function (response) {
            if (response.success) {
                console.log("success")
                $("#modal-record").modal('hide');
                $('#dt-records').DataTable().ajax.reload(null, false);
            }
            else {
                $("#modal-record").modal('hide');
                console.log("no success")
            }
        }
    });
}

function Eliminar() {
    console.log(IdRecord)
    $.ajax({
        type: 'POST',
        url: '/Producto/Eliminar/?IdProducto=' + IdRecord,
        success: function (response) {
            if (response.success) {
                console.log("deleted")
                $('#dt-records').DataTable().ajax.reload(null, false);
            } else {
                console.log("no success to delete")
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            console.log('AJAX Error:', textStatus, errorThrown);
        }
    });
}
