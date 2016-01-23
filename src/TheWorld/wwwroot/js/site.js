/* site.js */
(function () { //anonoymous function to avoid name collision with global functions



    //var ele = $("#username");

    //ele.text("Joe Bloggs");

    //var main = $("#main");

    //main.on("mouseenter", function () {
    //    main.style.backgroundColor = "#888";
    //});

    //main.on("mouseleave", function () {
    //    main.style.backgroundColor = "";
    //});

    //var menuitems = $("ul.menu li a");

    //menuitems.on("click", function () {
    //    var me = $(this);
    //    alert(me.text());
    //});

    // start name of jQuery object with $ for clarity
    var $sidebarAndWrapper = $("#sidebar, #wrapper");

    $("#sidebartoggle").on("click", function () {
        $sidebarAndWrapper.toggleClass("hide-sidebar");

        var me = $(this);

        if ($sidebarAndWrapper.hasClass("hide-sidebar")) {
            me.text("Show Sidebar");
        } else {
            me.text("Hide Sidebar");
        }
    });

    


})();//parms can be passsed in here

