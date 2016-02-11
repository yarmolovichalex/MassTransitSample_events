using MassTransit;
using MassTransit.RabbitMqTransport;
using System;
using Topshelf;
using Topshelf.Logging;
using System.Configuration;

namespace Service
{
	public class Service : ServiceControl
	{
		private readonly LogWriter log = HostLogger.Get<Service>();

		private IBusControl busControl;

		public bool Start(HostControl hostControl)
		{
			this.log.Info("Creating bus...");

			this.busControl = Bus.Factory.CreateUsingRabbitMq(x =>
			{
				IRabbitMqHost host = x.Host(new Uri(ConfigurationManager.AppSettings["RabbitMQHost"]), h =>
				{
					h.Username("guest");
					h.Password("guest");
				});

				x.ReceiveEndpoint(host, e => { e.Consumer<Consumer>(); });
			});

			this.log.Info("Starting bus...");

			this.busControl.Start();

			return true;
		}

		public bool Stop(HostControl hostControl)
		{
			this.log.Info("Stopping bus...");

			this.busControl?.Stop();

			return true;
		}
	}
}
