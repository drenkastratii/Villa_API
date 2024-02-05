﻿using System.ComponentModel.DataAnnotations;

namespace Villa_API.Models.DTO
{
    public class VillaDTO
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(30)]
        public string Name { get; set; }
        public int Occupany { get; set; }
        public int Sqft { get; set; }
    }
}
