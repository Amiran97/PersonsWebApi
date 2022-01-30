using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PersonsApp.Models.DTO.Validators
{
    public class AccountCredentialsDTOValidator : AbstractValidator<AccountCredentialsDTO>
    {
        public AccountCredentialsDTOValidator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.Password).NotEmpty(); 
            RuleFor(x => x.Username).NotEmpty();
        }
    }
}
