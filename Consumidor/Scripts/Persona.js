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
        IdRecord = data.IdUsuario;
    });

    $('#dt-records').on('click', 'button.btn-delete', function (e) {
        var _this = $(this).parents('tr');
        var data = tableHdr.row(_this).data();
        IdRecord = data.IdUsuario;
        if (confirm('¿Seguro de eliminar el registro?')) {
            Eliminar();
        }
    });

});

function loadData() {
    tableHdr = $('#dt-records').DataTable({
        responsive: true,
        destroy: true,
        ajax: "/Persona/Lista",
        order: [],
        columns: [
            { "data": "IdUsuario" },
            { "data": "Nombre" },
            { "data": "Apellido" },
            { "data": "Telefono" }
        ],
        // ... (Keep the rest of the DataTable configuration)
    });
}

function NewRecord() {
    // Update modal title to match 'Persona' context
    $(".modal-header h3").text("Crear Persona");

    // Clear the input fields
    $('#txtIdUsuario').val('');
    $('#txtNombrePersona').val('');
    $('#txtApellidoPersona').val('');
    $('#txtTelefonoPersona').val('');

    // Show the modal
    $('#modal-record').modal('toggle');
}

function loadDtl(data) {
    // Update modal title to match 'Persona' context
    $(".modal-header h3").text("Editar Persona");

    // Populate the input fields with data
    $('#txtIdUsuario').val(data.IdUsuario);
    $('#txtNombrePersona').val(data.Nombre);
    $("#txtApellidoPersona").val(data.Apellido);
    $("#txtTelefonoPersona").val(data.Telefono);

    // Show the modal
    $('#modal-record').modal('toggle');
}

function Guardar() {
    var record = "'IdUsuario':'" + $.trim($('#txtIdUsuario').val()) + "'";
    record += ",'Nombre':'" + $.trim($('#txtNombrePersona').val()) + "'";
    record += ",'Apellido':'" + $.trim($('#txtApellidoPersona').val()) + "'";
    record += ",'Telefono':'" + $.trim($('#txtTelefonoPersona').val()) + "'";
    console.log(record);

    // Make the AJAX call
    $.ajax({
        type: 'POST',
        url: '/Persona/Guardar',
        data: eval('({' + record + '})'),
        // ... (Keep the rest of the AJAX configuration)
    });
}

function Eliminar() {
    console.log(IdRecord)
    $.ajax({
        type: 'POST',
        url: '/Persona/Eliminar/?IdUsuario=' + IdRecord,
        // ... (Keep the rest of the AJAX configuration)
    });
}
