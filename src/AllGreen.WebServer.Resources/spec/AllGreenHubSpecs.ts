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
        this.connection.id = 'connectionId';
        this.connectionCallbacks = [];
        callbacks.forEach((callbackName) => {
            this.connection[callbackName].andCallFake((callback) => { this.connectionCallbacks[callbackName] = callback; });
        });

        this.hub = new AllGreen.Hub(this.connection, this.app);
    });

    it("Starts as disconnected", () => {
        expect(this.app.setServerStatus).toHaveBeenCalledWith('Disconnected');
    });

    it("Connects to server", () => {
        var hubProxy = this.hub.connect();

        expect(hubProxy).toBeDefined();
        expect(this.connection.createHubProxy).toHaveBeenCalledWith('runnerHub');
        expect(this.connection.start).toHaveBeenCalled();
        expect(this.hubProxy.on).toHaveBeenCalledWith('reload', jasmine.any(Function));
        expect(this.proxyCallback).toEqual(jasmine.any(Function));
        expect(this.connectionCallbacks['done']).toEqual(jasmine.any(Function));
        this.connectionCallbacks['done']();
        expect(this.app.setServerStatus).toHaveBeenCalledWith('Connected');
    });

    it("Should initialize after connect", () => {
        var hubProxy = this.hub.connect();
        this.connectionCallbacks['done']();
        expect(hubProxy.invoke).toHaveBeenCalledWith('register', 'connectionId', jasmine.any(String));
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
});

describe("AllGreen SignalR HubReporter", () => {
    beforeEach(() => {
        this.hubProxy = jasmine.createSpyObj('hubProxy', ['invoke']);
        this.hubProxy.connection = { id: 'connectionId' };
        this.hubReporter = new AllGreen.HubReporter(this.hubProxy);
    });

    it("Should report started", () => {
        this.hubReporter.started(10);
        expect(this.hubProxy.invoke).toHaveBeenCalledWith('started', 'connectionId', 10);
    });

    ['reset', 'finished'].forEach((method) => {
        it("Should report " + method, () => {
            this.hubReporter[method]();
            expect(this.hubProxy.invoke).toHaveBeenCalledWith(method, 'connectionId');
        })
    });

    it("Should report specUpdated", () => {
        var spec = { id: 123, name: 'test 1', suite: null, status: AllGreen.SpecStatus.Passed, steps: [] };
        this.hubReporter.specUpdated(spec);
        expect(this.hubProxy.invoke).toHaveBeenCalledWith('specUpdated', 'connectionId', spec);
    })
});
