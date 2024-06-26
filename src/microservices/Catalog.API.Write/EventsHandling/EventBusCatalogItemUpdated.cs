﻿using System;
using EventBus.Structures;

namespace Catalog.API.Write.EventsHandling;

public class EventBusCatalogItemUpdated : EventBusPublisher
{
    public EventBusCatalogItemUpdated(string hostName, string userName, string password, int port)
        : base("Exchange.CatalogItemUpdated", hostName, userName, password, port)
    {
    }
}

