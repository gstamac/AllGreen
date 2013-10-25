/// <reference path="../Scripts/typings/jasmine/jasmine.d.ts" />
/// <reference path="../Scripts/typings/signalr/signalr.d.ts" />
/// <reference path="../Client/hub.ts" />

describe("AllGreen SignalR Hub", () => {
    beforeEach(() => {
        this.app = jasmine.createSpyObj('app', ['reload', 'setServerStatus', 'specUpdated', 'log']);
        //this.app['log'].andCallFake(console.log);

        this.hubProxy = jasmine.createSpyObj('hubProxy', ['on', 'invoke']);
        this.proxyCallback = null;
        this.hubProxy['on'].andCallFake((eventName: string, callback: (...msg) => void) => { this.proxyCallback = callback; });

        var callbacks = ['stateChanged', 'error', 'reconnecting', 'reconnected', 'disconnected'];
        this.connection = jasmine.createSpyObj('connection', callbacks.concat(['createHubProxy', 'start', 'received']));
        this.connection['createHubProxy'].andReturn(this.hubProxy);
        this.connection['start'].andReturn({ done: (callback) => { this.connectionCallbacks['done'] = callback; } });
        this.connectionCallbacks = [];
        callbacks.forEach((callbackName) => {
            this.connection[callbackName].andCallFake((callback) => { this.connectionCallbacks[callbackName] = callback; });
        });

        this.hubReporter = jasmine.createSpyObj('hubReporter', ['hubProxyConnected']);
        this.hub = new AllGreen.Hub(this.connection, this.app, this.hubReporter, 1);
    });

    it("Starts as disconnected", () => {
        expect(this.app.setServerStatus).toHaveBeenCalledWith('Disconnected');
    });

    it("Connects to server", () => {
        this.hub.connect();

        expect(this.connection.createHubProxy).toHaveBeenCalledWith('runnerHub');
        expect(this.connection.start).toHaveBeenCalled();
        expect(this.hubProxy.on).toHaveBeenCalledWith('reload', jasmine.any(Function));
        expect(this.proxyCallback).toEqual(jasmine.any(Function));
        expect(this.connectionCallbacks['done']).toEqual(jasmine.any(Function));
        this.connectionCallbacks['done']();
        expect(this.app.setServerStatus).toHaveBeenCalledWith('Connected');
    });

    it("Should initialize after connect", () => {
        this.hub.connect();
        this.connectionCallbacks['done']();
        expect(this.hubReporter.hubProxyConnected).toHaveBeenCalledWith(this.hubProxy);
        expect(this.hubProxy.invoke).toHaveBeenCalledWith('register');
    });

    it("Should initialize after reconnect", () => {
        this.hub.connect();
        this.connectionCallbacks['reconnected']();
        expect(this.hubReporter.hubProxyConnected).toHaveBeenCalledWith(this.hubProxy);
        expect(this.hubProxy.invoke).toHaveBeenCalledWith('register');
    });

    it("Calls Env.reload on reload message", () => {
        this.hub.connect();
        this.proxyCallback();
        expect(this.app.reload).toHaveBeenCalled();
    });

    it("Displays status on error", () => {
        this.hub.connect();

        expect(this.connectionCallbacks['error']).toEqual(jasmine.any(Function));
        this.connectionCallbacks['error']('ERROR');
        expect(this.app.setServerStatus).toHaveBeenCalledWith('Error: ERROR');
    });

    [{ method: 'reconnecting', status: 'Reconnecting...' },
        { method: 'reconnected', status: 'Reconnected' },
        { method: 'disconnected', status: 'Disconnected' }]
        .forEach((data) => {
            it("Displays status on " + data.method, () => {
                this.hub.connect();

                expect(this.connectionCallbacks[data.method]).toEqual(jasmine.any(Function));
                this.connectionCallbacks[data.method]();
                expect(this.app.setServerStatus).toHaveBeenCalledWith(data.status);
            });
        });

    it("Reconnects after disconnect if enabled", () => {
        this.hub.connect();

        this.app.reconnectEnabled = true;
        expect(this.connectionCallbacks['disconnected']).toEqual(jasmine.any(Function));
        this.connectionCallbacks['disconnected']();
        waitsFor(() => this.connection.start.callCount == 2, "Reconnect wasn't called", 10);
    });

    it("Doesn't reconnect on disconnect if disabled", () => {
        runs(() => {
            this.hub.connect();

            this.app.reconnectEnabled = false;
            expect(this.connectionCallbacks['disconnected']).toEqual(jasmine.any(Function));
            this.connectionCallbacks['disconnected']();
        });
        waits(10);
        runs(() => {
            expect(this.connection.start.callCount).toBe(1);
        });
    });
});

describe("AllGreen SignalR HubReporter", () => {
    beforeEach(() => {
        this.hubProxy = jasmine.createSpyObj('hubProxy', ['invoke']);
        this.hubReporter = new AllGreen.HubReporter();
    });

    it("Should be ready after registered not before", () => {
        expect(this.hubReporter.isReady()).toBeFalsy();
        this.hubReporter.hubProxyConnected(this.hubProxy);
        expect(this.hubReporter.isReady()).toBeTruthy();
    });

    it("Should report started", () => {
        this.hubReporter.hubProxyConnected(this.hubProxy);
        this.hubReporter.started(10);
        expect(this.hubProxy.invoke).toHaveBeenCalledWith('started', 10);
    });

    ['reset', 'finished'].forEach((method) => {
        it("Should report " + method, () => {
            this.hubReporter.hubProxyConnected(this.hubProxy);
            this.hubReporter[method]();
            expect(this.hubProxy.invoke).toHaveBeenCalledWith(method);
        })
    });

    it("Should report specUpdated", () => {
        var spec = { id: 123, name: 'test 1', suite: null, status: AllGreen.SpecStatus.Passed, steps: [] };
        this.hubReporter.hubProxyConnected(this.hubProxy);
        this.hubReporter.specUpdated(spec);
        expect(this.hubProxy.invoke).toHaveBeenCalledWith('specUpdated', spec);
    })
});
