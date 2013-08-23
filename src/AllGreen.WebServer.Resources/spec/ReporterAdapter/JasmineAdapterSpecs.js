/// <reference path="../../Scripts/typings/jasmine/jasmine.d.ts" />
/// <reference path="../../Client/reporterAdapters/jasmineAdapter.ts" />
var _this = this;
describe("AllGreen JasmineAdapterFactory", function () {
    it("Creates Jasmine this.adapter", function () {
        var factory = new AllGreen.JasmineAdapterFactory();
        var reporter = jasmine.createSpyObj('this.reporter', ['reset', 'setServerStatus', 'started', 'finished', 'specUpdated']);
        var adapter = factory.create(_this.reporter);

        expect(adapter).toBeDefined();
        expect(adapter.start).toBeDefined();
    });
});

describe("AllGreen Jasmine this.adapter", function () {
    var createSpecResults = function (ispassed, isskipped, steps) {
        if (typeof steps === "undefined") { steps = []; }
        return function () {
            return {
                passed: function () {
                    return ispassed;
                },
                skipped: isskipped,
                getItems: function () {
                    return steps ? steps : [];
                }
            };
        };
    };

    var createSpecResultLogStep = function (message) {
        return {
            type: 'log',
            toString: function () {
                return message;
            }
        };
    };

    var createSpecResultExpectStep = function (ispassed, message_, traceStack) {
        return {
            type: 'expect',
            passed: function () {
                return ispassed;
            },
            message: message_,
            trace: { stack: traceStack != null ? traceStack : false }
        };
    };

    beforeEach(function () {
        jasmine.getEnv().currentSpec.addMatchers({
            toHaveBeenCalledForSpec: function (specId, specName, suiteId, suiteName, specStatus, steps) {
                var actual = this.actual;

                var specParams = {
                    id: specId,
                    name: specName,
                    suite: jasmine.objectContaining({
                        id: suiteId,
                        name: suiteName
                    }),
                    status: specStatus,
                    steps: []
                };
                if (steps) {
                    specParams.steps = [];
                    for (var i = 0; i < steps.length; i++) {
                        specParams.steps.push(jasmine.objectContaining(steps[i]));
                    }
                }

                expect(actual).toHaveBeenCalledWith(jasmine.objectContaining(specParams));

                return true;
            }
        });

        _this.reporter = jasmine.createSpyObj('reporter', ['reset', 'setServerStatus', 'started', 'finished', 'specUpdated']);
        _this.adapter = new AllGreen.JasmineAdapter(_this.reporter);

        var suite1 = {
            id: 881,
            description: 'Suite 1'
        };
        _this.spec = {
            id: 244,
            description: 'test 1',
            suite: suite1,
            results: createSpecResults(false, false)
        };
        _this.spec2 = {
            id: 344,
            description: 'test 2',
            suite: suite1,
            results: createSpecResults(false, false)
        };
    });

    afterEach(function () {
        _this.adapter = null;
    });

    it("Resets reporter on runner starting and displays this.reporter as started", function () {
        _this.adapter.reportRunnerStarting(_this.runner);
        expect(_this.reporter.started).toHaveBeenCalled();
    });

    it("Displays reporter as finished on runner finished", function () {
        _this.adapter.reportRunnerResults(_this.runner);
        expect(_this.reporter.finished).toHaveBeenCalled();
    });

    it("Shows started specs as running", function () {
        _this.adapter.reportSpecStarting(_this.spec);
        expect(_this.reporter.specUpdated).toHaveBeenCalledForSpec(244, 'test 1', 881, 'Suite 1', AllGreen.SpecStatus.Running, null);
    });

    it("Shows passed specs as passed", function () {
        _this.spec.results = createSpecResults(true, false);
        _this.adapter.reportSpecResults(_this.spec);
        expect(_this.reporter.specUpdated).toHaveBeenCalledForSpec(244, 'test 1', 881, 'Suite 1', AllGreen.SpecStatus.Passed, null);
    });

    it("Shows failed specs as failed", function () {
        _this.spec.results = createSpecResults(false, false, [
            createSpecResultLogStep('log message'),
            createSpecResultExpectStep(true, 'expected something and succeded', ''),
            createSpecResultExpectStep(false, 'expected something and failed', 'trace 1\ntrace 2')
        ]);
        _this.adapter.reportSpecResults(_this.spec);
        expect(_this.reporter.specUpdated).toHaveBeenCalledForSpec(244, 'test 1', 881, 'Suite 1', AllGreen.SpecStatus.Failed, [
            { message: 'log message' },
            { message: 'expected something and failed', status: AllGreen.SpecStatus.Failed, trace: 'trace 1\ntrace 2' }
        ]);
    });

    it("Removes jasmine entries from stack", function () {
        var trace = 'jasmine.ExpectationResult@http://localhost:8080/Scripts/jasmine.js:114\n' + 'trace 1\n' + 'jasmine.Block.prototype.execute@http://localhost:8080/Scripts/jasmine.js:1064\n' + 'trace 2\n' + 'jasmine.Queue.prototype.next_/onComplete@http://localhost:8080/Scripts/jasmine.js:2092' + 'jasmine.Suite.prototype.finish@http://localhost:8080/Scripts/jasmine.js:2478' + 'jasmine.Suite.prototype.execute/<@http://localhost:8080/Scripts/jasmine.js:2522';
        _this.spec.results = createSpecResults(false, false, [
            createSpecResultExpectStep(false, 'msg', trace)
        ]);
        _this.adapter.reportSpecResults(_this.spec);
        expect(_this.reporter.specUpdated).toHaveBeenCalledForSpec(244, 'test 1', 881, 'Suite 1', AllGreen.SpecStatus.Failed, [
            { message: 'msg', status: AllGreen.SpecStatus.Failed, trace: 'trace 1\ntrace 2' }
        ]);
    });

    it("Shows skipped specs as skipped", function () {
        _this.spec.results = createSpecResults(false, true);
        _this.adapter.reportSpecResults(_this.spec);
        expect(_this.reporter.specUpdated).toHaveBeenCalledForSpec(244, 'test 1', 881, 'Suite 1', AllGreen.SpecStatus.Skipped, null);
    });

    it("Shows specs from same suite as results from same suite", function () {
        _this.adapter.reportSpecResults(_this.spec);
        _this.adapter.reportSpecResults(_this.spec2);
        expect(_this.reporter.specUpdated).toHaveBeenCalledForSpec(244, 'test 1', 881, 'Suite 1', AllGreen.SpecStatus.Failed, null);
        expect(_this.reporter.specUpdated).toHaveBeenCalledForSpec(344, 'test 2', 881, 'Suite 1', AllGreen.SpecStatus.Failed, null);
    });

    it("Shows nested suites", function () {
        var suite2 = {
            id: 981,
            description: 'Suite 2',
            parentSuite: {
                id: 881,
                description: 'Suite 1'
            }
        };
        _this.spec.suite = suite2;
        _this.spec2.suite = suite2;
        _this.adapter.reportSpecResults(_this.spec);
        _this.adapter.reportSpecResults(_this.spec2);
        expect(_this.reporter.specUpdated).toHaveBeenCalledWith(jasmine.objectContaining({
            id: 244,
            name: 'test 1',
            suite: jasmine.objectContaining({
                id: 981,
                name: 'Suite 2',
                parentSuite: jasmine.objectContaining({
                    id: 881,
                    name: 'Suite 1'
                })
            }),
            status: AllGreen.SpecStatus.Failed
        }));
        expect(_this.reporter.specUpdated).toHaveBeenCalledWith(jasmine.objectContaining({
            id: 344,
            name: 'test 2',
            suite: jasmine.objectContaining({
                id: 981,
                name: 'Suite 2',
                parentSuite: jasmine.objectContaining({
                    id: 881,
                    name: 'Suite 1'
                })
            }),
            status: AllGreen.SpecStatus.Failed
        }));
    });
});