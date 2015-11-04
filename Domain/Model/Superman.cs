using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model
{
    /// <summary>
    /// used to show how to use autofac to create a new instance
    /// </summary>
    public enum Ability
    {
        MetalLiquidBody, AntiAging, MindControl
    }

    public interface ISuperman
    {
        void SaveTheWorld();

    }
    public class Superman : ISuperman
    {
        public Superman(Person Person, Ability Ability)
        {
            this.Person = Person;
            this.Ability = Ability;
        }

        public void SaveTheWorld()
        {
            
        }
        public Ability Ability { get; set; }
        public Person Person { get; set; }
    }
}
