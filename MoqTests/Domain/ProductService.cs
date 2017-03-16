using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoqTests.Domain
{
    public class ProductService
    {
        private IProductRepository _productRepository;
        private IProductIdBuilder _productIdBuilder;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public ProductService(IProductRepository productRepository, IProductIdBuilder productIdBuilder) 
        {
            this._productIdBuilder = productIdBuilder;
            _productRepository = productRepository;
        }

        public void Create(ProductViewModel productViewModel)
        {
            Product product = ConvertToDomain(productViewModel);

            product.Identifier = _productIdBuilder.BuildProductIdentifier();
            if (product.Identifier == null)
            {
                throw new InvalidProductIdException();
            }

            _productRepository.Save(product);
        }

        private Product ConvertToDomain(ProductViewModel productViewModel)
        {
            return new Product(productViewModel.Name, productViewModel.Description);
        }

        public void CreateMany(List<ProductViewModel> productViewModels)
        {
            foreach (ProductViewModel vm in productViewModels)
            {
                _productRepository.Save(new Product(vm.Name, vm.Description));
            }
        }
    }
}
