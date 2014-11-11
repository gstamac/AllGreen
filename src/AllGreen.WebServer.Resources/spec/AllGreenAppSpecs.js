/// <reference path="../Scripts/typings/jasmine/jasmine.d.ts" />
/// <reference path="../Scripts/typings/jasmine-jquery/jasmine-jquery.d.ts" />
/// <reference path="../Client/allgreen.ts" />
var _this = this;
describe("CompositeRunnerReporter", function () {
    beforeEach(function () {
        _this.composite = new AllGreen.CompositeRunnerReporter();
    });

    afterEach(function () {
        _this.composite = null;
    });

    it("Checks isReady for all reporters", function () {
        var runnerReporter1 = { isReady: function () {
                return true;
            } };
        _this.composite.registerRunnerReporter(runnerReporter1);
        var runnerReporter2 = { isReady: function () {
                return false;
            } };
        _this.composite.registerRunnerReporter(runnerReporter2);

        expect(_this.composite.isReady()).toBeFalsy();

        runnerReporter2.isReady = function () {
            return true;
        };

        expect(_this.composite.isReady()).toBeTruthy();
    });

    ['reset', 'started', 'finished', 'specUpdated'].forEach(function (method) {
        it("Forwards '" + method + "' to multiple reporters", function () {
            var runnerReporter1 = jasmine.createSpyObj('runnerReporter1', [method]);
            _this.composite.registerRunnerReporter(runnerReporter1);
            var runnerReporter2 = jasmine.createSpyObj('runnerReporter2', [method]);
            _this.composite.registerRunnerReporter(runnerReporter2);

            _this.composite[method]();

            expect(runnerReporter1[method]).toHaveBeenCalled();
            expect(runnerReporter2[method]).toHaveBeenCalled();
        });
    });
});

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

    var registerAdapterFactory = function (name) {
        var adapterFactory = jasmine.createSpyObj('adapterFactory', ['create', 'getName']);
        adapterFactory['create'].andReturn({ runTests: function () {
            } });
        adapterFactory['getName'].andReturn(name);
        this.app.registerAdapterFactory(adapterFactory);
        return adapterFactory;
    };

    var registerAdapter = function (name) {
        var adapter = jasmine.createSpyObj('adapter', ['runTests']);
        var adapterFactory = jasmine.createSpyObj('adapterFactory', ['create', 'getName']);
        adapterFactory['create'].andReturn(adapter);
        adapterFactory['getName'].andReturn(name);
        this.app.registerAdapterFactory(adapterFactory);
        return adapter;
    };

    it("Creates adapters after runner is loaded", function () {
        var adapterFactory1 = registerAdapterFactory('adapter1');
        var adapterFactory2 = registerAdapterFactory('adapter2');

        _this.app.runnerLoaded();
        expect(adapterFactory1.create).toHaveBeenCalledWith(jasmine.any(Object));
        expect(adapterFactory2.create).toHaveBeenCalledWith(jasmine.any(Object));
    });

    it("Should run tests after all reporters are ready", function () {
        var adapterFactory = registerAdapterFactory('adapter');
        var runnerReporter = { isReady: function () {
                return false;
            } };
        runs(function () {
            _this.app.registerRunnerReporter(runnerReporter);

            _this.app.runnerLoaded();
            expect(adapterFactory.create.callCount).toBe(0);
            runnerReporter.isReady = function () {
                return true;
            };
        });
        waits(20);
        runs(function () {
            expect(adapterFactory.create).toHaveBeenCalledWith(jasmine.any(Object));
        });
    });

    it("Adapters run tests is called after runner is loaded", function () {
        var adapter1 = registerAdapter('adapter1');
        var adapter2 = registerAdapter('adapter2');

        _this.app.runnerLoaded();
        expect(adapter1.runTests).toHaveBeenCalled();
        expect(adapter2.runTests).toHaveBeenCalled();
    });

    it("Can be started", function () {
        var runnerReporter = jasmine.createSpyObj('runnerReporter', ['reset', 'isReady']);
        runnerReporter['isReady'].andCallFake(function () {
            return true;
        });
        _this.app.registerRunnerReporter(runnerReporter);

        var adapterFactory1 = registerAdapterFactory('adapter1');

        _this.app.runTests();
        expect(runnerReporter.reset).toHaveBeenCalled();
        expect($('#runner-iframe')).toHaveAttr('src', '/~internal~/Client/runner.html');
        _this.app.runnerLoaded();
        expect(adapterFactory1.create).not.toHaveBeenCalled();
    });

    it("Registeres only first adapter factory with same name", function () {
        var adapterFactory1 = registerAdapterFactory('adapter1');
        var adapterFactory2 = registerAdapterFactory('adapter2');
        var adapterFactory3 = registerAdapterFactory('adapter1');

        _this.app.registerAdapterFactory(adapterFactory1);
        _this.app.registerAdapterFactory(adapterFactory2);
        _this.app.registerAdapterFactory(adapterFactory3);
        _this.app.runnerLoaded();
        expect(adapterFactory1.create).toHaveBeenCalledWith(jasmine.any(Object));
        expect(adapterFactory2.create).toHaveBeenCalledWith(jasmine.any(Object));
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
