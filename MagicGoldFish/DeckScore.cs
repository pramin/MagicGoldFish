namespace MagicGoldFish
{
    using System.Text;

    using MagicSimulator.Tests;

    public class DeckScore
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeckScore"/> class.
        /// </summary>
        /// <param name="cardPool">
        /// The card pool.
        /// </param>
        /// <param name="basicLands">
        /// The number of each basic land by color.
        /// </param>
        /// <param name="copies">
        /// The copies.
        /// </param>
        public DeckScore(Card[] cardPool, Mana basicLands, int[] copies)
        {
            this.CardPool = cardPool;
            this.BasicLands = basicLands;
            this.Copies = copies;
            this.Score = -1;
        }

        /// <summary>
        /// Gets the basic lands.
        /// </summary>
        public Mana BasicLands { get; }

        /// <summary>
        /// Gets the collection of cards in the deck.
        /// </summary>
        public Card[] CardPool { get; }

        /// <summary>
        /// Gets the number of copies of each card in the deck.
        /// </summary>
        public int[] Copies { get; }

        /// <summary>
        /// Gets the score for the deck.
        /// </summary>
        public long Score { get; private set; }

        /// <summary>
        /// Set the deck score.
        /// </summary>
        /// <param name="score">
        /// The score.
        /// </param>
        public void SetScore(long score)
        {
            this.Score = score;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine($"Score: {this.Score}");
            stringBuilder.AppendLine();

            // Add the lands
            if (this.BasicLands.WhiteAmount > 0) { stringBuilder.AppendLine($"{this.BasicLands.WhiteAmount} Plains"); }
            if (this.BasicLands.BlueAmount> 0) { stringBuilder.AppendLine($"{this.BasicLands.BlueAmount} Island"); }
            if (this.BasicLands.BlackAmount> 0) { stringBuilder.AppendLine($"{this.BasicLands.BlackAmount} Swamp"); }
            if (this.BasicLands.RedAmount> 0) { stringBuilder.AppendLine($"{this.BasicLands.RedAmount} Mountain"); }
            if (this.BasicLands.GreenAmount> 0) { stringBuilder.AppendLine($"{this.BasicLands.GreenAmount} Forest"); }
            if (this.BasicLands.ColorlessAmount> 0) { stringBuilder.AppendLine($"{this.BasicLands.ColorlessAmount} Wastes"); }

            // Add the cards
            for (var i = 0; i < CardPool.Length; i++)
            {
                // Skip missing cards
                if (this.Copies[i] == 0)
                {
                    continue;
                }

                stringBuilder.AppendLine($"{this.Copies[i]} {this.CardPool[i].Name} {this.CardPool[i].Cost}");
            }

            return stringBuilder.ToString();
        }
    }
}