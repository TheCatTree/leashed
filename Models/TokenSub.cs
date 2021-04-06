using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace leashApi.Models
{
    public class TokenSub
    {
        
        public int Id { get; set; }
        [Required]
        [StringLength(255)]
        public string tokenSub { get; set; }
        
        

        public TokenSub()
        {
            
        }
    }
}