﻿using System;
using System.ComponentModel.DataAnnotations;

namespace MvcMusicStore.Models
{
    public class Artist
    {
        public Guid ArtistId { get; set; }
        public string Name { get; set; }
    }
}