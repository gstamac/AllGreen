/// <reference path="../allgreen.ts" />
/// <reference path="../../Scripts/typings/jasmine/jasmine.d.ts" />
var AllGreen;
(function (AllGreen) {
    var JasmineAdapterFactory = (function () {
        function JasmineAdapterFactory() {
        }
        JasmineAdapterFactory.prototype.create = function (reporter) {
            return new JasmineAdapter(reporter);
        };
        return JasmineAdapterFactory;
    })();
    AllGreen.JasmineAdapterFactory = JasmineAdapterFactory;

    var JasmineAdapter = (function () {
        function JasmineAdapter(reporter) {
            this.reporter = reporter;
        }
        JasmineAdapter.prototype.reportRunnerStarting = function (runner) {
            this.reporter.started();
            //tc.info({ total: runner.specs().length });
        };

        JasmineAdapter.prototype.reportRunnerResults = function (runner) {
            this.reporter.finished();
            /*tc.complete({
            coverage: window.__coverage__
            });*/
        };

        JasmineAdapter.prototype.reportSuiteResults = function (jasmineSuite) {
            // memory clean up
            /*suite.after_ = null;
            suite.before_ = null;
            suite.queue = null;*/
        };

        JasmineAdapter.prototype.reportSpecStarting = function (jasmineSpec) {
            var spec = new JasmineAdapterSpec(jasmineSpec, AllGreen.SpecStatus.Running);
            this.reporter.specUpdated(spec);
        };

        JasmineAdapter.prototype.reportSpecResults = function (jasmineSpec) {
            this.reporter.specUpdated(new JasmineAdapterSpec(jasmineSpec));
            /*var result = {
            id: spec.id,
            description: spec.description,
            suite: [],
            success: spec.results_.failedCount === 0,
            skipped: spec.results_.skipped,
            time: spec.results_.skipped ? 0 : new Date().getTime() - spec.results_.time,
            log: []
            };
            
            var suitePointer = spec.suite;
            while (suitePointer) {
            result.suite.unshift(suitePointer.description);
            suitePointer = suitePointer.parentSuite;
            }
            
            if (!result.success) {
            var steps = spec.results_.items_;
            for (var i = 0; i < steps.length; i++) {
            if (!steps[i].passed_) {
            result.log.push(formatFailedStep(steps[i]));
            }
            }
            }
            
            tc.result(result);
            */
            // memory clean up
            /*spec.results_ = null;
            spec.spies_ = null;
            spec.queue = null;*/
        };

        JasmineAdapter.prototype.log = function () {
        };

        JasmineAdapter.prototype.specFilter = function (jasmineSpec) {
            return true;
        };

        JasmineAdapter.prototype.start = function () {
            var jasmineEnv = jasmine.getEnv();

            jasmineEnv.addReporter(this);
            jasmineEnv.execute();
        };
        return JasmineAdapter;
    })();
    AllGreen.JasmineAdapter = JasmineAdapter;

    var JasmineAdapterSpec = (function () {
        function JasmineAdapterSpec(jasmineSpec, status) {
            if (typeof status === "undefined") { status = AllGreen.SpecStatus.Undefined; }
            this.id = jasmineSpec.id;
            this.name = jasmineSpec.description;
            this.status = status;
            this.steps = [];
            this.suite = new JasmineAdapterSuite(jasmineSpec.suite);

            if (this.status == AllGreen.SpecStatus.Undefined)
                this.calculateStatus(jasmineSpec);
            if (this.status == AllGreen.SpecStatus.Failed)
                this.calculateSteps(jasmineSpec);
        }
        JasmineAdapterSpec.prototype.calculateStatus = function (jasmineSpec) {
            var results = jasmineSpec.results();
            if (results.skipped)
                this.status = AllGreen.SpecStatus.Skipped;
else {
                if (results.passed()) {
                    this.status = AllGreen.SpecStatus.Passed;
                } else {
                    this.status = AllGreen.SpecStatus.Failed;
                }
            }
        };

        JasmineAdapterSpec.prototype.calculateSteps = function (jasmineSpec) {
            var resultItems = jasmineSpec.results().getItems();
            if (resultItems && resultItems.length) {
                this.steps = [];

                for (var i = 0; i < resultItems.length; i++) {
                    var result = resultItems[i];

                    if (result.type === 'log') {
                        this.steps.push({ message: result.toString(), status: AllGreen.SpecStatus.Undefined, trace: '' });
                    } else if (result.type === 'expect') {
                        var expectationResult = result;
                        if (!expectationResult.passed()) {
                            if (expectationResult.trace.stack) {
                                this.steps.push({ message: expectationResult.message, status: AllGreen.SpecStatus.Failed, trace: this.formatTraceStack(expectationResult.trace.stack) });
                            } else {
                                this.steps.push({ message: expectationResult.message, status: AllGreen.SpecStatus.Failed, trace: '' });
                            }
                        }
                    }
                }
            }
        };

        JasmineAdapterSpec.prototype.formatTraceStack = function (stack) {
            return stack.replace(/[^\n]+jasmine\.js\:\d+(\n|$)/g, '').replace(/[\n]+$/, '');
        };
        return JasmineAdapterSpec;
    })();

    var JasmineAdapterSuite = (function () {
        function JasmineAdapterSuite(suite) {
            this.id = suite.id;
            this.name = suite.description;
            this.parentSuite = null;

            if (suite.parentSuite)
                this.parentSuite = new JasmineAdapterSuite(suite.parentSuite);
        }
        return JasmineAdapterSuite;
    })();
})(AllGreen || (AllGreen = {}));

(function () {
    var app = AllGreen.App.getCurrent();
    if (app != null) {
        console.log('registering Jasmine adapter factory');
        app.registerAdapterFactory(new AllGreen.JasmineAdapterFactory());
    }
    ;
})();
