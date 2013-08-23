/// <reference path="../Scripts/typings/jasmine/jasmine.d.ts" />
/// <reference path="../Scripts/typings/jasmine-jquery/jasmine-jquery.d.ts" />
/// <reference path="../Client/allgreen.ts" />

describe("App", () => {
    beforeEach(() => {
        jasmine.getFixtures().fixturesPath = '';
        var clientHtml = readFixtures('Client/client.html');
        clientHtml = clientHtml.replace(/<script[^>]*><\/script>/g, '');
        clientHtml = clientHtml.replace(/<script[^>]*>[^<]*AllGreen\.startApp()[^<]*<\/script>/g, '');
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

    it("Creates adapters on start", () => {
        var adapterFactory1 = jasmine.createSpyObj('adapterFactory1', ['create']);
        adapterFactory1['create'].andReturn({ start: function () { } });
        var adapterFactory2 = jasmine.createSpyObj('adapterFactory2', ['create']);
        adapterFactory2['create'].andReturn({ start: function () { } });

        this.app.registerAdapterFactory(adapterFactory1);
        this.app.registerAdapterFactory(adapterFactory2);
        this.app.start();
        expect(adapterFactory1.create).toHaveBeenCalledWith(this.app);
        expect(adapterFactory2.create).toHaveBeenCalledWith(this.app);
    });

    it("Adapters start is called", () => {
        var adapter1 = jasmine.createSpyObj('adapter1', ['start']);
        var adapterFactory1 = jasmine.createSpyObj('adapterFactory1', ['create']);
        adapterFactory1['create'].andReturn(adapter1);
        var adapter2 = jasmine.createSpyObj('adapter2', ['start']);
        var adapterFactory2 = jasmine.createSpyObj('adapterFactory2', ['create']);
        adapterFactory2['create'].andReturn(adapter2);

        this.app.registerAdapterFactory(adapterFactory1);
        this.app.registerAdapterFactory(adapterFactory2);
        this.app.start();
        expect(adapter1.start).toHaveBeenCalled();
        expect(adapter2.start).toHaveBeenCalled();
    });

    it("Can be reloaded", () => {
        var runnerReporter = jasmine.createSpyObj('runnerReporter', ['reset']);
        this.app.registerRunnerReporter(runnerReporter);

        var adapterFactory1 = jasmine.createSpyObj('adapterFactory1', ['create']);
        adapterFactory1['create'].andReturn({ start: function () { } });
        this.app.registerAdapterFactory(adapterFactory1);

        this.app.reload();
        expect(runnerReporter.reset).toHaveBeenCalled();
        expect($('#runner')).toHaveAttr('src', 'runner.html');
        this.app.start();
        expect(adapterFactory1.create).not.toHaveBeenCalled();
    });
})