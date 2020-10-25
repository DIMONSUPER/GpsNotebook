﻿using SQLite;

namespace GpsNotebook.Models
{
    public class UserModel : IEntityBase
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Unique]
        public string Email { get; set; }

        public string Name { get; set; }

        [MaxLength(16)]
        public string Password { get; set; }
    }
}
