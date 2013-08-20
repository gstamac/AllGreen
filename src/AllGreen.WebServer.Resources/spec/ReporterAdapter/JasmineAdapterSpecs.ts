/// <reference path="../../Scripts/typings/jasmine/jasmine.d.ts" />
/// <reference path="../../Client/ReporterAdapters/jasmineAdapter.ts" />

describe("AllGreen JasmineAdapterFactory", () => {
    it("Creates Jasmine adapter", () => {
        var factory = new AllGreen.JasmineAdapterFactory();
        reporter = jasmine.createSpyObj('reporter', ['reset', 'setServerStatus', 'setRunnerStatus', 'setSpecStatus']);
        var adapter = factory.create(reporter);

        expect(adapter).toBeDefined();
        expect(adapter.start).toBeDefined();
    });
});

declare module jasmine {
    interface Matchers {
        toHaveBeenCalledForSpec(specId, specName, suiteId, suiteName, specStatus, steps): boolean;
    }
}

var reporter;
var adapter;
var runner;
var spec;
var spec2;

describe("AllGreen Jasmine Adapter", () => {
    var createSpecResults = function (ispassed: boolean, isskipped: boolean, steps: any[] = []): Function {
        return () => {
            return {
                passed: () => ispassed,
                skipped: isskipped,
                getItems: () => steps ? steps : []
            }
        }
    }

    var createSpecResultLogStep = function (message: string) {
        return {
            type: 'log',
            toString: () => message
        }
    }

    var createSpecResultExpectStep = function (ispassed: boolean, message_: string, traceStack: any) {
        return {
            type: 'expect',
            passed: () => ispassed,
            message: message_,
            trace: { stack: traceStack != null ? traceStack : false }
        }
    }

    beforeEach(() => {
        jasmine.getEnv().currentSpec.addMatchers({
            toHaveBeenCalledForSpec: function (specId, specName, suiteId, suiteName, specStatus, steps): boolean {
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
                    for (var i = 0; i < steps.length; i++)
                    {
                        specParams.steps.push(jasmine.objectContaining(steps[i]));
                    }
                }

                expect(actual).toHaveBeenCalledWith(jasmine.objectContaining(specParams));

                return true;
            },
        });

        reporter = jasmine.createSpyObj('reporter', ['reset', 'setServerStatus', 'setRunnerStatus', 'setSpecStatus']);
        adapter = new AllGreen.JasmineAdapter(reporter);

        var suite1 = {
            id: 881,
            description: 'Suite 1'
        };
        spec = {
            id: 244,
            description: 'test 1',
            suite: suite1,
            results: createSpecResults(false, false)
        };
        spec2 = {
            id: 344,
            description: 'test 2',
            suite: suite1,
            results: createSpecResults(false, false)
        };
    });

    afterEach(() => {
        adapter = null;
    });

    it("Resets reporter on runner starting and displays reporter as started", () => {
        adapter.reportRunnerStarting(runner);
        expect(reporter.setRunnerStatus).toHaveBeenCalledWith('Running...');
    });

    it("Displays reporter as finished on runner finished", () => {
        adapter.reportRunnerResults(runner);
        expect(reporter.setRunnerStatus).toHaveBeenCalledWith('Finished');
    });

    it("Shows started specs as running", () => {
        adapter.reportSpecStarting(spec);
        expect(reporter.setSpecStatus).toHaveBeenCalledForSpec(244, 'test 1', 881, 'Suite 1', AllGreen.SpecStatus.Running, null);
    });

    it("Shows passed specs as passed", () => {
        spec.results = createSpecResults(true, false);
        adapter.reportSpecResults(spec);
        expect(reporter.setSpecStatus).toHaveBeenCalledForSpec(244, 'test 1', 881, 'Suite 1', AllGreen.SpecStatus.Passed, null);
    });

    it("Shows failed specs as failed", () => {
        spec.results = createSpecResults(false, false, [
            createSpecResultLogStep('log message'),
            createSpecResultExpectStep(true, 'expected something and succeded', ''),
            createSpecResultExpectStep(false, 'expected something and failed', 'trace 1\ntrace 2')
        ]);
        adapter.reportSpecResults(spec);
        expect(reporter.setSpecStatus).toHaveBeenCalledForSpec(244, 'test 1', 881, 'Suite 1', AllGreen.SpecStatus.Failed, [
            { message: 'log message' },
            { message: 'expected something and failed', status: AllGreen.SpecStatus.Failed, trace: 'trace 1\ntrace 2' }
        ]);
    });
    
    it("Removes jasmine entries from stack", () => {
        var trace =
            'jasmine.ExpectationResult@http://localhost:8080/Scripts/jasmine.js:114\n' +
            'trace 1\n' +
            'jasmine.Block.prototype.execute@http://localhost:8080/Scripts/jasmine.js:1064\n' +
            'trace 2\n' +
            'jasmine.Queue.prototype.next_/onComplete@http://localhost:8080/Scripts/jasmine.js:2092' +
            'jasmine.Suite.prototype.finish@http://localhost:8080/Scripts/jasmine.js:2478' +
            'jasmine.Suite.prototype.execute/<@http://localhost:8080/Scripts/jasmine.js:2522';
        spec.results = createSpecResults(false, false, [
            createSpecResultExpectStep(false, 'msg', trace)
        ]);
        adapter.reportSpecResults(spec);
        expect(reporter.setSpecStatus).toHaveBeenCalledForSpec(244, 'test 1', 881, 'Suite 1', AllGreen.SpecStatus.Failed, [
            { message: 'msg', status: AllGreen.SpecStatus.Failed, trace: 'trace 1\ntrace 2' }
        ]);
    });

    it("Shows skipped specs as skipped", () => {
        spec.results = createSpecResults(false, true);
        adapter.reportSpecResults(spec);
        expect(reporter.setSpecStatus).toHaveBeenCalledForSpec(244, 'test 1', 881, 'Suite 1', AllGreen.SpecStatus.Skipped, null);
    });

    it("Shows specs from same suite as results from same suite", () => {
        adapter.reportSpecResults(spec);
        adapter.reportSpecResults(spec2);
        expect(reporter.setSpecStatus).toHaveBeenCalledForSpec(244, 'test 1', 881, 'Suite 1', AllGreen.SpecStatus.Failed, null);
        expect(reporter.setSpecStatus).toHaveBeenCalledForSpec(344, 'test 2', 881, 'Suite 1', AllGreen.SpecStatus.Failed, null);
    });

    it("Shows nested suites", () => {
        var suite2 = {
            id: 981,
            description: 'Suite 2',
            parentSuite: {
                id: 881,
                description: 'Suite 1',
            }
        };
        spec.suite = suite2;
        spec2.suite = suite2;
        adapter.reportSpecResults(spec);
        adapter.reportSpecResults(spec2);
        expect(reporter.setSpecStatus).toHaveBeenCalledWith(jasmine.objectContaining({
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
        expect(reporter.setSpecStatus).toHaveBeenCalledWith(jasmine.objectContaining({
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

