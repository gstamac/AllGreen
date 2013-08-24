/// <reference path="allgreen.ts" />
/// <reference path="../Scripts/typings/jquery/jquery.d.ts" />
/// <reference path="../Scripts/typings/signalr/signalr.d.ts" />
var AllGreen;
(function (AllGreen) {
    var Hub = (function () {
        function Hub(connection, app) {
            this.connection = connection;
            this.app = app;
            app.setServerStatus('Disconnected');
        }
        Hub.prototype.connect = function () {
            this.hubProxy = this.attachToHub(this.connection, this.app);
            this.attachToConnectionEvents(this.connection, this.app);
            this.startConnection(this.connection, this.app);
            return this.hubProxy;
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
            });
            connection.disconnected(function () {
                app.log('disconnected');
                app.setServerStatus('Disconnected');
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
            this.hubProxy.invoke('register', this.connection.id, navigator.userAgent);
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
        function HubReporter(hubProxy) {
            this.hubProxy = hubProxy;
        }
        HubReporter.prototype.reset = function () {
            this.hubProxy.invoke('reset', this.hubProxy.connection.id);
        };

        HubReporter.prototype.started = function (totalSpecs) {
            this.hubProxy.invoke('started', this.hubProxy.connection.id, totalSpecs);
        };

        HubReporter.prototype.specUpdated = function (spec) {
            this.hubProxy.invoke('specUpdated', this.hubProxy.connection.id, spec);
        };

        HubReporter.prototype.finished = function () {
            this.hubProxy.invoke('finished', this.hubProxy.connection.id);
        };
        return HubReporter;
    })();
    AllGreen.HubReporter = HubReporter;
})(AllGreen || (AllGreen = {}));

(function () {
    var app = AllGreen.App.getCurrent();
    if (app != null) {
        app.log('registering signalR hub');
        var connection = $.hubConnection();
        var hub = new AllGreen.Hub(connection, app);
        var hubProxy = hub.connect();
        app.registerRunnerReporter(new AllGreen.HubReporter(hubProxy));
    }
})();
