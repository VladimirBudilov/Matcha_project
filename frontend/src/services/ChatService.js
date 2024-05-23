import * as signalR from "@microsoft/signalr";

function createConnection  (){
    return new signalR.HubConnectionBuilder()
        .withUrl("https://localhost:5101/chat",
     {
            accessTokenFactory: () => localStorage.getItem('token')
            })
        .withAutomaticReconnect()
        .configureLogging(signalR.LogLevel.Information)
        .build();
}

export default createConnection;