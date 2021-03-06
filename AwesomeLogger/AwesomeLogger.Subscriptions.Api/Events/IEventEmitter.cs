﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace AwesomeLogger.Subscriptions.Api.Events
{
    internal interface IEventEmitter
    {
        Task EmitAsync(Dictionary<string, string> properties);

        Task EmitAsync(string message, Dictionary<string, string> properties);
    }
}