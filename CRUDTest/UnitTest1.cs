namespace CRUDTest
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            //Arrange
            MyMath myMath = new MyMath();
            int input1 = 10;
            int input2 = 5;
            int expectedValue = 15;

            //Act
            int actualValue = myMath.Add(input1, input2);

            //Assert
            Assert.Equal(expectedValue, actualValue);
        }
    }
}