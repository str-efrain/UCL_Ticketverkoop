let connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

//Disable send button until connection is established
document.getElementById("sendButton").disabled = true;

// Call client methods from the hub
//To receive messages from the hub, define a method using the on method
connection.on("ReceiveMessage", (user, message) => {
    let msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    let encodedMsg = user + " says " + msg;
    let li = document.createElement("li");
    li.textContent = encodedMsg;
    document.getElementById("messagesList").appendChild(li);
});
connection.start().then(() => {
    document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});
document.getElementById("sendButton").addEventListener("click", (event) => {
    let user = document.getElementById("userInput").value;
    let message = document.getElementById("messageInput").value;
    //Call hub methods from the client
    connection.invoke("SendMessage", user, message).catch((err) => {
        return console.error(err.toString());
    });

    // The preventDefault() method cancels the event if it is cancelable, meaning that the default action that belongs to the event will not occur.
    event.preventDefault();
});