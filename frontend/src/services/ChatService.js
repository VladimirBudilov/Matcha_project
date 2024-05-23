import * as signalR from "@microsoft/signalr";

const connection = new signalR.HubConnectionBuilder()
    .withUrl("https://localhost:5101/chat", {
        accessTokenFactory: () => localStorage.getItem('token')
    })
    .configureLogging(signalR.LogLevel.Information)
    .build();

connection.start().catch(err => console.error(err.toString()));

export default connection;