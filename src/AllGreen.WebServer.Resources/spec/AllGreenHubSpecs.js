/// <reference path="../Scripts/typings/jasmine/jasmine.d.ts" />
/// <reference path="../Scripts/typings/signalr/signalr.d.ts" />
/// <reference path="../Client/hub.ts" />
var _this = this;
describe("AllGreen SignalR Hub", function () {
    beforeEach(function () {
        _this.app = jasmine.createSpyObj('app', ['reload', 'setServerStatus', 'specUpdated', 'log']);

        //this.app['log'].andCallFake(console.log);
        _this.hubProxy = jasmine.createSpyObj('hubProxy', ['on', 'invoke']);
        _this.proxyCallback = null;
        _this.hubProxy['on'].andCallFake(function (eventName, callback) {
            _this.proxyCallback = callback;
        });

        var callbacks = ['stateChanged', 'error', 'reconnecting', 'reconnected', 'disconnected'];
        _this.connection = jasmine.createSpyObj('connection', callbacks.concat(['createHubProxy', 'start', 'received']));
        _this.connection['createHubProxy'].andReturn(_this.hubProxy);
        _this.connection['start'].andReturn({ done: function (callback) {
                _this.connectionCallbacks['done'] = callback;
            } });
        _this.connection.id = 'connectionId';
        _this.connectionCallbacks = [];
        callbacks.forEach(function (callbackName) {
            _this.connection[callbackName].andCallFake(function (callback) {
                _this.connectionCallbacks[callbackName] = callback;
            });
        });

        _this.hub = new AllGreen.Hub(_this.connection, _this.app, 1);
    });

    it("Starts as disconnected", function () {
        expect(_this.app.setServerStatus).toHaveBeenCalledWith('Disconnected');
    });

    it("Connects to server", function () {
        var hubProxy = _this.hub.connect();

        expect(hubProxy).toBeDefined();
        expect(_this.connection.createHubProxy).toHaveBeenCalledWith('runnerHub');
        expect(_this.connection.start).toHaveBeenCalled();
        expect(_this.hubProxy.on).toHaveBeenCalledWith('reload', jasmine.any(Function));
        expect(_this.proxyCallback).toEqual(jasmine.any(Function));
        expect(_this.connectionCallbacks['done']).toEqual(jasmine.any(Function));
        _this.connectionCallbacks['done']();
        expect(_this.app.setServerStatus).toHaveBeenCalledWith('Connected');
    });

    it("Should initialize after connect", function () {
        var hubProxy = _this.hub.connect();
        _this.connectionCallbacks['done']();
        expect(hubProxy.invoke).toHaveBeenCalledWith('register', 'connectionId', jasmine.any(String));
    });

    it("Calls Env.reload on reload message", function () {
        _this.hub.connect();
        _this.proxyCallback();
        expect(_this.app.reload).toHaveBeenCalled();
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
        { method: 'disconnected', status: 'Disconnected' }
    ].forEach(function (data) {
        it("Displays status on " + data.method, function () {
            _this.hub.connect();

            expect(_this.connectionCallbacks[data.method]).toEqual(jasmine.any(Function));
            _this.connectionCallbacks[data.method]();
            expect(_this.app.setServerStatus).toHaveBeenCalledWith(data.status);
        });
    });

    it("Reconnects after disconnect", function () {
        _this.hub.connect();

        expect(_this.connectionCallbacks['disconnected']).toEqual(jasmine.any(Function));
        _this.connectionCallbacks['disconnected']();
        waitsFor(function () {
            return _this.connection.start.callCount == 2;
        }, "Reconnect wasn't called", 10);
    });
});

describe("AllGreen SignalR HubReporter", function () {
    beforeEach(function () {
        _this.hubProxy = jasmine.createSpyObj('hubProxy', ['invoke']);
        _this.hubProxy.connection = { id: 'connectionId' };
        _this.hubReporter = new AllGreen.HubReporter(_this.hubProxy);
    });

    it("Should report started", function () {
        _this.hubReporter.started(10);
        expect(_this.hubProxy.invoke).toHaveBeenCalledWith('started', 'connectionId', 10);
    });

    ['reset', 'finished'].forEach(function (method) {
        it("Should report " + method, function () {
            _this.hubReporter[method]();
            expect(_this.hubProxy.invoke).toHaveBeenCalledWith(method, 'connectionId');
        });
    });

    it("Should report specUpdated", function () {
        var spec = { id: 123, name: 'test 1', suite: null, status: AllGreen.SpecStatus.Passed, steps: [] };
        _this.hubReporter.specUpdated(spec);
        expect(_this.hubProxy.invoke).toHaveBeenCalledWith('specUpdated', 'connectionId', spec);
    });
});
