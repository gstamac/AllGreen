
describe("Suite 1", function () {
    it("Test 1", function () {
        expect(true).not.toBe(false);
    });

    it("Test 2", function () {
        expect(true).not.toBe(false);
        expect(true).toBe(false);
    });

    it("Test 3", function () {
        expect(true).not.toBe(false);
        expect(true).toBe(false);
    });
});

describe("Suite 2", function () {
    it("Test 1", function () {
        expect(true).not.toBe(false);
    });

    describe("Suite 3", function () {
        it("Test 2", function () {
            expect(true).not.toBe(false);
            expect(true).not.toBe(false);
            expect(true).toBe(false);
        });

        it("Test 3", function () {
            expect(true).not.toBe(false);
            expect(true).not.toBe(false);
            expect(true).not.toBe(false);
            expect(true).toBe(false);
        });
    });
});

/*allGreen.reporter.setHubStatus('Started');
allGreen.reporter.setRunnerStatus('Finished');
*/
/*var suite1 = { id: 444, name: 'suite 1', parent_suite: null };

allGreen.reporter.showSpec({
    id: 123,
    name: 'test 1',
    suite: suite1,
    status: allgreen.SpecStatus.Running
});

allGreen.reporter.showSpec({
    id: 244,
    name: 'test 2',
    suite: suite1,
    status: allgreen.SpecStatus.Undefined
});

var suite2 = { id: 844, name: 'suite 1', parent_suite: null };

allGreen.reporter.showSpec({
    id: 43,
    name: 'test 1',
    suite: suite2,
    status: allgreen.SpecStatus.Undefined
});

allGreen.reporter.showSpec({
    id: 24244,
    name: 'test 2',
    suite: suite2,
    status: allgreen.SpecStatus.Undefined
});

allGreen.reporter.showSpec({
    id: 1323,
    name: 'test 1',
    suite: { id: 44, name: 'suite 1', parent_suite: null },
    status: allgreen.SpecStatus.Skipped
});

allGreen.reporter.showSpec({
    id: 11,
    name: 'test 1',
    suite: { id: 45, name: 'suite 45', parent_suite: null },
    status: allgreen.SpecStatus.Passed
});
allGreen.reporter.showSpec({
    id: 12,
    name: 'test 2',
    suite: { id: 45, name: 'suite 45', parent_suite: null },
    status: allgreen.SpecStatus.Passed
});
*/