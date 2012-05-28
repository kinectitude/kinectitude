using System.Linq;
using MessagePassing.Public;

namespace MessagePassing.Components
{
    public class PhysicsManager : Manager<PhysicsComponent>
    {
        /// <summary>
        /// Test method called to simulate a manager triggering an event.
        /// </summary>
        public void SimulateCollision()
        {
            PhysicsComponent coin = Children.FirstOrDefault(x => x.Type == "Coin");
            PhysicsComponent player = Children.FirstOrDefault(x => x.Type == "Player");
            coin.Collide(player);
        }
    }
}
