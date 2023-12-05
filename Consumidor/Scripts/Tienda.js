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
        IdRecord = data.IdTienda;
    });

    $('#dt-records').on('click', 'button.btn-delete', function (e) {
        var _this = $(this).parents('tr');
        var data = tableHdr.row(_this).data();
        IdRecord = data.IdTienda;
        if (confirm('¿Seguro de eliminar el registro?')) {
            Eliminar();
        }
    });

});

function loadData() {
    tableHdr = $('#dt-records').DataTable({
        responsive: true,
        destroy: true,
        ajax: "/Tienda/Lista",
        order: [],
        columns: [
            { "data": "IdTienda" },
            { "data": "Nombre" },
            { "data": "Direccion" },
            { "data": "Ciudad" }
        ],
        // ... (Keep the rest of the DataTable configuration)
    });
}

function NewRecord() {
    $(".modal-header h3").text("Crear Tienda");

    $('#txtIdTienda').val('');
    $('#txtNombreTienda').val('');
    $('#txtDireccionTienda').val('');
    $('#txtCiudadTienda').val('');

    $('#modal-record').modal('toggle');
}

function loadDtl(data) {
    $(".modal-header h3").text("Editar Tienda");

    $('#txtIdTienda').val(data.IdTienda).prop('disabled', true);
    $('#txtNombreTienda').val(data.Nombre);
    $('#txtDireccionTienda').val(data.Direccion);
    $('#txtCiudadTienda').val(data.Ciudad);

    $('#modal-record').modal('toggle');
}

function Guardar() {
    var record = "'IdTienda':'" + $.trim($('#txtIdTienda').val()) + "'";
    record += ",'Nombre':'" + $.trim($('#txtNombreTienda').val()) + "'";
    record += ",'Direccion':'" + $.trim($('#txtDireccionTienda').val()) + "'";
    record += ",'Ciudad':'" + $.trim($('#txtCiudadTienda').val()) + "'";
    console.log(record);

    $.ajax({
        type: 'POST',
        url: '/Tienda/Guardar',
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
        url: '/Tienda/Eliminar/?IdTienda=' + IdRecord,
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
