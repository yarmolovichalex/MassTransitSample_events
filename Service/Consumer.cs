using System;
using System.Threading.Tasks;
using MassTransit;
using MassTransit.Logging;
using Contract;

namespace Service
{
	public class Consumer : IConsumer<NumberUpdated>
	{
		private readonly ILog log = Logger.Get<Consumer>();
		private readonly Random rand = new Random();

		public async Task Consume(ConsumeContext<NumberUpdated> context)
		{
			this.log.InfoFormat($"Received updated number: {context.Message.NewNumber}");

			await context.RespondAsync("test");
		}
	}
}
