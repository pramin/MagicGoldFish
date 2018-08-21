namespace MagicGoldFish
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using MagicSimulator.Tests;

    public class GameState
    {
        public GameState(IEnumerable<Card> deckList, bool playFirst = true)
        {
            this.Library = new Queue<Card>(deckList);
            this.Hand = new List<Card>();
            this.Hand.AddRange(this.Library.Draw(playFirst ? 6 : 7));
            this.PlayedCard = null;
        }

        public GameState(GameState state)
        {
            this.Library = new Queue<Card>(state.Library);
            this.Hand = new List<Card>(state.Hand);
            this.ManaAvailable = state.ManaAvailable.Clone();
            this.ManaPool = state.ManaPool.Clone();
            this.PlayedLand = state.PlayedLand;
            this.PlayedCard = null;
        }

        public ManaPool ManaPool { get; } = new ManaPool();

        public ManaPool ManaAvailable { get; private set; } = new ManaPool();

        public Queue<Card> Library { get; private set; }

        public List<Card> Hand { get; }

        public bool PlayedLand { get; set; }

        public Card PlayedCard { get; set; }

        public void StartTurn(bool draw = false)
        {
            this.ManaAvailable = this.ManaPool.Clone();
            if (draw)
            {
                this.Hand.AddRange(this.Library.Draw());
            }

            this.PlayedLand = false;
            this.PlayedCard = null;
        }

        public bool TryPlay(out GameState gameState, Card card)
        {
            gameState = new GameState(this);
            if (card.IsLand)
            {
                if (gameState.PlayedLand)
                {
                    return false;
                }

                gameState.PlayedLand = true;
            }
            else if (!gameState.ManaAvailable.TryPay(card.Cost))
            {
                return false;
            }

            gameState.Hand.Remove(card);
            gameState.ManaPool.AddManaSource(card.Produced);
            gameState.PlayedCard = card;
            if (!card.IsCreature)
            {
                gameState.ManaAvailable.AddManaSource(card.Produced);
            }

            return true;
        }
    }
}