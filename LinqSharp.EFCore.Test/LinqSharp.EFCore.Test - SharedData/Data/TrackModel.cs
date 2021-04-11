﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LinqSharp.EFCore.Data.Test
{
    public class TrackModel : IEntity<TrackModel>
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [AutoCreationTime]
        public DateTime CreationTime { get; set; }

        [AutoLastWriteTime]
        public DateTime LastWriteTime { get; set; }

        [AutoTrim]
        public string ForTrim { get; set; }

        [AutoUpper]
        public string ForUpper { get; set; }

        [AutoLower]
        public string ForLower { get; set; }

        [AutoCondensed]
        public string ForCondensed { get; set; }
    }
}
