using System;
using MedsProcessor.Common;

namespace MedsProcessor.Common.Models
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
				? LastRunStartedOn.Value.Subtract(DateTime.Now)
				: (TimeSpan?) null;

		public DateTime? LastRunFinishedOn { get; }
		public TimeSpan? LastRunDuration { get; }
		public int TimesProcessorHasRan { get; }
		public ProcessorState ProcessorState { get; }
	}
}