/// <reference path="../Scripts/typings/jasmine/jasmine.d.ts" />
/// <reference path="../Scripts/typings/jasmine-jquery/jasmine-jquery.d.ts" />
/// <reference path="../Client/allgreen.ts" />
var _this = this;
describe("App", function () {
    beforeEach(function () {
        jasmine.getFixtures().fixturesPath = '';
        var clientHtml = readFixtures('Client/client.html');
        expect(clientHtml).not.toBe('');
        clientHtml = clientHtml.replace(/~internal~\//g, '');
        clientHtml = clientHtml.replace(/<script[^>]*><\/script>/g, '');
        clientHtml = clientHtml.replace(/<script[^>]*>[^<]*new AllGreen\.App()[^<]*<\/script>/g, '');
        setFixtures(clientHtml);

        _this.app = new AllGreen.App();
    });

    afterEach(function () {
        _this.app = null;
    });

    it("Reports server status", function () {
        var serverReporter = jasmine.createSpyObj('serverReporter', ['setServerStatus']);
        _this.app.setServerReporter(serverReporter);

        _this.app.setServerStatus('server status');

        expect(serverReporter.setServerStatus).toHaveBeenCalledWith('server status');
    });

    it("Checks isReady for all reporters", function () {
        var runnerReporter1 = { isReady: function () {
                return true;
            } };
        _this.app.registerRunnerReporter(runnerReporter1);
        var runnerReporter2 = { isReady: function () {
                return false;
            } };
        _this.app.registerRunnerReporter(runnerReporter2);

        expect(_this.app.isReady()).toBeFalsy();

        runnerReporter2.isReady = function () {
            return true;
        };

        expect(_this.app.isReady()).toBeTruthy();
    });

    ['reset', 'started', 'finished', 'specUpdated'].forEach(function (method) {
        it("Forwards '" + method + "' to multiple reporters", function () {
            var runnerReporter1 = jasmine.createSpyObj('runnerReporter1', [method]);
            _this.app.registerRunnerReporter(runnerReporter1);
            var runnerReporter2 = jasmine.createSpyObj('runnerReporter2', [method]);
            _this.app.registerRunnerReporter(runnerReporter2);

            _this.app[method]();

            expect(runnerReporter1[method]).toHaveBeenCalled();
            expect(runnerReporter2[method]).toHaveBeenCalled();
        });
    });

    var registerAdapterFactory = function (name) {
        var adapterFactory = jasmine.createSpyObj('adapterFactory', ['create', 'getName']);
        adapterFactory['create'].andReturn({ start: function () {
            } });
        adapterFactory['getName'].andReturn(name);
        this.app.registerAdapterFactory(adapterFactory);
        return adapterFactory;
    };

    var registerAdapter = function (name) {
        var adapter = jasmine.createSpyObj('adapter', ['start']);
        var adapterFactory = jasmine.createSpyObj('adapterFactory', ['create', 'getName']);
        adapterFactory['create'].andReturn(adapter);
        adapterFactory['getName'].andReturn(name);
        this.app.registerAdapterFactory(adapterFactory);
        return adapter;
    };

    it("Creates adapters on run tests", function () {
        var adapterFactory1 = registerAdapterFactory('adapter1');
        var adapterFactory2 = registerAdapterFactory('adapter2');

        _this.app.runTests();
        expect(adapterFactory1.create).toHaveBeenCalledWith(_this.app);
        expect(adapterFactory2.create).toHaveBeenCalledWith(_this.app);
    });

    it("Should run tests after all reporters are ready", function () {
        var adapterFactory = registerAdapterFactory('adapter');
        var runnerReporter = { isReady: function () {
                return false;
            } };
        runs(function () {
            _this.app.registerRunnerReporter(runnerReporter);

            _this.app.runTests();
            expect(adapterFactory.create.callCount).toBe(0);
            runnerReporter.isReady = function () {
                return true;
            };
        });
        waits(20);
        runs(function () {
            expect(adapterFactory.create).toHaveBeenCalledWith(_this.app);
        });
    });

    it("Adapters start is called", function () {
        var adapter1 = registerAdapter('adapter1');
        var adapter2 = registerAdapter('adapter2');

        _this.app.runTests();
        expect(adapter1.start).toHaveBeenCalled();
        expect(adapter2.start).toHaveBeenCalled();
    });

    it("Can be reloaded", function () {
        var runnerReporter = jasmine.createSpyObj('runnerReporter', ['reset', 'isReady']);
        runnerReporter['isReady'].andCallFake(function () {
            return true;
        });
        _this.app.registerRunnerReporter(runnerReporter);

        var adapterFactory1 = registerAdapterFactory('adapter1');

        _this.app.reload();
        expect(runnerReporter.reset).toHaveBeenCalled();
        expect($('#runner-iframe')).toHaveAttr('src', '/~internal~/Client/runner.html');
        _this.app.runTests();
        expect(adapterFactory1.create).not.toHaveBeenCalled();
    });

    it("Registeres only first adapter factory with same name", function () {
        var adapterFactory1 = registerAdapterFactory('adapter1');
        var adapterFactory2 = registerAdapterFactory('adapter2');
        var adapterFactory3 = registerAdapterFactory('adapter1');

        _this.app.registerAdapterFactory(adapterFactory1);
        _this.app.registerAdapterFactory(adapterFactory2);
        _this.app.registerAdapterFactory(adapterFactory3);
        _this.app.runTests();
        expect(adapterFactory1.create).toHaveBeenCalledWith(_this.app);
        expect(adapterFactory2.create).toHaveBeenCalledWith(_this.app);
        expect(adapterFactory3.create).not.toHaveBeenCalled();
    });

    it("Enables reconnect by default", function () {
        expect(_this.app.reconnectEnabled).toBeTruthy();
    });

    it("Can enable/disable reconnect", function () {
        _this.app.enableReconnect(true);
        expect(_this.app.reconnectEnabled).toBeTruthy();
        _this.app.enableReconnect(false);
        expect(_this.app.reconnectEnabled).toBeFalsy();
        _this.app.enableReconnect(true);
        expect(_this.app.reconnectEnabled).toBeTruthy();
    });
});
//# sourceMappingURL=AllGreenAppSpecs.js.map
