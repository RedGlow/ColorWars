using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColorWars.Controller.Colors
{
    public enum SortKind
    {
        [Description("Code")]
        Code,

        [Description("Hue")]
        Hue,

        [Description("Saturation")]
        Saturation,

        [Description("Value")]
        Value,

        [Description("Alphabetical")]
        Alphabetical,

        [Description("Nearest to...")]
        NearestTo,
    }
}
