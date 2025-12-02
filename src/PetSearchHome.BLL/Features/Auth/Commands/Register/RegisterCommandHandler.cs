using MediatR;
using PetSearchHome.DAL.Contracts.Persistence; // ? Потрібно для доступу до бази
using PetSearchHome.DAL.Domain.Entities;       // ? Потрібно для створення юзера
using PetSearchHome.DAL.Domain.Enums;
using PetSearchHome.BLL.DTOs;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PetSearchHome.BLL.Features.Auth.Commands.Register;

public class RegisterCommandHandler :
    IRequestHandler<RegisterIndividualCommand, LoginResultDto>,
    IRequestHandler<RegisterShelterCommand, LoginResultDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    // ? Інжектуємо репозиторії через конструктор
    public RegisterCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    // --- Обробка реєстрації людини ---
    public async Task<LoginResultDto> Handle(RegisterIndividualCommand request, CancellationToken cancellationToken)
    {
        // 1. ? ПЕРЕВІРКА НА ДУБЛІКАТ (Це виправить тест!)
        var existingUser = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (existingUser != null)
        {
            throw new Exception($"Користувач з email '{request.Email}' вже існує.");
        }

        // 2. Створення сутності користувача
        var newUser = new RegisteredUser
        {
            Email = request.Email,
            // УВАГА: Тут має бути хешування пароля (наприклад, через BCrypt), але поки що так:
            PasswordHash = request.Password,
            UserType = UserType.individual,
            IsAdmin = false,
            IsActive = true,

            // Створення профілю
            IndividualProfile = new IndividualProfile
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Phone = request.Phone,
                City = request.City,
                District = request.District
            }
        };

        // 3. Збереження в базу
        await _userRepository.AddAsync(newUser, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 4. Повернення результату
        return new LoginResultDto
        {
            IsSuccess = true,
            User = new UserProfileDto
            {
                Id = newUser.Id,
                Email = newUser.Email,
                IsAdmin = newUser.IsAdmin,
                // Заповнюємо DTO для відповіді
                IndividualProfile = new IndividualProfileDto
                {
                    FirstName = newUser.IndividualProfile.FirstName,
                    LastName = newUser.IndividualProfile.LastName
                }
            },
            Token = "fake-jwt-token" // Генерацію токена додасте пізніше
        };
    }

    // --- Обробка реєстрації притулку ---
    public async Task<LoginResultDto> Handle(RegisterShelterCommand request, CancellationToken cancellationToken)
    {
        // 1. ? Перевірка на дублікат
        var existingUser = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (existingUser != null)
        {
            throw new Exception($"Користувач з email '{request.Email}' вже існує.");
        }

        // 2. Створення сутності
        var newUser = new RegisteredUser
        {
            Email = request.Email,
            PasswordHash = request.Password, // Додайте хешування!
            UserType = UserType.shelter,
            IsAdmin = false,
            IsActive = true,

            ShelterProfile = new ShelterProfile
            {
                Name = request.Name,
                ContactPerson = request.ContactPerson,
                Phone = request.Phone,
                Address = request.Address
            }
        };

        // 3. Збереження
        await _userRepository.AddAsync(newUser, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new LoginResultDto
        {
            IsSuccess = true,
            User = new UserProfileDto
            {
                Id = newUser.Id,
                Email = newUser.Email,
                IsAdmin = newUser.IsAdmin,
                ShelterProfile = new ShelterProfileDto { Name = newUser.ShelterProfile.Name }
            },
            Token = "fake-jwt-token"
        };
    }
}