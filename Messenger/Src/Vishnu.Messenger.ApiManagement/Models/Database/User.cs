namespace Vishnu.Messenger.ApiManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class User
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(50)]
        public string UserId { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(50)]
        public string UserName { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(50)]
        public string Password { get; set; }

        [Key]
        [Column(Order = 3)]
        [StringLength(50)]
        public string DisplayName { get; set; }

        [StringLength(50)]
        public string EmailId { get; set; }
    }
}
