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
                contentType: "application/json; charset=utf-8",
                dataType: "html",
                url: window.location.protocol + "//" + window.location.host + "/" + entityName + "/"+action+"/",
                data: { "id": 0 },
                success: function (data) {
                    if (modalName) {
                        $.changeModalName(modalName);
                    }                    
                    var html = data.trim();                    
                    $("#" + idModalElement + " .modal-body").html(html);
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
    },
    changeModalName: function (title) {        
        $(".modal-dialog .modal-content .modal-header h4:eq(0)").html(title);        
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
    },

    daysInMonth:function(month,year) {
        return new Date(year, month, 0).getDate();
    }
});

function jspdfTable(tableId) {

    /* map html table */
    var table = $('table#' + tableId + ' tr').get().map(function (row) {
        return $(row).find('td, th').get().map(function (cell) {
            return $(cell).text().replace(/\n/g, '').trim();
        });
    });
    /* end */

    var doc = new jsPDF('l', 'pt', 'a4', true);     // l: horizontal 
    /* end */

    /* table to PDF */
    doc.cellInitialize();

    var p_x_init = 10;
    //var p_y_init = 50;
    var p_y_init = 50;

    //var p_x = 10;
    //var p_y = 50;
    //var p_w = 130;
    //var p_h = 50;

    var p_x = 5;
    var p_y = 50;
    var p_w = 60;
    var p_h = 20;

    var _isFirstRow = true;
    var _rowNumber = 0;

    $.each(table, function (i, row) {
        //console.debug(row);

        if (_isFirstRow) {
            p_y = p_y_init;
        } else {
            p_y += p_h;
        }

        var _isFirstCol = true;

        $.each(row, function (j, cell) {

            if (_isFirstCol) {
                p_x = p_x_init;
            } else {
                p_x += p_w;
            }

            var arrayColor = [];

            //console.log('p_x: ' + p_x + '; p_y: ' + p_y + '; p_w: ' + p_w + '; p_h: ' + p_h + '; cell: ' + cell + '; i: ' + i);
            doc.cell(p_x, p_y, p_w, p_h, cell, i, null, arrayColor);  // 2nd parameter=top margin,1st=left margin 3rd=row cell width 4th=Row height
            _isFirstCol = false;
        })
        _isFirstRow = false;
        _rowNumber++;
    })
    /* end */

    /* save PDF */
    //doc.output('datauri');            // doesn't work in IE
    //doc.output('dataurlnewwindow');   // doesn't work in IE
    doc.save('ejemplo1.pdf');
}