using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model
{
    /// <summary>
    /// an arbitrary class, with some properties in various data type just to show that the generic function can handle these types well
    /// </summary>
    public class Person
    {
        public int PersonID { get; set; }
        public string Name { get; set; }
        public DateTime BornDate { get; set; }
        public string PhoneNr { get; set; }
        public bool HasSupernaturalAbility { get; set; }
    }
}
