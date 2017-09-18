// Write your Javascript code.

/*
    Function                    Function                                    Used in
    -------------------------------------------------------------------------------------------------------
    deleteRecord                                                            Index.cshtml of all controllers
    deleteConfirmed                                                         ditto
    showComponents                                                          ditto
    formatThis              
    jQuery.fn.pageme                                                        superceded by the paging functionality of DataTables
    loadjscssfile
    removejscssfile         
    checkloadjscssfile
    showComponentDDL                                                        Create and Edit of SystemTab
    addIMTE                                                                 ditto
    returnIMTE                                                              Edit of SystemTab
*/

function deleteRecord(removethis, tableName) {
    var bgColor = $(removethis).closest('tr').css("background-color");
    var color = $(removethis).closest('tr').css("color");

    $(removethis).closest('tr').css({ "background-color": "yellow", "color": "blue" });
    //$(removethis).closest('tr').addClass("selected").addClass("highlight");
    var dialog = $("#dialog-confirm").dialog({
        /*
        autoOpen: false,
        height: 400,
        width: 350,
        modal: false, */
        autoOpen: false,
        resizable: false,
        height: 170,
        width: 350,
        //show: { effect: 'drop', direction: "up" },
        show: {
            effect: "blind",
            duration: 1000
        },
        hide: {
            effect: "explode",
            duration: 1000
        },
        modal: true,
        draggable: true,

        buttons: {
            OK: function () {
                event.preventDefault();
                deleteConfirmed(removethis, tableName, bgColor, color);
                dialog.dialog("close");
            },
            Cancel: function () {
                $(removethis).closest('tr').css({ "background-color": bgColor, "color": color });
                //$(removethis).closest('tr').removeClass("selected").removeClass("highlight");
                dialog.dialog("close");
            }
        }
    });

    dialog.dialog('open');
}

function deleteConfirmed(removethis, tableName, bgColor, color) {
    /*
    var r = confirm("Delete this row?");
    if (r == true) {
    */
    var value = $(removethis).attr('rel');     // or: var value = (removethis.id).substr(1);
    var url = "/" + tableName + "/Remove";

    // Send the data using post. See https://api.jquery.com/jquery.post/
    var posting = $.post(url, { id: value });
    /*  above is equivalent to:
        $.ajax({
          type: "POST",
          url: url,
          data: { id: value },
          success: success,
          dataType: dataType
        });
    */

    // remove record from list
    posting.done(function (data, textStatus, xhr) {
        // See http://api.jquery.com/jQuery.ajax/#jqXHR for various properties and methods of the jqXHR object
        /*
        alert(JSON.stringify(data));    // {"success":true, "status":"Completed Successfully"}      {"success":false, "status:"Can't delete component because CedarITT is using it"}
        alert(textStatus);              // success                                                  success
        alert(xhr.status);              // 200                                                      200
        alert(xhr.statusText);          // OK                                                       OK
        alert(xhr.responseText);        // {"success":true, "status":"Completed Successfully"}      {"success":false, "status:"Can't delete component because CedarITT is using it"}   
        */

        // See http://www.w3schools.com/json/json_eval.asp to parse a JSON string
        var obj = JSON.parse(xhr.responseText);
        if (obj.success) {
            $(removethis).closest('tr').remove();
        }
        else {
            alert(obj.status);
            $(removethis).closest('tr').css({ "background-color": bgColor, "color": color });
            //$(removethis).closest('tr').removeClass("selected").removeClass("highlight");
        }

    });

    // alert if not successful
    posting.fail(function (xhr, textStatus, errorThrown) {
        if(xhr.status !== null) {
            switch (xhr.status) {
                case "404":
                    alert(xhr.status + ": Page not found ");
                    break;

                default:
                    alert(xhr.status + ": " + xhr.statusText) ;
                    break;
            }
        };

        // possible values for the second argument (besides null) are "timeout", "error", "abort", and "parsererror". 
        if(textStatus !== null) alert("Error Status: " + textStatus);

        // when an HTTP error occurs, errorThrown receives the textual portion of the HTTP status, such as "Not Found" or "Internal Server Error." 
        alert("HTTP error thrown: " + errorThrown);
    });
}

// used in
/*  http://stackoverflow.com/questions/3090230/in-jquery-what-does-fn-mean
    It allows you to extend jQuery with your own functions.
    For example, $.fn.something = function{} will allow you to use $("#element").something().
    $.fn is also synonymous with jQuery.fn which might make your Google searches easier.

    See jQuery Plugins/Authoring
*/

/*
jQuery.fn.pageMe = function (opts) {    // opts are definedwhen pageMe is invoked during loading of DOM
    var $this = this,                   // "this" is #myTable
        defaults = {
            perPage: 7,
            showPrevNext: false,
            hidePageNumbers: false
        },
        settings = $.extend(defaults, opts); // merge opts into defaults and overrides similar fields in defaults https://api.jquery.com/jquery.extend/

    var listElement = $this;
    //alert(JSON.stringify(settings));
    var perPage = settings.perPage;
    var children = listElement.children();
    //alert(JSON.stringify(children));
    var pager = $('.pager');

    if (typeof settings.childSelector != "undefined") {
        children = listElement.find(settings.childSelector);
    }

    if (typeof settings.pagerSelector != "undefined") {
        pager = $(settings.pagerSelector);
    }

    var numItems = children.size();
    var numPages = Math.ceil(numItems / perPage);

    pager.data("curr", 0);

    if (settings.showPrevNext) {
        $('<li><a href="#" class="prev_link">«</a></li>').appendTo(pager);
    }

    var curr = 0;
    while (numPages > curr && (settings.hidePageNumbers == false)) {
        $('<li><a href="#" class="page_link">' + (curr + 1) + '</a></li>').appendTo(pager);
        curr++;
    }

    if (settings.showPrevNext) {
        $('<li><a href="#" class="next_link">»</a></li>').appendTo(pager);
    }

    pager.find('.page_link:first').addClass('active');
    pager.find('.prev_link').hide();
    if (numPages <= 1) {
        pager.find('.next_link').hide();
    }
    pager.children().eq(1).addClass("active");

    children.hide();
    children.slice(0, perPage).show();

    pager.find('li .page_link').click(function () {
        var clickedPage = $(this).html().valueOf() - 1;
        goTo(clickedPage, perPage);
        return false;
    });
    pager.find('li .prev_link').click(function () {
        previous();
        return false;
    });
    pager.find('li .next_link').click(function () {
        next();
        return false;
    });

    function previous() {
        var goToPage = parseInt(pager.data("curr")) - 1;
        goTo(goToPage);
    }

    function next() {
        goToPage = parseInt(pager.data("curr")) + 1;
        goTo(goToPage);
    }

    function goTo(page) {
        var startAt = page * perPage,
            endOn = startAt + perPage;

        children.css('display', 'none').slice(startAt, endOn).show();

        if (page >= 1) {
            pager.find('.prev_link').show();
        }
        else {
            pager.find('.prev_link').hide();
        }

        if (page < (numPages - 1)) {
            pager.find('.next_link').show();
        }
        else {
            pager.find('.next_link').hide();
        }

        pager.data("curr", page);
        pager.children().removeClass("active");
        pager.children().eq(page + 1).addClass("active");

    }
};
*/

function showRelatedTable(value, tableName) {
    var url = "/" + tableName;
    switch (tableName){
        case "SystemsDescriptions":
            url += "/LoadSystems";
            break;

        case "Systems":
            url += "/LoadDeployments";
            break;

        case "Components":
            url += "/LoadActivities";
            break;

        case "Locations":
            url += "/LoadSystems";
            break;

        default:
            url += "/LoadComponents";
            break;
    }

    var nestedTable = null;

    // get related data
    // See http://api.jquery.com/jQuery.ajax/
    $.ajax({
        method: "GET",
        url: url,
        data: { descId: value },
        async: false,
        cache: false
    })
    .done(function (data) {
        switch (tableName) {
            case "Components":
                if (data.length > 0) nestedTable = showComponentActivities(data);
                break;

            case "Systems":
                if (data.length > 0) nestedTable = showDeployments(data);
                break;

            case "SystemsDescriptions":
                if (data.length > 0) nestedTable = showSystems(data);
                break;

            case "Locations":
                if (data.length > 0) nestedTable = showSystems(data);
                break;

            default:
                if (data.length > 0) nestedTable = showRelatedComponents(data);
                break;
        }
    });

    if (nestedTable !== null) {
        return nestedTable;
    }

    return '<p> No Records to Display </p>';
}

function showComponentActivities(data) {
    nestedTable =
        '<tr>' +
            '<td colspan="4">' +
                '<table class="table table-striped table-condensed table-hover table-component" cellpadding="5" cellspacing="0" border="0" style="padding-left:50px;">' +
                    '<thead>' +
                            '<th>' + 'Deployment ID' + '</th>' +
                            '<th>' + 'Test System' + '</th>' +
                            '<th>' + 'Work Order' + '</th>' +
                            '<th>' + 'Schedule' + '</th>' +
                            '<th>' + 'eRecord' + '</th>' +
                            '<th>' + 'Deployment Date' + '</th>' +
                            '<th>' + 'WO Scheduled Due' + '</th>' +
                            '<th>' + 'WO Done Date' + '</th>' +
                            '<th>' + 'WO Calculated Due' + '</th>' +
                            '<th>' + 'Activity Type' + '</th>' +
                            '<th>' + 'Service Provider' + '</th>' +
                            '<th>' + 'Status' + '</th>' +
                        '</tr>' +
                    '</thead>' +
                    '<tbody>';
    $.each(data, function (index, itemData) {
        nestedTable +=
            '<tr>' +
                '<td>' + itemData.deploymentID + '</td>' +
                '<td>' + itemData.systemID + '</td>' +
                '<td>' + itemData.work_Order + '</td>' +
                '<td>' + itemData.schedule + '</td>' +
                '<td>' + itemData.eRecord + '</td>';

        if (itemData.deploymentDate !== null) nestedTable += '<td>' + formattedDate(itemData.deploymentDate) + '</td>';
        else nestedTable += '<td></td>';

        if(itemData.wO_Scheduled_Due !== null) nestedTable += '<td>' + formattedDate(itemData.wO_Scheduled_Due) + '</td>';
        else nestedTable += '<td></td>';

        if(itemData.wO_Done_Date !== null) nestedTable += '<td>' + formattedDate(itemData.wO_Done_Date) + '</td>';
        else nestedTable += '<td></td>';

        if(itemData.wO_Calculated_Due_Date !== null) nestedTable += '<td>' + formattedDate(itemData.wO_Calculated_Due_Date) + '</td>';
        else nestedTable += '<td></td>';

        if (itemData.activityTypeID !== null) nestedTable += '<td>' + itemData.activityType.desc + '</td>';
        else nestedTable += '<td></td>';

        if (itemData.serviceProviderID !== null) nestedTable += '<td>' + itemData.serviceProvider.desc + '</td>';
        else nestedTable += '<td></td>';

        if (itemData.statusID !== null) nestedTable += '<td>' + itemData.status.desc + '</td>';
        else nestedTable += '<td></td>';
        
        nestedTable += '</tr>';
    });

    nestedTable += '</tbody>' + '</table>' + '</td>' + '</tr>';

    return nestedTable;
}

function showSystems(data) {
    nestedTable =
        '<tr>' +
            '<td colspan="4">' +
                '<table class="table table-striped table-condensed table-hover table-component" cellpadding="5" cellspacing="0" border="0" style="padding-left:50px;">' +
                    '<thead>' +
                        '<tr>' +
                            '<th>' + 'IMTE' + '</th>' +
                            '<th>' + 'Reference No.' + '</th>' +
                            '<th>' + 'System Description' + '</th>' +
                            '<th>' + 'Deployment Date' + '</th>' +
                            '<th>' + 'Location' + '</th>' + '</tr>' +
                    '</thead>' +
                    '<tbody>';

    $.each(data, function (index, itemData) {
        // add detail row
        nestedTable +=
            '<tr>' +
                '<td>' + itemData.imte + '</td>' +
                '<td>' + itemData.referenceNo + '</td>';

        if (itemData.systemsDescriptionID !== null) nestedTable += '<td>' + itemData.systemsDescription.desc + '</td>';
        else nestedTable += '<td></td>';

        if (itemData.deploymentDate !== null) nestedTable += '<td>' + formattedDate(itemData.deploymentDate) + '</td>';
        else nestedTable += '<td></td>';

        if (itemData.locationID !== null) nestedTable += '<td>' + itemData.location.desc + '</td>';
        else nestedTable += '<td></td>';

        nestedTable += '</tr>';
    });

    nestedTable += '</tbody>' + '</table>' + '</td>' + '</tr>';

    return nestedTable;
}

function showRelatedComponents(data) {
    nestedTable =
        '<tr>' +
            '<td colspan="4">' +
                '<table class="table table-striped table-condensed table-hover table-component" cellpadding="5" cellspacing="0" border="0" style="padding-left:50px;">' +
                    '<thead>' +
                        '<tr>' +
                            '<th>' + 'Asset#' + '</th>' +
                            '<th>' + 'IMTE' + '</th>' +
                            '<th>' + 'Serial Number' + '</th>' +
                            '<th>' + 'Description' + '</th>' +
                            '<th>' + 'Owner' + '</th>' +
                            '<th>' + 'Status' + '</th>' +
                            '<th>' + 'Model/Manufacturer' + '</th>' +
                            '<th>' + 'Service Provider' + '</th>' +
                            '<th>' + 'Calibration Due Date' + '</th>' +
                            '<th>' + 'Maintenance Due Date' + '</th>' +
                        '</tr>' +
                    '</thead>' +
                    '<tbody>';

    $.each(data, function (index, itemData) {
        // add detail row 
        nestedTable +=
            '<tr>' +
                '<td>' + itemData.assetnumber + '</td>' +
                '<td>' + itemData.imte + '</td>' +
                '<td>' + itemData.serialnumber + '</td>';

        if (itemData.descriptionID !== null) nestedTable += '<td>' + itemData.description.desc + '</td>';
        else nestedTable += '<td></td>';

        if (itemData.ownerID !== null) nestedTable += '<td>' + itemData.owner.desc + '</td>';
        else nestedTable += '<td></td>';

        if (itemData.statusID !== null) nestedTable += '<td>' + itemData.status.desc + '</td>';
        else nestedTable += '<td></td>';

        if (itemData.model_ManufacturerID !== null) nestedTable += '<td>' + itemData.model_Manufacturer.desc + '</td>';
        else nestedTable += '<td></td>';

        if (itemData.serviceProviderID !== null) nestedTable += '<td>' + itemData.serviceProvider.desc + '</td>';
        else nestedTable += '<td></td>';

        if (itemData.calibrationDate !== null) nestedTable += '<td>' + formattedDate(itemData.calibrationDate) + '</td>';
        else nestedTable += '<td></td>';

        if (itemData.maintenanceDate !== null) nestedTable += '<td>' + formattedDate(itemData.maintenanceDate) + '</td>';
        else nestedTable += '<td></td>';

    });

    nestedTable += '</tbody>' + '</table>' + '</td>' + '</tr>';

    return nestedTable;
}

function showDeployments(data) {
    nestedTable =
        '<tr>' +
            '<td colspan="4">' +
                '<table class="table table-striped table-condensed table-hover table-component" cellpadding="5" cellspacing="0" border="0" style="padding-left:50px;">' +
                    '<thead>' +
                            '<th>' + 'Deployment ID' + '</th>' +
                            '<th>' + 'Reference No.' + '</th>' +
                            '<th>' + 'Deployment Date' + '</th>' +
                            '<th>' + 'Location' + '</th>' +
                        '</tr>' +
                    '</thead>' +
                    '<tbody>';
    $.each(data, function (index, itemData) {
        nestedTable +=
            '<tr>' +
                '<td>' + itemData.deploymentID + '</td>' +
                '<td>' + itemData.referenceNo + '</td>';

        if (itemData.deploymentDate !== null) nestedTable += '<td>' + formattedDate(itemData.deploymentDate) + '</td>';
        else nestedTable += '<td></td>';

        if (itemData.locationID !== null) nestedTable += '<td>' + itemData.location.desc + '</td>';
        else nestedTable += '<td></td>';

        nestedTable += '</tr>';
    });

    nestedTable += '</tbody>' + '</table>' + '</td>' + '</tr>';

    return nestedTable;
}

// http://stackoverflow.com/questions/13459866/javascript-change-date-into-format-of-dd-mm-yyyy
function formattedDate(date) {
    var d = new Date(date || Date.now()),
        day = '' + d.getDate(),
        month = '' + (d.getMonth() + 1),
        year = d.getFullYear();

    if (month.length < 2) month = '0' + month;
    if (day.length < 2) day = '0' + day;

    return [day, month, year].join('/');
}

// http://stackoverflow.com/questions/13459866/javascript-change-date-into-format-of-dd-mm-yyyy
function convertDate(inputFormat) {
    function pad(s) { return s < 10 ? '0' + s : s; }
    var d = new Date(inputFormat);
    return [pad(d.getDate()), pad(d.getMonth() + 1), d.getFullYear()].join('/');
}

function formatThis(d) {
    //alert("Inside formatThis function");
    // `d` is the original data object for the row
    return '<table cellpadding="5" cellspacing="0" border="0" style="padding-left:50px;">' +
        '<tr>' +
            '<td>IMTE:</td>' +
            '<td>' + d[1] + '</td>' +
        '</tr>' +
        '<tr>' +
            '<td>Serial number:</td>' +
            '<td>' + d[2] + '</td>' +
        '</tr>' +
        '<tr>' +
            '<td>Extra info:</td>' +
            '<td>And any further details here (images etc)...</td>' +
        '</tr>' +
    '</table>';
}

//var filesadded = "" //list of files already added

// http://www.javascriptkit.com/javatutors/loadjavascriptcss.shtml
function loadjscssfile(filename, filetype) {
    if (filetype === "js") { //if filename is a external JavaScript file
        alert("inside loadjscssfile");
        var fileref = document.createElement('script');
        fileref.setAttribute("type", "text/javascript");
        fileref.setAttribute("src", filename);
        alert("file loaded");
    }
    else if (filetype === "css") { //if filename is an external CSS file
        fileref = document.createElement("link");
        fileref.setAttribute("rel", "stylesheet");
        fileref.setAttribute("type", "text/css");
        fileref.setAttribute("href", filename);
    }
    if (typeof fileref !== "undefined")
        document.getElementsByTagName("head")[0].appendChild(fileref);
}

// http://www.javascriptkit.com/javatutors/loadjavascriptcss2.shtml
function removejscssfile(filename, filetype) {
    alert("inside removejscssfile");
    var targetelement = filetype === "js" ? "script" : filetype === "css" ? "link" : "none"; //determine element type to create nodelist from
    var targetattr = filetype === "js" ? "src" : filetype === "css" ? "href" : "none"; //determine corresponding attribute to test for
    var allsuspects = document.getElementsByTagName(targetelement);
    for (var i = allsuspects.length; i >= 0; i--) { //search backwards within nodelist for matching elements to remove
        if (allsuspects[i] && allsuspects[i].getAttribute(targetattr) !== null && allsuspects[i].getAttribute(targetattr).indexOf(filename) !== -1)
            allsuspects[i].parentNode.removeChild(allsuspects[i]); //remove element by calling parentNode.removeChild()
    }
}

//removejscssfile("somestyle.css", "css") //remove all occurences "somestyle.css" on page

function checkloadjscssfile(filename, filetype) {
    if (filesadded.indexOf("[" + filename + "]") === -1) {
        loadjscssfile(filename, filetype);
        filesadded += "[" + filename + "]"; //List of files added in the form "[filename1],[filename2],etc"
        alert("file loaded");
    }
    else
        alert("file already added!");
}

/*  These functions are solely for SystemTabs */
// Compose Select List of Components
function showComponentDDL(myform) {
    /*  Or, if inside $(function) {
        $('#ComponentDescID').change(function () {
    */
    $("#divComponent").parent().addClass("hidden");

    var selectedDesc = $("#ComponentDescID option:selected").val();
    if (selectedDesc === null) return;

    // get components
    $.getJSON("/Systems/LoadComponents", { descId: selectedDesc },

        // then: compose
        function (data) {
            var select = $("#ComponentID");
            select.empty();
            select.append($('<option/>',
            {
                value: 0,
                text: "Select a Component"
            }));
            $.each(data, function (index, itemData) {
                select.append($('<option/>',
                {
                    value: itemData.value,
                    text: itemData.text
                }));
            });

            $("#divComponent").parent().slideDown().removeClass("hidden");

            // disable all components that have been previously selected
            $('#imteX tr').each(function () {
                $(this).children('td').each(function (i) {
                    if (i === 0) {
                        var imtenum = $(this).text().trim();  // always the first td

                        // If already selected: disable and strike it through
                        $('#ComponentID option:contains(' + imtenum + ')').prop('disabled', true).addClass("strikethrough");  // strikethrough: Works in IE11, FireFox; not in Chrome, Safari
                    }
                });
            });
        });

    // show
    //$("#divComponent").parent().slideDown().removeClass("hidden");
}

// Compose Table of Selected Components
function addIMTE(myform) {
    /*  Or, If inside $(function) {:
        $('#ComponentID').change(function () { 
    */
    // compose heading if the first row
    if (document.getElementById("imteX").rows.length === 0) {        //if ($('#imteX').rows.length == 0) {  // does not work
        // $('#imteX').append('<thead><tr><th scope="col" + >IMTE</th><th scope="col" + >Description</th><th scope="col" + >Calibration Date</th><th scope="col" + >Orientation</th></tr></thead>');
        $('#imteX').append('<thead><tr><th scope="col">IMTE</th>' +
                                      '<th scope="col">Description</th>' +
                                      '<th scope="col">Calibration Date</th>' +
                                      '<th scope="col">Maintenance Date</th>' +
                                      '<th scope="col" + >Orientation</th></tr></thead>');
    }

    // show details
    $("#ComponentID option:selected")
        .each(function () {
            // split DDL text into two parts: IMTE and Calibration Date/Time  alert($(this)[0].text);

            /* Note: Special character "/" need not be escaped inside [].
               See https://developer.mozilla.org/en-US/docs/Web/JavaScript/Guide/Regular_Expressions Section: "Special characters in regular expressions." [xyz] */
            //var partsArray = /\s*(\w+)\s*\(?([A-Za-z0-9/-]*)\)?$/.exec($(this)[0].text);
            var partsArray = /\s*(\w+)\s*\{?([A-Za-z0-9/-]*)\}?\{?([A-Za-z0-9/-]*)\}?$/.exec($(this)[0].text);
            //alert(partsArray);
            //alert(partsArray[1]);       // \s*(\w+)\s* : all alpha preceded and followed by 0|n spaces
            //alert(partsArray[2]);       // \{?([A-Za-z0-9/]*)\}?$ : 0|1 "{" followed by 0|n of [A-Za-z0-9/-] then followed by 0|1 "}"
            //alert(partsArray[3]);       // \{?([A-Za-z0-9/]*)\}?$ : 0|1 "{" followed by 0|n of [A-Za-z0-9/-] then followed by 0|1 "}"
            
            // get orientation value of selected option: 0 for Left or 1 for Right
            var checkedValue = $('.moduleRadio:checked').val();

            // get orientation text of selected option using id of the parent Label tag
            var checkedInnerHTML = document.querySelector('.moduleRadio:checked').parentNode.id;
            //var submodule = checkedValue + $(this)[0].value;

            // append the selected option
            $('#imteX > tbody:last')
                .append('<tr><td> <input type = "textbox" name = "selectedIMTEs" value = ' + checkedValue + $(this)[0].value + ' hidden >'
                    + partsArray[1] + '</td><td>'                                   // IMTE
                    + $("#ComponentDescID option:selected").val() + '</td><td>'     // Description
                    + partsArray[2] + '</td><td>'                                   // Calibration Date
                    + partsArray[3] + '</td><td>'                                   // Maintenance Date
                    + checkedInnerHTML + '</td><td><a href="#!" id=a' + $(this)[0].value + ' class="remove" rel='
                    + $(this)[0].value + ' onclick="returnIMTE(this)">Remove</a></td></tr>');

            // $(this).remove();  // remove selected option from DDL; commented out in favour of disabled and strikethrough
            $(this).prop('disabled', true).addClass("strikethrough");  // strikethrough: Works in IE11, FireFox; not in Chrome, Safari
        });
    $("#ComponentID option:eq(0)").prop('selected', true);  // default to first option
}

// Remove Selected Component and Return to cList of Components
function returnIMTE(removethis) {
    var value = $('#' + removethis.id).attr('rel');
    $("#ComponentID option[value=" + value + "]").removeAttr('disabled').removeClass("strikethrough");
    $('#a' + value).closest('tr').remove();
}

$(document).ready(function () {

    //checkloadjscssfile("https://cdn.datatables.net/r/bs-3.3.5/jqc-1.11.3,dt-1.10.8/datatables.min.js", "js") //success

    // Superceded by Data Tables
    //$('#myTable').pageMe({ pagerSelector: '#myPager', showPrevNext: true, hidePageNumbers: false, perPage: 10 }); // invokes jQuery.fn.pageMe plugin

    $('table.dataTable').DataTable({
        "lengthMenu": [[25, 10, 50, -1], [25, 10, 50, "All"]]
    });

    $('table' + '#scrollingTable').DataTable({
        "scrollY": "218px",
        "scrollCollapse": false,
        "paging": false
    });

    // Add event listener for opening and closing details
    // See https://datatables.net/examples/api/row_details.html
    $('#nestedTable tbody').on('click', 'td.details-control', function () {
        var table = $('#nestedTable').DataTable();
        //table.rows().every(function () {
        //    this.child('Row details for row: ' + this.index());
        //});
        var tr = $(this).closest('tr');
        var value = tr.attr('rel').trim();
        var partsArray = /\s*(\w+)\s*(.+$)/.exec(value);
        //alert("id: " + partsArray[1] + " table: " + partsArray[2])
        var row = table.row(tr);
        if (row.child.isShown()) {
            // This row is already open - close it
            row.child.hide();
            tr.removeClass('shown');
        }
        else {
            // Open this row
            row.child(showRelatedTable(partsArray[1], partsArray[2])).show();
            // row.child(formatThis(row.data())).show();
            tr.addClass('shown');
        }
    });

    // Deleting records
    /*
    $("#deleteRecord").on("click", function () {
        dialog.dialog("open");
    });
    */

    // https://www.mindstick.com/Articles/1117/crud-operation-using-modal-dialog-in-asp-dot-net-mvc
    //$.ajaxSetup({ cache: false });   
    /*
    $("#openDialog").on("click", function (e) {
        e.preventDefault();
        var url = $(this).attr('href');
        alert(url);

        $("#dialog-edit").dialog({
            title: 'Add Record',
            autoOpen: false,
            resizable: false,
            height: 500,
            width: 500,
            show: { effect: 'drop', direction: "up" },
            modal: true,
            draggable: true,
            open: function (event, ui) {
                event.preventDefault();
                $(this).load(url);
            },
            close: function (event, ui) {
                $(this).dialog('close');
            }
        });

        $("#dialog-edit").dialog('open');
        return false;
    });

    $(".editDialog").on("click", function (e) {
        e.preventDefault(); // added jss 09Aug'16
        var url = $(this).attr('href');

        $("#dialog-edit").dialog({
            title: 'Edit Record',
            autoOpen: false,
            resizable: false,
            height: 355,
            width: 400,
            show: { effect: 'drop', direction: "up" },
            modal: true,
            draggable: true,
            open: function (event, ui) {
                $(this).load(url);

            },
            close: function (event, ui) {
                $(this).dialog('close');
            }
        });

        $("#dialog-edit").dialog('open');
        return false;
    });
    */
    /*
    $(".confirmDialog").on("click", function (e) {
        alert("here");
        var url = $(this).attr('href');
        alert(url);
        $("#dialog-confirm").dialog({
            autoOpen: false,
            resizable: false,
            height: 170,
            width: 350,
            show: { effect: 'drop', direction: "up" },
            modal: true,
            draggable: true,
            buttons: {
                "OK": function () {
                    alert("OK");
                    $(this).dialog("close");
                    window.location = url;

                },
                "Cancel": function () {
                    alert("cancel");
                    $(this).dialog("close");

                }
            }
        });
        alert("there");
        $("#dialog-confirm").dialog('open');
        return false;
    });

    $(".viewDialog").live("click", function (e) {
        var url = $(this).attr('href');
        $("#dialog-view").dialog({
            title: 'View Description',
            autoOpen: false,
            resizable: false,
            height: 355,
            width: 400,
            show: { effect: 'drop', direction: "up" },
            modal: true,
            draggable: true,
            open: function (event, ui) {
                $(this).load(url);

            },
            buttons: {
                "Close": function () {
                    $(this).dialog("close");

                }
            },
            close: function (event, ui) {
                $(this).dialog('close');
            }
        });

        $("#dialog-view").dialog('open');
        return false;
    });

    $("#btncancel").live("click", function (e) {
        $("#dialog-edit").dialog('close');

    }); */

});

