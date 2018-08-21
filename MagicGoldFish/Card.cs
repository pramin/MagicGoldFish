namespace MagicGoldFish
{
    using MagicSimulator.Tests;

    public class Card
    {
        public int Score { get; set; }

        public string Name { get; set; }

        public bool IsLand { get; set; }

        public Mana Cost { get; set; }

        public Mana[] Produced { get; set; }

        public bool IsCreature { get; set; }

        public Card()
        {
        }

        public Card(Card other)
        {
            this.Name = other.Name;
            this.Cost = other.Cost;
            this.IsLand = other.IsLand;
            this.Produced = other.Produced;
            this.IsCreature = other.IsCreature;
            this.Score = other.Score;
        }

        public static Card Land(int score, string name, params Mana[] produced)
        {
            return new Card { Score = score, Name = name, IsLand = true, Cost = Mana.Zero, Produced = produced };
        }

        public static Card PayLand(int score, string name, bool tapped, Mana cost, params Mana[] produced)
        {
            return new Card { Score = score, Name = name, IsLand = true, Cost = cost, Produced = produced, IsCreature = tapped };
        }

        public static Card Artifact(int score, string name, Mana cost, params Mana[] produced)
        {
            return new Card { Score = score, Name = name, IsLand = false, Cost = cost, Produced = produced, IsCreature = false };
        }

        public static Card Creature(int score, string name, Mana cost, params Mana[] produced)
        {
            return new Card { Score = score, Name = name, IsLand = false, Cost = cost, Produced = produced, IsCreature = true };
        }

        public static Card Spell(int score, string name, Mana cost)
        {
            return new Card { Score = score, Name = name, IsLand = false, Cost = cost, Produced = new Mana[0] };
        }

        public override string ToString()
        {
            return $"{this.Name} ({this.Cost})";
        }
    }
}