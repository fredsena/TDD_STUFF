using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoqTests.Domain
{
    public class Product
    {
        public ProductIdentifier Identifier { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public Product(string name, string description)
        {
            Name = name;
            Description = description;
        }
    }
}
