namespace MagicSimulator.Tests
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Represents a pool of mana which can be used to
    /// pay for spells and effects.
    /// </summary>
    public class ManaPool
    {
        /// <summary>
        /// The set of combinations produced by all available
        /// mana producers.
        /// </summary>
        private HashSet<Mana> combinations = new HashSet<Mana>();

        /// <summary>
        /// Gets the amount of mana available in the mana pool.
        /// </summary>
        public int AvailableMana =>
            this.combinations == null || this.combinations.Count == 0
                ? 0
                : this.combinations.Max(combination => combination.ConvertedAmount);

        /// <summary>
        /// Add a mana source to the mana pool.
        /// </summary>
        /// <param name="producedCombinations">
        /// The combinations of mana produced by the source.
        /// </param>
        public void AddManaSource(IList<Mana> producedCombinations)
        {
            // No-op if nothing to add
            if (producedCombinations == null || producedCombinations.Count == 0)
            {
                return;
            }

            if (this.combinations == null || this.combinations.Count == 0)
            {
                this.combinations = new HashSet<Mana>(producedCombinations);
                return;
            }

            var newCombinations = new HashSet<Mana>();
            foreach (var combination in this.combinations)
            {
                foreach (var producedCombination in producedCombinations)
                {
                    newCombinations.Add(combination + producedCombination);
                }
            }

            this.combinations = newCombinations;
        }

        /// <summary>
        /// Determines if any combination of mana can be used to
        /// pay the specified <paramref name="cost"/>
        /// </summary>
        /// <param name="cost">
        /// The cost to pay.
        /// </param>
        /// <returns>
        /// True if the cost can be paid, false otherwise.
        /// </returns>
        public bool CanPay(Mana cost)
        {
            return this.combinations.Any(combination => !(combination - cost).IsOverdrawn);
        }

        /// <summary>
        /// Tries to pay the mana cost if any combination of mana can be used to
        /// pay the specified <paramref name="cost"/>
        /// </summary>
        /// <param name="cost">
        /// The cost to pay.
        /// </param>
        /// <returns>
        /// True if the cost can be paid, false otherwise.
        /// </returns>
        public bool TryPay(Mana cost)
        {
            // Special case when cost is 0
            if (cost.ConvertedAmount == 0)
            {
                return true;
            }

            var remainingCombinations = new HashSet<Mana>();
            var canPay = false;
            foreach (var combination in this.combinations)
            {
                var remaining = combination - cost;
                if (remaining.IsOverdrawn)
                {
                    continue;
                }

                remainingCombinations.Add(remaining);
                canPay = true;
            }

            if (canPay)
            {
                this.combinations = remainingCombinations;
            }

            return canPay;
        }

        /// <summary>
        /// Clone the <see cref="ManaPool"/>
        /// </summary>
        /// <returns>
        /// The <see cref="ManaPool"/>.
        /// </returns>
        public ManaPool Clone()
        {
            return new ManaPool { combinations = new HashSet<Mana>(this.combinations) };
        }

        public override string ToString()
        {
            return string.Join(", ", this.combinations);
        }
    }
}