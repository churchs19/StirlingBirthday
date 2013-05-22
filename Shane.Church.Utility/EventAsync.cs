using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Shane.Church.Utility
{
	public static class EventAsync
	{
		// TODO: We're skipping a *lot* of error checking here.
		private sealed class EventHandlerTask<TEventArgs>
		{
			private readonly TaskCompletionSource<TEventArgs> tcs;
			private readonly Delegate subscription;
			private readonly object target;
			private readonly EventInfo eventInfo;

			public EventHandlerTask(object target, string eventName)
			{
				this.tcs = new TaskCompletionSource<TEventArgs>(TaskCreationOptions.AttachedToParent);
				this.target = target;
				this.eventInfo = target.GetType().GetEvent(eventName);
				this.subscription = Delegate.CreateDelegate(this.eventInfo.EventHandlerType, this, "EventCompleted");
				this.eventInfo.AddEventHandler(target, this.subscription);
			}

			public Task<TEventArgs> Task
			{
				get { return tcs.Task; }
			}

			private void EventCompleted(object sender, TEventArgs args)
			{
				this.eventInfo.RemoveEventHandler(this.target, this.subscription);
				this.tcs.SetResult(args);
			}
		}

		public static Task<TEventArgs> FromEvent<TEventArgs>(object target, string eventName) where TEventArgs : EventArgs
		{
			return new EventHandlerTask<TEventArgs>(target, eventName).Task;
		}
	}
}