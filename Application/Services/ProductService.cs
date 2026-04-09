using Application.Common;
using Application.DTOs.Product;
using Application.Services.Interfaces;
using AutoMapper;
using Core.Entities;
using Core.Repositories;
using FluentValidation;

namespace Application.Services;

public sealed class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateProductDto> _createValidator;
    private readonly IValidator<UpdateProductDto> _updateValidator;

    public ProductService(
        IProductRepository productRepository,
        IMapper mapper,
        IValidator<CreateProductDto> createValidator,
        IValidator<UpdateProductDto> updateValidator)
    {
        _productRepository = productRepository;
        _mapper = mapper;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<PagedResponse<ProductResponseDto>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var products = await _productRepository.GetAllAsync(cancellationToken);
        var total = products.Count();
        var paged = products.Skip((page - 1) * pageSize).Take(pageSize);
        var dtos = _mapper.Map<IEnumerable<ProductResponseDto>>(paged);
        return PagedResponse<ProductResponseDto>.Ok(dtos, page, pageSize, total);
    }

    public async Task<ApiResponse<ProductResponseDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetByIdAsync(id, cancellationToken);
        if (product is null)
            return ApiResponse<ProductResponseDto>.Fail("Produto não encontrado.");

        return ApiResponse<ProductResponseDto>.Ok(_mapper.Map<ProductResponseDto>(product));
    }

    public async Task<ApiResponse<IEnumerable<ProductResponseDto>>> GetInStockAsync(CancellationToken cancellationToken = default)
    {
        var products = await _productRepository.GetInStockAsync(cancellationToken);
        return ApiResponse<IEnumerable<ProductResponseDto>>.Ok(_mapper.Map<IEnumerable<ProductResponseDto>>(products));
    }

    public async Task<ApiResponse<ProductResponseDto>> CreateAsync(CreateProductDto dto, CancellationToken cancellationToken = default)
    {
        await _createValidator.ValidateAndThrowAsync(dto, cancellationToken);

        var existing = await _productRepository.GetByNameAsync(dto.Name, cancellationToken);
        if (existing is not null)
            return ApiResponse<ProductResponseDto>.Fail("Já existe um produto com este nome.");

        var product = _mapper.Map<Product>(dto);
        var created = await _productRepository.AddAsync(product, cancellationToken);
        return ApiResponse<ProductResponseDto>.Ok(_mapper.Map<ProductResponseDto>(created), "Produto criado com sucesso.");
    }

    public async Task<ApiResponse<ProductResponseDto>> UpdateAsync(Guid id, UpdateProductDto dto, CancellationToken cancellationToken = default)
    {
        await _updateValidator.ValidateAndThrowAsync(dto, cancellationToken);

        var product = await _productRepository.GetByIdAsync(id, cancellationToken);
        if (product is null)
            return ApiResponse<ProductResponseDto>.Fail("Produto não encontrado.");

        _mapper.Map(dto, product);
        product.UpdatedAt = DateTime.UtcNow;

        var updated = await _productRepository.UpdateAsync(product, cancellationToken);
        return ApiResponse<ProductResponseDto>.Ok(_mapper.Map<ProductResponseDto>(updated), "Produto atualizado com sucesso.");
    }

    public async Task<ApiResponse<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var exists = await _productRepository.ExistsAsync(id, cancellationToken);
        if (!exists)
            return ApiResponse<bool>.Fail("Produto não encontrado.");

        await _productRepository.DeleteAsync(id, cancellationToken);
        return ApiResponse<bool>.Ok(true, "Produto removido com sucesso.");
    }
}
