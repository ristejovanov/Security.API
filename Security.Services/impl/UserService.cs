using AutoMapper;
using Microsoft.Extensions.Logging;
using Security.DataServices.interfaces;
using Security.DataServices.interfaces.Helpers;
using Security.Domain;
using Security.Repositories.interfaces;
using Security.Shared;
using Security.Shared.Dtos;
using Security.Shared.Requests;

namespace Security.DataServices.impl
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository; 
        private readonly IHelper _passwordHelper;
        private readonly IMapper _mapper;
        private readonly ILogger<UserService> _logger;

        public UserService(IUserRepository userRepository, IHelper passwordHelper, IMapper mapper, ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _passwordHelper = passwordHelper;
            _mapper = mapper;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task<CreateUserDto> Create(UserRequest userDto)
        {

            var email = userDto.Email.Trim().ToLowerInvariant();
            var userName = userDto.UserName.Trim();

            if (await _userRepository.EmailExists(email))
                throw new ConflictException("A user with this email already exists.");
            if (await _userRepository.UserNameExists(userName))
                throw new ConflictException("A user with this username already exists.");


            var(hash, salt) = _passwordHelper.Hash(userDto.Password);

            var entity = new User
            {
                Id = Guid.NewGuid(),
                UserName = userName,
                FullName = userDto.FullName.Trim(),
                Email = email,
                MobileNumber =  userDto.MobileNumber.Trim(),
                Language = userDto.Language,
                Culture = userDto.Culture,
                PasswordHash = hash,
                PasswordSalt = salt
            };

            await _userRepository.Add(entity);
            _logger.LogInformation("New user created: {UserName}, ID={Id}", entity.UserName, entity.Id);

            return _mapper.Map<CreateUserDto>(entity);
        }

        /// <inheritdoc />
        public async Task<ReturnUserDto?> GetById(Guid id)
        {
            var entity = await _userRepository.GetById(id) ?? throw new NotFoundException("User not found.");
            return _mapper.Map<ReturnUserDto>(entity);
        }

        /// <inheritdoc />
        public async Task<bool> Update(Guid id, UserRequest userRequest)
        {
            var entity = await _userRepository.GetById(id) ?? throw new NotFoundException("User not found.");

            if (userRequest.Email != entity.Email && await _userRepository.EmailExists(userRequest.Email))
                throw new ConflictException("A user with this email already exists.");
            if (userRequest.UserName != entity.UserName&& await _userRepository.UserNameExists(userRequest.UserName))
                throw new ConflictException("A user with this username already exists.");

            entity.UserName = userRequest.UserName;
            entity.Email = userRequest.Email;
            entity.FullName = userRequest.FullName.Trim();
            entity.MobileNumber = userRequest.MobileNumber.Trim(); 
            entity.Language = userRequest.Language;
            entity.Culture = userRequest.Culture!;

            var (hash, salt) = _passwordHelper.Hash(userRequest.Password);
            entity.PasswordHash = hash;
            entity.PasswordSalt = salt;

            var response = await _userRepository.Update(entity);
            _logger.LogInformation("User updated: {UserName}, ID={Id}", entity.UserName, entity.Id);
            return response;
        }

        /// <inheritdoc />
        public async Task<bool> Delete(Guid id)
        {
            var ok = await _userRepository.Delete(id);
            if (!ok) throw new NotFoundException("User not found.");

            _logger.LogInformation("User deleted: ID={Id}", id);
            return ok;
        }

        /// <inheritdoc />
        public async Task<bool> ValidatePassword(Guid userId, ValidatePasswordRequest request)
        {
            var u = await _userRepository.GetById(userId);
            if (u is null) throw new NotFoundException("User not found.");

            return _passwordHelper.Verify(request.Password, u.PasswordSalt, u.PasswordHash);
        }
    }
}
