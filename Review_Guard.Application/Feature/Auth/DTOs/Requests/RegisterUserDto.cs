namespace Review_Guard.Application.Feature.Auth.DTOs.Requests;

public record RegisterUserDto
(
     string FullName,
     string Email,
     string Password,
     string ConfirmPassword
);

