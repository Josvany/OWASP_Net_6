# Secure Coding with OWASP in ASP.NET Core 6 or 7
--- 
## 1. Authentication:
    - Paquetes a instalar 
        dotnet add package Microsoft.VisualStudio.Web.CodeGeneration.Design
        dotnet tool install -g dotnet-aspnet-codegenerator
        dotnet add package Microsoft.VisualStudio.Web.CodeGeneration.Design
        dotnet add package Microsoft.EntityFrameworkCore.Design
        dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore
        dotnet add package Microsoft.AspNetCore.Identity.UI
        dotnet add package Microsoft.EntityFrameworkCore.Sqlite
        dotnet add package Microsoft.EntityFrameworkCore.Tools

    - Listar las plantillas o scaffols de microsoft identity
        dotnet aspnet-codegenerator identity --listFiles --no-build

    - Usar plantilla o scaffolding (para registrar usuarios)
        dotnet aspnet-codegenerator identity -dc Globomantics.Survey.Data.IdentityDbContext(ruta del proyecto donde se colocara dichas plantillas) --files "Account.Register;Account.Login;Account.Logout" --useSqLite


    - Crear la migracion de ef identity
       dotnet ef migrations add identity --context IdentityDbContext
       dotnet ef database update --context IdentityDbContext

    - Confirmación de E-Mail
      dotnet aspnet-codegenerator identity -dc Globomantics.Survey.Data.IdentityDbContext --files "Account.RegisterConfirmation;Account.ResendEmailConfirmation" --useSqLite

    -  Password Reset
       dotnet aspnet-codegenerator identity -dc Globomantics.Survey.Data.IdentityDbContext --files "Account.ResetPassword;Account.ResetPasswordConfirmation" --useSqLite
    
    - Two Factor authentication 
      dotnet aspnet-codegenerator identity -dc Globomantics.Survey.Data.IdentityDbContext --files "Account.Manage.Disable2fa;Account.Manage.TwoFactorAuthentication" --useSqLite

## 2. Session Management
    - Crear pin para ingresar a la pagina
      dotnet aspnet-codegenerator identity -dc Globomantics.Survey.Data.IdentityDbContext --files "Account.LoginWith2fa" --useSqLite 