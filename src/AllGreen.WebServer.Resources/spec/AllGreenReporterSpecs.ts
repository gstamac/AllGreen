/// <reference path="../Scripts/typings/jasmine/jasmine.d.ts" />
/// <reference path="../Scripts/typings/jasmine-jquery/jasmine-jquery.d.ts" />
/// <reference path="../Client/reporter.ts" />

declare module jasmine {
    interface Matchers {
        toBeSpecElement(specName, statusClass): boolean;
        toBeSuiteElement(suiteName, statusClass): boolean;
    }
}

describe("AllGreen.RunnerReporter", () => {
    beforeEach(() => {
        jasmine.getEnv().currentSpec.addMatchers({
            toBeSpecElement: function (specName, statusClass) {
                var actual = this.actual;

                expect(actual).toHaveLength(1);
                expect(actual).toBe('div');
                expect(actual).toHaveClass('spec');
                expect(actual).toHaveClass(statusClass);

                var specNameElement = actual.children('.spec-name');
                expect(specNameElement).toHaveLength(1);
                expect(specNameElement).toBe('a');
                expect(specNameElement).toHaveText(specName);
                expect(specNameElement).toHaveAttr('href', '#');

                var spyEvent = spyOnEvent(specNameElement, 'click');
                specNameElement.click();
                expect(spyEvent).toHaveBeenTriggered();

                return true;
            },

            toBeSuiteElement: function (suiteName, statusClass) {
                var actual = this.actual;

                expect(actual).toHaveLength(1);
                expect(actual).toBe('div');
                expect(actual).toHaveClass('suite');
                expect(actual).toHaveClass(statusClass);

                var suiteNameElement = actual.children('.suite-name');
                expect(suiteNameElement).toHaveLength(1);
                expect(suiteNameElement).toBe('a');
                expect(suiteNameElement).toHaveText(suiteName);
                expect(suiteNameElement).toHaveAttr('href', '#');

                var spyEvent = spyOnEvent(suiteNameElement, 'click');
                suiteNameElement.click();
                expect(spyEvent).toHaveBeenTriggered();

                var suiteContentElement = actual.children('.suite-content');
                expect(suiteContentElement).toHaveLength(1);
                expect(suiteContentElement).toBe('div');

                return true;
            }
        });

        jasmine.getFixtures().fixturesPath = '';
        var clientHtml = readFixtures('Client/client.html');
        expect(clientHtml).not.toBe('');
        clientHtml = clientHtml.replace(/~internal~\//g, '');
        clientHtml = clientHtml.replace(/<script[^>]*><\/script>/g, '');
        clientHtml = clientHtml.replace(/<script[^>]*>[^<]*new AllGreen\.App()[^<]*<\/script>/g, '');
        setFixtures(clientHtml);

        this.serverReporter = new AllGreen.ServerReporter();
        this.runnerReporter = new AllGreen.RunnerReporter();

        this.suite1 = { id: 444, name: 'suite 1', parentSuite: null };
    });

    afterEach(() => {
        this.serverReporter = null;
        this.runnerReporter = null;
    });

    it("Is Default", () => {
        expect($('#server-status')).toHaveHtml('Connecting...');
        expect($('#runner-status')).toHaveHtml('Waiting...');
        expect($('#spec-results')).toBeEmpty();
    });
    
    it("Should be read", () => {
        expect(this.runnerReporter.isReady()).toBeTruthy();
    });

    it("Can be reset", () => {
        this.runnerReporter.started();
        this.runnerReporter.specUpdated({ id: 123, name: 'test 1', suite: this.suite1, status: AllGreen.SpecStatus.Passed });
        this.runnerReporter.reset();
        expect($('#runner-status')).toHaveHtml('Waiting...');
        expect($('#spec-results')).toBeEmpty();
    });

    it("Updates server status", () => {
        this.serverReporter.setServerStatus('NEW SERVER STATUS');
        expect($('#server-status')).toHaveHtml('NEW SERVER STATUS');
    });

    it("Updates runner status", () => {
        this.runnerReporter.started(20);
        expect($('#runner-status')).toHaveHtml('Running 20 tests...');
    });

    it("Displays spec status", () => {
        this.runnerReporter.specUpdated({ id: 123, name: 'test 1', suite: this.suite1, status: AllGreen.SpecStatus.Passed });
        expect(this.suite1.status).toBe(AllGreen.SpecStatus.Passed);
        expect($('#spec-results > #suite-444 > .suite-content > #spec-123')).toBeSpecElement('test 1', 'passed');
        expect($('#spec-results > #suite-444')).toBeSuiteElement('suite 1', 'passed');
        expect($('#spec-results')).toHaveClass('passed');
    });
    
    it("Displays failed spec steps", () => {
        var steps = [
            { message: 'log message' },
            { message: 'expected something and succeded', status: AllGreen.SpecStatus.Passed },
            { message: 'expected something and failed', status: AllGreen.SpecStatus.Failed, trace: 'trace 1\ntrace 2' }
        ];
        this.runnerReporter.specUpdated({ id: 123, name: 'test 1', suite: this.suite1, status: AllGreen.SpecStatus.Failed, steps: steps });
        expect($('#spec-results > #suite-444 > .suite-content > #spec-123')).toBeSpecElement('test 1', 'failed');
        expect($('#spec-results > #suite-444 > .suite-content > #spec-123 > .spec-steps > .spec-step')).toHaveLength(3);
        expect($('#spec-results > #suite-444 > .suite-content > #spec-123 > .spec-steps > .spec-step.log')).toHaveLength(1);
        expect($('#spec-results > #suite-444 > .suite-content > #spec-123 > .spec-steps > .spec-step.passed')).toHaveLength(1);
        expect($('#spec-results > #suite-444 > .suite-content > #spec-123 > .spec-steps > .spec-step.failed')).toHaveLength(1);
        expect($('#spec-results > #suite-444 > .suite-content > #spec-123 > .spec-steps > .spec-step.failed > .spec-step-trace')).toHaveHtml('trace 1<br/>trace 2');
    });

    it("Updates spec status", () => {
        this.runnerReporter.specUpdated({ id: 123, name: 'test 1', suite: this.suite1, status: AllGreen.SpecStatus.Passed });
        this.runnerReporter.specUpdated({ id: 123, name: 'test 1', suite: this.suite1, status: AllGreen.SpecStatus.Failed });
        expect(this.suite1.status).toBe(AllGreen.SpecStatus.Failed);
        expect($('#spec-results > #suite-444 > .suite-content > #spec-123')).toBeSpecElement('test 1', 'failed');
        expect($('#spec-results > #suite-444')).toBeSuiteElement('suite 1', 'failed');
        expect($('#spec-results')).toHaveClass('failed');
    });

    it("Updates suite status", () => {
        this.runnerReporter.specUpdated({ id: 123, name: 'test 1', suite: this.suite1, status: AllGreen.SpecStatus.Passed });
        this.runnerReporter.specUpdated({ id: 323, name: 'test 2', suite: this.suite1, status: AllGreen.SpecStatus.Failed });
        expect(this.suite1.status).toBe(AllGreen.SpecStatus.Failed);
        expect($('#spec-results > #suite-444 > .suite-content > #spec-123')).toBeSpecElement('test 1', 'passed');
        expect($('#spec-results > #suite-444 > .suite-content > #spec-323')).toBeSpecElement('test 2', 'failed');
        expect($('#spec-results > #suite-444')).toBeSuiteElement('suite 1', 'failed');
        expect($('#spec-results')).toHaveClass('failed');
    });

    it("Supports nested suites", () => {
        var suite2 = {
            id: 744, name: 'suite 2', parentSuite: this.suite1, status: AllGreen.SpecStatus.Undefined
        };
        this.runnerReporter.specUpdated({ id: 123, name: 'test 1', suite: suite2, status: AllGreen.SpecStatus.Passed });
        this.runnerReporter.specUpdated({ id: 323, name: 'test 2', suite: suite2, status: AllGreen.SpecStatus.Failed });
        expect(this.suite1.status).toBe(AllGreen.SpecStatus.Failed);
        expect(suite2.status).toBe(AllGreen.SpecStatus.Failed);
        expect($('#spec-results > #suite-444 > .suite-content > #suite-744 > .suite-content > #spec-123')).toBeSpecElement('test 1', 'passed');
        expect($('#spec-results > #suite-444 > .suite-content > #suite-744 > .suite-content > #spec-323')).toBeSpecElement('test 2', 'failed');
        expect($('#spec-results > #suite-444 > .suite-content > #suite-744')).toBeSuiteElement('suite 2', 'failed');
        expect($('#spec-results > #suite-444')).toBeSuiteElement('suite 1', 'failed');
        expect($('#spec-results')).toHaveClass('failed');
    });

    it("Shows final suite status", () => {
        this.runnerReporter.specUpdated({ id: 123, name: 'test 1', suite: this.suite1, status: AllGreen.SpecStatus.Running });
        this.runnerReporter.specUpdated({ id: 223, name: 'test 2', suite: this.suite1, status: AllGreen.SpecStatus.Running });
        expect(this.suite1.status).toBe(AllGreen.SpecStatus.Running);
        expect($('#spec-results > #suite-444')).toBeSuiteElement('suite 1', 'running');
        expect($('#spec-results')).toHaveClass('running');
        this.runnerReporter.specUpdated({ id: 123, name: 'test 1', suite: this.suite1, status: AllGreen.SpecStatus.Passed });
        expect(this.suite1.status).toBe(AllGreen.SpecStatus.Running);
        expect($('#spec-results > #suite-444')).toBeSuiteElement('suite 1', 'running');
        expect($('#spec-results')).toHaveClass('running');
        this.runnerReporter.specUpdated({ id: 223, name: 'test 2', suite: this.suite1, status: AllGreen.SpecStatus.Failed });
        expect(this.suite1.status).toBe(AllGreen.SpecStatus.Failed);
        expect($('#spec-results > #suite-444')).toBeSuiteElement('suite 1', 'failed');
        expect($('#spec-results')).toHaveClass('failed');
    });
});