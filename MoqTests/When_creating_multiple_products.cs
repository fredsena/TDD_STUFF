using System;
using Xunit;
using Moq;
using MoqTests.Domain;
using System.Collections.Generic;

namespace MoqTests
{

    public class When_creating_multiple_products
    {
        [Fact]
        public void TestMethod1()
        {
            var mockProductRepository = new Mock<IProductRepository>();

            mockProductRepository.Setup(p => p.Save(It.IsAny<Product>()));

            var service = new ProductService(mockProductRepository.Object);

            service.Create(new ProductViewModel());

            mockProductRepository.VerifyAll();
        }

        [Fact]
        public void Then_product_repository_should_be_called_once_per_product()
        {

            var productViewModels = new List<ProductViewModel>()
            {
                new ProductViewModel(){Name = "ProductA", Description="Great product"}
                , new ProductViewModel(){Name = "ProductB", Description="Bad product"}
                , new ProductViewModel(){Name = "ProductC", Description="Cheap product"}
                , new ProductViewModel(){Name = "ProductD", Description="Expensive product"}
            };

            var mockProductRepository = new Mock<IProductRepository>();

            mockProductRepository.Setup(p => p.Save(It.IsAny<Product>()));

            var productService = new ProductService(mockProductRepository.Object);            
            
            productService.CreateMany(productViewModels);
            
            mockProductRepository.VerifyAll();

            mockProductRepository.Verify(p => p.Save(
                It.IsAny<Product>()), 
                Times.Exactly(productViewModels.Count));
        }

        [Fact]        
        public void An_exception_should_be_thrown_if_id_is_not_created()
        {
            var productViewModel = new ProductViewModel() { Description = "Nice product", Name = "ProductA" };
            var mockIdBuilder = new Mock<IProductIdBuilder>();
            var mockProductRepository = new Mock<IProductRepository>();

            mockProductRepository.Setup(p => p.Save(It.IsAny<Product>())).Throws<InvalidProductIdException>();

            mockIdBuilder.Setup(x => x.BuildProductIdentifier()).Returns(new ProductIdentifier());

            mockIdBuilder.Setup(i => i.BuildProductIdentifier()).Returns(() => null);

            var productService = new ProductService(mockProductRepository.Object, mockIdBuilder.Object);

            Assert.Throws<InvalidProductIdException>(() => productService.Create(productViewModel));

        }

        [Fact]
        public void The_product_should_be_saved_if_id_was_created()
        {
            
            var productViewModel = new ProductViewModel() { Description = "Nice product", Name = "ProductA" };

            var mockProductRepository = new Mock<IProductRepository>();

            var mockIdBuilder = new Mock<IProductIdBuilder>();

            mockIdBuilder.Setup(i => i.BuildProductIdentifier()).Returns(new ProductIdentifier());

            var productService = new ProductService(mockProductRepository.Object, mockIdBuilder.Object);
            
            productService.Create(productViewModel);
            
            mockProductRepository.Verify(p => p.Save(It.IsAny<Product>()));
        }

    }
}
