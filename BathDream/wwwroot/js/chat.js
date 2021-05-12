
var connection = new signalR.HubConnectionBuilder().withUrl("/chat").build();

connection.on("Send", function (data) {

    let elem = document.createElement("p");
    elem.insertAdjacentHTML('afterbegin', data);
    let cr = document.getElementById("chatroom");
    cr.insertAdjacentElement('beforeend', elem);
});


document.getElementById("sendBtn").addEventListener("click", function (e) {
    let message = document.getElementById("message").value;
    connection.invoke("Send", message);
});

document.getElementById("message").addEventListener("keyup", function (event) {
    if (!event.shiftKey && event.keyCode === 13) {
        event.preventDefault();
        document.getElementById("sendBtn").click();
        document.getElementById("message").value = "";
    }
});

connection.start();
