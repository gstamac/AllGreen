/// <reference path="../Scripts/typings/jasmine/jasmine.d.ts" />
/// <reference path="../Scripts/typings/jasmine-jquery/jasmine-jquery.d.ts" />
/// <reference path="../Client/allgreen.ts" />
var _this = this;
describe("App", function () {
    beforeEach(function () {
        jasmine.getFixtures().fixturesPath = '';
        var clientHtml = readFixtures('Client/client.html');
        clientHtml = clientHtml.replace(/<script[^>]*><\/script>/g, '');
        clientHtml = clientHtml.replace(/<script[^>]*>[^<]*AllGreen\.startApp()[^<]*<\/script>/g, '');
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

    it("Creates adapters on start", function () {
        var adapterFactory1 = jasmine.createSpyObj('adapterFactory1', ['create']);
        adapterFactory1['create'].andReturn({ start: function () {
            } });
        var adapterFactory2 = jasmine.createSpyObj('adapterFactory2', ['create']);
        adapterFactory2['create'].andReturn({ start: function () {
            } });

        _this.app.registerAdapterFactory(adapterFactory1);
        _this.app.registerAdapterFactory(adapterFactory2);
        _this.app.start();
        expect(adapterFactory1.create).toHaveBeenCalledWith(_this.app);
        expect(adapterFactory2.create).toHaveBeenCalledWith(_this.app);
    });

    it("Should start after all reporters are ready", function () {
        var adapterFactory = jasmine.createSpyObj('adapterFactory', ['create']);
        var runnerReporter = { isReady: function () {
                return false;
            } };
        runs(function () {
            adapterFactory['create'].andReturn({ start: function () {
                } });
            _this.app.registerAdapterFactory(adapterFactory);
            _this.app.registerRunnerReporter(runnerReporter);

            _this.app.start();
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
        var adapter1 = jasmine.createSpyObj('adapter1', ['start']);
        var adapterFactory1 = jasmine.createSpyObj('adapterFactory1', ['create']);
        adapterFactory1['create'].andReturn(adapter1);
        var adapter2 = jasmine.createSpyObj('adapter2', ['start']);
        var adapterFactory2 = jasmine.createSpyObj('adapterFactory2', ['create']);
        adapterFactory2['create'].andReturn(adapter2);

        _this.app.registerAdapterFactory(adapterFactory1);
        _this.app.registerAdapterFactory(adapterFactory2);
        _this.app.start();
        expect(adapter1.start).toHaveBeenCalled();
        expect(adapter2.start).toHaveBeenCalled();
    });

    it("Can be reloaded", function () {
        var runnerReporter = jasmine.createSpyObj('runnerReporter', ['reset', 'isReady']);
        runnerReporter['isReady'].andCallFake(function () {
            return true;
        });
        _this.app.registerRunnerReporter(runnerReporter);

        var adapterFactory1 = jasmine.createSpyObj('adapterFactory1', ['create']);
        adapterFactory1['create'].andReturn({ start: function () {
            } });
        _this.app.registerAdapterFactory(adapterFactory1);

        _this.app.reload();
        expect(runnerReporter.reset).toHaveBeenCalled();
        expect($('#runner')).toHaveAttr('src', 'runner.html');
        _this.app.start();
        expect(adapterFactory1.create).not.toHaveBeenCalled();
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
