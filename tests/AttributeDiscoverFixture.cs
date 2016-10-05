using System;

namespace EventBuster.UnitTests
{
    public class AttributeDiscoverFixture
    {
#if NetCore
        [Xunit.Fact]
#else
        [NUnit.Framework.Test]
#endif
        public void ValidateGenericMethod()
        {
            Assert.Throws<InvalidOperationException>(EventBus.Register<GenericActionTarget>);
        }

#if NetCore
        [Xunit.Fact]
#else
        [NUnit.Framework.Test]
#endif
        public void ValidateOutParameter()
        {
            Assert.Throws<InvalidOperationException>(EventBus.Register<OutParameterActionTarget>);
        }

#if NetCore
        [Xunit.Fact]
#else
        [NUnit.Framework.Test]
#endif
        public void ValidateRefParameter()
        {
            Assert.Throws<InvalidOperationException>(EventBus.Register<RefParameterActionTarget>);
        }

#if NetCore
        [Xunit.Fact]
#else
        [NUnit.Framework.Test]
#endif
        public void ValidateMultipleParameter()
        {
            Assert.Throws<InvalidOperationException>(EventBus.Register<MultipleParameterActionTarget>);
        }

#if NetCore
        [Xunit.Fact]
#else
        [NUnit.Framework.Test]
#endif
        public void ValidateInvalidReturnSyncMethod()
        {
            Assert.Throws<InvalidOperationException>(EventBus.Register<InvalidReturnSyncTarget>);
        }

#if !Net35
         
#if NetCore
        [Xunit.Fact]
#else
        [NUnit.Framework.Test]
#endif
        public void ValidateInvalidReturnAsyncMethod()
        {
            Assert.Throws<InvalidOperationException>(EventBus.Register<InvalidReturnAsyncTarget>);
        }

#endif

#if NetCore
        [Xunit.Fact]
#else
        [NUnit.Framework.Test]
#endif
        public void DiscoverLegalActions()
        {
            Assert.NotThrow(EventBus.Register<LegalEventTarget>);
        }
    }
}
