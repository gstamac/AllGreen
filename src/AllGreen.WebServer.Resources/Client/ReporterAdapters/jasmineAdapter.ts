/// <reference path="../allgreen.ts" />
/// <reference path="../../Scripts/typings/jasmine/jasmine.d.ts" />

module AllGreen {
    export class JasmineAdapterFactory implements IAdapterFactory {
        public create(reporter: IReporter): IAdapter {
            return new JasmineAdapter(reporter);
        }
    }

    export class JasmineAdapter implements IAdapter, jasmine.Reporter {
        private reporter: IReporter;

        constructor(reporter: IReporter) {
            this.reporter = reporter;
        }

        reportRunnerStarting(runner) {
            this.reporter.setRunnerStatus('Running...');
            //tc.info({ total: runner.specs().length });
        }

        reportRunnerResults(runner) {
            this.reporter.setRunnerStatus('Finished');
            /*tc.complete({
                coverage: window.__coverage__
            });*/
        }

        reportSuiteResults(jasmineSuite) {
            // memory clean up
            /*suite.after_ = null;
            suite.before_ = null;
            suite.queue = null;*/
        }

        reportSpecStarting(jasmineSpec) {
            var spec = new JasmineAdapterSpec(jasmineSpec, AllGreen.SpecStatus.Running);
            this.reporter.setSpecStatus(spec);
        }

        reportSpecResults(jasmineSpec) {
            this.reporter.setSpecStatus(new JasmineAdapterSpec(jasmineSpec));
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
        }

        log() { }

        specFilter(jasmineSpec) {
            /*if (!focusedSpecName()) {
                return true;
            }

            return spec.getFullName().indexOf(focusedSpecName()) === 0;*/
        }

        start() {
            var jasmineEnv = jasmine.getEnv();

            jasmineEnv.addReporter(this);
            jasmineEnv.execute();
        }
    }

    class JasmineAdapterSpec implements ISpec {
        public id: any;
        public name: string;
        public suite: ISuite;
        public status: SpecStatus;
        public steps: ISpecStep[];

        constructor(jasmineSpec: jasmine.Spec, status: SpecStatus = AllGreen.SpecStatus.Undefined) {
            this.id = jasmineSpec.id;
            this.name = jasmineSpec.description;
            this.status = status;
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
                    var step = resultItems[i];

                    if (step.type === 'log') {
                        this.steps.push({ message: step.toString(), status: AllGreen.SpecStatus.Undefined, trace: '' });
                    } else if (step.type === 'expect' && !step.passed()) {
                        if (step.trace.stack) {
                            this.steps.push({ message: step.message, status: AllGreen.SpecStatus.Failed, trace: this.formatTraceStack(step.trace.stack) });
                        } else {
                            this.steps.push({ message: step.message, status: AllGreen.SpecStatus.Failed, trace: '' });
                        }
                    }
                }
            }
        }

        formatTraceStack(stack) {
            return stack.replace(/[^\n]+jasmine\.js\:\d+(\n|$)/g, '').replace(/[\n]+$/, '');
        }
    }

    class JasmineAdapterSuite implements ISuite {
        public id: any;
        public name: string;
        public parentSuite: ISuite;
        public status: SpecStatus;

        constructor(suite) {
            this.id = suite.id;
            this.name = suite.description;
            this.parentSuite = null;

            if (suite.parentSuite)
                this.parentSuite = new JasmineAdapterSuite(suite.parentSuite);
        }
    }
}

() => {
    var app = AllGreen.App.getCurrent();
    if (app != null) {
        console.log('registering Jasmine adapter factory');
        app.registerAdapterFactory(new AllGreen.JasmineAdapterFactory());
    };
} ();
