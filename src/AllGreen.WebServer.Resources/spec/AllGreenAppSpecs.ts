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

    it("Can use custom reporter", () => {
        var reporter = {};
        this.app.setReporter(reporter);

        var adapterFactory1 = jasmine.createSpyObj('adapterFactory1', ['create']);
        adapterFactory1['create'].andReturn({ start: function () { } });
        this.app.registerAdapterFactory(adapterFactory1);
        this.app.start();

        expect(adapterFactory1.create).toHaveBeenCalledWith(reporter);
    });

    it("Sets server status", () => {
        var reporter = jasmine.createSpyObj('reporter', ['setServerStatus']);
        this.app.setReporter(reporter);

        this.app.setServerStatus('server status');

        expect(reporter.setServerStatus).toHaveBeenCalledWith('server status');
    });

    it("Creates adapters on start", () => {
        var adapterFactory1 = jasmine.createSpyObj('adapterFactory1', ['create']);
        adapterFactory1['create'].andReturn({ start: function () { } });
        var adapterFactory2 = jasmine.createSpyObj('adapterFactory2', ['create']);
        adapterFactory2['create'].andReturn({ start: function () { } });

        this.app.registerAdapterFactory(adapterFactory1);
        this.app.registerAdapterFactory(adapterFactory2);
        this.app.start();
        expect(adapterFactory1.create).toHaveBeenCalled();
        expect(adapterFactory2.create).toHaveBeenCalled();
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

    it("Can be reset", () => {
        var reporter = jasmine.createSpyObj('reporter', ['reset']);
        this.app.setReporter(reporter);

        var adapterFactory1 = jasmine.createSpyObj('adapterFactory1', ['create']);
        adapterFactory1['create'].andReturn({ start: function () { } });
        this.app.registerAdapterFactory(adapterFactory1);

        this.app.reset();
        expect(reporter.reset).toHaveBeenCalled();
        this.app.start();
        expect(adapterFactory1.create).not.toHaveBeenCalled();
        expect($('#runner')).toHaveAttr('src', 'about:blank');
    });

    it("Can be reloaded", () => {
        var reporter = jasmine.createSpyObj('reporter', ['reset']);
        this.app.setReporter(reporter);

        var adapterFactory1 = jasmine.createSpyObj('adapterFactory1', ['create']);
        adapterFactory1['create'].andReturn({ start: function () { } });
        this.app.registerAdapterFactory(adapterFactory1);

        this.app.reload();
        expect($('#runner')).toHaveAttr('src', 'runner.html');
    });
})