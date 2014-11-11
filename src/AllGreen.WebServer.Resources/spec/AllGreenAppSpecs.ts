/// <reference path="../Scripts/typings/jasmine/jasmine.d.ts" />
/// <reference path="../Scripts/typings/jasmine-jquery/jasmine-jquery.d.ts" />
/// <reference path="../Client/allgreen.ts" />

describe("CompositeRunnerReporter", () => {
    beforeEach(() => {
        this.composite = new AllGreen.CompositeRunnerReporter();
    });

    afterEach(() => {
        this.composite = null;
    });

    it("Checks isReady for all reporters", () => {
        var runnerReporter1 = { isReady: () => true };
        this.composite.registerRunnerReporter(runnerReporter1);
        var runnerReporter2 = { isReady: () => false };
        this.composite.registerRunnerReporter(runnerReporter2);

        expect(this.composite.isReady()).toBeFalsy();

        runnerReporter2.isReady = () => true;

        expect(this.composite.isReady()).toBeTruthy();
    });

    ['reset', 'started', 'finished', 'specUpdated'].forEach((method) => {
        it("Forwards '" + method + "' to multiple reporters", () => {
            var runnerReporter1 = jasmine.createSpyObj('runnerReporter1', [method]);
            this.composite.registerRunnerReporter(runnerReporter1);
            var runnerReporter2 = jasmine.createSpyObj('runnerReporter2', [method]);
            this.composite.registerRunnerReporter(runnerReporter2);

            this.composite[method]();

            expect(runnerReporter1[method]).toHaveBeenCalled();
            expect(runnerReporter2[method]).toHaveBeenCalled();
        })
    });
})

describe("App", () => {
    beforeEach(() => {
        jasmine.getFixtures().fixturesPath = '';
        var clientHtml = readFixtures('Client/client.html');
        expect(clientHtml).not.toBe('');
        clientHtml = clientHtml.replace(/~internal~\//g, '');
        clientHtml = clientHtml.replace(/<script[^>]*><\/script>/g, '');
        clientHtml = clientHtml.replace(/<script[^>]*>[^<]*new AllGreen\.App()[^<]*<\/script>/g, '');
        setFixtures(clientHtml);

        this.app = new AllGreen.App();
    });

    afterEach(() => {
        this.app = null;
    });

    it("Reports server status", () => {
        var serverReporter = jasmine.createSpyObj('serverReporter', ['setServerStatus']);
        this.app.setServerReporter(serverReporter);

        this.app.setServerStatus('server status');

        expect(serverReporter.setServerStatus).toHaveBeenCalledWith('server status');
    });

    var registerAdapterFactory = function (name) {
        var adapterFactory = jasmine.createSpyObj('adapterFactory', ['create', 'getName']);
        adapterFactory['create'].andReturn({ runTests: function () { } });
        adapterFactory['getName'].andReturn(name);
        this.app.registerAdapterFactory(adapterFactory);
        return adapterFactory;
    }

    var registerAdapter = function (name: string) {
        var adapter = jasmine.createSpyObj('adapter', ['runTests']);
        var adapterFactory = jasmine.createSpyObj('adapterFactory', ['create', 'getName']);
        adapterFactory['create'].andReturn(adapter);
        adapterFactory['getName'].andReturn(name);
        this.app.registerAdapterFactory(adapterFactory);
        return adapter;
    }

    it("Creates adapters after runner is loaded", () => {
        var adapterFactory1 = registerAdapterFactory('adapter1');
        var adapterFactory2 = registerAdapterFactory('adapter2');

        this.app.runnerLoaded();
        expect(adapterFactory1.create).toHaveBeenCalledWith(jasmine.any(Object));
        expect(adapterFactory2.create).toHaveBeenCalledWith(jasmine.any(Object));
    });

    it("Should run tests after all reporters are ready", () => {
        var adapterFactory = registerAdapterFactory('adapter');
        var runnerReporter = { isReady: () => false };
        runs(() => {
            this.app.registerRunnerReporter(runnerReporter);

            this.app.runnerLoaded();
            expect(adapterFactory.create.callCount).toBe(0);
            runnerReporter.isReady = () => true;
        });
        waits(20);
        runs(() => {
            expect(adapterFactory.create).toHaveBeenCalledWith(jasmine.any(Object));
        });
    });

    it("Adapters run tests is called after runner is loaded", () => {
        var adapter1 = registerAdapter('adapter1');
        var adapter2 = registerAdapter('adapter2');

        this.app.runnerLoaded();
        expect(adapter1.runTests).toHaveBeenCalled();
        expect(adapter2.runTests).toHaveBeenCalled();
    });

    it("Can be started", () => {
        var runnerReporter = jasmine.createSpyObj('runnerReporter', ['reset', 'isReady']);
        runnerReporter['isReady'].andCallFake(() => true);
        this.app.registerRunnerReporter(runnerReporter);

        var adapterFactory1 = registerAdapterFactory('adapter1');

        this.app.runTests();
        expect(runnerReporter.reset).toHaveBeenCalled();
        expect($('#runner-iframe')).toHaveAttr('src', '/~internal~/Client/runner.html');
        this.app.runnerLoaded();
        expect(adapterFactory1.create).not.toHaveBeenCalled();
    });

    it("Registeres only first adapter factory with same name", () => {
        var adapterFactory1 = registerAdapterFactory('adapter1');
        var adapterFactory2 = registerAdapterFactory('adapter2');
        var adapterFactory3 = registerAdapterFactory('adapter1');

        this.app.registerAdapterFactory(adapterFactory1);
        this.app.registerAdapterFactory(adapterFactory2);
        this.app.registerAdapterFactory(adapterFactory3);
        this.app.runnerLoaded();
        expect(adapterFactory1.create).toHaveBeenCalledWith(jasmine.any(Object));
        expect(adapterFactory2.create).toHaveBeenCalledWith(jasmine.any(Object));
        expect(adapterFactory3.create).not.toHaveBeenCalled();
    });

    it("Enables reconnect by default", () => {
        expect(this.app.reconnectEnabled).toBeTruthy();
    });

    it("Can enable/disable reconnect", () => {
        this.app.enableReconnect(true);
        expect(this.app.reconnectEnabled).toBeTruthy();
        this.app.enableReconnect(false);
        expect(this.app.reconnectEnabled).toBeFalsy();
        this.app.enableReconnect(true);
        expect(this.app.reconnectEnabled).toBeTruthy();
    });
})