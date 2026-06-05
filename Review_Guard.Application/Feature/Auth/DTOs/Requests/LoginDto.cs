namespace Review_Guard.Application.Feature.Auth.DTOs.Requests;

public record LoginDto(
    string Email,
    string Password
);
