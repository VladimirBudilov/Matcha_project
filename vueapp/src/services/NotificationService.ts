import { HubConnection, HubConnectionBuilder, HttpTransportType } from "@microsoft/signalr";
import type { IHttpConnectionOptions } from "@microsoft/signalr";

function createConnection(): HubConnection {
    const options: IHttpConnectionOptions = {
        transport: HttpTransportType.WebSockets,
        accessTokenFactory: () => localStorage.getItem('token') || ''
    };

    return new HubConnectionBuilder()
        .withUrl(process.env.BASE_URL+"notification", options)
        .withAutomaticReconnect()
        .build();
}

export default createConnection;