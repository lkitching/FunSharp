using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunSharp.Tasks
{
    [TestFixture]
    public class TaskOfTests
    {
        /// <summary>Tests a failed task is created with the given inner exception.</summary>
        [Test]
        public void ShouldCreateFailedTask()
        {
            var ex = new InvalidOperationException("!!!");
            var task = TaskOf.Failed<string>(ex);

            Assert.IsTrue(task.IsFaulted, "Created task not faulted");
            Assert.AreEqual(task.Exception.InnerException, ex, "Unexpected inner exception for failed task");
        }

        /// <summary>Tests a canceled task is created.</summary>
        [Test]
        public void ShouldCreateCanceledTask()
        {
            var task = TaskOf.Canceled<int>();
            Assert.IsTrue(task.IsCanceled, "Created task is not canceled");
        }
    }
}
