using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Core.Models
{
    public class ClientDTO
    {
        public string Id { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        public string ClientName { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Email { get; set; }
        [JsonIgnore]
        [Required]
        public string Password { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        public ClientStates States { get; set; }
    }

    public enum ClientStates
    {
        Registered, BookedRoom, CheckIn, CheckOut
    }
}
