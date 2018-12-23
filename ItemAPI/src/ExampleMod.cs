using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ItemAPI.src
{
    class ExampleMod : ETGModule
    {
        public override void Exit()
        {
        }

        public override void Init()
        {
        }

        public override void Start()
        {
            ItemBuilder.Init();
            ExampleActive.Init();
            ExamplePassive.Init();
            ExampleGun.Init();
        }
    }
}
