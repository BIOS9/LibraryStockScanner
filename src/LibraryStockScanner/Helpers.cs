using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIA.LibraryStockScanner
{
    public static class Helpers
    {
        public static string ToReadable(this byte[] data, string format = "X")
        {
            return string.Join(", ", data.Select(x => "0x" + x.ToString(format)).ToArray());
        }
    }
}
