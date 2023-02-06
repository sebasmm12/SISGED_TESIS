using FluentValidation;
using SISGED.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SISGED.Shared.Validators
{
    public class UserSelfRegisterValidator : AbstractValidator<UserSelfRegisterDTO>
    {

        public UserSelfRegisterValidator()
        {
            RuleFor(x => x.Username)
                .NotNull()
                .WithMessage("Debe ingresar su nombre de usuario.")
                .NotEmpty()
                .WithMessage("Debe ingresar el nombre de usuario.")
                .Matches("^[^ ]*$")
                .WithMessage("Nombre de usuario no debe contener espacios.");

            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Debe ingresar su nombre.")
                .Matches("^[^\\d]*$")
                .WithMessage("Nombre no debe contener números.");

            RuleFor(x => x.LastName)
                .NotEmpty()
                .WithMessage("Debe ingresar su apellido.")
                .Matches("^[^\\d]*$")
                .WithMessage("Apellidos no debe contener números.");

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("Debe ingresar la contraseña.");

            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Debe ingresar su correo.")
                .EmailAddress()
                .WithMessage("Se requiere un correo electrónico válido.");

            RuleFor(x => x.DocumentType)
                .NotNull()
                .WithMessage("Debe ingresar el tipo de documento.");
            
            RuleFor(x => x.BornDate)
                .NotNull()
                .WithMessage("Debe ingresar la fecha de nacimiento.")
                .Must(BeAValidAge)
                .WithMessage("Debe ser mayor de edad.");


            RuleFor(x => x.DocumentNumber)
                .NotEmpty()
                .WithMessage("Debe ingresar un documento de identidad");
            
            RuleFor(x => x.Address)
                .NotEmpty()
                .WithMessage("Debe ingresar una dirección");

            When(x => x.DocumentType.Name == "DNI", () =>
            {
                RuleFor(x => x.DocumentNumber).Length(8).WithMessage("Debe ingresar 8 dígitos para este tipo de documento.");
                RuleFor(x => x.DocumentNumber).Matches("^[^\\D^]*$").WithMessage("Debe ingresar solo dígitos para este tipo de documento.");
            }).Otherwise(() =>
            {
                RuleFor(x => x.DocumentNumber).MaximumLength(12).WithMessage("Debe ingresar como máximo 12 caracteres para este tipo de documento.");
                RuleFor(x => x.DocumentNumber).Matches("^[\\d\\w^\\S]*$").WithMessage("Debe ingresar solo números o letras para este tipo de documento.");
            });
        }

        public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
        {
            var result = await ValidateAsync(ValidationContext<UserSelfRegisterDTO>
                    .CreateWithOptions((UserSelfRegisterDTO)model, x => x.IncludeProperties(propertyName)));

            if (result.IsValid)
                return Array.Empty<string>();

            return result.Errors.Select(error => error.ErrorMessage);
        };

        protected static bool BeAValidAge(DateTime? date)
        {
            DateTime current = DateTime.UtcNow.AddHours(-5).AddYears(-18);
            DateTime birth = date.GetValueOrDefault();

            if (birth == default)
            {
                return false;
            }

            if (current >= birth)
            {
                return true;
            }

            return false;
        }
    }
}
