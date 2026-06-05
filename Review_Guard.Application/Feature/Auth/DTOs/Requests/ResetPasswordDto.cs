namespace Review_Guard.Application.Feature.Auth.DTOs.Requests;

public record ResetPasswordDto(string Email, string Code, string NewPassword, string ConfirmPassword);
