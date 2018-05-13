/// <reference path="../Scripts/typings/jasmine/jasmine.d.ts" />
describe("Suite 1", function () {
    it("Test 1", function () {
        expect(true).not.toBe(false);
    });

    it("Test 2", function () {
        expect(true).not.toBe(false);
        expect(true).toBe(true);
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

    function level1() {
        return level2();
    }

    function level2() {
        throw new Error('Something Went Terribly Wrong');
    }

    describe("Suite 3", function () {
        it("Test 2", function () {
            runs(function () {
                expect(level1()).toBe(false);
            });
            runs(function () {
                expect(true).not.toBe(false);
            });
            runs(function () {
                expect(true).toBe(false);
            });
        });

        it("Test 3", function () {
            expect(true).not.toBe(false);
            expect(true).not.toBe(false);
            expect(true).not.toBe(false);
            expect(true).toBe(false);
        });
    });
});
//# sourceMappingURL=testScript.js.map
