/// <reference path="allgreen.ts" />
/// <reference path="../Scripts/typings/jquery/jquery.d.ts" />
/// <reference path="../Scripts/typings/signalr/signalr.d.ts" />

module AllGreen {
    export class Hub {
        private connection: HubConnection;
        private app: App;

        constructor(connection: HubConnection, app: App) {
            this.connection = connection;
            this.app = app;
            app.setServerStatus('Disconnected');
        }

        public connect(): HubProxy {
            var hubProxy = this.attachToHub(this.connection, this.app);
            this.attachToConnectionEvents(this.connection, this.app);
            this.startConnection(this.connection, this.app);
            return hubProxy;
        }

        private attachToHub(connection: HubConnection, app: App): HubProxy {
            var hubProxy = connection.createHubProxy('runnerHub');
            hubProxy.on('reload', () => {
                console.log('reloading...');
                app.reload();
            });
            return hubProxy;
        }

        private attachToConnectionEvents(connection: HubConnection, app: App) {
            connection.stateChanged((change) => {
                console.log('state changed ', change, this.stateString(change.oldState), ' -> ', this.stateString(change.newState));
            });
            connection.error((error) => {
                console.log('error', error);
                app.setServerStatus('Error: ' + error);
            });
            connection.reconnecting(() => {
                console.log('reconnecting');
                app.setServerStatus('Reconnecting...');
            });
            connection.reconnected(() => {
                console.log('reconnected');
                app.setServerStatus('Reconnected');
            });
            connection.disconnected(() => {
                console.log('disconnected');
                app.setServerStatus('Disconnected');
            });
        }

        private startConnection(connection: HubConnection, app: App) {
            connection.start().done(() => {
                console.log('done');
                app.setServerStatus('Done');
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

    export class HubReporter implements IRunnerReporter {
        private hubProxy: HubProxy;

        constructor(hubProxy: HubProxy) {
            this.hubProxy = hubProxy;
        }

        public reset() {
            this.hubProxy.invoke('reset');
        }

        public started() {
            this.hubProxy.invoke('started');
        }

        public specUpdated(spec: ISpec) {
            this.hubProxy.invoke('specUpdated', spec);
        }

        public finished() {
            this.hubProxy.invoke('finished');
        }

    }
}

() => {
    var app = AllGreen.App.getCurrent();
    if (app != null) {
        console.log('registering signalR hub');
        var connection = $.hubConnection();
        var hub = new AllGreen.Hub(connection, app);
        var hubProxy = hub.connect();
        app.registerRunnerReporter(new AllGreen.HubReporter(hubProxy));
    }
} ();
