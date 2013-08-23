/// <reference path="../../Scripts/typings/jasmine/jasmine.d.ts" />
/// <reference path="../../Client/reporterAdapters/jasmineAdapter.ts" />

describe("AllGreen JasmineAdapterFactory", () => {
    it("Creates Jasmine this.adapter", () => {
        var factory = new AllGreen.JasmineAdapterFactory();
        var reporter = jasmine.createSpyObj('this.reporter', ['reset', 'setServerStatus', 'started', 'finished', 'specUpdated']);
        var adapter = factory.create(this.reporter);

        expect(adapter).toBeDefined();
        expect(adapter.start).toBeDefined();
    });
});

declare module jasmine {
    interface Matchers {
        toHaveBeenCalledForSpec(specId, specName, suiteId, suiteName, specStatus, steps): boolean;
    }
}

describe("AllGreen Jasmine this.adapter", () => {
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

        this.reporter = jasmine.createSpyObj('reporter', ['reset', 'setServerStatus', 'started', 'finished', 'specUpdated']);
        this.adapter = new AllGreen.JasmineAdapter(this.reporter);

        var suite1 = {
            id: 881,
            description: 'Suite 1'
        };
        this.spec = {
            id: 244,
            description: 'test 1',
            suite: suite1,
            results: createSpecResults(false, false)
        };
        this.spec2 = {
            id: 344,
            description: 'test 2',
            suite: suite1,
            results: createSpecResults(false, false)
        };
    });

    afterEach(() => {
        this.adapter = null;
    });

    it("Resets reporter on runner starting and displays this.reporter as started", () => {
        this.adapter.reportRunnerStarting(this.runner);
        expect(this.reporter.started).toHaveBeenCalled();
    });

    it("Displays reporter as finished on runner finished", () => {
        this.adapter.reportRunnerResults(this.runner);
        expect(this.reporter.finished).toHaveBeenCalled();
    });

    it("Shows started specs as running", () => {
        this.adapter.reportSpecStarting(this.spec);
        expect(this.reporter.specUpdated).toHaveBeenCalledForSpec(244, 'test 1', 881, 'Suite 1', AllGreen.SpecStatus.Running, null);
    });

    it("Shows passed specs as passed", () => {
        this.spec.results = createSpecResults(true, false);
        this.adapter.reportSpecResults(this.spec);
        expect(this.reporter.specUpdated).toHaveBeenCalledForSpec(244, 'test 1', 881, 'Suite 1', AllGreen.SpecStatus.Passed, null);
    });

    it("Shows failed specs as failed", () => {
        this.spec.results = createSpecResults(false, false, [
            createSpecResultLogStep('log message'),
            createSpecResultExpectStep(true, 'expected something and succeded', ''),
            createSpecResultExpectStep(false, 'expected something and failed', 'trace 1\ntrace 2')
        ]);
        this.adapter.reportSpecResults(this.spec);
        expect(this.reporter.specUpdated).toHaveBeenCalledForSpec(244, 'test 1', 881, 'Suite 1', AllGreen.SpecStatus.Failed, [
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
        this.spec.results = createSpecResults(false, false, [
            createSpecResultExpectStep(false, 'msg', trace)
        ]);
        this.adapter.reportSpecResults(this.spec);
        expect(this.reporter.specUpdated).toHaveBeenCalledForSpec(244, 'test 1', 881, 'Suite 1', AllGreen.SpecStatus.Failed, [
            { message: 'msg', status: AllGreen.SpecStatus.Failed, trace: 'trace 1\ntrace 2' }
        ]);
    });

    it("Shows skipped specs as skipped", () => {
        this.spec.results = createSpecResults(false, true);
        this.adapter.reportSpecResults(this.spec);
        expect(this.reporter.specUpdated).toHaveBeenCalledForSpec(244, 'test 1', 881, 'Suite 1', AllGreen.SpecStatus.Skipped, null);
    });

    it("Shows specs from same suite as results from same suite", () => {
        this.adapter.reportSpecResults(this.spec);
        this.adapter.reportSpecResults(this.spec2);
        expect(this.reporter.specUpdated).toHaveBeenCalledForSpec(244, 'test 1', 881, 'Suite 1', AllGreen.SpecStatus.Failed, null);
        expect(this.reporter.specUpdated).toHaveBeenCalledForSpec(344, 'test 2', 881, 'Suite 1', AllGreen.SpecStatus.Failed, null);
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
        this.spec.suite = suite2;
        this.spec2.suite = suite2;
        this.adapter.reportSpecResults(this.spec);
        this.adapter.reportSpecResults(this.spec2);
        expect(this.reporter.specUpdated).toHaveBeenCalledWith(jasmine.objectContaining({
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
        expect(this.reporter.specUpdated).toHaveBeenCalledWith(jasmine.objectContaining({
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

