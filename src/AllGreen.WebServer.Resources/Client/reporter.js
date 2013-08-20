/// <reference path="allgreen.ts" />
/// <reference path="../Scripts/typings/jquery/jquery.d.ts" />
var AllGreen;
(function (AllGreen) {
    (function (SpecStatus) {
        SpecStatus[SpecStatus["Running"] = 0] = "Running";
        SpecStatus[SpecStatus["Failed"] = 1] = "Failed";
        SpecStatus[SpecStatus["Undefined"] = 2] = "Undefined";
        SpecStatus[SpecStatus["Passed"] = 3] = "Passed";
        SpecStatus[SpecStatus["Skipped"] = 4] = "Skipped";
    })(AllGreen.SpecStatus || (AllGreen.SpecStatus = {}));
    var SpecStatus = AllGreen.SpecStatus;

    var Reporter = (function () {
        function Reporter() {
            this.DEFAULT_SERVER_STATUS = 'Connecting...';
            this.DEFAULT_RUNNER_STATUS = 'Waiting...';
            this.totalStatus = null;
        }
        Reporter.prototype.reset = function () {
            this.setServerStatus(this.DEFAULT_SERVER_STATUS);
            this.setRunnerStatus(this.DEFAULT_RUNNER_STATUS);
            this.getSpecResultsElement().text('');
            this.totalStatus = null;
        };

        Reporter.prototype.setServerStatus = function (status) {
            this.getServerStatusElement().html(status);
        };

        Reporter.prototype.setRunnerStatus = function (status) {
            this.getRunnerStatusElement().html(status);
        };

        Reporter.prototype.setSpecStatus = function (spec) {
            var specElement = this.getSpecElement(spec);
            this.setSpecElementClass(specElement, spec.status);
            this.setSpecElementSteps(specElement, spec.steps);
            this.updateSuiteStatus(spec.suite, spec.status);
        };

        Reporter.prototype.getServerStatusElement = function () {
            return $('#server-status');
        };

        Reporter.prototype.getRunnerStatusElement = function () {
            return $('#runner-status');
        };

        Reporter.prototype.getSpecResultsElement = function () {
            return $('#spec-results');
        };

        Reporter.prototype.getSpecElement = function (spec) {
            var specElement = $('#spec-' + spec.id);
            if (specElement.length <= 0) {
                specElement = this.createSpecElement(spec);
            }
            return specElement;
        };

        Reporter.prototype.createSpecElement = function (spec) {
            var suiteElement = this.getSuiteContentElement(spec.suite);
            var specElement = $('<div/>').addClass('spec').prop('id', 'spec-' + spec.id);
            $('<a/>').prop('href', '#').html(spec.name).addClass('spec-name').click(function () {
                $('#spec-' + spec.id + ' > .spec-steps').toggle();
            }).appendTo(specElement);

            specElement.appendTo(suiteElement);
            return specElement;
        };

        Reporter.prototype.setSpecElementClass = function (element, status) {
            element.prop('class', 'spec ' + this.statusClass(status));
        };

        Reporter.prototype.setSpecElementSteps = function (specElement, steps) {
            var stepsElement = specElement.children('.spec-steps');
            if (steps && steps.length) {
                if (!stepsElement.length) {
                    stepsElement = $('<div/>').addClass('spec-steps');
                    stepsElement.appendTo(specElement);
                }
                stepsElement.html('');
                for (var i = 0; i < steps.length; i++) {
                    var step = steps[i];
                    var stepElement = $('<div/>').html(step.message).addClass('spec-step');
                    if (step.status)
                        stepElement.addClass(this.statusClass(step.status));
else
                        stepElement.addClass('log');
                    if (step.trace) {
                        $('<div/>').html(step.trace.replace(/\n/g, '<br/>')).addClass('spec-step-trace').appendTo(stepElement);
                    }
                    stepElement.appendTo(stepsElement);
                }
            } else {
                if (stepsElement.length)
                    stepsElement.remove();
            }
        };

        Reporter.prototype.updateSuiteStatus = function (suite, specStatus) {
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
        };

        Reporter.prototype.investigateSuiteStatus = function (suiteContentElement) {
            if (suiteContentElement.children('.running').length)
                return AllGreen.SpecStatus.Running;
            if (suiteContentElement.children('.failed').length)
                return AllGreen.SpecStatus.Failed;
            if (suiteContentElement.children('.undefined').length)
                return AllGreen.SpecStatus.Undefined;
            if (suiteContentElement.children('.passed').length)
                return AllGreen.SpecStatus.Passed;
            return AllGreen.SpecStatus.Skipped;
        };

        Reporter.prototype.setSuiteElementClass = function (element, status) {
            element.prop('class', 'suite ' + this.statusClass(status));
        };

        Reporter.prototype.getSuiteContentElement = function (suite) {
            if (suite) {
                var suiteElement = $('#suite-' + suite.id + ' > .suite-content');
                if (suiteElement.length <= 0) {
                    suiteElement = this.createSuiteElement(suite).children('.suite-content');
                }

                return suiteElement;
            } else {
                return this.getSpecResultsElement();
            }
        };

        Reporter.prototype.createSuiteElement = function (suite) {
            var parentElement = this.getSuiteContentElement(suite.parentSuite);
            var suiteElement = $('<div/>').addClass('suite').prop('id', 'suite-' + suite.id);
            $('<a/>').prop('href', '#').html(suite.name).addClass('suite-name').click(function () {
                $('#suite-' + suite.id + ' > .suite-content').toggle();
            }).appendTo(suiteElement);
            $('<div/>').addClass('suite-content').appendTo(suiteElement);
            suiteElement.appendTo(parentElement);
            return suiteElement;
        };

        Reporter.prototype.statusClass = function (status) {
            var statusName = SpecStatus[status];
            return statusName.toLowerCase();
        };
        return Reporter;
    })();
    AllGreen.Reporter = Reporter;
})(AllGreen || (AllGreen = {}));

(function () {
    var app = AllGreen.App.getCurrent();
    if (app != null) {
        console.log('registering reporter factory');
        app.setReporter(new AllGreen.Reporter());
    }
})();
