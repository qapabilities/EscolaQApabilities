using EscolaQApabilities.StudentService.Domain.Entities;
using EscolaQApabilities.StudentService.Domain.Enums;
using EscolaQApabilities.StudentService.Domain.Exceptions;
using FluentAssertions;
using Xunit;

namespace EscolaQApabilities.StudentService.Tests.Domain;

public class StudentTests
{
    [Fact]
    public void CreateStudent_WithValidData_ShouldCreateSuccessfully()
    {
        // Arrange
        var name = "João Silva";
        var email = "joao.silva@email.com";
        var phone = "11987654321";
        var birthDate = new DateTime(2000, 1, 1);
        var address = "Rua das Flores, 123";
        var city = "São Paulo";
        var state = "SP";
        var zipCode = "01234567";

        // Act
        var student = new Student(name, email, phone, birthDate, address, city, state, zipCode);

        // Assert
        student.Should().NotBeNull();
        student.Name.Should().Be(name);
        student.Email.Should().Be(email.ToLower());
        student.Phone.Should().Be(phone);
        student.BirthDate.Should().Be(birthDate);
        student.Address.Should().Be(address);
        student.City.Should().Be(city);
        student.State.Should().Be(state);
        student.ZipCode.Should().Be(zipCode);
        student.Status.Should().Be(StudentStatus.Active);
        student.EnrollmentDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Theory]
    [InlineData("", "Email é obrigatório")]
    [InlineData(" ", "Email é obrigatório")]
    [InlineData("joao", "Email inválido")]
    [InlineData("joao@", "Email inválido")]
    [InlineData("@email.com", "Email inválido")]
    [InlineData("joao@email", "Email inválido. Formato correto: email@dominio.com")]
    public void CreateStudent_WithInvalidEmail_ShouldThrowException(string email, string expectedError)
    {
        // Arrange
        var name = "João Silva";
        var phone = "11987654321";
        var birthDate = new DateTime(2000, 1, 1);
        var address = "Rua das Flores, 123";
        var city = "São Paulo";
        var state = "SP";
        var zipCode = "01234567";

        // Act & Assert
        var action = () => new Student(name, email, phone, birthDate, address, city, state, zipCode);
        action.Should().Throw<StudentDomainException>().WithMessage($"*{expectedError}*");
    }

    [Theory]
    [MemberData(nameof(InvalidNameData))]
    public void CreateStudent_WithInvalidName_ShouldThrowException(string name, string expectedError)
    {
        // Arrange
        var email = "joao.silva@email.com";
        var phone = "11987654321";
        var birthDate = new DateTime(2000, 1, 1);
        var address = "Rua das Flores, 123";
        var city = "São Paulo";
        var state = "SP";
        var zipCode = "01234567";

        // Act & Assert
        var action = () => new Student(name, email, phone, birthDate, address, city, state, zipCode);
        action.Should().Throw<StudentDomainException>().WithMessage($"*{expectedError}*");
    }

    public static IEnumerable<object[]> InvalidNameData => new[]
    {
        new object[] { "", "Nome é obrigatório" },
        new object[] { "J", "Nome deve ter entre 2 e 100 caracteres" },
        new object[] { new string('A', 101), "Nome deve ter entre 2 e 100 caracteres" }
    };

    [Fact]
    public void CreateStudent_WithFutureBirthDate_ShouldThrowException()
    {
        // Arrange
        var name = "João Silva";
        var email = "joao.silva@email.com";
        var phone = "11987654321";
        var birthDate = DateTime.Today.AddDays(1);
        var address = "Rua das Flores, 123";
        var city = "São Paulo";
        var state = "SP";
        var zipCode = "01234567";

        // Act & Assert
        var action = () => new Student(name, email, phone, birthDate, address, city, state, zipCode);
        action.Should().Throw<StudentDomainException>().WithMessage("*Data de nascimento*");
    }

    [Fact]
    public void UpdatePersonalInfo_WithValidData_ShouldUpdateSuccessfully()
    {
        // Arrange
        var student = CreateValidStudent();
        var newName = "João Silva Santos";
        var newEmail = "joao.santos@email.com";
        var newPhone = "11987654322";
        var newBirthDate = new DateTime(1995, 5, 15);
        var newAddress = "Av. Paulista, 1000";
        var newCity = "São Paulo";
        var newState = "SP";
        var newZipCode = "01310100";

        // Act
        student.UpdatePersonalInfo(newName, newEmail, newPhone, newBirthDate, newAddress, newCity, newState, newZipCode);

        // Assert
        student.Name.Should().Be(newName);
        student.Email.Should().Be(newEmail.ToLower());
        student.Phone.Should().Be(newPhone);
        student.BirthDate.Should().Be(newBirthDate);
        student.Address.Should().Be(newAddress);
        student.City.Should().Be(newCity);
        student.State.Should().Be(newState);
        student.ZipCode.Should().Be(newZipCode);
    }

    [Fact]
    public void UpdateContactInfo_WithValidData_ShouldUpdateSuccessfully()
    {
        // Arrange
        var student = CreateValidStudent();
        var parentName = "Maria Silva";
        var parentPhone = "11987654323";
        var parentEmail = "maria.silva@email.com";
        var emergencyContact = "José Silva";
        var emergencyPhone = "11987654324";

        // Act
        student.UpdateContactInfo(parentName, parentPhone, parentEmail, emergencyContact, emergencyPhone);

        // Assert
        student.ParentName.Should().Be(parentName);
        student.ParentPhone.Should().Be(parentPhone);
        student.ParentEmail.Should().Be(parentEmail.ToLower());
        student.EmergencyContact.Should().Be(emergencyContact);
        student.EmergencyPhone.Should().Be(emergencyPhone);
    }

    [Fact]
    public void Activate_WhenInactive_ShouldActivateSuccessfully()
    {
        // Arrange
        var student = CreateValidStudent();
        student.Deactivate();

        // Act
        student.Activate();

        // Assert
        student.Status.Should().Be(StudentStatus.Active);
    }

    [Fact]
    public void Activate_WhenAlreadyActive_ShouldThrowException()
    {
        // Arrange
        var student = CreateValidStudent();

        // Act & Assert
        var action = () => student.Activate();
        action.Should().Throw<StudentDomainException>().WithMessage("*já está ativo*");
    }

    [Fact]
    public void Deactivate_WhenActive_ShouldDeactivateSuccessfully()
    {
        // Arrange
        var student = CreateValidStudent();

        // Act
        student.Deactivate();

        // Assert
        student.Status.Should().Be(StudentStatus.Inactive);
    }

    [Fact]
    public void Deactivate_WhenAlreadyInactive_ShouldThrowException()
    {
        // Arrange
        var student = CreateValidStudent();
        student.Deactivate();

        // Act & Assert
        var action = () => student.Deactivate();
        action.Should().Throw<StudentDomainException>().WithMessage("*já está inativo*");
    }

    [Fact]
    public void Suspend_WhenActive_ShouldSuspendSuccessfully()
    {
        // Arrange
        var student = CreateValidStudent();

        // Act
        student.Suspend();

        // Assert
        student.Status.Should().Be(StudentStatus.Suspended);
    }

    [Fact]
    public void GetAge_ShouldReturnCorrectAge()
    {
        // Arrange
        var birthDate = DateTime.Today.AddYears(-25);
        var student = new Student(
            "João Silva",
            "joao.silva@email.com",
            "11987654321",
            birthDate,
            "Rua das Flores, 123",
            "São Paulo",
            "SP",
            "01234567");

        // Act
        var age = student.GetAge();

        // Assert
        age.Should().Be(25);
    }

    [Fact]
    public void IsMinor_WhenUnder18_ShouldReturnTrue()
    {
        // Arrange
        var birthDate = DateTime.Today.AddYears(-17);
        var student = new Student(
            "João Silva",
            "joao.silva@email.com",
            "11987654321",
            birthDate,
            "Rua das Flores, 123",
            "São Paulo",
            "SP",
            "01234567");

        // Act
        var isMinor = student.IsMinor();

        // Assert
        isMinor.Should().BeTrue();
    }

    [Fact]
    public void IsMinor_WhenOver18_ShouldReturnFalse()
    {
        // Arrange
        var birthDate = DateTime.Today.AddYears(-19);
        var student = new Student(
            "João Silva",
            "joao.silva@email.com",
            "11987654321",
            birthDate,
            "Rua das Flores, 123",
            "São Paulo",
            "SP",
            "01234567");

        // Act
        var isMinor = student.IsMinor();

        // Assert
        isMinor.Should().BeFalse();
    }

    private static Student CreateValidStudent()
    {
        return new Student(
            "João Silva",
            "joao.silva@email.com",
            "11987654321",
            new DateTime(2000, 1, 1),
            "Rua das Flores, 123",
            "São Paulo",
            "SP",
            "01234567");
    }
} 