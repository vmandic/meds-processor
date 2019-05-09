namespace MedsProcessor.Common.Extensions
{
	public static class EnumExtensions
	{
		public static T2 Parse<T1, T2>(string input) =>
			(T2) System.Enum.Parse(typeof(T1), input);
	}
}