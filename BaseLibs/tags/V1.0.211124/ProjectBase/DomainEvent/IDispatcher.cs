﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectBase.DomainEvent
{
    public interface IDispatcher
    {
        void Publish<T>(T events) where T :IDomainEvent;
    }
}
