/// <reference path="../allgreen.ts" />
/// <reference path="../../Scripts/typings/jasmine/jasmine.d.ts" />

module AllGreen {
    export class JasmineAdapterFactory implements IAdapterFactory {
        public create(reporter: IRunnerReporter): IAdapter {
            return new JasmineAdapter(reporter);
        }

        getName(): string {
            return 'jasmine';
        }
    }

    export class JasmineAdapter implements IAdapter, jasmine.Reporter {
        private reporter: IRunnerReporter;

        constructor(reporter: IRunnerReporter) {
            this.reporter = reporter;
        }

        reportRunnerStarting(runner: jasmine.Runner) {
            this.reporter.started(runner.specs().length);
        }

        reportRunnerResults(runner: jasmine.Runner) {
            this.reporter.finished();
        }

        reportSuiteResults(jasmineSuite: jasmine.Suite) {
            // TODO
        }

        reportSpecStarting(jasmineSpec: jasmine.Spec) {
            var spec = new JasmineAdapterSpec(jasmineSpec, AllGreen.SpecStatus.Running);
            this.reporter.specUpdated(spec);
        }

        reportSpecResults(jasmineSpec: jasmine.Spec) {
            this.reporter.specUpdated(new JasmineAdapterSpec(jasmineSpec));
        }

        log() { }

        specFilter(jasmineSpec: jasmine.Spec) {
            // TODO
            return true;
        }

        runTests() {
            var jasmineEnv = jasmine.getEnv();
            jasmineEnv.addReporter(this);
            jasmineEnv.execute();
        }

        static convertIdToGuid(id: number): string {
            var id16 = '000000000000000000000000000000000000000' + id;
            return id16.substr(-32, 8) + '-' +
                id16.substr(-24, 4) + '-' +
                id16.substr(-20, 4) + '-' +
                id16.substr(-16, 4) + '-' +
                id16.substr(-12);
        }
    }

    class JasmineAdapterSpec implements ISpec {
        public id: string;
        public name: string;
        public suite: ISuite;
        public status: SpecStatus;
        public time: number;
        public steps: ISpecStep[];

        constructor(jasmineSpec: jasmine.Spec, status: SpecStatus = AllGreen.SpecStatus.Undefined) {
            this.id = JasmineAdapter.convertIdToGuid(jasmineSpec.id);
            this.name = jasmineSpec.description;
            this.status = status;
            this.time = Date.now();
            this.steps = [];
            this.suite = new JasmineAdapterSuite(jasmineSpec.suite);

            if (this.status == AllGreen.SpecStatus.Undefined) this.calculateStatus(jasmineSpec);
            if (this.status == AllGreen.SpecStatus.Failed) this.calculateSteps(jasmineSpec);
        }

        calculateStatus(jasmineSpec: jasmine.Spec) {
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
        }

        calculateSteps(jasmineSpec: jasmine.Spec) {
            var resultItems = jasmineSpec.results().getItems();
            if (resultItems && resultItems.length) {
                this.steps = [];

                for (var i = 0; i < resultItems.length; i++) {
                    var result = resultItems[i];

                    if (result.type === 'log') {
                        this.steps.push({
                            message: result.toString(),
                            errorLocation: null,
                            status: AllGreen.SpecStatus.Undefined,
                            trace: ''
                        });
                    } else if (result.type === 'expect') {
                        var expectationResult = <jasmine.ExpectationResult>result;
                        if (!expectationResult.passed()) {
                            this.steps.push(this.createFailedStep(expectationResult.message, expectationResult.trace.stack));
                        }
                    }
                }
            }
        }

        createFailedStep(error, stack): AllGreen.ISpecStep {
            var message = this.formatException(error);
            var trace = stack ? this.formatTraceStack(stack, message) : '';
            var step : AllGreen.ISpecStep = {
                message: message,
                errorLocation: null,
                status: AllGreen.SpecStatus.Failed,
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
        }

        formatException(error): string {
            if (typeof (error) === "string")
                return error;

            return error.name + ': ' + error.message;
        }

        formatTraceStack(stack: string, message: string) {
            if (stack.substring(0, message.length + 1) == message + "\n")
                stack = stack.substring(message.length + 1);
            return stack
                .replace('/^' + message + '\n/g', '')
                .replace(/[^\n]+\/jasmine\.js[\:\d)]+(\n|$)/g, '')
                .replace(/[\n]+$/, '');
        }
    }

    class JasmineAdapterSuite implements ISuite {
        public id: string;
        public name: string;
        public parentSuite: ISuite;
        public status: SpecStatus;

        constructor(suite: jasmine.Suite) {
            this.id = JasmineAdapter.convertIdToGuid(suite.id);
            this.name = suite.description;
            this.parentSuite = null;

            if (suite.parentSuite)
                this.parentSuite = new JasmineAdapterSuite(suite.parentSuite);
        }
    }
}

declare module jasmine {
    var util: any;
}

(() => {
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
        }
    }
}) ();
