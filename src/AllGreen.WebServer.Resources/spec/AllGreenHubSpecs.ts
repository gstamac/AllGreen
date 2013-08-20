/// <reference path="../Scripts/typings/jasmine/jasmine.d.ts" />
/// <reference path="../Scripts/typings/signalr/signalr.d.ts" />
/// <reference path="../Client/hub.ts" />

describe("AllGreen SignalR Hub", () => {
    beforeEach(() => {
        this.env = jasmine.createSpyObj('env', ['reload', 'setServerStatus']);

        this.hubProxy = jasmine.createSpyObj('hubProxy', ['on']);
        this.proxyCallback = null;
        this.hubProxy['on'].andCallFake((eventName: string, callback: (...msg) => void) => { this.proxyCallback = callback; });

        var callbacks = ['stateChanged', 'error', 'reconnecting', 'reconnected', 'disconnected'];
        this.connection = jasmine.createSpyObj('connection', callbacks.concat(['createHubProxy', 'start']));
        this.connection['createHubProxy'].andReturn(this.hubProxy);
        this.connection['start'].andReturn({ done: (callback) => { this.connectionCallbacks['done'] = callback; } });
        this.connectionCallbacks = [];
        callbacks.forEach((callbackName) => {
            this.connection[callbackName].andCallFake((callback) => { this.connectionCallbacks[callbackName] = callback; });
        });

        this.hub = new AllGreen.Hub(this.connection, this.env);
    });

    it("Starts as disconnected", () => {
        expect(this.env.setServerStatus).toHaveBeenCalledWith('Disconnected');
    });

    it("Connects to server", () => {
        this.hub.connect();

        expect(this.connection.createHubProxy).toHaveBeenCalledWith('runnerHub');
        expect(this.connection.start).toHaveBeenCalled();
        expect(this.hubProxy.on).toHaveBeenCalledWith('reload', jasmine.any(Function));
        expect(this.proxyCallback).toEqual(jasmine.any(Function));
        expect(this.connectionCallbacks['done']).toEqual(jasmine.any(Function));
        this.connectionCallbacks['done']();
        expect(this.env.setServerStatus).toHaveBeenCalledWith('Done');
    });

    it("Calls Env.reload on reload message", () => {
        this.hub.connect();
        this.proxyCallback();
        expect(this.env.reload).toHaveBeenCalled();
    });

    it("Displays status on error", () => {
        this.hub.connect();

        expect(this.connectionCallbacks['error']).toEqual(jasmine.any(Function));
        this.connectionCallbacks['error']('ERROR');
        expect(this.env.setServerStatus).toHaveBeenCalledWith('Error: ERROR');
    });

    it("Displays status on reconnecting", () => {
        this.hub.connect();

        expect(this.connectionCallbacks['reconnecting']).toEqual(jasmine.any(Function));
        this.connectionCallbacks['reconnecting']();
        expect(this.env.setServerStatus).toHaveBeenCalledWith('Reconnecting...');
    });

    it("Displays status on reconnected", () => {
        this.hub.connect();

        expect(this.connectionCallbacks['reconnected']).toEqual(jasmine.any(Function));
        this.connectionCallbacks['reconnected']();
        expect(this.env.setServerStatus).toHaveBeenCalledWith('Reconnected');
    });
});
