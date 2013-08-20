/// <reference path="allgreen.ts" />
/// <reference path="../Scripts/typings/jquery/jquery.d.ts" />
/// <reference path="../Scripts/typings/signalr/signalr.d.ts" />
var AllGreen;
(function (AllGreen) {
    var Hub = (function () {
        function Hub(connection, env) {
            this.attachToHub(connection, env);
            this.attachToConnectionEvents(connection, env);
        }
        Hub.prototype.attachToHub = function (connection, env) {
            connection.createHubProxy('runnerHub').on('reload', function () {
                console.log('reloading...');
                env.reload();
            });
        };

        Hub.prototype.attachToConnectionEvents = function (connection, env) {
            var _this = this;
            connection.stateChanged(function (change) {
                console.log('state changed ', change, _this.stateString(change.oldState), ' -> ', _this.stateString(change.newState));
            });
            connection.error(function (error) {
                console.log('error', error);
                env.setServerStatus('Error: ' + error);
            });
            connection.reconnecting(function () {
                console.log('reconnecting');
                env.setServerStatus('Reconnecting...');
            });
            connection.reconnected(function () {
                console.log('reconnected');
                env.setServerStatus('Reconnected');
            });
            connection.disconnected(function () {
                console.log('disconnected');
                env.setServerStatus('Disconnected');
            });
            connection.received(function (data) {
                console.log('received', data);
                env.setServerStatus('Received');
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
})(AllGreen || (AllGreen = {}));
