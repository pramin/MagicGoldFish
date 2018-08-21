namespace MagicGoldFish
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    using MagicSimulator.Tests;

    public class DeckEvaluator
    {
        /// <summary>
        /// Worker thread pool
        /// </summary>
        private Thread[] threadPool;

        /// <summary>
        /// The pending decks.
        /// </summary>
        private BlockingCollection<DeckScore> pendingDecks;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeckEvaluator"/> class.
        /// </summary>
        public DeckEvaluator()
        {
            this.TopDecks = new List<DeckScore>();
            this.pendingDecks = new BlockingCollection<DeckScore>();
            this.threadPool = new Thread[Environment.ProcessorCount * 2];
            for (var i = 0; i < this.threadPool.Length; i++)
            {
                this.threadPool[i] = new Thread(this.DeckScorer);
                this.threadPool[i].Start();
            }
        }

        /// <summary>
        /// Gets the top decks.
        /// </summary>
        public List<DeckScore> TopDecks { get; }

        public void SetCardPool(IEnumerable<Card> cardPool)
        {
            this.CardPool = cardPool.OrderBy(c => c.Cost.ConvertedAmount).ThenBy(c => c.Name).ToArray();
        }

        public Card[] CardPool { get; private set; }

        public void AddDeck(Mana basicMana, int[] copies)
        {
            var deckScore = new DeckScore(this.CardPool, basicMana, copies);
            this.pendingDecks.Add(deckScore);
        }

        public void WaitForComplete()
        {
            foreach (var thread in this.threadPool)
            {
                thread.Join();
            }
        }

        private void DeckScorer()
        {
            foreach (var deckScore in this.pendingDecks.GetConsumingEnumerable())
            {
                var deck = new List<Card>();

                // Add cards
                for (var i = 0; i < deckScore.Copies.Length; i++)
                {
                    for (var j = 0; j < deckScore.Copies[i]; j++)
                    {
                        deck.Add(new Card(deckScore.CardPool[i]));
                    }
                }

                // Add basic lands
                for (var j = 0; j < deckScore.BasicLands.WhiteAmount; j++) { deck.Add(Card.Land(10, "Plains", Mana.White)); }
                for (var j = 0; j < deckScore.BasicLands.BlueAmount; j++) { deck.Add(Card.Land(10, "Island", Mana.Blue)); }
                for (var j = 0; j < deckScore.BasicLands.BlackAmount; j++) { deck.Add(Card.Land(10, "Swamp", Mana.Black)); }
                for (var j = 0; j < deckScore.BasicLands.RedAmount; j++) { deck.Add(Card.Land(10, "Mountain", Mana.Red)); }
                for (var j = 0; j < deckScore.BasicLands.GreenAmount; j++) { deck.Add(Card.Land(10, "Forest", Mana.Green)); }
                for (var j = 0; j < deckScore.BasicLands.ColorlessAmount; j++) { deck.Add(Card.Land(10, "Wastes", Mana.Colorless)); }

                // Score deck
                deckScore.SetScore(ScoreDeck(deck.ToArray(), 100, 12));

                // Update top deck
                lock (this.TopDecks)
                {
                    if (this.TopDecks.Count == 0)
                    {
                        this.TopDecks.Add(deckScore);
                    }
                    else if (deckScore.Score > this.TopDecks[0].Score)
                    {
                        this.TopDecks[0] = deckScore;
                        this.TopDecks.Sort((left, right) => left.Score.CompareTo(right.Score));
                    }
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

        private static long ScoreDeck(Card[] deck, long tries, int turns, bool playFirst = true)
        {
            var playedNonLand = new Dictionary<int, int>();
            for (var run = 0; run < tries; run++)
            {
                var samples = GoldFish(deck, playFirst, turns).ToArray();
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
    }
}