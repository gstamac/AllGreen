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
            var spec = new JasmineAdapterSpec(jasmineSpec, 1 /* Running */);
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

        JasmineAdapter.prototype.runTests = function () {
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
            if (typeof status === "undefined") { status = 0 /* Undefined */; }
            this.id = JasmineAdapter.convertIdToGuid(jasmineSpec.id);
            this.name = jasmineSpec.description;
            this.status = status;
            this.time = Date.now();
            this.steps = [];
            this.suite = new JasmineAdapterSuite(jasmineSpec.suite);

            if (this.status == 0 /* Undefined */)
                this.calculateStatus(jasmineSpec);
            if (this.status == 3 /* Failed */)
                this.calculateSteps(jasmineSpec);
        }
        JasmineAdapterSpec.prototype.calculateStatus = function (jasmineSpec) {
            var results = jasmineSpec.results();
            if (results.skipped)
                this.status = 4 /* Skipped */;
            else {
                if (results.passed()) {
                    this.status = 2 /* Passed */;
                } else {
                    this.status = 3 /* Failed */;
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
                            errorLocation: null,
                            status: 0 /* Undefined */,
                            trace: ''
                        });
                    } else if (result.type === 'expect') {
                        var expectationResult = result;
                        if (!expectationResult.passed()) {
                            this.steps.push(this.createFailedStep(expectationResult.message, expectationResult.trace.stack));
                        }
                    }
                }
            }
        };

        JasmineAdapterSpec.prototype.createFailedStep = function (error, stack) {
            var message = this.formatException(error);
            var trace = stack ? this.formatTraceStack(stack, message) : '';
            var step = {
                message: message,
                errorLocation: null,
                status: 3 /* Failed */,
                trace: trace
            };
            if (error.fileName || error.sourceURL) {
                step.errorLocation = (error.fileName || error.sourceURL);
                if (error.line || error.lineNumber) {
                    step.errorLocation += ":" + (error.line || error.lineNumber);
                    if (error.columnNumber) {
                        step.errorLocation += ":" + error.columnNumber;
                    }
                }
            }
            return step;
        };

        JasmineAdapterSpec.prototype.formatException = function (error) {
            if (typeof (error) === "string")
                return error;

            return error.name + ': ' + error.message;
        };

        JasmineAdapterSpec.prototype.formatTraceStack = function (stack, message) {
            if (stack.substring(0, message.length + 1) == message + "\n")
                stack = stack.substring(message.length + 1);
            return stack.replace('/^' + message + '\n/g', '').replace(/[^\n]+\/jasmine\.js[\:\d)]+(\n|$)/g, '').replace(/[\n]+$/, '');
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

        jasmine.util.formatException = function (error) {
            /*var message = error.name + ': ' + error.message;
            
            if (error.fileName || error.sourceURL) {
            message += " in " + (error.fileName || error.sourceURL);
            if (error.line || error.lineNumber) {
            message += ":" + (error.line || error.lineNumber);
            if (error.columnNumber) {
            message += ":" + error.columnNumber;
            }
            }
            }
            
            return message;*/
            return error;
        };
    }
})();
//# sourceMappingURL=jasmineAdapter.js.map
