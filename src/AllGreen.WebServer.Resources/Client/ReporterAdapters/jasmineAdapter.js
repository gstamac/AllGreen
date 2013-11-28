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

        JasmineAdapterFactory.prototype.getName = function () {
            return 'jasmine';
        };
        return JasmineAdapterFactory;
    })();
    AllGreen.JasmineAdapterFactory = JasmineAdapterFactory;

    var JasmineAdapter = (function () {
        function JasmineAdapter(reporter) {
            this.reporter = reporter;
        }
        JasmineAdapter.prototype.reportRunnerStarting = function (runner) {
            this.reporter.started(runner.specs().length);
        };

        JasmineAdapter.prototype.reportRunnerResults = function (runner) {
            this.reporter.finished();
        };

        JasmineAdapter.prototype.reportSuiteResults = function (jasmineSuite) {
            // TODO
        };

        JasmineAdapter.prototype.reportSpecStarting = function (jasmineSpec) {
            var spec = new JasmineAdapterSpec(jasmineSpec, AllGreen.SpecStatus.Running);
            this.reporter.specUpdated(spec);
        };

        JasmineAdapter.prototype.reportSpecResults = function (jasmineSpec) {
            this.reporter.specUpdated(new JasmineAdapterSpec(jasmineSpec));
        };

        JasmineAdapter.prototype.log = function () {
        };

        JasmineAdapter.prototype.specFilter = function (jasmineSpec) {
            // TODO
            return true;
        };

        JasmineAdapter.prototype.start = function () {
            var jasmineEnv = jasmine.getEnv();
            jasmineEnv.addReporter(this);
            jasmineEnv.execute();
        };

        JasmineAdapter.convertIdToGuid = function (id) {
            var id16 = '000000000000000000000000000000000000000' + id;
            return id16.substr(-32, 8) + '-' + id16.substr(-24, 4) + '-' + id16.substr(-20, 4) + '-' + id16.substr(-16, 4) + '-' + id16.substr(-12);
        };
        return JasmineAdapter;
    })();
    AllGreen.JasmineAdapter = JasmineAdapter;

    var JasmineAdapterSpec = (function () {
        function JasmineAdapterSpec(jasmineSpec, status) {
            if (typeof status === "undefined") { status = AllGreen.SpecStatus.Undefined; }
            this.id = JasmineAdapter.convertIdToGuid(jasmineSpec.id);
            this.name = jasmineSpec.description;
            this.status = status;
            this.time = Date.now();
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
                        this.steps.push({
                            message: result.toString(),
                            status: AllGreen.SpecStatus.Undefined,
                            filename: null,
                            lineNumber: -1,
                            trace: ''
                        });
                    } else if (result.type === 'expect') {
                        var expectationResult = result;
                        if (!expectationResult.passed()) {
                            if (expectationResult.trace.stack) {
                                this.steps.push({
                                    message: expectationResult.message,
                                    status: AllGreen.SpecStatus.Failed,
                                    filename: null,
                                    lineNumber: -1,
                                    trace: this.formatTraceStack(expectationResult.trace.stack)
                                });
                            } else {
                                this.steps.push({
                                    message: expectationResult.message,
                                    status: AllGreen.SpecStatus.Failed,
                                    filename: null,
                                    lineNumber: -1,
                                    trace: ''
                                });
                            }
                        }
                    }
                }
            }
        };

        JasmineAdapterSpec.prototype.formatTraceStack = function (stack) {
            return stack.replace(/[^\n]+\/jasmine\.js\:\d+(\n|$)/g, '').replace(/[\n]+$/, '');
        };
        return JasmineAdapterSpec;
    })();

    var JasmineAdapterSuite = (function () {
        function JasmineAdapterSuite(suite) {
            this.id = JasmineAdapter.convertIdToGuid(suite.id);
            this.name = suite.description;
            this.parentSuite = null;

            if (suite.parentSuite)
                this.parentSuite = new JasmineAdapterSuite(suite.parentSuite);
        }
        return JasmineAdapterSuite;
    })();
})(AllGreen || (AllGreen = {}));

(function () {
    if (AllGreenApp != null) {
        AllGreenApp.log('registering Jasmine adapter factory');
        AllGreenApp.registerAdapterFactory(new AllGreen.JasmineAdapterFactory());
    }
})();
