using System.Collections.Generic;

namespace ZqSpider.Core
{
    public class Item
    {
        public Item()
        {
            this.Data = new Dictionary<string, dynamic>();
        }

        public Dictionary<string, dynamic> Data { get; set; }
    }
}
