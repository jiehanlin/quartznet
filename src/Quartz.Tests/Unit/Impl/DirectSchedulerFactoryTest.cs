/* 
 * Copyright 2004-2006 OpenSymphony 
 * 
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not 
 * use this file except in compliance with the License. You may obtain a copy 
 * of the License at 
 * 
 *   http://www.apache.org/licenses/LICENSE-2.0 
 *   
 * Unless required by applicable law or agreed to in writing, software 
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT 
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the 
 * License for the specific language governing permissions and limitations 
 * under the License.
 */
using System.Collections;
using System.Text;
using System.Threading;

using NUnit.Framework;

using Quartz.Impl;
using Quartz.Simpl;
using Quartz.Spi;

namespace Quartz.Tests.Unit.Impl
{
	[TestFixture]
	public class DirectSchedulerFactoryTest
	{
		[Test]
		public void TestPlugins()
		{
			StringBuilder result = new StringBuilder();

			IDictionary data = new Hashtable();
			data["TestPlugin"] = new TestPlugin(result);

			IThreadPool threadPool = new SimpleThreadPool(1, ThreadPriority.Normal);
			threadPool.Initialize();
			DirectSchedulerFactory.Instance.CreateScheduler(
				"MyScheduler", "Instance1", threadPool,
				new RAMJobStore(), data, 
				0, 0);
            

			IScheduler scheduler = DirectSchedulerFactory.Instance.GetScheduler("MyScheduler");
			scheduler.Start();
			scheduler.Shutdown();

			Assert.AreEqual("TestPlugin|MyScheduler|Start|Shutdown", result.ToString());
		}

		class TestPlugin : ISchedulerPlugin
		{
			StringBuilder result;

			public TestPlugin(StringBuilder result)
			{
				this.result = result;
			}

			public void Initialize(string name, IScheduler scheduler)
			{
				result.Append(name).Append("|").Append(scheduler.SchedulerName);
			}

			public void Start()
			{
				result.Append("|Start");
			}

			public void Shutdown()
			{
				result.Append("|Shutdown");
			}
		}
	}
}