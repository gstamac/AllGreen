/// <reference path="../Scripts/typings/jasmine/jasmine.d.ts" />
/// <reference path="../Scripts/typings/jasmine-jquery/jasmine-jquery.d.ts" />
/// <reference path="../Client/allgreen.ts" />

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

    it("Checks isReady for all reporters", () => {
        var runnerReporter1 = { isReady: () => true };
        this.app.registerRunnerReporter(runnerReporter1);
        var runnerReporter2 = { isReady: () => false };
        this.app.registerRunnerReporter(runnerReporter2);

        expect(this.app.isReady()).toBeFalsy();

        runnerReporter2.isReady = () => true;

        expect(this.app.isReady()).toBeTruthy();
    });

    ['reset', 'started', 'finished', 'specUpdated'].forEach((method) => {
        it("Forwards '" + method + "' to multiple reporters", () => {
            var runnerReporter1 = jasmine.createSpyObj('runnerReporter1', [method]);
            this.app.registerRunnerReporter(runnerReporter1);
            var runnerReporter2 = jasmine.createSpyObj('runnerReporter2', [method]);
            this.app.registerRunnerReporter(runnerReporter2);

            this.app[method]();

            expect(runnerReporter1[method]).toHaveBeenCalled();
            expect(runnerReporter2[method]).toHaveBeenCalled();
        })
    });

    var registerAdapterFactory = function (name) {
        var adapterFactory = jasmine.createSpyObj('adapterFactory', ['create', 'getName']);
        adapterFactory['create'].andReturn({ start: function () { } });
        adapterFactory['getName'].andReturn(name);
        this.app.registerAdapterFactory(adapterFactory);
        return adapterFactory;
    }

    var registerAdapter = function (name: string) {
        var adapter = jasmine.createSpyObj('adapter', ['start']);
        var adapterFactory = jasmine.createSpyObj('adapterFactory', ['create', 'getName']);
        adapterFactory['create'].andReturn(adapter);
        adapterFactory['getName'].andReturn(name);
        this.app.registerAdapterFactory(adapterFactory);
        return adapter;
    }

    it("Creates adapters on run tests", () => {
        var adapterFactory1 = registerAdapterFactory('adapter1');
        var adapterFactory2 = registerAdapterFactory('adapter2');

        this.app.runTests();
        expect(adapterFactory1.create).toHaveBeenCalledWith(this.app);
        expect(adapterFactory2.create).toHaveBeenCalledWith(this.app);
    });

    it("Should run tests after all reporters are ready", () => {
        var adapterFactory = registerAdapterFactory('adapter');
        var runnerReporter = { isReady: () => false };
        runs(() => {
            this.app.registerRunnerReporter(runnerReporter);

            this.app.runTests();
            expect(adapterFactory.create.callCount).toBe(0);
            runnerReporter.isReady = () => true;
        });
        waits(20);
        runs(() => {
            expect(adapterFactory.create).toHaveBeenCalledWith(this.app);
        });
    });

    it("Adapters start is called", () => {
        var adapter1 = registerAdapter('adapter1');
        var adapter2 = registerAdapter('adapter2');

        this.app.runTests();
        expect(adapter1.start).toHaveBeenCalled();
        expect(adapter2.start).toHaveBeenCalled();
    });

    it("Can be reloaded", () => {
        var runnerReporter = jasmine.createSpyObj('runnerReporter', ['reset', 'isReady']);
        runnerReporter['isReady'].andCallFake(() => true);
        this.app.registerRunnerReporter(runnerReporter);

        var adapterFactory1 = registerAdapterFactory('adapter1');

        this.app.reload();
        expect(runnerReporter.reset).toHaveBeenCalled();
        expect($('#runner-iframe')).toHaveAttr('src', '/~internal~/Client/runner.html');
        this.app.runTests();
        expect(adapterFactory1.create).not.toHaveBeenCalled();
    });

    it("Registeres only first adapter factory with same name", () => {
        var adapterFactory1 = registerAdapterFactory('adapter1');
        var adapterFactory2 = registerAdapterFactory('adapter2');
        var adapterFactory3 = registerAdapterFactory('adapter1');

        this.app.registerAdapterFactory(adapterFactory1);
        this.app.registerAdapterFactory(adapterFactory2);
        this.app.registerAdapterFactory(adapterFactory3);
        this.app.runTests();
        expect(adapterFactory1.create).toHaveBeenCalledWith(this.app);
        expect(adapterFactory2.create).toHaveBeenCalledWith(this.app);
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