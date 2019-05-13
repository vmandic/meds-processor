using System;
using MedsProcessor.Common;

namespace MedsProcessor.WebAPI.Core
{
	public class HzzoDataProcessorStatus
	{
		public HzzoDataProcessorStatus(
			bool wasDataLoaded,
			ProcessorState processorState,
			int timesRan)
		{
			this.IsDataLoaded = wasDataLoaded;
			this.TimesProcessorHasRan = timesRan;
			this.ProcessorState = processorState;
		}

		public HzzoDataProcessorStatus(
			bool wasDataLoaded,
			ProcessorState processorState,
			int timesRan,
			DateTime? lastRunFinishedOn,
			TimeSpan? lastRunDuration) : this(wasDataLoaded, processorState, timesRan)
		{
			this.LastRunFinishedOn = lastRunFinishedOn;
			this.LastRunDuration = lastRunDuration;
		}

		public bool IsDataLoaded { get; }
		public DateTime? LastRunStartedOn =>
			LastRunFinishedOn.HasValue
				? LastRunFinishedOn.Value.Add(LastRunDuration.Value)
				: (DateTime?) null;

		public TimeSpan? LastRunStartedBefore =>
			LastRunFinishedOn.HasValue
				? DateTime.Now.Subtract(LastRunStartedOn.Value)
				: (TimeSpan?) null;

		public DateTime? LastRunFinishedOn { get; }
		public TimeSpan? LastRunDuration { get; }
		public int TimesProcessorHasRan { get; }
		public ProcessorState ProcessorState { get; }
	}
}