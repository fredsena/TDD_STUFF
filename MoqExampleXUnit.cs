using Moq;
using Xunit;
using System;

namespace MoqTests
{
    public class MoqExampleXUnit 
    {
        private Order order;
        private Mock<IFileWriter> mockFileWriter;
        private OrderWriter orderWriter;

        [Fact]
        public void It_should_pass_data_to_file_writer()
        {
            //Discaimer: Code and comments based on:
            //http://www.codethinked.com/beginning-mocking-with-moq-3-part-1
            /*
                1. Create an instance of Moq’s “Mock” class that takes a interface or class with 
                    virtual methods as a generic parameter.
                2. Tell the Mock what you want to happen, specifying parameters, and even 
                    return values. We will get deeper into this later on.
                3. Get back the mocked interface which will behave in the way in which you told it 
                    to. Use this mocked interface in your code.
                4. Call “Verify” or “VerifyAll” on the mock to ensure that what you said would 
                    happen actually happened. This step is not always required.   
                    We are merely telling Moq what to look for, not running any code.
            */
            
            //Setup
            order = new Order();
            order.OrderId = 1001;
            order.OrderDescription = "test";

            //Arrange
            mockFileWriter = new Mock<IFileWriter>();
            mockFileWriter.Setup(mock => mock.WriteLine("1001,test"));

            //Act
            orderWriter = new OrderWriter(mockFileWriter.Object);
            orderWriter.WriteOrder(order);

            //Assert
            mockFileWriter.Verify(fw => fw.WriteLine("1001,test"), Times.Exactly(1));
            mockFileWriter.Verify(fw => fw.WriteLine(It.IsAny<string>()), Times.Exactly(1));
            mockFileWriter.Verify(fw => fw.WriteLine(It.IsRegex("^1001")), Times.Exactly(1));
            mockFileWriter.Verify(fw => fw.WriteLine(It.Is<string>(s => s.Length > 3)), Times.Exactly(1));
            mockFileWriter.Verify(fw => fw.WriteLine(It.Is<string>(s => s.StartsWith("10"))), Times.Exactly(1));
            
            //mockFileWriter.VerifyAll();
            //TODO: test "IsInRange" (will check to see if a numerical value is within a particular range.)
        }
    }
    
    public interface IFileWriter
    {
        void WriteLine(string line);   
    }    
    
    public class Order
    {
        public int OrderId { get; set; }
        public string OrderDescription { get; set; }
    }    
    
    public class OrderWriter
    {
        private readonly IFileWriter fileWriter;

        public OrderWriter(IFileWriter fileWriter)
        {
            this.fileWriter = fileWriter;
        }

        public void WriteOrder(Order order)
        {
            fileWriter.WriteLine(String.Format("{0},{1}", order.OrderId, order.OrderDescription));
        }
    }
}


