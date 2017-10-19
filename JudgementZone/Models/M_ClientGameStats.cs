using Realms;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.Collections.Generic;

namespace JudgementZone.Models
{
    public class M_ClientGameStats : RealmObject
    {

        // KEY
		[PrimaryKey]
        public string GameKey { get; set; }

        // LIST OF GAME STATS OBJECTS FOR EACH PLAYER
        public IList<M_ClientPlayerGameStats> ClientPlayerGameStats { get; set; }

        public M_ClientGameStats()
        {
        }
    }
}
