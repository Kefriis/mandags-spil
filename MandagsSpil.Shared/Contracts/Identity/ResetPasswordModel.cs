using System;

namespace MandagsSpil.Shared.Contracts.Identity;
public record ForgotPasswordModel(string Email, string CallBackUrl);
public record ResetPasswordModel(string Email, string NewPassword, string Token);