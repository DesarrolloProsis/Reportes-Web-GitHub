//$(function () {
//    $(window).on("load", function () {
//        /*Al terminar de cargar el sitio, primero se va la animación del Preloader*/
//        $("#loader").fadeOut();
//        /*Medio segundo despues, se va poco a poco el fondo del preloader*/
//        $("#loader-wrapper").delay(500).fadeOut("slow");
//    });
//});

function OutLoader() {
    /*Al terminar de cargar el sitio, primero se va la animación del Preloader*/
    $("#loader").fadeOut();
    /*Medio segundo despues, se va poco a poco el fondo del preloader*/
    $("#loader-wrapper").delay(500).fadeOut("slow");
}

function InLoaderVal() {
    /*Al terminar de cargar el sitio, primero se va la animación del Preloader*/
    $("#loader").fadeIn();
    /*Medio segundo despues, se va poco a poco el fondo del preloader*/
    $("#loader-wrapper").delay(500).fadeIn("slow");
}