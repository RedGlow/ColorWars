using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColorWars.Model.Gw2SpidyApi
{
    /// <summary>
    /// The result from a single GW2Spidy query.
    /// </summary>
    [Serializable]
    public class Result
    {
#pragma warning disable 649
        public int data_id;
        public string name;
        public int rarity;
        public int restriction_level;
        public string img;
        public int type_id;
        public int sub_type_id;
        public string price_last_changed;
        public int max_offer_unit_price;
        public int min_sale_unit_price;
        public int offer_availability;
        public int sale_availability;
        public int sale_price_change_last_hour;
        public int offer_price_change_last_hour;
#pragma warning restore 649
    }
}
