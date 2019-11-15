using System.ComponentModel.DataAnnotations.Schema;

namespace BTS.API.ENTITY
{
    public abstract class EntityBase : IObjectState
    {
        [NotMapped]
        public ObjectState ObjectState { get; set; } //TODO: Renamed since a possible coflict with State entity column
    }
}