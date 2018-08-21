namespace MagicSimulator.Tests
{
    using System.Text;

    /// <summary>
    /// Represents an amount of mana.
    /// </summary>
    public struct Mana
    {
        /// <summary>
        /// One mana of any color
        /// </summary>
        public static Mana Zero = new Mana();

        /// <summary>
        /// One mana of any color
        /// </summary>
        public static Mana Any = new Mana { AnyAmount = 1 };

        /// <summary>
        /// One white mana
        /// </summary>
        public static Mana White = new Mana { WhiteAmount = 1 };

        /// <summary>
        /// One blue mana
        /// </summary>
        public static Mana Blue = new Mana { BlueAmount = 1 };

        /// <summary>
        /// One black mana
        /// </summary>
        public static Mana Black = new Mana { BlackAmount = 1 };

        /// <summary>
        /// One red mana
        /// </summary>
        public static Mana Red = new Mana { RedAmount = 1 };

        /// <summary>
        /// One green mana
        /// </summary>
        public static Mana Green = new Mana { GreenAmount = 1 };

        /// <summary>
        /// One colorless mana
        /// </summary>
        public static Mana Colorless = new Mana { ColorlessAmount = 1 };

        /// <summary>
        /// Amount of mana of any color.
        /// </summary>
        public int AnyAmount;

        /// <summary>
        /// Amount of white mana
        /// </summary>
        public int WhiteAmount;

        /// <summary>
        /// Amount of blue mana
        /// </summary>
        public int BlueAmount;

        /// <summary>
        /// Amount of black mana
        /// </summary>
        public int BlackAmount;

        /// <summary>
        /// Amount of red mana
        /// </summary>
        public int RedAmount;

        /// <summary>
        /// Amount of green mana
        /// </summary>
        public int GreenAmount;

        /// <summary>
        /// Amount of colorless mana
        /// </summary>
        public int ColorlessAmount;

        /// <summary>
        /// Gets the converted mana value.
        /// </summary>
        public int ConvertedAmount =>
            this.AnyAmount + this.WhiteAmount + this.BlueAmount + this.BlackAmount + this.RedAmount + this.GreenAmount
            + this.ColorlessAmount;

        /// <summary>
        /// Gets a value indicating whether this <see cref="Mana"/> value
        /// represents an overdrawn value, e.g. one or more colors have
        /// sub-zero amounts which cannot be covered by <see cref="AnyAmount"/>.
        /// </summary>
        public bool IsOverdrawn
        {
            get
            {
                // If there is more color debt than any amount can cover, this
                // mana is overdrawn
                var colorDebt = (this.BlackAmount > 0 ? 0 : this.BlackAmount)
                                + (this.GreenAmount > 0 ? 0 : this.GreenAmount)
                                + (this.WhiteAmount > 0 ? 0 : this.WhiteAmount)
                                + (this.BlueAmount > 0 ? 0 : this.BlueAmount)
                                + (this.RedAmount > 0 ? 0 : this.RedAmount)
                                + (this.ColorlessAmount > 0 ? 0 : this.ColorlessAmount);
                if (colorDebt < 0)
                {
                    return this.AnyAmount + colorDebt < 0;
                }

                return this.ConvertedAmount < 0;
            }
        }

        /// <summary>
        /// The plus operator.
        /// </summary>
        /// <param name="left">
        /// The left mana value.
        /// </param>
        /// <param name="right">
        /// The right mana value.
        /// </param>
        /// <returns>
        /// A <see cref="Mana"/> value which is the sum
        /// of left and right.
        /// </returns>
        public static Mana operator +(Mana left, Mana right)
        {
            return new Mana
                       {
                           AnyAmount = left.AnyAmount + right.AnyAmount,
                           WhiteAmount = left.WhiteAmount + right.WhiteAmount,
                           BlueAmount = left.BlueAmount + right.BlueAmount,
                           BlackAmount = left.BlackAmount + right.BlackAmount,
                           RedAmount = left.RedAmount + right.RedAmount,
                           GreenAmount = left.GreenAmount + right.GreenAmount,
                           ColorlessAmount = left.ColorlessAmount + right.ColorlessAmount,
                       };
        }

        /// <summary>
        /// The plus operator.
        /// </summary>
        /// <param name="left">
        /// The left mana value.
        /// </param>
        /// <param name="right">
        /// The right mana value.
        /// </param>
        /// <returns>
        /// A <see cref="Mana"/> value which is the sum
        /// of left and right.
        /// </returns>
        public static Mana operator -(Mana left, Mana right)
        {
            return new Mana
                       {
                           AnyAmount = left.AnyAmount - right.AnyAmount,
                           WhiteAmount = left.WhiteAmount - right.WhiteAmount,
                           BlueAmount = left.BlueAmount - right.BlueAmount,
                           BlackAmount = left.BlackAmount - right.BlackAmount,
                           RedAmount = left.RedAmount - right.RedAmount,
                           GreenAmount = left.GreenAmount - right.GreenAmount,
                           ColorlessAmount = left.ColorlessAmount - right.ColorlessAmount,
                       };
        }

        /// <summary>
        /// The scalar multiplication operator.
        /// </summary>
        /// <param name="scalar">
        /// The scalar.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The value scaled by the scalar.
        /// </returns>
        public static Mana operator *(int scalar, Mana value)
        {
            return new Mana
                       {
                           AnyAmount = scalar * value.AnyAmount,
                           WhiteAmount = scalar * value.WhiteAmount,
                           BlueAmount = scalar * value.BlueAmount,
                           BlackAmount = scalar * value.BlackAmount,
                           RedAmount = scalar * value.RedAmount,
                           GreenAmount = scalar * value.GreenAmount,
                           ColorlessAmount = scalar * value.ColorlessAmount,
                       };
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            if (this.AnyAmount > 0)
            {
                builder.Append(this.AnyAmount);
            }

            for (var i = 0; i < this.WhiteAmount; i++) { builder.Append('W'); }
            for (var i = 0; i < this.BlueAmount; i++) { builder.Append('U'); }
            for (var i = 0; i < this.BlackAmount; i++) { builder.Append('B'); }
            for (var i = 0; i < this.RedAmount; i++) { builder.Append('R'); }
            for (var i = 0; i < this.GreenAmount; i++) { builder.Append('G'); }
            for (var i = 0; i < this.ColorlessAmount; i++) { builder.Append('C'); }

            if (this.AnyAmount < 0)
            {
                builder.Append(this.AnyAmount);
            }

            if (builder.Length > 0)
            {
                builder.Append(' ');
            }

            builder.AppendFormat($"({this.ConvertedAmount})");

            return builder.ToString();
        }
    }
}