namespace CRUDTests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            MyMath mm = new MyMath(); 
            double x1= 10, y1= 5;

            double sum = mm.add(x1,y1);

            Assert.Equal(15,sum);
        }
    }
}
