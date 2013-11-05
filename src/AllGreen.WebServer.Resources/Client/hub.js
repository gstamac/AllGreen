/// <reference path="allgreen.ts" />
/// <reference path="../Scripts/typings/jquery/jquery.d.ts" />
/// <reference path="../Scripts/typings/signalr/signalr.d.ts" />
var AllGreen;
(function (AllGreen) {
    var Hub = (function () {
        function Hub(connection, app, reporter, reconnectTimeout) {
            if (typeof reconnectTimeout === "undefined") { reconnectTimeout = 5000; }
            this.connection = connection;
            this.app = app;
            this.reporter = reporter;
            this.reconnectTimeout = reconnectTimeout;
            app.setServerStatus('Disconnected');
        }
        Hub.prototype.connect = function () {
            this.hubProxy = this.attachToHub(this.connection, this.app);
            this.attachToConnectionEvents(this.connection, this.app);
            this.startConnection(this.connection, this.app);
        };

        Hub.prototype.attachToHub = function (connection, app) {
            var hubProxy = connection.createHubProxy('runnerHub');
            hubProxy.on('reload', function () {
                app.log('reloading...');
                app.reload();
            });
            return hubProxy;
        };

        Hub.prototype.attachToConnectionEvents = function (connection, app) {
            var _this = this;
            connection.stateChanged(function (change) {
                app.log('state changed ', change, _this.stateString(change.oldState), ' -> ', _this.stateString(change.newState));
            });
            connection.error(function (error) {
                app.log('error', error);
                app.setServerStatus('Error: ' + error);
            });
            connection.reconnecting(function () {
                app.log('reconnecting');
                app.setServerStatus('Reconnecting...');
            });
            connection.reconnected(function () {
                app.log('reconnected');
                app.setServerStatus('Reconnected');
                _this.register();
            });
            connection.disconnected(function () {
                app.log('disconnected');
                app.setServerStatus('Disconnected');
                app.log('reconnecting in 5s');
                setTimeout(function () {
                    if (app.reconnectEnabled) {
                        _this.startConnection(connection, app);
                    }
                }, _this.reconnectTimeout);
            });
        };

        Hub.prototype.startConnection = function (connection, app) {
            var _this = this;
            connection.start().done(function () {
                app.log('connected as ' + connection.id);
                app.setServerStatus('Connected');
                _this.register();
            });
        };

        Hub.prototype.register = function () {
            this.reporter.hubProxyConnected(this.hubProxy);
            this.hubProxy.invoke('register');
        };

        Hub.prototype.stateString = function (state) {
            if (state === $.signalR.connectionState.connecting)
                return 'connecting';
            if (state === $.signalR.connectionState.connected)
                return 'connected';
            if (state === $.signalR.connectionState.reconnecting)
                return 'reconnecting';
            if (state === $.signalR.connectionState.disconnected)
                return 'disconnected';
        };
        return Hub;
    })();
    AllGreen.Hub = Hub;

    var HubReporter = (function () {
        function HubReporter() {
            this.hubProxy = null;
        }
        HubReporter.prototype.isReady = function () {
            return this.hubProxy != null;
        };

        HubReporter.prototype.hubProxyConnected = function (hubProxy) {
            this.hubProxy = hubProxy;
        };

        HubReporter.prototype.reset = function () {
            this.hubProxy.invoke('reset');
        };

        HubReporter.prototype.started = function (totalSpecs) {
            this.hubProxy.invoke('started', totalSpecs);
        };

        HubReporter.prototype.specUpdated = function (spec) {
            this.hubProxy.invoke('specUpdated', spec);
        };

        HubReporter.prototype.finished = function () {
            this.hubProxy.invoke('finished');
        };
        return HubReporter;
    })();
    AllGreen.HubReporter = HubReporter;
})(AllGreen || (AllGreen = {}));

(function () {
    var app = AllGreen.App.getCurrent();
    if (app != null) {
        app.log('registering signalR hub');
        var reporter = new AllGreen.HubReporter();
        app.registerRunnerReporter(reporter);
        var connection = $.hubConnection();
        var hub = new AllGreen.Hub(connection, app, reporter);
        hub.connect();
    }
})();
//# sourceMappingURL=hub.js.map
