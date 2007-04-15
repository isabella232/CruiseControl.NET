using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Config;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.UnitTests.Core
{
	[TestFixture]
	public class IntegrationQueueManagerTest
	{
		private const string TestQueueName = "ProjectQueueOne";
		private const string TestQueueName2 = "ProjectQueueTwo";
		private const string TestQueueName3 = "ProjectQueueThree";

		private Project project;
		private Configuration configuration;
		private IntegrationQueueManager queueManager;

		[SetUp]
		public void SetUp()
		{
			project = new Project();
			project.Name = TestQueueName;

			configuration = new Configuration();
			configuration.AddProject(project);

			queueManager = new IntegrationQueueManager(new ProjectIntegratorListFactory(), configuration);
		}

		[Test]
		public void QueueNamesShouldBePopulatedFromProjectList()
		{
			queueManager = new IntegrationQueueManager(new ProjectIntegratorListFactory(), configuration);
			string[] queueNames = queueManager.GetQueueNames();
			Assert.AreEqual(1, queueNames.Length);
			Assert.AreEqual(project.Name, queueNames[0]);
		}

		[Test]
		public void StopAllProjectsRemovesAllKnownQueueNames()
		{
			queueManager = new IntegrationQueueManager(new ProjectIntegratorListFactory(), configuration);
			string[] queueNames = queueManager.GetQueueNames();
			Assert.AreEqual(1, queueNames.Length);

			queueManager.StopAllProjects();

			queueNames = queueManager.GetQueueNames();
			Assert.AreEqual(0, queueNames.Length);
		}

		[Test]
		public void GetQueueNamesOrderedAlphabetically()
		{
			Project project2 = new Project();
			project2.Name = TestQueueName2;
			Project project3 = new Project();
			project3.Name = TestQueueName3;
			configuration.AddProject(project2);
			configuration.AddProject(project3);

			queueManager = new IntegrationQueueManager(new ProjectIntegratorListFactory(), configuration);
			string[] queueNames = queueManager.GetQueueNames();
			Assert.AreEqual(TestQueueName, queueNames[0]);
			Assert.AreEqual(TestQueueName3, queueNames[1]);
			Assert.AreEqual(TestQueueName2, queueNames[2]);
		}

		[Test]
		public void EmptyIntegrationQueueReportsNamesCorrectly()
		{
			queueManager = new IntegrationQueueManager(new ProjectIntegratorListFactory(), new Configuration());
			string[] queueNames = queueManager.GetQueueNames();
			Assert.AreEqual(0, queueNames.Length);
		}

        [Test]
        public void GetCruiseServerSnapshotWithNoProjects()
        {
            // Remove the project added in the test setup
            configuration.DeleteProject(TestQueueName);

            queueManager = new IntegrationQueueManager(new ProjectIntegratorListFactory(), configuration);
            CruiseServerSnapshot cruiseServerSnapshot = queueManager.GetCruiseServerSnapshot();
            Assert.IsNotNull(cruiseServerSnapshot);
            Assert.AreEqual(0, cruiseServerSnapshot.ProjectStatuses.Length);
            Assert.IsNotNull(cruiseServerSnapshot.QueueSetSnapshot);
            Assert.AreEqual(0, cruiseServerSnapshot.QueueSetSnapshot.Queues.Count);
        }

        [Test]
        public void GetCruiseServerSnapshotWithProjectsAdded()
        {
            Project project2 = new Project();
            project2.Name = TestQueueName2;
            Project project3 = new Project();
            project3.Name = TestQueueName3;
            configuration.AddProject(project2);
            configuration.AddProject(project3);

            queueManager = new IntegrationQueueManager(new ProjectIntegratorListFactory(), configuration);
            CruiseServerSnapshot cruiseServerSnapshot = queueManager.GetCruiseServerSnapshot();
            Assert.IsNotNull(cruiseServerSnapshot);
            Assert.AreEqual(TestQueueName, cruiseServerSnapshot.ProjectStatuses[0].Name);
            Assert.AreEqual(TestQueueName2, cruiseServerSnapshot.ProjectStatuses[1].Name);
            Assert.AreEqual(TestQueueName3, cruiseServerSnapshot.ProjectStatuses[2].Name);
        }
    }
}