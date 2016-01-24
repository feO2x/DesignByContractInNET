using System;
using System.Collections;
using System.Linq;
using FluentAssertions;
using Xunit;
using DbcProductionCode;
using TestData = System.Collections.Generic.IEnumerable<object[]>;

namespace DbcAutomatedTests
{
    public class ListTests
    {
        [Theory]
        [MemberData("CountTestDataForAdd")]
        public void CountReflectsTheNumberOfItemsThatWereAddedToTheList(int[] itemsToAdd)
        {
            var testTarget = new ListBuilder<int>().WithItems(itemsToAdd)
                                                   .Build();

            testTarget.Count.Should().Be(itemsToAdd.Length);
        }

        public static readonly TestData CountTestDataForAdd =
            new[]
            {
                new object[] {new[] {1, 2, 3, 4}},
                new object[] {new[] {33, 22, 11}},
                new object[] {new[] {76, 103, 105, 87, 66, 53, 89}}
            };

        [Theory]
        [MemberData("IndexTestDataForAdd")]
        public void ItemsThatAreAddedMustBeRetrievableViaIndex<T>(T[] itemsToBeAdded,
                                                                  int index,
                                                                  T expected)
        {
            var testTarget = new ListBuilder<T>().WithItems(itemsToBeAdded)
                                                 .Build();

            testTarget[index].Should().Be(expected);
        }

        public static readonly TestData IndexTestDataForAdd =
            new[]
            {
                new object[] {new[] {1, 2, 3}, 1, 2},
                new object[] {new[] {"Hello", "World"}, 0, "Hello"}
            };

        [Fact]
        public void InternalArrayMustBeResizedAutomaticallyWhenExceedingItsCapacity()
        {
            const int initialCapacity = 4;
            var testTarget = new List<int>(initialCapacity);
            testTarget.Capacity.Should().Be(initialCapacity);

            var random = new Random();
            for (var i = 0; i < 5; i++)
            {
                testTarget.Add(random.Next());
            }

            testTarget.Capacity.Should().BeGreaterThan(initialCapacity);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(0)]
        [InlineData(-42)]
        public void ConstructorMustThrowExceptionWhenInitialCapacityIsLessThanTwo(int initialCapacity)
        {
            // ReSharper disable once ObjectCreationAsStatement
            Action act = () => new List<int>(initialCapacity);

            act.ShouldThrow<ArgumentException>();
        }

        [Theory]
        [MemberData("ForeachTestData")]
        public void TestTargetMustBeIterableWithForeachLoop<T>(T[] items)
        {
            var testTarget = new ListBuilder<T>().WithItems(items)
                                                 .Build();

            var index = 0;
            foreach (var item in testTarget)
            {
                items[index].Should().Be(item);
                index++;
            }
        }

        [Theory]
        [MemberData("ForeachTestData")]
        public void TestTargetMustBeIterableWithIEnumerableInterface<T>(T[] items)
        {
            var testTarget = new ListBuilder<T>().WithItems(items)
                                                 .Build();

            var castedTestTarget = (IEnumerable) testTarget;

            var index = 0;
            foreach (T item in castedTestTarget)
            {
                items[index].Should().Be(item);
                index++;
            }
        }

        public static readonly TestData ForeachTestData =
            new[]
            {
                new object[] {new[] {"Hello", "World", "Foo"}},
                new object[] {new[] {new object(), new object()}}
            };

        [Fact]
        public void TestTargetImplementsListOfT()
        {
            typeof (List<>).Should().Implement(typeof (System.Collections.Generic.IList<>));
        }

        [Fact]
        public void IsReadOnlyMustReturnFalse()
        {
            new List<int>().IsReadOnly.Should().Be(false);
        }

        [Theory]
        [MemberData("IndexSetTestData")]
        public void ItemCanBeExchangedViaTheIndexOperator<T>(T[] items, int index, T newItem)
        {
            var testTarget = new ListBuilder<T>().WithItems(items)
                                                 .Build();

            testTarget[index] = newItem;

            testTarget[index].Should().Be(newItem);
        }

        public static readonly TestData IndexSetTestData =
            new[]
            {
                new object[] {new[] {"Hello", "There"}, 1, "World"},
                new object[] {new object[] {1, 2, 3, 4}, 3, 87}
            };

        [Theory]
        [MemberData("IndexSetAtEndTestData")]
        public void ItemIsAddedToTheEndOfTheCollectionWhenIndexIsEqualToCount<T>(T[] items, T newItem)
        {
            var testTarget = new ListBuilder<T>().WithItems(items)
                                                 .Build();

            testTarget[testTarget.Count] = newItem;

            var expected = items.Concat(new[] {newItem})
                                .ToList();
            testTarget.ShouldBeEquivalentTo(expected);
        }

        public static readonly TestData IndexSetAtEndTestData =
            new[]
            {
                new object[] {new[] {"Foo", "Bar", "Baz"}, "Qux"},
                new object[] {new object[] {1, 2, 3, 4, 5}, 87}
            };

        [Theory]
        [InlineData(-1)]
        [InlineData(-10)]
        [InlineData(2)]
        [InlineData(5)]
        public void ExceptionIsThrownWhenInvalidIndexIsSpecifiedOnSet(int invalidIndex)
        {
            Action act = () => new List<int>()[invalidIndex] = 42;

            act.ShouldThrow<IndexOutOfRangeException>();
        }

        [Theory]
        [MemberData("ClearTestData")]
        public void ClearEmptiesTheTestTarget<T>(T[] items)
        {
            var testTarget = new ListBuilder<T>().WithItems(items)
                                                 .Build();

            testTarget.Clear();

            testTarget.Should().BeEmpty();
        }

        public static readonly TestData ClearTestData =
            new[]
            {
                new object[] {new object[] {1, 2, 3, 4,}},
                new object[] {new object[] {}},
                new object[] {new[] {"Hello", "There"}}
            };


        [Fact]
        public void TestTargetMustNotHoldAReferenceToItemsAfterCallingClear()
        {
            var item = new object();
            var weakReferenceToItem = new WeakReference<object>(item);
            var testTarget = new ListBuilder<object>().WithItems(item)
                                                      .Build();

            testTarget.Clear();

            // ReSharper disable once RedundantAssignment
            item = null;
            GC.Collect();
            object retrievedItem;
            weakReferenceToItem.TryGetTarget(out retrievedItem).Should().Be(false);
            retrievedItem.Should().BeNull();
        }

        [Fact]
        public void InitialCapacityMustBe4ByDefault()
        {
            new List<string>().Capacity.Should().Be(4);
        }

        [Theory]
        [MemberData("IndexOfTestData")]
        public void IndexOfReturnsValidIndexOrMinusOneWhenItemCannotBeFound<T>(T[] items,
                                                                               T itemBeingSearchedFor,
                                                                               int expectedIndex)
        {
            var testTarget = new ListBuilder<T>().WithItems(items)
                                                 .Build();

            var actualIndex = testTarget.IndexOf(itemBeingSearchedFor);

            actualIndex.Should().Be(expectedIndex);
        }

        public static readonly TestData IndexOfTestData =
            new[]
            {
                new object[] {new[] {"Hello", "World"}, "World", 1},
                new object[] {new[] {"Hello", "World"}, "Foo", -1},
                new object[] {new object[] {1, 2, 3, 4, 3}, 3, 2},
                new object[] {new[] {null, new object()}, null, 0}
            };

        [Theory]
        [MemberData("InsertTestData")]
        public void InsertMovesExistingItemsAndInsertsNewOneCorrectly<T>(T[] items,
                                                                         int index,
                                                                         T newItem,
                                                                         T[] expected)
        {
            var testTarget = new ListBuilder<T>().WithItems(items)
                                                 .Build();

            testTarget.Insert(index, newItem);

            testTarget.ShouldBeEquivalentTo(expected);
        }

        public static readonly TestData InsertTestData =
            new[]
            {
                new object[] {new[] {"1", "2", "3"}, 1, "87", new[] {"1", "87", "2", "3"}},
                new object[] {new[] {"1", "3", "5", "7"}, 4, "42", new[] {"1", "3", "5", "7", "42"}}
            };

        [Theory]
        [InlineData(-1)]
        [InlineData(-42)]
        [InlineData(2)]
        [InlineData(8)]
        public void InsertThrowsExceptionWhenInvalidIndexIsSpecified(int invalidIndex)
        {
            Action act = () => new List<string>().Insert(invalidIndex, null);

            act.ShouldThrow<ArgumentOutOfRangeException>();
        }

        [Theory]
        [MemberData("RemoveAtTestData")]
        public void RemoveAtRemovesItemsFromTheCollectionCorrectly<T>(T[] items,
                                                                      int index,
                                                                      T[] expected)
        {
            var testTarget = new ListBuilder<T>().WithItems(items)
                                                 .Build();

            testTarget.RemoveAt(index);

            testTarget.ShouldBeEquivalentTo(expected);
        }

        public static readonly TestData RemoveAtTestData =
            new[]
            {
                new object[] {new[] {"Foo", "Bar", "Baz"}, 1, new[] {"Foo", "Baz"}},
                new object[] {new object[] {1, 2, 3, 4, 5}, 3, new object[] {1, 2, 3, 5}},
                new object[] {new[] {"Hello", "World", "Foo", "What's up?"}, 3, new[] {"Hello", "World", "Foo"}}
            };

        [Theory]
        [InlineData(-1)]
        [InlineData(-42)]
        [InlineData(2)]
        [InlineData(10)]
        public void RemoveAtThrowsExceptionWhenInvalidIndexIsPassed(int invalidIndex)
        {
            var testTarget = new List<string>();

            Action act = () => testTarget.RemoveAt(invalidIndex);

            act.ShouldThrow<ArgumentOutOfRangeException>();
        }

        [Theory]
        [MemberData("ContainsTestData")]
        public void ContainsRetrievesItemsCorrectly<T>(T[] items, T itemBeingSearchedFor, bool expected)
        {
            var testTarget = new ListBuilder<T>().WithItems(items)
                                                 .Build();

            var actual = testTarget.Contains(itemBeingSearchedFor);

            actual.Should().Be(expected);
        }

        public static readonly TestData ContainsTestData =
            new[]
            {
                new object[] {new[] {"1", "2", "3"}, "1", true},
                new object[] {new object[] {1, 2, 3, 4, 5}, 42, false},
                new object[] {new object[] {33, 42, 27, 42, 55}, 42, true},
                new object[] {new object[] {1, 2, 3, 4}, 4, true},
                new object[] {new[] {new object(), null}, null, true}
            };

        [Theory]
        [MemberData("RemoveTestData")]
        public void RemoveWorksCorrectly<T>(T[] items, T itemBeingRemoved, T[] expectedCollection, bool expectedResult)
        {
            var testTarget = new ListBuilder<T>().WithItems(items)
                                                 .Build();

            var actualResult = testTarget.Remove(itemBeingRemoved);

            testTarget.ShouldBeEquivalentTo(expectedCollection);
            actualResult.Should().Be(expectedResult);
        }

        public static readonly TestData RemoveTestData =
            new[]
            {
                new object[] {new[] {"1", "2", "3"}, "3", new[] {"1", "2"}, true},
                new object[] {new object[] {33, 44, 55}, 66, new object[] {33, 44, 55}, false},
                new object[] {new object[] {false, true, false, false}, false, new object[] {true, false, false}, true}
            };

        [Fact]
        public void CopyToThrowsExceptionWhenArrayIsNull()
        {
            var testTarget = new List<object>();

            // ReSharper disable once AssignNullToNotNullAttribute
            Action act = () => testTarget.CopyTo(null, 0);

            act.ShouldThrow<ArgumentNullException>();
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(-42)]
        [InlineData(-3)]
        public void CopyToThrowsExceptionWhenIndexIsNull(int invalidIndex)
        {
            var testTarget = new List<object>();

            Action act = () => testTarget.CopyTo(new object[4], invalidIndex);

            act.ShouldThrow<ArgumentOutOfRangeException>();
        }

        [Theory]
        [MemberData("CopyToTestData")]
        public void CopyToCopiesElementsToTargetArrayCorrectly<T>(T[] items,
                                                                  T[] targetArray,
                                                                  int arrayIndex,
                                                                  T[] expectedArray)
        {
            var testTarget = new ListBuilder<T>().WithItems(items)
                                                 .Build();

            testTarget.CopyTo(targetArray, arrayIndex);

            targetArray.ShouldBeEquivalentTo(expectedArray);
        }

        public static readonly TestData CopyToTestData =
            new[]
            {
                new object[] {new[] {"1", "2"}, new string[2], 0, new[] {"1", "2"}},
                new object[] {new[] {"1", "2"}, new string[4], 0, new[] {"1", "2", null, null}},
                new object[] {new object[] {3, 4, 5}, new object[] {1, 2, 0, 0, 0}, 2, new object[] {1, 2, 3, 4, 5}}
            };

        [Theory]
        [MemberData("CopyToTargetArrayTooSmallTestData")]
        public void CopyToThrowsExceptionWhenTargetArrayIsTooSmall<T>(T[] items,
                                                                      T[] targetArray,
                                                                      int arrayIndex)
        {
            var testTarget = new ListBuilder<T>().WithItems(items)
                                                 .Build();

            Action act = () => testTarget.CopyTo(targetArray, arrayIndex);

            act.ShouldThrow<ArgumentException>();
        }

        public static readonly TestData CopyToTargetArrayTooSmallTestData =
            new[]
            {
                new object[] {new object[] {1, 2, 3}, new object[] {1}, 0},
                new object[] {new[] {"Foo", "Bar"}, new[] {"1", "2", null}, 2}
            };
    }
}