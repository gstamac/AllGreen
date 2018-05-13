/// <reference path="../Scripts/typings/jasmine/jasmine.d.ts" />

describe("Suite 1", () => {
    it("Test 1", () => {
        expect(true).not.toBe(false);
    });

    it("Test 2", () => {
        expect(true).not.toBe(false);
        expect(true).toBe(true);
    });

    it("Test 3", () => {
        expect(true).not.toBe(false);
        expect(true).toBe(false);
    });
});

describe("Suite 2", () => {
    it("Test 1", () => {
        expect(true).not.toBe(false);
    });

    function level1() {
        return level2();
    }

    function level2() {
        throw new Error('Something Went Terribly Wrong');
    }

    describe("Suite 3", () => {
        it("Test 2", () => {
            runs(() => { expect(level1()).toBe(false); });
            runs(() => { expect(true).not.toBe(false); });
            runs(() => { expect(true).toBe(false); });
        });

        it("Test 3", () => {
            expect(true).not.toBe(false); expect(true).not.toBe(false); expect(true).not.toBe(false); expect(true).toBe(false);
        });
    });
});
