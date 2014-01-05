using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColorWars.Controller.Prices
{
    /// <summary>
    /// The price of a dye.
    /// </summary>
    [Serializable]
    public class Price
    {
        /// <summary>
        /// The internal result structure returned by the model.
        /// </summary>
        private Model.Gw2SpidyApi.Result result;

        /// <summary>
        /// Name of the dye.
        /// </summary>
        public string Name { get { return result == null ? null : result.name; } }

        /// <summary>
        /// Whether this price element is valid (the dye wasn't a starter one).
        /// </summary>
        public bool Valid { get { return result != null; } }

        /// <summary>
        /// Rarity level of this dye.
        /// </summary>
        public RarityKind? Rarity
        {
            get
            {
                if (result == null)
                    return null;
                switch (result.rarity)
                {
                    case 4:
                        return Prices.RarityKind.Rare;
                    case 3:
                        return Prices.RarityKind.Masterwork;
                    case 2:
                        return Prices.RarityKind.Fine;
                    default:
                        throw new InvalidOperationException("Unknown rarity level " + result.rarity.ToString());
                }
            }
        }

        /// <summary>
        /// Whether this dye is of rarity "rare".
        /// </summary>
        public bool IsRare { get { return Rarity != null && Rarity.Value == RarityKind.Rare; } }

        /// <summary>
        /// Whether this dye is of rarity "masterwork".
        /// </summary>
        public bool IsMasterwork { get { return Rarity != null && Rarity.Value == RarityKind.Masterwork; } }

        /// <summary>
        /// Whether this dye is of rarity "fine".
        /// </summary>
        public bool IsFine { get { return Rarity != null && Rarity.Value == RarityKind.Fine; } }

        /// <summary>
        /// Total (order) price of the dye in coppers.
        /// </summary>
        public int? TotalCopperPrice { get { return result == null ? null : (int?)result.max_offer_unit_price; } }

        /// <summary>
        /// Gold part of the (order) price of the dye.
        /// </summary>
        public int? GoldPrice { get { return result == null ? null : TotalCopperPrice / 10000; } }

        /// <summary>
        /// Silver part of the (order) price of the dye.
        /// </summary>
        public int? SilverPrice { get { return result == null ? null : (TotalCopperPrice / 100) % 100; } }

        /// <summary>
        /// Copper part of the (order) price of the dye.
        /// </summary>
        public int? CopperPrice { get { return result == null ? null : TotalCopperPrice % 100; } }

        /// <summary>
        /// Create a new Price object.
        /// </summary>
        /// <param name="result">The model object to get the value from.</param>
        public Price(Model.Gw2SpidyApi.Result result)
        {
            this.result = result;
        }
    }
}
