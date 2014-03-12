using System;
using Moq;

namespace TestingExtensions
{
    public static class MoqExtensions
    {
        public static Mock<T> Mock<T>(this T mocked) where T : class
        {
            return Moq.Mock.Get<T>(mocked);
        }
    }
}
