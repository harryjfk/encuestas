jQuery.extend({

    InitTooltip: function () {
        $("*[data-toggle='tooltip']").tooltip();
    },

    CreateItemOpenModal: function (idCreateBtn, idModalElement, entityName,action,modalName) {
        //idCreateBtn: id del boton de crear un nuevo elemento del CRUD
        //idModalElement: id del elemento html del modal
        //entityName: nombre de la entidad del CRUD
        //El nombre con el que se quiere mostrar el modal
        action = action ? action : "Edit";
        $("#" + idCreateBtn).click(function () {
            $.ajax({
                cache: false,
                type: "GET",
                url: window.location.protocol + "//" + window.location.host + "/" + entityName + "/"+action+"/",
                data: { "id": 0 },
                success: function (data) {
                    $("#" + idModalElement + " .modal-body").html(data);
                    $("#" + idModalElement).modal("show");
                    if (modalName) {
                        $.changeModalName(modalName);
                    }
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert('Ocurrió un error.');
                }
            });

        });
    },

    origin: function () {
        return window.location.protocol + "//" + window.location.host;
    },
    changeModalName: function (title) {
        var el = $(".modal-dialog .modal-content .modal-header h4")[0];
        $(el).html(title);
    },
    
    search: function (element) {
        setTimeout(function () {           
            $(element).chosen({ disable_search_threshold: 10, no_results_text: "No se encontraron coincidencias con ", placeholder_text_single: "Seleccione un elemento" });
        $("div[class*='chosen-container chosen-container-single']").each(function () { $(this).addClass("form-control"); });
        var a = $("a[class*='chosen-single']");
        if (a != null) {
            a.css("border", "0");
            a.css("background", "white");
            a.css("box-shadow", "0 0 0 white inset,0 0 0 rgba(0,0,0,.1)");
        }
    },1000);
        //$(element).chosen({ disable_search_threshold: 10, no_results_text: "No se encontraron coincidencias con ", placeholder_text_single: "Seleccione elemento" });
        //$("div[class*='chosen-container chosen-container-single chosen-container-single-nosearch']").addClass("form-control");
        //var a = $("a[class*='chosen-single chosen-default']");
        //var a1 = $("a[class*='chosen-single']");
        //if (a != null) {
        //    a.css("border", "0");
        //    a.css("background", "white");
        //    a.css("box-shadow", "0 0 0 white inset,0 0 0 rgba(0,0,0,.1)");
        //}
        //if (a1 != null) {
        //    a1.css("border", "0");
        //    a1.css("background", "white");
        //    a1.css("box-shadow", "0 0 0 white inset,0 0 0 rgba(0,0,0,.1)");
        //}
       
    },
    getDropDrown: function (path, parameters, idContainer,succesFunc,onchangeFunc) {
        $.ajax({
            cache: false,
            type: "POST",
            url: $.origin() + path,
            data: parameters,
            success: function (data) {
                $(idContainer).html(data);
                
                if (onchangeFunc) {
                    if (parameters.nombre) {
                       
                        $("#" + parameters.nombre).on("change", function () { onchangeFunc(); });
                    }
                }
                if (succesFunc) {
                    succesFunc();
                }

                
            },
            error: function (xhr, ajaxOptions, thrownError) {

                alert('Failed to retrieve data.');
            }
        });
    },
    //para especificar al usuario que le faltan campos requeridos por poner su valor
    errorIsRequired: function () {
        var errors = "<li> Debe llenar los campos requeridos</li>";
        $("#errors-container").removeClass("hidden").find("ul").html(errors);
    }


});
$(document).ready(function () {
    
});