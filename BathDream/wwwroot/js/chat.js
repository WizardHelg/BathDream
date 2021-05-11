"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chat").build();

connection.on("Send", function (data) {

    let elem = document.createElement("p");
    elem.appendChild(document.createTextNode(data));
    let firstElem = document.getElementById("chatroom").firstChild;
    document.getElementById("chatroom").insertBefore(elem, firstElem);

});


document.getElementById("sendBtn").addEventListener("click", function (e) {
    let message = document.getElementById("message").value;
    connection.invoke("Send", message);
});

connection.start();
