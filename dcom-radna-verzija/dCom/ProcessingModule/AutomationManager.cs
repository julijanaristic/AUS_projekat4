using Common;
using System;
using System.Collections.Generic;
using System.Threading;

namespace ProcessingModule
{
    /// <summary>
    /// Class containing logic for automated work.
    /// </summary>
    public class AutomationManager : IAutomationManager, IDisposable
	{
		private Thread automationWorker;
        private AutoResetEvent automationTrigger;
        private IStorage storage;
		private IProcessingManager processingManager;
		private int delayBetweenCommands;
        private IConfiguration configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="AutomationManager"/> class.
        /// </summary>
        /// <param name="storage">The storage.</param>
        /// <param name="processingManager">The processing manager.</param>
        /// <param name="automationTrigger">The automation trigger.</param>
        /// <param name="configuration">The configuration.</param>
        public AutomationManager(IStorage storage, IProcessingManager processingManager, AutoResetEvent automationTrigger, IConfiguration configuration)
		{
			this.storage = storage;
			this.processingManager = processingManager;
            this.configuration = configuration;
            this.automationTrigger = automationTrigger;
        }

        /// <summary>
        /// Initializes and starts the threads.
        /// </summary>
		private void InitializeAndStartThreads()
		{
			InitializeAutomationWorkerThread();
			StartAutomationWorkerThread();
		}

        /// <summary>
        /// Initializes the automation worker thread.
        /// </summary>
		private void InitializeAutomationWorkerThread()
		{
			automationWorker = new Thread(AutomationWorker_DoWork);
			automationWorker.Name = "Aumation Thread";
		}

        /// <summary>
        /// Starts the automation worker thread.
        /// </summary>
		private void StartAutomationWorkerThread()
		{
			automationWorker.Start();
		}


		private void AutomationWorker_DoWork()
		{
			EGUConverter eguConverter = new EGUConverter();

			PointIdentifier analogOut = new PointIdentifier(PointType.ANALOG_OUTPUT, 1000);
			PointIdentifier digitalOut1 = new PointIdentifier(PointType.DIGITAL_OUTPUT, 3000);
			PointIdentifier digitalOut2 = new PointIdentifier(PointType.DIGITAL_OUTPUT, 3001);
			PointIdentifier digitalOut3 = new PointIdentifier(PointType.DIGITAL_OUTPUT, 4000); // v1
			PointIdentifier digitalOut4 = new PointIdentifier(PointType.DIGITAL_OUTPUT, 4001); // v2
			PointIdentifier digitalOut5 = new PointIdentifier(PointType.DIGITAL_OUTPUT, 4002); // v2
			PointIdentifier digitalOut6 = new PointIdentifier(PointType.DIGITAL_OUTPUT, 4003); // v4

			List<PointIdentifier> pointList = new List<PointIdentifier> { analogOut, digitalOut1, digitalOut2, digitalOut3, digitalOut4, digitalOut5, digitalOut6};

			while (!disposedValue)
			{
				List<IPoint> points = storage.GetPoints(pointList);
				int initValue = (int)eguConverter.ConvertToEGU(points[0].ConfigItem.ScaleFactor, points[0].ConfigItem.Deviation, points[0].RawValue); // pogledati posljednji parametar
				int value = initValue;

				if (points[1].RawValue == 1)
				{
					if (points[3].RawValue != 1)
					{
						processingManager.ExecuteWriteCommand(points[3].ConfigItem, configuration.GetTransactionId(), configuration.UnitAddress, 4000, 1);
					}

					int chocolate = 50;
					int milk = 50;
					int water = 30;

					if (points[3].RawValue == 1)
					{
						value += chocolate;
					}
					else if (points[4].RawValue == 1)
					{
						value += milk;
					}
					else if (points[5].RawValue == 1)
					{
						value += water;
					}

					if(value == 100)
					{
						if (points[3].RawValue != 0)
						{
							processingManager.ExecuteWriteCommand(points[3].ConfigItem, configuration.GetTransactionId(), configuration.UnitAddress, 4000, 0);
						}

						if (points[3].RawValue != 1)
						{
                            processingManager.ExecuteWriteCommand(points[3].ConfigItem, configuration.GetTransactionId(), configuration.UnitAddress, 4000, 1);
                        }
                    }
				}
			}
		}

		#region IDisposable Support
		private bool disposedValue = false; // To detect redundant calls


        /// <summary>
        /// Disposes the object.
        /// </summary>
        /// <param name="disposing">Indication if managed objects should be disposed.</param>
		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
				}
				disposedValue = true;
			}
		}


		// This code added to correctly implement the disposable pattern.
		public void Dispose()
		{
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(true);
			// GC.SuppressFinalize(this);
		}

        /// <inheritdoc />
        public void Start(int delayBetweenCommands)
		{
			this.delayBetweenCommands = delayBetweenCommands*1000;
            InitializeAndStartThreads();
		}

        /// <inheritdoc />
        public void Stop()
		{
			Dispose();
		}
		#endregion
	}
}
