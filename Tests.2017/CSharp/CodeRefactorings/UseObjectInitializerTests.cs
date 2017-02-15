using RefactoringEssentials.CSharp.CodeRefactorings;
using Xunit;

namespace RefactoringEssentials.Tests.CSharp.CodeRefactorings
{
    public class UseObjectInitializerTests : CSharpCodeRefactoringTestBase
    {
        [Fact]
        public void TestSimpleCase()
        {
            Test<UseObjectInitializerCodeRefactoringProvider>(@"
class TestClass
{   
    class SomeObject
    {
        public int X { get; set; }
        public int Y { get; set; }
    }

    void TestMethod()
    {
        SomeObject obj;
        obj = $new SomeObject();
        obj.X = 10;
        obj.Y = 20;
    }
}", @"
class TestClass
{   
    class SomeObject
    {
        int X { get; set; }
        int Y { get; set; }
    }

    void TestMethod()
    {
        var obj = new SomeObject
        {
            X = 10;
            Y = 20;
        };
    }
}");
        }
    }
}
