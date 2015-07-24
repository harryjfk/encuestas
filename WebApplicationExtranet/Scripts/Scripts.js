jQuery.extend({

    InitTooltip: function () {
        $("*[data-toggle='tooltip']").tooltip();
    },

    CreateItemOpenModal: function (idCreateBtn, idModalElement, entityName) {
        //idCreateBtn: id del boton de crear un nuevo elemento del CRUD
        //idModalElement: id del elemento html del modal
        //entityName: nombre de la entidad del CRUD
        $("#" + idCreateBtn).click(function () {
            $.ajax({
                cache: false,
                type: "GET",
                url: window.location.protocol + "//" + window.location.host + "/" + entityName + "/Edit/",
                data: { "id": 0 },
                success: function (data) {
                    $("#" + idModalElement + " .modal-body").html(data);
                    $("#" + idModalElement).modal("show");
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert('Ocurrió un error.');
                }
            });

        });
    },

    origin: function () {
        return window.location.protocol + "//" + window.location.host;
    }

});
$(document).ready(function () {
   
});