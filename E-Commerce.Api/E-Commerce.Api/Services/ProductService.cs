using AutoMapper;
using E_Commerce.Api.DTOs;
using E_Commerce.Api.DTOs.ProductDTOs;
using E_Commerce.Api.Models;
using E_Commerce.Api.Repository.Interfaces;
using E_Commerce.Api.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace E_Commerce.Api.Services
{
    public class ProductService : IProductService
    {
        //  private readonly IProductRepository _productRepo;
        private readonly IMapper _mapper;

        private readonly IMemoryCache _cache;
        private readonly TimeSpan _timeOut = TimeSpan.FromMinutes(30);
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(IUnitOfWork unitOfWork, IMapper mapper, IMemoryCache cache)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<ApiResponse<ProductResponseDTO>> CreateProductAsync(ProductCreateDTO dto)
        {
            try
            {
                if (await _unitOfWork.Products.ProductNameExistsAsync(dto.Name))
                    return new ApiResponse<ProductResponseDTO>(400, "Product name already exists.");

                if (!await _unitOfWork.Products.CategoryExistsAsync(dto.CategoryId))
                    return new ApiResponse<ProductResponseDTO>(400, "Category does not exist.");

                Product product = _mapper.Map<Product>(dto);
                product.IsAvailable = true;

                await _unitOfWork.Products.CreateAsync(product);
                await _unitOfWork.SaveChangesAsync();

                ProductResponseDTO response = _mapper.Map<ProductResponseDTO>(product);
                return new ApiResponse<ProductResponseDTO>(201, response);
            }
            catch (Exception ex)
            {
                return new ApiResponse<ProductResponseDTO>(500, ex.Message);
            }
        }

        public async Task<ApiResponse<ProductResponseDTO>> GetProductByIdAsync(int id)
        {
            try
            {
                Product product = await _unitOfWork.Products.GetProductByIdAsync(id);
                if (product == null)
                    return new ApiResponse<ProductResponseDTO>(404, "Product not found");

                return new ApiResponse<ProductResponseDTO>(200, _mapper.Map<ProductResponseDTO>(product));
            }
            catch (Exception ex)
            {
                return new ApiResponse<ProductResponseDTO>(500, ex.Message);
            }
        }

        public async Task<ApiResponse<List<ProductResponseDTO>>> GetAllProductsAsync()
        {
            try
            {
                var products = await _unitOfWork.Products.GetAllAsync();
                var result = _mapper.Map<List<ProductResponseDTO>>(products);

                return new ApiResponse<List<ProductResponseDTO>>(200, result);
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<ProductResponseDTO>>(500, ex.Message);
            }
        }

        public async Task<ApiResponse<List<ProductResponseDTO>>> GetProductsByCategoryAsync(int categoryId)
        {
            string cacheKey = $"categoryId_{categoryId}";
            MemoryCacheEntryOptions options = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(_timeOut)
                .SetPriority(CacheItemPriority.Normal);

            try
            {
                if (!_cache.TryGetValue(cacheKey, out var products))
                {
                    products = await _unitOfWork.Products.GetProductsByCategoryAsync(categoryId);
                    if (products == null)
                        return new ApiResponse<List<ProductResponseDTO>>(404, "No products found.");
                    _cache.Set(cacheKey, products, options);
                }

                return new ApiResponse<List<ProductResponseDTO>>(200,
                    _mapper.Map<List<ProductResponseDTO>>(products));
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<ProductResponseDTO>>(500, ex.Message);
            }
        }

        public async Task<ApiResponse<ConfirmationResponseDTO>> UpdateProductAsync(ProductUpdateDTO dto)
        {
            try
            {
                Product product = await _unitOfWork.Products.GetProductByIdAsync(dto.Id);
                if (product == null)
                    return new ApiResponse<ConfirmationResponseDTO>(404, "Product not found.");

                if (await _unitOfWork.Products.ProductNameExistsAsync(dto.Name, dto.Id))
                    return new ApiResponse<ConfirmationResponseDTO>(400, "Product name already exists.");

                if (!await _unitOfWork.Products.CategoryExistsAsync(dto.CategoryId))
                    return new ApiResponse<ConfirmationResponseDTO>(400, "Category does not exist.");

                _mapper.Map(dto, product);
                _unitOfWork.Products.Update(product);
                await _unitOfWork.SaveChangesAsync();

                return new ApiResponse<ConfirmationResponseDTO>(200,
                    new ConfirmationResponseDTO { Message = "Product updated successfully." });
            }
            catch (Exception ex)
            {
                return new ApiResponse<ConfirmationResponseDTO>(500, ex.Message);
            }
        }

        public async Task<ApiResponse<ConfirmationResponseDTO>> UpdateProductStatusAsync(ProductStatusUpdateDTO dto)
        {
            try
            {
                Product product = await _unitOfWork.Products.GetProductByIdAsync(dto.Id);
                if (product == null)
                    return new ApiResponse<ConfirmationResponseDTO>(404, "Product not found.");

                product.IsAvailable = dto.IsAvailable;
                _unitOfWork.Products.Update(product);
                await _unitOfWork.SaveChangesAsync();
                return new ApiResponse<ConfirmationResponseDTO>(200,
                    new ConfirmationResponseDTO { Message = "Product status updated." });
            }
            catch (Exception ex)
            {
                return new ApiResponse<ConfirmationResponseDTO>(500, ex.Message);
            }
        }

        public async Task<ApiResponse<ConfirmationResponseDTO>> DeleteProductAsync(int id)
        {
            try
            {
                Product product = await _unitOfWork.Products.GetProductByIdAsync(id);
                if (product == null)
                    return new ApiResponse<ConfirmationResponseDTO>(404, "Product not found.");

                product.IsAvailable = false;
                _unitOfWork.Products.Delete(product);
                await _unitOfWork.SaveChangesAsync();
                return new ApiResponse<ConfirmationResponseDTO>(200,
                    new ConfirmationResponseDTO { Message = "Product deleted successfully." });
            }
            catch (Exception ex)
            {
                return new ApiResponse<ConfirmationResponseDTO>(500, ex.Message);
            }
        }
    }
}