jQuery(function ($) {
    $('input.datetimepicker').datepicker({
        duration: '',
        changeMonth: false,
        changeYear: false,
        yearRange: '1920:2020',
        showTime: false,
        time24h: true
    });

    $.datepicker.regional['sq'] = {
        closeText: "mbylle",
        prevText: "&#x3C;mbrapa",
        nextText: "Përpara&#x3E;",
        currentText: "sot",
        monthNames: ["Janar", "Shkurt", "Mars", "Prill", "Maj", "Qershor",
            "Korrik", "Gusht", "Shtator", "Tetor", "Nëntor", "Dhjetor"],
        monthNamesShort: ["Jan", "Shk", "Mar", "Pri", "Maj", "Qer",
            "Kor", "Gus", "Sht", "Tet", "Nën", "Dhj"],
        dayNames: ["E Diel", "E Hënë", "E Martë", "E Mërkurë", "E Enjte", "E Premte", "E Shtune"],
        dayNamesShort: ["Di", "Hë", "Ma", "Më", "En", "Pr", "Sh"],
        dayNamesMin: ["Di", "Hë", "Ma", "Më", "En", "Pr", "Sh"],
        weekHeader: "Ja",
        dateFormat: "dd.mm.yy",
        firstDay: 1,
        isRTL: false,
        showMonthAfterYear: false,
        yearSuffix: ""
    };

    $.datepicker.setDefaults($.datepicker.regional['sq']);
});