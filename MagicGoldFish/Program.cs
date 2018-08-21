using System;

namespace MagicGoldFish
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.CompilerServices;

    using MagicSimulator.Tests;

    class Program
    {
        static void OldMain(string[] args)
        {
            var sway = 4;
            var cards = new[]
                            {
                                Card.Artifact(10, "Manalith", 3 * Mana.Colorless, Mana.Any),
                                Card.Artifact(10, "Dragon's Horde", 3 * Mana.Colorless, Mana.Any),
                                // W Ajani's Welcome
                                Card.Spell(1, "Ajani's Welcome", Mana.White),
                                // W Leonin Vangard
                                Card.Creature(1, "Leonin Vangard", Mana.White),
                                // 1W Ajani's Pridemate
                                Card.Creature(1, "Ajani's Pridemate", 1 * Mana.Any + 1 * Mana.White),
                                // 2WW Leonin Warleader
                                Card.Creature(1, "Leonin Warleader", 2 * Mana.Any + 2 * Mana.White),
                                // 3W Dwarven Priest
                                Card.Creature(1, "Dwarven Priest", 3 * Mana.Any + 1 * Mana.White),
                                // 1WW Resplendent Angel
                                Card.Creature(1, "Resplendent Angel", Mana.Any + 2 * Mana.White),
                                // 2W Luminous Bonds
                                Card.Spell(1, "Luminous Bonds", 2*Mana.Any + Mana.White),
                                // 1B Cast Down
                                Card.Spell(1, "Cast Down", 1 * Mana.Any + 1 * Mana.Black),
                                // 4WW Evra Halcyon Witness
                                Card.Creature(1, "Evra, Halcyon Witness", 4 * Mana.Any + 2 * Mana.White),
                                // 3WW Lyra Dawnbringer
                                Card.Creature(1, "Lyra Dawnbringer", 3 * Mana.Any + 2 * Mana.White),
                                // 3BW Regal Bloodlord
                                Card.Creature(1, "Regal Bloodlord", 3 * Mana.Any + Mana.Black + Mana.White),
                                // 2BW Aryel, Knight of Windgrace
                                Card.Creature(
                                    1,
                                    "Aryel, Knight of Windgrace",
                                    2 * Mana.Any + Mana.Black + Mana.White),
                            };
            cards = cards.OrderBy(c => c.Cost.ConvertedAmount).ThenBy(c=>c.Name).ToArray();

            var deckEvaluator = new DeckEvaluator();
            deckEvaluator.SetCardPool(cards);

            int[] bestLandCombination = null;
            int[] bestCardCombination = null;
            var bestShift = 0;
            var bestScore = 0L;

            for (var nonLandCardCount = 20; nonLandCardCount < 4 * cards.Length; nonLandCardCount++)
            {
                foreach (var combination in CardCombinations(nonLandCardCount, cards.Length))
                {
                    var combinationArray = CombinationArray(combination);
                    var needShift = combinationArray.Any(c => combinationArray[0] != c);
                    foreach (var landCombination in LandCombinations(40 - nonLandCardCount, 2))
                    {
                        var shiftCount = needShift ? cards.Length : 1;
                        for (var shift = 0; shift < shiftCount; shift++)
                        {
                            Console.Clear();
                            if (bestLandCombination != null)
                            {
                                Console.WriteLine($"{bestLandCombination[0]} Swamp");
                                Console.WriteLine($"{bestLandCombination[1]} Plains");
                                for (var k = 0; k < cards.Length; k++)
                                {
                                    var cardCount = bestCardCombination[(k + bestShift) % combinationArray.Length];
                                    var card = cards[k];
                                    Console.WriteLine($"{cardCount} {card}");
                                }

                                Console.WriteLine($"Best Score: {bestScore}");
                                Console.WriteLine();
                            }

                            var copies = new int[combination.Length];
                            for (var i = 0; i < copies.Length; i++)
                            {
                                copies[i] = combinationArray[(i + shift) % combinationArray.Length];
                            }

                            deckEvaluator.AddDeck(
                                landCombination[0] * Mana.Black + landCombination[1] * Mana.White, copies);

                            var deckBuilder = new DeckBuilder();
                            deckBuilder.Add(landCombination[0], Card.Land(10, "Swamp", Mana.Black));
                            Console.WriteLine($"{landCombination[0]} Swamp");
                            deckBuilder.Add(landCombination[1], Card.Land(10, "Plains", Mana.White));
                            Console.WriteLine($"{landCombination[1]} Plains");

                            // Add Cards
                            for (var k = 0; k < cards.Length; k++)
                            {
                                var cardCount = combinationArray[(k + shift) % combinationArray.Length];
                                var card = cards[k];
                                deckBuilder.Add(cardCount, card);
                                Console.WriteLine($"{cardCount} {card}");
                            }

                            var tries = 10L;
                            var turns = 12;
                            var deck = deckBuilder.GetDeck();

                            var score = ScoreDeck(deck, tries, turns);
                            Console.WriteLine($"{score} vs {bestScore}");
                            if (score > bestScore)
                            {
                                score = ScoreDeck(deck, 10 * tries, turns);
                                if (score > bestScore)
                                {
                                    bestLandCombination = landCombination;
                                    bestCardCombination = combinationArray;
                                    bestShift = shift;
                                    bestScore = score;
                                }
                            }
                        }
                    }
                }
            }

            /*

            deckBuilder.Add(8 + sway, Card.Land(10, "Plains", Mana.White));
            deckBuilder.Add(8 - sway, Card.Land(10, "Swamp", Mana.Black));

            // W Ajani's Welcome
            deckBuilder.Add(3, Card.Spell(1, "Ajani's Welcome", Mana.White));
            // W Leonin Vangard
            deckBuilder.Add(4, Card.Creature(1, "Leonin Vangard", Mana.White));
            // 1W Ajani's Pridemate
            deckBuilder.Add(3, Card.Creature(1, "Ajani's Pridemate", 1*Mana.Any + 1*Mana.White));
            // 2WW Leonin Warleader
            deckBuilder.Add(2, Card.Creature(1, "Leonin Warleader", 2 * Mana.Any + 2*Mana.White));
            // 3W Dwarven Priest
            deckBuilder.Add(1, Card.Creature(1, "Dwarven Priest", 3 * Mana.Any + 1*Mana.White));
            // 1WW Resplendent Angel
            deckBuilder.Add(1, Card.Creature(1, "Resplendent Angel", Mana.Any + 2*Mana.White));
            // 2W Luminous Bonds
            // deckBuilder.Add(4, Card.Spell(1, "Luminous Bonds", 2*Mana.Any + Mana.White));
            // 1B Cast Down
            deckBuilder.Add(4, Card.Spell(1, "Cast Down", 1*Mana.Any + 1*Mana.Black));
            // 4WW Evra Halcyon Witness
            deckBuilder.Add(1, Card.Creature(1, "Evra, Halcyon Witness", 4 * Mana.Any + 2*Mana.White));
            // 3WW Lyra Dawnbringer
            deckBuilder.Add(1, Card.Creature(1, "Lyra Dawnbringer", 3 * Mana.Any + 2*Mana.White));
            // 3BW Regal Bloodlord
            deckBuilder.Add(2, Card.Creature(1, "Regal Bloodlord", 3 * Mana.Any + Mana.Black + Mana.White));
            // 2BW Aryel, Knight of Windgrace
            deckBuilder.Add(2, Card.Creature(1, "Aryel, Knight of Windgrace", 2*Mana.Any + Mana.Black + Mana.White));
            */
            Console.WriteLine("Done");
            Console.ReadLine();
        }

        static void Main(string[] args)
        {
            var cards = new[]
                            {
                                Card.Artifact(10, "Manalith", 3 * Mana.Colorless, Mana.Any),
                                Card.Artifact(10, "Dragon's Horde", 3 * Mana.Colorless, Mana.Any),
                                // W Ajani's Welcome
                                Card.Spell(1, "Ajani's Welcome", Mana.White),
                                // W Leonin Vangard
                                Card.Creature(1, "Leonin Vangard", Mana.White),
                                // 1W Ajani's Pridemate
                                Card.Creature(1, "Ajani's Pridemate", 1 * Mana.Any + 1 * Mana.White),
                                // 2WW Leonin Warleader
                                Card.Creature(1, "Leonin Warleader", 2 * Mana.Any + 2 * Mana.White),
                                // 3W Dwarven Priest
                                Card.Creature(1, "Dwarven Priest", 3 * Mana.Any + 1 * Mana.White),
                                // 1WW Resplendent Angel
                                Card.Creature(1, "Resplendent Angel", Mana.Any + 2 * Mana.White),
                                // 2W Luminous Bonds
                                Card.Spell(1, "Luminous Bonds", 2*Mana.Any + Mana.White),
                                // 1B Cast Down
                                Card.Spell(1, "Cast Down", 1 * Mana.Any + 1 * Mana.Black),
                                // 4WW Evra Halcyon Witness
                                Card.Creature(1, "Evra, Halcyon Witness", 4 * Mana.Any + 2 * Mana.White),
                                // 3WW Lyra Dawnbringer
                                Card.Creature(1, "Lyra Dawnbringer", 3 * Mana.Any + 2 * Mana.White),
                                // 3BW Regal Bloodlord
                                Card.Creature(1, "Regal Bloodlord", 3 * Mana.Any + Mana.Black + Mana.White),
                                // 2BW Aryel, Knight of Windgrace
                                Card.Creature(
                                    1,
                                    "Aryel, Knight of Windgrace",
                                    2 * Mana.Any + Mana.Black + Mana.White),
                            };

            var deckEvaluator = new DeckEvaluator();
            deckEvaluator.SetCardPool(cards);

            int[] bestLandCombination = null;
            int[] bestCardCombination = null;
            var bestShift = 0;
            var bestScore = 0L;

            for (var nonLandCardCount = 20; nonLandCardCount < 4 * cards.Length; nonLandCardCount++)
            {
                foreach (var combination in CardCombinations(nonLandCardCount, cards.Length))
                {
                    var combinationArray = CombinationArray(combination);
                    var needShift = combinationArray.Any(c => combinationArray[0] != c);
                    foreach (var landCombination in LandCombinations(40 - nonLandCardCount, 2))
                    {
                        var shiftCount = needShift ? cards.Length : 1;
                        for (var shift = 0; shift < shiftCount; shift++)
                        {
                            Console.Clear();
                            if (bestLandCombination != null)
                            {
                                Console.WriteLine($"{bestLandCombination[0]} Swamp");
                                Console.WriteLine($"{bestLandCombination[1]} Plains");
                                for (var k = 0; k < cards.Length; k++)
                                {
                                    var cardCount = bestCardCombination[(k + bestShift) % combinationArray.Length];
                                    var card = cards[k];
                                    Console.WriteLine($"{cardCount} {card}");
                                }

                                Console.WriteLine($"Best Score: {bestScore}");
                                Console.WriteLine();
                            }

                            var copies = new int[combination.Length];
                            for (var i = 0; i < copies.Length; i++)
                            {
                                copies[i] = combinationArray[(i + shift) % combinationArray.Length];
                            }

                            deckEvaluator.AddDeck(
                                landCombination[0] * Mana.Black + landCombination[1] * Mana.White, copies);

                            var deckBuilder = new DeckBuilder();
                            deckBuilder.Add(landCombination[0], Card.Land(10, "Swamp", Mana.Black));
                            Console.WriteLine($"{landCombination[0]} Swamp");
                            deckBuilder.Add(landCombination[1], Card.Land(10, "Plains", Mana.White));
                            Console.WriteLine($"{landCombination[1]} Plains");

                            // Add Cards
                            for (var k = 0; k < cards.Length; k++)
                            {
                                var cardCount = combinationArray[(k + shift) % combinationArray.Length];
                                var card = cards[k];
                                deckBuilder.Add(cardCount, card);
                                Console.WriteLine($"{cardCount} {card}");
                            }

                            var tries = 10L;
                            var turns = 12;
                            var deck = deckBuilder.GetDeck();

                            var score = ScoreDeck(deck, tries, turns);
                            Console.WriteLine($"{score} vs {bestScore}");
                            if (score > bestScore)
                            {
                                score = ScoreDeck(deck, 10 * tries, turns);
                                if (score > bestScore)
                                {
                                    bestLandCombination = landCombination;
                                    bestCardCombination = combinationArray;
                                    bestShift = shift;
                                    bestScore = score;
                                }
                            }
                        }
                    }
                }
            }

            /*

            deckBuilder.Add(8 + sway, Card.Land(10, "Plains", Mana.White));
            deckBuilder.Add(8 - sway, Card.Land(10, "Swamp", Mana.Black));

            // W Ajani's Welcome
            deckBuilder.Add(3, Card.Spell(1, "Ajani's Welcome", Mana.White));
            // W Leonin Vangard
            deckBuilder.Add(4, Card.Creature(1, "Leonin Vangard", Mana.White));
            // 1W Ajani's Pridemate
            deckBuilder.Add(3, Card.Creature(1, "Ajani's Pridemate", 1*Mana.Any + 1*Mana.White));
            // 2WW Leonin Warleader
            deckBuilder.Add(2, Card.Creature(1, "Leonin Warleader", 2 * Mana.Any + 2*Mana.White));
            // 3W Dwarven Priest
            deckBuilder.Add(1, Card.Creature(1, "Dwarven Priest", 3 * Mana.Any + 1*Mana.White));
            // 1WW Resplendent Angel
            deckBuilder.Add(1, Card.Creature(1, "Resplendent Angel", Mana.Any + 2*Mana.White));
            // 2W Luminous Bonds
            // deckBuilder.Add(4, Card.Spell(1, "Luminous Bonds", 2*Mana.Any + Mana.White));
            // 1B Cast Down
            deckBuilder.Add(4, Card.Spell(1, "Cast Down", 1*Mana.Any + 1*Mana.Black));
            // 4WW Evra Halcyon Witness
            deckBuilder.Add(1, Card.Creature(1, "Evra, Halcyon Witness", 4 * Mana.Any + 2*Mana.White));
            // 3WW Lyra Dawnbringer
            deckBuilder.Add(1, Card.Creature(1, "Lyra Dawnbringer", 3 * Mana.Any + 2*Mana.White));
            // 3BW Regal Bloodlord
            deckBuilder.Add(2, Card.Creature(1, "Regal Bloodlord", 3 * Mana.Any + Mana.Black + Mana.White));
            // 2BW Aryel, Knight of Windgrace
            deckBuilder.Add(2, Card.Creature(1, "Aryel, Knight of Windgrace", 2*Mana.Any + Mana.Black + Mana.White));
            */
            Console.WriteLine("Done");
            Console.ReadLine();
        }

        private static long ScoreDeck(Card[] deck, long tries, int turns)
        {
            var playedNonLand = new Dictionary<int, int>();
            for (var run = 0; run < tries; run++)
            {
                var samples = GoldFish(deck, true, turns).ToArray();
                for (var i = 0; i < samples.Length; i++)
                {
                    if (!playedNonLand.ContainsKey(i))
                    {
                        playedNonLand.Add(i, 0);
                    }

                    if (samples[i].Any(s => s.PlayedCard != null && !s.PlayedCard.IsLand))
                    {
                        playedNonLand[i]++;
                    }
                }
            }

            var score = playedNonLand.Select(p => p.Value * 100 / tries).Sum();
            return score;
        }

        private static int[] CombinationArray(int[] combination)
        {
            var combinationArray = new int[combination.Sum()];
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
                if (uniqueRemaining <= totalCards)
                {
                    yield return new[] { totalCards, uniqueRemaining - totalCards };
                }

                yield break;
            }

            var max = totalCards / copies;
            for (var i = 0; i <= max; i++)
            {
                var remaining = totalCards - i * copies;
                foreach (var subCombination in CardCombinations(remaining, uniqueRemaining - i, copies - 1))
                {
                    var combination = new int[copies + 1];
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
                yield return new[] { total };
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

        public static List<List<GameState>> GoldFish(Card[] deck, bool playFirst = true, int turns = 10)
        {
            deck.Shuffle();
            var gameState = new GameState(deck, true);
            var gameLog = new List<List<GameState>>();

            // Play turns
            for (var turn = 0; turn < turns; turn++)
            {
                // Save current state
                gameState = new GameState(gameState);
                gameLog.Add(new List<GameState>());
                gameLog[gameLog.Count - 1].Add(gameState);

                // Yield the current pool
                gameState.StartTurn(true);

                /*
                Console.WriteLine($"Hand - {gameState.ManaPool} available");
                foreach (var card in gameState.Hand)
                {
                    Console.WriteLine($"[{turn}]  {card}");
                }
                */

                while (TryPlayBestCard(out var bestCard, out gameState, out _, gameState))
                {
                    gameLog[gameLog.Count - 1].Add(gameState);
                    //Console.WriteLine($"[{turn}] Played {bestCard.Name}");
                    //Console.WriteLine($"[{turn}] {gameState.Hand.Count} cards in hand - {gameState.ManaAvailable} available");
                }

                /*
                while (gameState.Hand.Count > 7)
                {
                    GetWorstCard(out var worstCard, gameState);
                    Console.WriteLine($"[{turn}] Discard {worstCard.Name}");
                    gameState.Hand.Remove(worstCard);
                }
                */

                //Console.WriteLine();
            }

            return gameLog;
        }


        private static bool TryPlayBestCard(out Card bestCard, out GameState bestGameState, out int bestScore, GameState gameState)
        {
            bestGameState = gameState;
            bestCard = null;
            bestScore = 0;
            var playedSomething = false;
            foreach (var card in gameState.Hand)
            {
                // Nothing to do if can't play
                if (!gameState.TryPlay(out var playedGameState, card))
                {
                    continue;
                }

                // Look ahead in the turn (don't care if played nothing)
                TryPlayBestCard(out _, out _, out var nextBestScore, playedGameState);

                // Update statistics
                playedSomething = true;
                nextBestScore += card.Score;

                if (nextBestScore > bestScore)
                {
                    bestGameState = playedGameState;
                    bestScore = nextBestScore;
                    bestCard = card;
                }
            }

            return playedSomething;
        }
    }

    public static class DeckExtensions
    {
        private static Random r = new Random();

        public static IEnumerable<T> Draw<T>(this Queue<T> queue, int count = 1)
        {
            for (var i = 0; i < count; i++)
            {
                yield return queue.Dequeue();
            }
        }

        public static void Shuffle<T>(this T[] array)
        {
            int n = array.Length;
            while (n > 1)
            {
                int k = r.Next(n--);
                T temp = array[n];
                array[n] = array[k];
                array[k] = temp;
            }
        }
    }

    public class DeckBuilder
    {
        private List<Card> deckList = new List<Card>();

        public Card[] GetDeck()
        {
            return this.deckList.ToArray();
        }

        public void Add(int count, Card card)
        {
            for (var i = 0; i < count; i++)
            {
                this.Add(card);
            }
        }

        public void Fill(int size, Card card)
        {
            while (this.deckList.Count < size)
            {
                this.Add(card);
            }
        }

        private void Add(Card card)
        {
            var addedCard = new Card(card);
            this.deckList.Add(addedCard);
        }
    }
}
