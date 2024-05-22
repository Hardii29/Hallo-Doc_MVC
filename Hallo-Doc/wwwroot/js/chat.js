const connection = new signalR.HubConnectionBuilder()
    .withUrl("/ChatHub")
    .build();

$(document).ready(function () {
    var userId = $("#siteUser").val();
    var receiver = $("#userName").val();

    $.ajax({
        url: '/Admin/GetChatHistory',
        type: 'GET',
        data: { Sender: userId, Reciever : receiver },
        success: function (data) {
            console.log(data);
            data.forEach(message => {
                const isReciever = message.reciever === receiver;
                if (isReciever == true) {
                    $("#messagesList").append("<li><div class='d-flex flex-row justify-content-end mb-4'><div class= 'p-3 border' style = 'border-radius: 15px; background-color: #fbfbfb;'><p class='small mb-0'>" + message.message + "</p></div ></div ></li> ");
                }
                else {
                    $("#messagesList").append("<li><div class='d-flex flex-row justify-content-start mb-4'><div class='p-3 border border-info' style='border-radius: 15px; background-color: rgba(57, 192, 237,.2);'><p class='small mb-0'>" + message.message + "</p></div></div></li>");
                }
              
            });
        },
        error: function (error) {
            console.error('Error fetching chat history:', error);
        }
    });
});

//This method receive the message and Append to our list  
connection.on("ReceiveMessage", (user, message) => {
    const msg = message.replace(/&/g, "&").replace(/</g, "<").replace(/>/g, ">");
    const encodedMsg = user + " :: " + msg;
    const li = document.createElement("li");
    li.textContent = encodedMsg;
    $("#messagesList").append("<li><div class='d-flex flex-row justify-content-start mb-4'><div class='p-3 border border-info' style='border-radius: 15px; background-color: rgba(57, 192, 237,.2);'><p class='small mb-0'>" + msg + "</p></div></div></li>");
    $("#userMessage").val("");
    /*document.getElementById("messagesList").appendChild(li);*/
});

connection.start().catch(err => console.error(err.toString()));

//Send the message  

document.getElementById("sendMessage").addEventListener("click", event => {
    const user = document.getElementById("userName").value;
    const message = document.getElementById("userMessage").value;
    console.log(user);
    console.log(message);
    connection.invoke("SendMessage", user, message).catch(err => console.error(err.toString()));
    $("#messagesList").append("<li><div class='d-flex flex-row justify-content-end mb-4'><div class= 'p-3 border' style = 'border-radius: 15px; background-color: #fbfbfb;'><p class='small mb-0'>" + message + "</p></div ></div ></li> ");
    $("#userMessage").val("");
    event.preventDefault();
});