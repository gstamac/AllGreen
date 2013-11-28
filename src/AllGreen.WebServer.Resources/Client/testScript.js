
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
            runs(function () {
                expect(true).toBe(false);
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
