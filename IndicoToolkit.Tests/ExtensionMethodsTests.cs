using Xunit;
using IndicoToolkit;
using System;
using System.Collections.Generic;

namespace IndicoToolkit.Tests
{
    [Serializable]
    public class ExtensionMethodsTests
    {

        [Fact]
        public void TestDeepCopy()
        {
            
            List<string> A = new List<string>()
            {
                "one",
                "two",
                "three"
            };
            List<string> B = A.CloneList();
            Assert.Equal(A[0], B[0]);
            Assert.Equal(A[1], B[1]);
            Assert.Equal(A[2], B[2]);
            A.Add("four");
            Assert.NotEqual(A, B);
        }
    }
}