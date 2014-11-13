/// <reference path="../../Scripts/typings/jasmine/jasmine.d.ts" />
/// <reference path="../../Client/reporterAdapters/jasmineAdapter.ts" />
var _this = this;
describe("AllGreen JasmineAdapterFactory", function () {
    it("Creates Jasmine this.adapter", function () {
        var factory = new AllGreen.JasmineAdapterFactory();
        var reporter = jasmine.createSpyObj('this.reporter', ['reset', 'setServerStatus', 'started', 'finished', 'specUpdated']);
        var adapter = factory.create(_this.reporter);

        expect(adapter).toBeDefined();
        expect(adapter.runTests).toBeDefined();
    });

    it("Should have the correct name", function () {
        var factory = new AllGreen.JasmineAdapterFactory();
        expect(factory.getName()).toBe('jasmine');
    });
});

describe("AllGreen JasmineAdapter", function () {
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
                    time: jasmine.any(Number),
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
        _this.suite1Guid = '00000000-0000-0000-0000-000000000881';
        _this.spec = {
            id: 244,
            description: 'test 1',
            suite: suite1,
            results: createSpecResults(false, false)
        };
        _this.specGuid = '00000000-0000-0000-0000-000000000244';
        _this.spec2 = {
            id: 344,
            description: 'test 2',
            suite: suite1,
            results: createSpecResults(false, false)
        };
        _this.spec2Guid = '00000000-0000-0000-0000-000000000344';
    });

    afterEach(function () {
        _this.adapter = null;
    });

    it("Resets reporter on runner starting and displays reporter as started", function () {
        var runner = jasmine.createSpyObj('runner', ['specs']);
        runner['specs'].andReturn({ length: 10 });
        _this.adapter.reportRunnerStarting(runner);
        expect(_this.reporter.started).toHaveBeenCalledWith(10);
    });

    it("Displays reporter as finished on runner finished", function () {
        var runner = jasmine.createSpyObj('runner', ['specs']);
        _this.adapter.reportRunnerResults(_this.runner);
        expect(_this.reporter.finished).toHaveBeenCalled();
    });

    it("Shows started specs as running", function () {
        _this.adapter.reportSpecStarting(_this.spec);
        expect(_this.reporter.specUpdated).toHaveBeenCalledForSpec(_this.specGuid, 'test 1', _this.suite1Guid, 'Suite 1', 1 /* Running */, null);
    });

    it("Shows passed specs as passed", function () {
        _this.spec.results = createSpecResults(true, false);
        _this.adapter.reportSpecResults(_this.spec);
        expect(_this.reporter.specUpdated).toHaveBeenCalledForSpec(_this.specGuid, 'test 1', _this.suite1Guid, 'Suite 1', 2 /* Passed */, null);
    });

    it("Shows failed specs as failed", function () {
        _this.spec.results = createSpecResults(false, false, [
            createSpecResultLogStep('log message'),
            createSpecResultExpectStep(true, 'expected something and succeded', ''),
            createSpecResultExpectStep(false, 'expected something and failed', 'trace 1\ntrace 2')
        ]);
        _this.adapter.reportSpecResults(_this.spec);
        expect(_this.reporter.specUpdated).toHaveBeenCalledForSpec(_this.specGuid, 'test 1', _this.suite1Guid, 'Suite 1', 3 /* Failed */, [
            { message: 'log message' },
            { message: 'expected something and failed', status: 3 /* Failed */, trace: 'trace 1\ntrace 2' }
        ]);
    });

    it("Handles string messages", function () {
        _this.spec.results = createSpecResults(false, false, [
            {
                type: 'expect',
                passed: function () {
                    return false;
                },
                message: 'expected something and failed',
                trace: ''
            }
        ]);
        _this.adapter.reportSpecResults(_this.spec);
        expect(_this.reporter.specUpdated).toHaveBeenCalledForSpec(_this.specGuid, 'test 1', _this.suite1Guid, 'Suite 1', 3 /* Failed */, [
            { message: 'expected something and failed' }
        ]);
    });

    it("Handles Firefox error object messages", function () {
        _this.spec.results = createSpecResults(false, false, [
            {
                type: 'expect',
                passed: function () {
                    return false;
                },
                message: {
                    name: 'Error',
                    message: 'expected something and failed',
                    fileName: 'file.js',
                    lineNumber: 10,
                    columnNumber: 12
                },
                trace: ''
            }
        ]);
        _this.adapter.reportSpecResults(_this.spec);
        expect(_this.reporter.specUpdated).toHaveBeenCalledForSpec(_this.specGuid, 'test 1', _this.suite1Guid, 'Suite 1', 3 /* Failed */, [
            {
                message: 'Error: expected something and failed',
                errorLocation: 'file.js:10:12'
            }
        ]);
    });

    it("Handles Safari error object messages", function () {
        _this.spec.results = createSpecResults(false, false, [
            {
                type: 'expect',
                passed: function () {
                    return false;
                },
                message: {
                    name: 'Error',
                    message: 'expected something and failed',
                    sourceURL: 'file.js',
                    line: 10
                },
                trace: ''
            }
        ]);
        _this.adapter.reportSpecResults(_this.spec);
        expect(_this.reporter.specUpdated).toHaveBeenCalledForSpec(_this.specGuid, 'test 1', _this.suite1Guid, 'Suite 1', 3 /* Failed */, [
            {
                message: 'Error: expected something and failed',
                errorLocation: 'file.js:10'
            }
        ]);
    });

    it("Handles IE,Opera,Chrome error object messages", function () {
        _this.spec.results = createSpecResults(false, false, [
            {
                type: 'expect',
                passed: function () {
                    return false;
                },
                message: {
                    name: 'Error',
                    message: 'expected something and failed'
                },
                trace: ''
            }
        ]);
        _this.adapter.reportSpecResults(_this.spec);
        expect(_this.reporter.specUpdated).toHaveBeenCalledForSpec(_this.specGuid, 'test 1', _this.suite1Guid, 'Suite 1', 3 /* Failed */, [
            {
                message: 'Error: expected something and failed'
            }
        ]);
    });

    it("Removes error message from first line of stack", function () {
        var trace = 'error message\n' + 'trace 1\n' + 'trace 2\n';
        _this.spec.results = createSpecResults(false, false, [
            createSpecResultExpectStep(false, 'error message', trace)
        ]);
        _this.adapter.reportSpecResults(_this.spec);
        expect(_this.reporter.specUpdated).toHaveBeenCalledForSpec(_this.specGuid, 'test 1', _this.suite1Guid, 'Suite 1', 3 /* Failed */, [
            { message: 'error message', status: 3 /* Failed */, trace: 'trace 1\ntrace 2' }
        ]);
    });

    it("Removes jasmine entries from stack", function () {
        var trace = 'jasmine.ExpectationResult@http://localhost:8080/Scripts/jasmine.js:114\n' + 'trace 1\n' + 'jasmine.Block.prototype.execute@http://localhost:8080/Scripts/jasmine.js:1064\n' + 'trace 2\n' + 'jasmine.Queue.prototype.next_/onComplete@http://localhost:8080/Scripts/jasmine.js:2092' + 'jasmine.Suite.prototype.finish@http://localhost:8080/Scripts/jasmine.js:2478' + 'jasmine.Suite.prototype.execute/<@http://localhost:8080/Scripts/jasmine.js:2522';
        _this.spec.results = createSpecResults(false, false, [
            createSpecResultExpectStep(false, 'msg', trace)
        ]);
        _this.adapter.reportSpecResults(_this.spec);
        expect(_this.reporter.specUpdated).toHaveBeenCalledForSpec(_this.specGuid, 'test 1', _this.suite1Guid, 'Suite 1', 3 /* Failed */, [
            { message: 'msg', status: 3 /* Failed */, trace: 'trace 1\ntrace 2' }
        ]);
    });

    it("Shows skipped specs as skipped", function () {
        _this.spec.results = createSpecResults(false, true);
        _this.adapter.reportSpecResults(_this.spec);
        expect(_this.reporter.specUpdated).toHaveBeenCalledForSpec(_this.specGuid, 'test 1', _this.suite1Guid, 'Suite 1', 4 /* Skipped */, null);
    });

    it("Shows specs from same suite as results from same suite", function () {
        _this.adapter.reportSpecResults(_this.spec);
        _this.adapter.reportSpecResults(_this.spec2);
        expect(_this.reporter.specUpdated).toHaveBeenCalledForSpec(_this.specGuid, 'test 1', _this.suite1Guid, 'Suite 1', 3 /* Failed */, null);
        expect(_this.reporter.specUpdated).toHaveBeenCalledForSpec(_this.spec2Guid, 'test 2', _this.suite1Guid, 'Suite 1', 3 /* Failed */, null);
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
            id: _this.specGuid,
            name: 'test 1',
            suite: jasmine.objectContaining({
                id: '00000000-0000-0000-0000-000000000981',
                name: 'Suite 2',
                parentSuite: jasmine.objectContaining({
                    id: _this.suite1Guid,
                    name: 'Suite 1'
                })
            }),
            status: 3 /* Failed */
        }));
        expect(_this.reporter.specUpdated).toHaveBeenCalledWith(jasmine.objectContaining({
            id: _this.spec2Guid,
            name: 'test 2',
            suite: jasmine.objectContaining({
                id: '00000000-0000-0000-0000-000000000981',
                name: 'Suite 2',
                parentSuite: jasmine.objectContaining({
                    id: _this.suite1Guid,
                    name: 'Suite 1'
                })
            }),
            status: 3 /* Failed */
        }));
    });

    it("Should convert number to GUID", function () {
        [
            { id: 123, guid: '00000000-0000-0000-0000-000000000123' },
            { id: 9000000123, guid: '00000000-0000-0000-0000-009000000123' },
            { id: 80009000000123, guid: '00000000-0000-0000-0080-009000000123' },
            { id: 70000009000000120, guid: '00000000-0000-0007-0000-009000000120' }
        ].forEach(function (data) {
            expect(AllGreen.JasmineAdapter.convertIdToGuid(data.id)).toBe(data.guid);
        });
    });
});
//# sourceMappingURL=JasmineAdapterSpecs.js.map
