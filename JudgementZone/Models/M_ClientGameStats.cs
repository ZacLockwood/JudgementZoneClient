using Realms;
using System.Collections.Generic;

namespace JudgementZone.Models
{
    public class M_ClientGameStats : RealmObject
    {

        // KEY
		[PrimaryKey]
        public string GameKey { get; set; }

        // LIST OF GAME STATS OBJECTS FOR EACH PLAYER
        public IList<M_ClientPlayerGameStats> ClientPlayerGameStats { get; }

        public M_ClientGameStats()
        {
        }
    }
}
