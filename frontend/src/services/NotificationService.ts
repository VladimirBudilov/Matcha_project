import { HubConnection, HubConnectionBuilder, LogLevel, HttpTransportType } from "@microsoft/signalr";
import type { IHttpConnectionOptions } from "@microsoft/signalr";

function createConnection(): HubConnection {
    const options: IHttpConnectionOptions = {
        transport: HttpTransportType.WebSockets,
        accessTokenFactory: () => localStorage.getItem('token') || ''
    };

    return new HubConnectionBuilder()
        .withUrl("https://localhost:5101/notification", options)
        .withAutomaticReconnect()
        .configureLogging(LogLevel.Information)
        .build();
}

export default createConnection;