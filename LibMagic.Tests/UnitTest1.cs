using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LibMagic.Tests
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            foreach (var combination in LandCombinations(17, 3))
            {
                Assert.AreEqual(17, combination.Sum());
                Debug.WriteLine(string.Join(",", combination));
            }
        }

        [TestMethod]
        public void TestMethod2()
        {
            foreach (var combination in CardCombinations(23, 11))
            {
                Assert.AreEqual(23, combination[0] * 4 + combination[1] * 3 + combination[2] * 2 + combination[3] * 1);
                Assert.AreEqual(11, combination.Sum());
                var combinationText = string.Join(",", combination);

                var combinationArray = CombinationArray(combination, 11);
                Assert.AreEqual(23, combinationArray.Sum());

                var combinationArrayText = string.Join(",", combinationArray);
                Debug.WriteLine($"{combinationText} => {combinationArrayText} => {combination.Sum()}");
            }
        }

        private static int[] CombinationArray(int[] combination, int uniqueCards)
        {
            var combinationArray = new int[uniqueCards];
            var k = 0;
            var c = 4;
            for (var i = 0; i < combination.Length; i++)
            {
                for (var j = 0; j < combination[i]; j++)
                {
                    combinationArray[k++] = c;
                }

                c--;
            }

            return combinationArray;
        }

        public static IEnumerable<int[]> CardCombinations(int totalCards, int uniqueRemaining, int copies = 4)
        {
            if (copies == 1)
            {
                if (uniqueRemaining == totalCards)
                {
                    yield return new[] {totalCards};
                }

                yield break;
            }

            var max = totalCards / copies;
            for (var i = 0; i <= max; i++)
            {
                var remaining = totalCards - i * copies;
                foreach (var subCombination in CardCombinations(remaining, uniqueRemaining - i, copies - 1))
                {
                    var combination = new int[copies];
                    combination[0] = i;
                    for (var j = 1; j < copies; j++)
                    {
                        combination[j] = subCombination[j - 1];
                    }

                    yield return combination;
                }
            }
        }

        public static IEnumerable<int[]> LandCombinations(int total, int colors)
        {
            if (colors == 1)
            {
                yield return  new[] {total};
                yield break;
            }

            for (var i = 0; i <= total; i++)
            {
                foreach (var subCombination in LandCombinations(total - i, colors - 1))
                {
                    var combination = new int[colors];
                    combination[0] = i;
                    for (var j = 1; j < colors; j++)
                    {
                        combination[j] = subCombination[j - 1];
                    }

                    yield return combination;
                }
            }
        }
    }
}
