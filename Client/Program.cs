using Contract;
using log4net.Config;
using MassTransit;
using MassTransit.Log4NetIntegration.Logging;
using System;
using System.Configuration;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
	class Program
	{
		static void Main(string[] args)
		{
			ConfigureLogger();

			Log4NetLogger.Use();

			var busControl = CreateBus();

			busControl.Start();

			try
			{
				while(true)
				{
					Console.Write("Enter number: (quit exits): ");
					string input = Console.ReadLine();
					if (input == "quit")
					{
						break;
					}

					var number = int.Parse(input);

					Task.Run(async () =>
					{
						 await busControl.Publish<NumberUpdated>(new NumberUpdatedEvent(number));
					}).Wait();
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			finally
			{
				busControl.Stop();
			}
		}

		static void ConfigureLogger()
		{
			const string logConfig = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
									<log4net>
									  <root>
										<level value=""INFO"" />
										<appender-ref ref=""console"" />
									  </root>
									  <appender name=""console"" type=""log4net.Appender.ColoredConsoleAppender"">
										<layout type=""log4net.Layout.PatternLayout"">
										  <conversionPattern value=""%m%n"" />
										</layout>
									  </appender>
									</log4net>";

			using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(logConfig)))
			{
				XmlConfigurator.Configure(stream);
			}
		}

		static IBusControl CreateBus()
		{
			return Bus.Factory.CreateUsingRabbitMq(x => x.Host(new Uri(ConfigurationManager.AppSettings["RabbitMQHost"]), h =>
			{
				h.Username("guest");
				h.Password("guest");
			}));
		}

		class NumberUpdatedEvent : NumberUpdated
		{
			private readonly int newNumber;
			public int NewNumber
			{
				get { return this.newNumber; }
			}

			public NumberUpdatedEvent(int newNumber)
			{
				this.newNumber = newNumber;
			}
		}
	}
}
