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

    it("Can use custom reporter", function () {
        var reporter = {};
        _this.app.setReporter(reporter);

        var adapterFactory1 = jasmine.createSpyObj('adapterFactory1', ['create']);
        adapterFactory1['create'].andReturn({ start: function () {
            } });
        _this.app.registerAdapterFactory(adapterFactory1);
        _this.app.start();

        expect(adapterFactory1.create).toHaveBeenCalledWith(reporter);
    });

    it("Sets server status", function () {
        var reporter = jasmine.createSpyObj('reporter', ['setServerStatus']);
        _this.app.setReporter(reporter);

        _this.app.setServerStatus('server status');

        expect(reporter.setServerStatus).toHaveBeenCalledWith('server status');
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
        expect(adapterFactory1.create).toHaveBeenCalled();
        expect(adapterFactory2.create).toHaveBeenCalled();
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

    it("Can be reset", function () {
        var reporter = jasmine.createSpyObj('reporter', ['reset']);
        _this.app.setReporter(reporter);

        var adapterFactory1 = jasmine.createSpyObj('adapterFactory1', ['create']);
        adapterFactory1['create'].andReturn({ start: function () {
            } });
        _this.app.registerAdapterFactory(adapterFactory1);

        _this.app.reset();
        expect(reporter.reset).toHaveBeenCalled();
        _this.app.start();
        expect(adapterFactory1.create).not.toHaveBeenCalled();
        expect($('#runner')).toHaveAttr('src', 'about:blank');
    });

    it("Can be reloaded", function () {
        var reporter = jasmine.createSpyObj('reporter', ['reset']);
        _this.app.setReporter(reporter);

        var adapterFactory1 = jasmine.createSpyObj('adapterFactory1', ['create']);
        adapterFactory1['create'].andReturn({ start: function () {
            } });
        _this.app.registerAdapterFactory(adapterFactory1);

        _this.app.reload();
        expect($('#runner')).toHaveAttr('src', 'runner.html');
    });
});
