/// <reference path="allgreen.ts" />
/// <reference path="../Scripts/typings/jquery/jquery.d.ts" />
/// <reference path="../Scripts/typings/signalr/signalr.d.ts" />

module AllGreen {
    export class Hub {
        private connection: HubConnection;
        private env: App;

        constructor(connection: HubConnection, env: App) {
            this.connection = connection;
            this.env = env;
            env.setServerStatus('Disconnected');
        }

        public connect() {
            this.attachToHub(this.connection, this.env);
            this.attachToConnectionEvents(this.connection, this.env);
            this.startConnection(this.connection, this.env);
        }

        private attachToHub(connection: HubConnection, env: App) {
            connection.createHubProxy('runnerHub').on('reload', () => {
                console.log('reloading...');
                env.reload();
            });
        }

        private attachToConnectionEvents(connection: HubConnection, env: App) {
            connection.stateChanged((change) => {
                console.log('state changed ', change, this.stateString(change.oldState), ' -> ', this.stateString(change.newState));
            });
            connection.error((error) => {
                console.log('error', error);
                env.setServerStatus('Error: ' + error);
            });
            connection.reconnecting(() => {
                console.log('reconnecting');
                env.setServerStatus('Reconnecting...');
            });
            connection.reconnected(() => {
                console.log('reconnected');
                env.setServerStatus('Reconnected');
            });
            connection.disconnected(() => {
                console.log('disconnected');
                env.setServerStatus('Disconnected');
            });
        }

        private startConnection(connection: HubConnection, env: App) {
            connection.start().done(() => {
                console.log('done');
                env.setServerStatus('Done');
            });
        }

        private stateString(state) {
            if (state === $.signalR.connectionState.connecting)
                return 'connecting';
            if (state === $.signalR.connectionState.connected)
                return 'connected';
            if (state === $.signalR.connectionState.reconnecting)
                return 'reconnecting';
            if (state === $.signalR.connectionState.disconnected)
                return 'disconnected';
        }
    }
}

() => {
    var app = AllGreen.App.getCurrent();
    if (app != null) {
        console.log('registering signalR hub');
        var connection = $.hubConnection();
        var hub = new AllGreen.Hub(connection, app);
        hub.connect();
    }
} ();
