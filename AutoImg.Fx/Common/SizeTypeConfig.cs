using System;
using System.Collections.Generic;
using System.Text;

namespace AutoImg.Fx.Common
{
    public class SizeTypeConfig : JsonConfigItem
    {
        public override string CfgFile => "SizeTypes.json";

        public IEnumerable<SizeType> Types
        {
            get; set;
        }
    }
}
