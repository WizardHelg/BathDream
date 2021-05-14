
var connection = new signalR.HubConnectionBuilder().withUrl("/chat").build();
const messages = document.getElementById('msg_history');

connection.on("Send", function (data) {

    var when = new Date(data.when);
    var options = {
        year: 'numeric',
        month: 'long',
        day: 'numeric',
        hour: 'numeric',
        minute: 'numeric'
    };
    let elem = document.createElement("div");
    if (data.isMe == 1) {
        
        elem.className = "outgoing_msg";

        let elem2 = document.createElement('div');
        elem2.className = "sent_msg";

        let elem3 = document.createElement('p');
        elem3.innerHTML = data.message;

        let elemWhen = document.createElement('span');
        elemWhen.innerHTML = when.toLocaleString("ru", options);
        elemWhen.className = "time_date";

        elem2.appendChild(elem3);
        elem2.appendChild(elemWhen);

        elem.appendChild(elem2);
        elem.appendChild(elem2);

    }
    else {
        elem.className = "incoming_msg";
        let elem2 = document.createElement('div');
        elem2.className = "received_msg";
        let elem3 = document.createElement('div');
        elem3.className = "received_withd_msg";
        let elem4 = document.createElement('p');
        elem4.innerHTML = data.message;

        let elemWhen = document.createElement('span');
        elemWhen.className = "time_date";
        elemWhen.innerHTML = when.toLocaleString("ru", options);

        elem3.appendChild(elem4);
        elem3.appendChild(elemWhen);

        elem2.appendChild(elem3);
        elem.appendChild(elem2);
    }


    let cr = document.getElementById("chatroom");
    cr.insertAdjacentElement('beforeend', elem);
    scrollToBottom();
});

function scrollToBottom() {
    messages.scrollTop = messages.scrollHeight;
}

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
