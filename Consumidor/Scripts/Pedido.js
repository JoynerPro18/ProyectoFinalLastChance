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
        IdRecord = data.IdPedido;
    });

    $('#dt-records').on('click', 'button.btn-delete', function (e) {
        var _this = $(this).parents('tr');
        var data = tableHdr.row(_this).data();
        IdRecord = data.IdPedido;
        if (confirm('¿Seguro de eliminar el registro?')) {
            Eliminar();
        }
    });

});

function loadData() {
    tableHdr = $('#dt-records').DataTable({
        responsive: true,
        destroy: true,
        ajax: "/Pedido/Lista",
        order: [],
        columns: [
            { "data": "IdPedido" },
            { "data": "IdUsuario" },
            { "data": "IdProducto" },
            { "data": "Cantidad" }
        ],
        // ... (Keep the rest of the DataTable configuration)
    });
}

function NewRecord() {
    $(".modal-header h3").text("Crear Pedido");

    $('#txtIdPedido').val('');
    $('#txtIdUsuarioPedido').val('');
    $('#txtIdProductoPedido').val('');
    $('#txtCantidadPedido').val('');

    $('#modal-record').modal('toggle');
}

function loadDtl(data) {
    $(".modal-header h3").text("Editar Pedido");

    $('#txtIdPedido').val(data.IdPedido).prop('disabled', true);
    $('#txtIdUsuarioPedido').val(data.IdUsuario);
    $("#txtIdProductoPedido").val(data.IdProducto);
    $("#txtCantidadPedido").val(data.Cantidad);

    $('#modal-record').modal('toggle');
}

function Guardar() {
    var record = "'IdPedido':'" + $.trim($('#txtIdPedido').val()) + "'";
    record += ",'IdUsuario':'" + $.trim($('#txtIdUsuarioPedido').val()) + "'";
    record += ",'IdProducto':'" + $.trim($('#txtIdProductoPedido').val()) + "'";
    record += ",'Cantidad':'" + $.trim($('#txtCantidadPedido').val()) + "'";
    console.log(record);

    $.ajax({
        type: 'POST',
        url: '/Pedido/Guardar',
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
        url: '/Pedido/Eliminar/?IdPedido=' + IdRecord,
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
