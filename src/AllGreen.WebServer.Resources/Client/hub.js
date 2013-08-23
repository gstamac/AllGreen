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
            var hubProxy = this.attachToHub(this.connection, this.app);
            this.attachToConnectionEvents(this.connection, this.app);
            this.startConnection(this.connection, this.app);
            return hubProxy;
        };

        Hub.prototype.attachToHub = function (connection, app) {
            var hubProxy = connection.createHubProxy('runnerHub');
            hubProxy.on('reload', function () {
                console.log('reloading...');
                app.reload();
            });
            return hubProxy;
        };

        Hub.prototype.attachToConnectionEvents = function (connection, app) {
            var _this = this;
            connection.stateChanged(function (change) {
                console.log('state changed ', change, _this.stateString(change.oldState), ' -> ', _this.stateString(change.newState));
            });
            connection.error(function (error) {
                console.log('error', error);
                app.setServerStatus('Error: ' + error);
            });
            connection.reconnecting(function () {
                console.log('reconnecting');
                app.setServerStatus('Reconnecting...');
            });
            connection.reconnected(function () {
                console.log('reconnected');
                app.setServerStatus('Reconnected');
            });
            connection.disconnected(function () {
                console.log('disconnected');
                app.setServerStatus('Disconnected');
            });
        };

        Hub.prototype.startConnection = function (connection, app) {
            connection.start().done(function () {
                console.log('done');
                app.setServerStatus('Done');
            });
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
            this.hubProxy.invoke('reset');
        };

        HubReporter.prototype.started = function () {
            this.hubProxy.invoke('started');
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
        console.log('registering signalR hub');
        var connection = $.hubConnection();
        var hub = new AllGreen.Hub(connection, app);
        var hubProxy = hub.connect();
        app.registerRunnerReporter(new AllGreen.HubReporter(hubProxy));
    }
})();
