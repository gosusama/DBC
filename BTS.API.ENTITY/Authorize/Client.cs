using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BTS.API.ENTITY.Authorize
{
    [Table("AU_CLIENT")]
    public class Client : DataInfoEntity
    {

        [Required]
        [Column("SECRECT")]
        public string Secret { get; set; }
        [Required]
        [MaxLength(100)]
        [Column("NAME")]
        public string Name { get; set; }
        [Column("APPLICATIONTYPE")]
        public ApplicationTypes ApplicationType { get; set; }
        [Column("ACTIVE")]
        public bool Active { get; set; }
        [Column("REFRESHTOKENLIFETIME")]
        public int RefreshTokenLifeTime { get; set; }
        [MaxLength(100)]
        [Column("ALLOWEDORIGIN")]
        public string AllowedOrigin { get; set; }
    }
    [Table("AU_REFRESHTOKN")]
    public class RefreshToken : DataInfoEntity
    {
        [Required]
        [MaxLength(50)]
        [Column("SUBJECT")]
        public string Subject { get; set; }
        [Required]
        [MaxLength(50)]
        [Column("CLIENTID")]
        public string ClientId { get; set; }
        [Column("ISUSEDUTC")]
        public DateTime IssuedUtc { get; set; }
        [Column("EXPIRESUTC")]
        public DateTime ExpiresUtc { get; set; }
        [Required]
        [Column("PROTECTEDTICKET")]
        public string ProtectedTicket { get; set; }
    }
    public enum ApplicationTypes
    {
        JavaScript = 0,
        NativeConfidential = 1
    };
}
