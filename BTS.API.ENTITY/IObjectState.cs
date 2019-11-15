
using System.ComponentModel.DataAnnotations.Schema;

namespace BTS.API.ENTITY
{
    public interface IObjectState
    {
        [NotMapped]
        ObjectState ObjectState { get; set; }
    }
}