using Catalog.API.Entities;
using Catalog.API.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Catalog.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CatalogController : ControllerBase
    {
        private readonly IProductRepository _repository;
        private readonly ILogger<CatalogController> _logger;

        public CatalogController(IProductRepository repository, ILogger<CatalogController> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// GetProducts - Method used to return the list of Products
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            var products = await _repository.GetProducts();
            return Ok(products);
        }

        /// <summary>
        /// GetProduct - Method used to return the product by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id:length(24)}", Name ="GetProduct")]
        public async Task<IActionResult> GetProductById(string id)
        {
            var product = await _repository.GetProductById(id);

            if(product == null)
            {
                _logger.LogError($"product with id: {id}, not found");
                return NotFound();
            }
            return Ok(product);
        }

        /// <summary>
        /// GetProductByCategory - Method used to return the product by category name
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        [Route("[action]/{category}", Name = "GetProductByCategory")]
        [HttpGet]
        public async Task<IActionResult> GetProductByCategory(string category)
        {
            var product = await _repository.GetProductByCategory(category);
            if (product == null)
            {
                _logger.LogError($"product with category: {category}, not found");
                return NotFound();
            }
            return Ok(product);
        }

        /// <summary>
        /// CreateProduct - Method used to create the new product
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> CreateProduct([FromBody] Product product)
        {
            await _repository.CreateProduct(product);

            return CreatedAtRoute("GetProduct", new { id = product.Id }, product);
        }

        /// <summary>
        /// UpdateProduct - Method used to update the product
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IActionResult> UpdateProduct([FromBody] Product product)
        {
            return Ok(await _repository.UpdateProduct(product));
        }

        /// <summary>
        /// DeleteProduct - Method used to delete the product
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id:length(24)}", Name = "DeleteProduct")]
        public async Task<IActionResult> DeleteProductById(string id)
        {
            return Ok(await _repository.DeleteProduct(id));
        }
    }
}
