using System;
using NUnit.Framework;

namespace EventBuster.UnitTests
{
    public class DiscoverFixture
    {
        [Test]
        public void ValidateGenericMethod()
        {
            Assert.Throws<InvalidOperationException>(EventBus.Register<GenericActionTarget>);
        }

        [Test]
        public void ValidateOutParameter()
        {
            Assert.Throws<InvalidOperationException>(EventBus.Register<OutParameterActionTarget>);
        }

        [Test]
        public void ValidateRefParameter()
        {
            Assert.Throws<InvalidOperationException>(EventBus.Register<RefParameterActionTarget>);
        }

        [Test]
        public void ValidateMultipleParameter()
        {
            Assert.Throws<InvalidOperationException>(EventBus.Register<MultipleParameterActionTarget>);
        }

        [Test]
        public void ValidateInvalidReturnSyncMethod()
        {
            Assert.Throws<InvalidOperationException>(EventBus.Register<InvalidReturnSyncTarget>);
        }

        [Test]
        public void ValidateInvalidReturnAsyncMethod()
        {
            Assert.Throws<InvalidOperationException>(EventBus.Register<InvalidReturnAsyncTarget>);
        }

        [Test]
        public void DiscoverLegalActions()
        {
            Assert.NotThrow(EventBus.Register<LegalEventTarget>);
        }
    }
}
