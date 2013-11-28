/// <reference path="allgreen.ts" />
/// <reference path="../Scripts/typings/jquery/jquery.d.ts" />

module AllGreen {
    export class ServerReporter implements IServerReporter {
        private DEFAULT_SERVER_STATUS: string = 'Connecting...';

        setServerStatus(status: string) {
            this.getServerStatusElement().html(status);
        }

        private getServerStatusElement() {
            return $('#server-status');
        }
    }

    export class RunnerReporter implements IRunnerReporter {
        private DEFAULT_RUNNER_STATUS: string = 'Waiting...';

        private totalStatus = null;

        constructor() { }

        public isReady(): boolean {
            return true;
        }

        public reset() {
            this.setRunnerStatus(this.DEFAULT_RUNNER_STATUS);
            this.getSpecResultsElement().text('');
            this.totalStatus = null;
        }

        public started(totalSpecs: number) {
            this.setRunnerStatus('Running ' + totalSpecs + ' tests...');
        }

        public specUpdated(spec: ISpec) {
            var specElement = this.getSpecElement(spec);
            this.setSpecElementClass(specElement, spec.status);
            this.setSpecElementSteps(specElement, spec.steps);
            this.updateSuiteStatus(spec.suite, spec.status);
        }

        public finished() {
            this.setRunnerStatus('Finished');
        }

        private setRunnerStatus(status: string) {
            this.getRunnerStatusElement().html(status);
        }

        private getRunnerStatusElement() {
            return $('#runner-status');
        }

        private getSpecResultsElement() {
            return $('#spec-results');
        }

        private getSpecElement(spec: ISpec) {
            var specElement = $('#spec-' + spec.id);
            if (specElement.length <= 0) {
                specElement = this.createSpecElement(spec);
            }
            return specElement;
        }

        private createSpecElement(spec: ISpec) {
            var suiteElement = this.getSuiteContentElement(spec.suite);
            var specElement = $('<div/>')
                .addClass('spec')
                .prop('id', 'spec-' + spec.id);
            $('<a/>')
                .prop('href', '#')
                .html(spec.name)
                .addClass('spec-name')
                .click(function () { $('#spec-' + spec.id + ' > .spec-steps').toggle(); })
                .appendTo(specElement);

            specElement.appendTo(suiteElement);
            return specElement;
        }

        private setSpecElementClass(element: JQuery, status: SpecStatus) {
            element.prop('class', 'spec ' + this.statusClass(status));
        }

        private setSpecElementSteps(specElement: JQuery, steps) {
            var stepsElement = specElement.children('.spec-steps');
            if (steps && steps.length) {
                if (!stepsElement.length) {
                    stepsElement = $('<div/>')
                        .addClass('spec-steps');
                    stepsElement.appendTo(specElement);
                }
                stepsElement.html('');
                for (var i = 0; i < steps.length; i++) {
                    var step = steps[i];
                    var stepElement = $('<div/>')
                        .html(step.message)
                        .addClass('spec-step');
                    if (step.status)
                        stepElement.addClass(this.statusClass(step.status));
                    else
                        stepElement.addClass('log');
                    if (step.trace) {
                        $('<div/>')
                            .html(step.trace.replace(/\n/g, '<br/>'))
                            .addClass('spec-step-trace')
                            .appendTo(stepElement);
                    }
                    stepElement.appendTo(stepsElement);
                }
            } else {
                if (stepsElement.length)
                    stepsElement.remove();
            }
        }

        private updateSuiteStatus(suite: ISuite, specStatus: SpecStatus) {
            if (suite) {
                var suiteElement = $('#suite-' + suite.id);
                if (suiteElement) {
                    suite.status = this.investigateSuiteStatus(suiteElement.children('.suite-content'));
                    this.setSuiteElementClass(suiteElement, suite.status);
                }
                this.updateSuiteStatus(suite.parentSuite, specStatus);
            } else {
                var suiteElement = this.getSpecResultsElement();
                if (suiteElement) {
                    this.totalStatus = this.investigateSuiteStatus(suiteElement);
                    this.setSuiteElementClass(suiteElement, this.totalStatus);
                }
            }
        }

        private investigateSuiteStatus(suiteContentElement: JQuery) {
            if (suiteContentElement.children('.running').length)
                return AllGreen.SpecStatus.Running;
            if (suiteContentElement.children('.failed').length)
                return AllGreen.SpecStatus.Failed;
            if (suiteContentElement.children('.undefined').length)
                return AllGreen.SpecStatus.Undefined;
            if (suiteContentElement.children('.passed').length)
                return AllGreen.SpecStatus.Passed;
            return AllGreen.SpecStatus.Skipped;
        }

        private setSuiteElementClass(element: JQuery, status: SpecStatus) {
            element.prop('class', 'suite ' + this.statusClass(status));
        }

        private getSuiteContentElement(suite: ISuite) {
            if (suite) {
                var suiteElement = $('#suite-' + suite.id + ' > .suite-content');
                if (suiteElement.length <= 0) {
                    suiteElement = this.createSuiteElement(suite).children('.suite-content');
                }

                return suiteElement;
            } else {
                return this.getSpecResultsElement();
            }
        }

        private createSuiteElement(suite: ISuite) {
            var parentElement = this.getSuiteContentElement(suite.parentSuite);
            var suiteElement = $('<div/>')
                .addClass('suite')
                .prop('id', 'suite-' + suite.id);
            $('<a/>')
                .prop('href', '#')
                .html(suite.name)
                .addClass('suite-name')
                .click(function () { $('#suite-' + suite.id + ' > .suite-content').toggle(); })
                .appendTo(suiteElement);
            $('<div/>')
                .addClass('suite-content')
                .appendTo(suiteElement);
            suiteElement.appendTo(parentElement);
            return suiteElement;
        }

        private statusClass(status: SpecStatus): string {
            var statusName: string = SpecStatus[status];
            return statusName.toLowerCase();
        }
    }

    export function initializeDefaultReporters(app: App) {
        app.log('registering reporter factory');
        app.setServerReporter(new AllGreen.ServerReporter());
        app.registerRunnerReporter(new AllGreen.RunnerReporter());
    }
}
