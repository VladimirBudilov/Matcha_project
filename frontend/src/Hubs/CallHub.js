import { HubConnectionBuilder } from '@microsoft/signalr';

class CallHub {
    constructor() {
        this.connection = new HubConnectionBuilder()
            .withUrl("/callHub")
            .build();
    }

    start() {
        return this.connection.start();
    }

    on(method, handler) {
        this.connection.on(method, handler);
    }

    invoke(method, ...args) {
        return this.connection.invoke(method, ...args);
    }
}
 
export default new CallHub();