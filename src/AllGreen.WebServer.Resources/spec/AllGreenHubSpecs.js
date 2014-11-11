/// <reference path="../Scripts/typings/jasmine/jasmine.d.ts" />
/// <reference path="../Scripts/typings/signalr/signalr.d.ts" />
/// <reference path="../Client/hub.ts" />
var _this = this;
describe("AllGreen SignalR Hub", function () {
    beforeEach(function () {
        _this.app = jasmine.createSpyObj('app', ['runTests', 'setServerStatus', 'specUpdated', 'log']);

        //this.app['log'].andCallFake(console.log);
        _this.hubProxy = jasmine.createSpyObj('hubProxy', ['on', 'invoke']);
        _this.proxyCallback = null;
        _this.hubProxy['on'].andCallFake(function (eventName, callback) {
            _this.proxyCallback = callback;
        });

        _this.connectionCallbacks = [];
        var callbacks = ['stateChanged', 'error', 'reconnecting', 'reconnected', 'disconnected'];
        _this.connection = jasmine.createSpyObj('connection', callbacks.concat(['createHubProxy', 'start', 'received']));
        _this.connection['createHubProxy'].andReturn(_this.hubProxy);
        _this.connection['start'].andReturn({ done: function (callback) {
                _this.connectionCallbacks['done'] = callback;
            } });
        callbacks.forEach(function (callbackName) {
            _this.connection[callbackName].andCallFake(function (callback) {
                _this.connectionCallbacks[callbackName] = callback;
            });
        });

        _this.hubReporter = jasmine.createSpyObj('hubReporter', ['hubProxyConnected']);
        _this.hub = new AllGreen.Hub(_this.connection, _this.app, _this.hubReporter, 1);
    });

    it("Starts as disconnected", function () {
        expect(_this.app.setServerStatus).toHaveBeenCalledWith('Disconnected');
    });

    it("Connects to server", function () {
        _this.hub.connect();

        expect(_this.connection.createHubProxy).toHaveBeenCalledWith('runnerHub');
        expect(_this.connection.start).toHaveBeenCalled();
        expect(_this.hubProxy.on).toHaveBeenCalledWith('runTests', jasmine.any(Function));
        expect(_this.proxyCallback).toEqual(jasmine.any(Function));
        expect(_this.connectionCallbacks['done']).toEqual(jasmine.any(Function));
        _this.connectionCallbacks['done']();
        expect(_this.app.setServerStatus).toHaveBeenCalledWith('Connected');
    });

    it("Should initialize after connect", function () {
        _this.hub.connect();
        _this.connectionCallbacks['done']();
        expect(_this.hubReporter.hubProxyConnected).toHaveBeenCalledWith(_this.hubProxy);
        expect(_this.hubProxy.invoke).toHaveBeenCalledWith('register');
    });

    it("Should initialize after reconnect", function () {
        _this.hub.connect();
        _this.connectionCallbacks['reconnected']();
        expect(_this.hubReporter.hubProxyConnected).toHaveBeenCalledWith(_this.hubProxy);
        expect(_this.hubProxy.invoke).toHaveBeenCalledWith('register');
    });

    it("Calls App.runTests on runTests message", function () {
        _this.hub.connect();
        _this.proxyCallback();
        expect(_this.app.runTests).toHaveBeenCalled();
    });

    it("Displays status on error", function () {
        _this.hub.connect();

        expect(_this.connectionCallbacks['error']).toEqual(jasmine.any(Function));
        _this.connectionCallbacks['error']('ERROR');
        expect(_this.app.setServerStatus).toHaveBeenCalledWith('Error: ERROR');
    });

    [
        { method: 'reconnecting', status: 'Reconnecting...' },
        { method: 'reconnected', status: 'Reconnected' },
        { method: 'disconnected', status: 'Disconnected' }].forEach(function (data) {
        it("Displays status on " + data.method, function () {
            _this.hub.connect();

            expect(_this.connectionCallbacks[data.method]).toEqual(jasmine.any(Function));
            _this.connectionCallbacks[data.method]();
            expect(_this.app.setServerStatus).toHaveBeenCalledWith(data.status);
        });
    });

    it("Reconnects after disconnect if enabled", function () {
        _this.hub.connect();

        _this.app.reconnectEnabled = true;
        expect(_this.connectionCallbacks['disconnected']).toEqual(jasmine.any(Function));
        _this.connectionCallbacks['disconnected']();
        waitsFor(function () {
            return _this.connection.start.callCount == 2;
        }, "Reconnect wasn't called", 10);
    });

    it("Doesn't reconnect on disconnect if disabled", function () {
        runs(function () {
            _this.hub.connect();

            _this.app.reconnectEnabled = false;
            expect(_this.connectionCallbacks['disconnected']).toEqual(jasmine.any(Function));
            _this.connectionCallbacks['disconnected']();
        });
        waits(10);
        runs(function () {
            expect(_this.connection.start.callCount).toBe(1);
        });
    });
});

describe("AllGreen SignalR HubReporter", function () {
    beforeEach(function () {
        _this.hubProxy = jasmine.createSpyObj('hubProxy', ['invoke']);
        _this.hubReporter = new AllGreen.HubReporter();
    });

    it("Should be ready after registered not before", function () {
        expect(_this.hubReporter.isReady()).toBeFalsy();
        _this.hubReporter.hubProxyConnected(_this.hubProxy);
        expect(_this.hubReporter.isReady()).toBeTruthy();
    });

    it("Should report started", function () {
        _this.hubReporter.hubProxyConnected(_this.hubProxy);
        _this.hubReporter.started(10);
        expect(_this.hubProxy.invoke).toHaveBeenCalledWith('started', 10);
    });

    ['reset', 'finished'].forEach(function (method) {
        it("Should report " + method, function () {
            _this.hubReporter.hubProxyConnected(_this.hubProxy);
            _this.hubReporter[method]();
            expect(_this.hubProxy.invoke).toHaveBeenCalledWith(method);
        });
    });

    it("Should report specUpdated", function () {
        var spec = { id: 123, name: 'test 1', suite: null, status: 2 /* Passed */, steps: [] };
        _this.hubReporter.hubProxyConnected(_this.hubProxy);
        _this.hubReporter.specUpdated(spec);
        expect(_this.hubProxy.invoke).toHaveBeenCalledWith('specUpdated', spec);
    });
});
//# sourceMappingURL=AllGreenHubSpecs.js.map
