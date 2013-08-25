/// <reference path="allgreen.ts" />
/// <reference path="../Scripts/typings/jquery/jquery.d.ts" />
/// <reference path="../Scripts/typings/signalr/signalr.d.ts" />

module AllGreen {
    export class Hub {
        private connection: HubConnection;
        private hubProxy: HubProxy;
        private app: App;
        private reconnectTimeout: number;

        constructor(connection: HubConnection, app: App, reconnectTimeout: number = 5000) {
            this.connection = connection;
            this.app = app;
            this.reconnectTimeout = reconnectTimeout;
            app.setServerStatus('Disconnected');
        }

        public connect(): HubProxy {
            this.hubProxy = this.attachToHub(this.connection, this.app);
            this.attachToConnectionEvents(this.connection, this.app);
            this.startConnection(this.connection, this.app);
            return this.hubProxy;
        }

        private attachToHub(connection: HubConnection, app: App): HubProxy {
            var hubProxy = connection.createHubProxy('runnerHub');
            hubProxy.on('reload', () => {
                app.log('reloading...');
                app.reload();
            });
            return hubProxy;
        }

        private attachToConnectionEvents(connection: HubConnection, app: App) {
            connection.stateChanged((change: SignalRStateChange) => {
                app.log('state changed ', change, this.stateString(change.oldState), ' -> ', this.stateString(change.newState));
            });
            connection.error((error: string) => {
                app.log('error', error);
                app.setServerStatus('Error: ' + error);
            });
            connection.reconnecting(() => {
                app.log('reconnecting');
                app.setServerStatus('Reconnecting...');
            });
            connection.reconnected(() => {
                app.log('reconnected');
                app.setServerStatus('Reconnected');
            });
            connection.disconnected(() => {
                app.log('disconnected');
                app.setServerStatus('Disconnected');
                app.log('reconnecting in 5s');
                setTimeout(() => {
                    this.startConnection(connection, app);
                }, this.reconnectTimeout); // Restart connection after 5 seconds.
            });
        }

        private startConnection(connection: HubConnection, app: App) {
            connection.start().done(() => {
                app.log('connected as ' + connection.id);
                app.setServerStatus('Connected');
                this.register();
            });
        }

        private register() {
            this.hubProxy.invoke('register', this.connection.id, navigator.userAgent);
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
            this.hubProxy.invoke('reset', this.hubProxy.connection.id);
        }

        public started(totalSpecs: number) {
            this.hubProxy.invoke('started', this.hubProxy.connection.id, totalSpecs);
        }

        public specUpdated(spec: ISpec) {
            this.hubProxy.invoke('specUpdated', this.hubProxy.connection.id, spec);
        }

        public finished() {
            this.hubProxy.invoke('finished', this.hubProxy.connection.id);
        }
    }
}

() => {
    var app = AllGreen.App.getCurrent();
    if (app != null) {
        app.log('registering signalR hub');
        var connection = $.hubConnection();
        var hub = new AllGreen.Hub(connection, app);
        var hubProxy = hub.connect();
        app.registerRunnerReporter(new AllGreen.HubReporter(hubProxy));
    }
} ();
