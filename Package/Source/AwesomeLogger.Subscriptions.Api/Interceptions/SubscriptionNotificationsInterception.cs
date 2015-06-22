using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AwesomeLogger.Subscriptions.Api.DAL;
using AwesomeLogger.Subscriptions.Api.Events;
using Microsoft.Practices.Unity.InterceptionExtension;

namespace AwesomeLogger.Subscriptions.Api.Interceptions
{
    internal class SubscriptionNotificationsInterception : IInterceptionBehavior
    {
        private readonly IEventEmitter _eventEmitter;

        public SubscriptionNotificationsInterception(IEventEmitter eventEmitter)
        {
            _eventEmitter = eventEmitter;
        }

        public IMethodReturn Invoke(IMethodInvocation input, GetNextInterceptionBehaviorDelegate getNext)
        {
            var result = getNext()(input, getNext);

            // after method call
            if (result.Exception != null)
            {
                return result;
            }

            if (input.MethodBase.Name == "AddAsync")
            {
                var sub = (Subscription) input.Arguments[0];
                NotifyInBackground(sub);
            }
            else if (input.MethodBase.Name == "UpdateAsync")
            {
                // notify original machine
                var sub = (Subscription) input.Arguments[0];
                NotifyInBackground(sub);

                // notify new machine
                var resultSubTask = (Task<Subscription>) result.ReturnValue;
                resultSubTask.ContinueWith(t =>
                {
                    var resultSub = t.Result;
                    if (resultSub.MachineName != sub.MachineName)
                    {
                        NotifyInBackground(resultSub);
                    }
                }, TaskContinuationOptions.OnlyOnRanToCompletion);
            }
            else if (input.MethodBase.Name == "DeleteAsync")
            {
                var sub = (Subscription) input.Arguments[0];
                NotifyInBackground(sub);
            }

            return result;
        }

        public IEnumerable<Type> GetRequiredInterfaces()
        {
            return Type.EmptyTypes;
        }

        public bool WillExecute
        {
            get { return true; }
        }

        private void NotifyInBackground(Subscription sub)
        {
            Task.Run(() =>
            {
                _eventEmitter.EmitAsync(new Dictionary<string, string>
                {
                    {"MachineName", sub.MachineName},
                    {"Type", "Update"}
                });
            });
        }
    }
}