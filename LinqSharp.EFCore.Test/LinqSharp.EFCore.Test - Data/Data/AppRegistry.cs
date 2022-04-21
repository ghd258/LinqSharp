﻿using System;

namespace LinqSharp.EFCore.Data.Test
{
    public class AppRegistry : KvEntity { }
    public class AppRegistryAgent : KvEntityAgent<AppRegistryAgent, AppRegistry>
    {
        public virtual string Name { get; set; }

        public virtual int Age { get; set; }

        public virtual DateTime? Birthday { get; set; }

        public virtual string NickName { get; set; } = "haha";

        public virtual string Address { get; set; }

        public virtual bool Enable { get; set; }
    }

}
