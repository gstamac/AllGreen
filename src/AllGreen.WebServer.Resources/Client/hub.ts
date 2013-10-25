/// <reference path="allgreen.ts" />
/// <reference path="../Scripts/typings/jquery/jquery.d.ts" />
/// <reference path="../Scripts/typings/signalr/signalr.d.ts" />

module AllGreen {
    export class Hub {
        private connection: HubConnection;
        private hubProxy: HubProxy;
        private app: App;
        private reporter: HubReporter;
        private reconnectTimeout: number;

        constructor(connection: HubConnection, app: App, reporter: HubReporter, reconnectTimeout: number = 5000) {
            this.connection = connection;
            this.app = app;
            this.reporter = reporter;
            this.reconnectTimeout = reconnectTimeout;
            app.setServerStatus('Disconnected');
        }

        public connect() {
            this.hubProxy = this.attachToHub(this.connection, this.app);
            this.attachToConnectionEvents(this.connection, this.app);
            this.startConnection(this.connection, this.app);
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
                this.register();
            });
            connection.disconnected(() => {
                app.log('disconnected');
                app.setServerStatus('Disconnected');
                app.log('reconnecting in 5s');
                setTimeout(() => {
                    if (app.reconnectEnabled) {
                        this.startConnection(connection, app);
                    }
                }, this.reconnectTimeout);
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
            this.reporter.hubProxyConnected(this.hubProxy);
            this.hubProxy.invoke('register');
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

        constructor() {
            this.hubProxy = null;
        }

        public isReady(): boolean {
            return this.hubProxy != null;
        }

        public hubProxyConnected(hubProxy: HubProxy) {
            this.hubProxy = hubProxy;
        }

        public reset() {
            this.hubProxy.invoke('reset');
        }

        public started(totalSpecs: number) {
            this.hubProxy.invoke('started', totalSpecs);
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
        app.log('registering signalR hub');
        var reporter = new AllGreen.HubReporter();
        app.registerRunnerReporter(reporter);
        var connection = $.hubConnection();
        var hub = new AllGreen.Hub(connection, app, reporter);
        hub.connect();
    }
} ();
