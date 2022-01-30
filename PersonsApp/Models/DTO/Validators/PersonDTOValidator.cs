using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PersonsApp.Models.DTO.Validators
{
    public class PersonDTOValidator : AbstractValidator<Person>
    {
        public PersonDTOValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty()
                .MaximumLength(60);

            RuleFor(x => x.LastName)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.PhoneNumber)
                .NotEmpty()
                .MaximumLength(10)
                .Matches("^0[0-9]{9}$").WithMessage("Phone is not valid!");

            RuleFor(x => x.Email)
                .NotEmpty()
                .MaximumLength(100)
                .EmailAddress();

            RuleFor(x => x.Birthday)
                .NotEmpty();
        }
    }
}
