using System.ComponentModel.DataAnnotations;

namespace RoyalVilla_API.Models.DTO
{
    public class UserDTO
    {
        public int Id { get; set; }

        public string Email { get; set; } = default!;
        
        public string Name { get; set; } = default!;

        public string Role { get; set; } = default!; //default exclamatory sign
    }
   /* Interview-Friendly Definition

default! is called the default value with the null-forgiving operator. It tells the compiler to ignore nullable warnings because the property will be initialized later.

Easy Memory Trick
default  = give default value
!        = don't warn me

default! = give default value and don't warn me*/

}
