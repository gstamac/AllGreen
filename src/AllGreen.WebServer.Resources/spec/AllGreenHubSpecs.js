/// <reference path="../Scripts/typings/jasmine/jasmine.d.ts" />
/// <reference path="../Scripts/typings/signalr/signalr.d.ts" />
/// <reference path="../Client/hub.ts" />
var _this = this;
describe("AllGreen SignalR Hub", function () {
    beforeEach(function () {
        _this.env = jasmine.createSpyObj('env', ['reload', 'setServerStatus']);

        _this.hubProxy = jasmine.createSpyObj('hubProxy', ['on']);
        _this.proxyCallback = null;
        _this.hubProxy['on'].andCallFake(function (eventName, callback) {
            _this.proxyCallback = callback;
        });

        var callbacks = ['stateChanged', 'error', 'reconnecting', 'reconnected', 'disconnected'];
        _this.connection = jasmine.createSpyObj('connection', callbacks.concat(['createHubProxy', 'start']));
        _this.connection['createHubProxy'].andReturn(_this.hubProxy);
        _this.connection['start'].andReturn({ done: function (callback) {
                _this.connectionCallbacks['done'] = callback;
            } });
        _this.connectionCallbacks = [];
        callbacks.forEach(function (callbackName) {
            _this.connection[callbackName].andCallFake(function (callback) {
                _this.connectionCallbacks[callbackName] = callback;
            });
        });

        _this.hub = new AllGreen.Hub(_this.connection, _this.env);
    });

    it("Starts as disconnected", function () {
        expect(_this.env.setServerStatus).toHaveBeenCalledWith('Disconnected');
    });

    it("Connects to server", function () {
        _this.hub.connect();

        expect(_this.connection.createHubProxy).toHaveBeenCalledWith('runnerHub');
        expect(_this.connection.start).toHaveBeenCalled();
        expect(_this.hubProxy.on).toHaveBeenCalledWith('reload', jasmine.any(Function));
        expect(_this.proxyCallback).toEqual(jasmine.any(Function));
        expect(_this.connectionCallbacks['done']).toEqual(jasmine.any(Function));
        _this.connectionCallbacks['done']();
        expect(_this.env.setServerStatus).toHaveBeenCalledWith('Done');
    });

    it("Calls Env.reload on reload message", function () {
        _this.hub.connect();
        _this.proxyCallback();
        expect(_this.env.reload).toHaveBeenCalled();
    });

    it("Displays status on error", function () {
        _this.hub.connect();

        expect(_this.connectionCallbacks['error']).toEqual(jasmine.any(Function));
        _this.connectionCallbacks['error']('ERROR');
        expect(_this.env.setServerStatus).toHaveBeenCalledWith('Error: ERROR');
    });

    it("Displays status on reconnecting", function () {
        _this.hub.connect();

        expect(_this.connectionCallbacks['reconnecting']).toEqual(jasmine.any(Function));
        _this.connectionCallbacks['reconnecting']();
        expect(_this.env.setServerStatus).toHaveBeenCalledWith('Reconnecting...');
    });

    it("Displays status on reconnected", function () {
        _this.hub.connect();

        expect(_this.connectionCallbacks['reconnected']).toEqual(jasmine.any(Function));
        _this.connectionCallbacks['reconnected']();
        expect(_this.env.setServerStatus).toHaveBeenCalledWith('Reconnected');
    });
});
