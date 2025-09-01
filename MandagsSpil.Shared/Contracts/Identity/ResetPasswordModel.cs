using System;

namespace MandagsSpil.Shared.Contracts.Identity;
public record ForgotPasswordModel(string Email);
public record ResetPasswordModel(string Email, string NewPassword, string Token);