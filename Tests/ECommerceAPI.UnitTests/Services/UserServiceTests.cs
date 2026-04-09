using Application.DTOs.User;
using Application.Services;
using Application.Validators.User;
using AutoMapper;
using Core.Entities;
using Core.Repositories;
using ECommerceAPI.UnitTests.Builders;
using ECommerceAPI.UnitTests.Fixtures;
using FluentValidation;
using NSubstitute;

namespace ECommerceAPI.UnitTests.Services;

public sealed class UserServiceTests : IClassFixture<AutoMapperFixture>
{
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly IMapper         _mapper;
    private readonly IValidator<CreateUserDto> _createValidator = new CreateUserDtoValidator();
    private readonly IValidator<UpdateUserDto> _updateValidator = new UpdateUserDtoValidator();
    private readonly UserService _sut;

    public UserServiceTests(AutoMapperFixture mapperFixture)
    {
        _mapper = mapperFixture.Mapper;
        _sut    = new UserService(_userRepository, _mapper, _createValidator, _updateValidator);
    }

    // ─── GetAllAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task GetAllAsync_WithUsers_ReturnsPagedResponse()
    {
        var users = new UserFaker().Generate(5);
        _userRepository.GetAllAsync(Arg.Any<CancellationToken>()).Returns(users);

        var result = await _sut.GetAllAsync(1, 10);

        Assert.True(result.Success);
        Assert.Equal(5, result.Data!.Count());
        Assert.Equal(1, result.Pagination.Page);
        Assert.Equal(5, result.Pagination.TotalCount);
    }

    [Fact]
    public async Task GetAllAsync_WithPagination_ReturnsCorrectPage()
    {
        var users = new UserFaker().Generate(10);
        _userRepository.GetAllAsync(Arg.Any<CancellationToken>()).Returns(users);

        var result = await _sut.GetAllAsync(2, 3);

        Assert.True(result.Success);
        Assert.Equal(3, result.Data!.Count());
        Assert.Equal(2, result.Pagination.Page);
    }

    // ─── GetByIdAsync ─────────────────────────────────────────────────────────

    [Fact]
    public async Task GetByIdAsync_WithExistingId_ReturnsSuccess()
    {
        var user = new UserFaker().Generate();
        _userRepository.GetByIdAsync(user.Id, Arg.Any<CancellationToken>()).Returns(user);

        var result = await _sut.GetByIdAsync(user.Id);

        Assert.True(result.Success);
        Assert.Equal(user.Email, result.Data!.Email);
    }

    [Fact]
    public async Task GetByIdAsync_WithNonExistingId_ReturnsFail()
    {
        _userRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((User?)null);

        var result = await _sut.GetByIdAsync(Guid.NewGuid());

        Assert.False(result.Success);
        Assert.Contains("não encontrado", result.Message, StringComparison.OrdinalIgnoreCase);
    }

    // ─── CreateAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task CreateAsync_WithValidDto_ReturnsSuccess()
    {
        var dto = DtoFakers.ValidCreateUserDto();
        _userRepository.GetByEmailAsync(dto.Email, Arg.Any<CancellationToken>()).Returns((User?)null);
        _userRepository.AddAsync(Arg.Any<User>(), Arg.Any<CancellationToken>())
                       .Returns(ci => ci.Arg<User>());

        var result = await _sut.CreateAsync(dto);

        Assert.True(result.Success);
        Assert.Equal(dto.Email, result.Data!.Email);
    }

    [Fact]
    public async Task CreateAsync_WithDuplicateEmail_ReturnsFail()
    {
        var dto          = DtoFakers.ValidCreateUserDto();
        var existingUser = new UserFaker().Generate();
        _userRepository.GetByEmailAsync(dto.Email, Arg.Any<CancellationToken>()).Returns(existingUser);

        var result = await _sut.CreateAsync(dto);

        Assert.False(result.Success);
        Assert.Contains("e-mail", result.Message, StringComparison.OrdinalIgnoreCase);
    }

    // ─── UpdateAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task UpdateAsync_WithExistingUser_ReturnsSuccess()
    {
        var user = new UserFaker().Generate();
        var dto  = DtoFakers.ValidUpdateUserDto();
        _userRepository.GetByIdAsync(user.Id, Arg.Any<CancellationToken>()).Returns(user);
        _userRepository.UpdateAsync(Arg.Any<User>(), Arg.Any<CancellationToken>())
                       .Returns(ci => ci.Arg<User>());

        var result = await _sut.UpdateAsync(user.Id, dto);

        Assert.True(result.Success);
        Assert.Equal(dto.Email, result.Data!.Email);
    }

    [Fact]
    public async Task UpdateAsync_WithNonExistingUser_ReturnsFail()
    {
        _userRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((User?)null);

        var result = await _sut.UpdateAsync(Guid.NewGuid(), DtoFakers.ValidUpdateUserDto());

        Assert.False(result.Success);
    }

    // ─── DeleteAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task DeleteAsync_WithExistingUser_ReturnsSuccess()
    {
        var id = Guid.NewGuid();
        _userRepository.ExistsAsync(id, Arg.Any<CancellationToken>()).Returns(true);
        _userRepository.DeleteAsync(id, Arg.Any<CancellationToken>()).Returns(Task.FromResult(true));

        var result = await _sut.DeleteAsync(id);

        Assert.True(result.Success);
        Assert.True(result.Data);
    }

    [Fact]
    public async Task DeleteAsync_WithNonExistingUser_ReturnsFail()
    {
        _userRepository.ExistsAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(false);

        var result = await _sut.DeleteAsync(Guid.NewGuid());

        Assert.False(result.Success);
    }
}
