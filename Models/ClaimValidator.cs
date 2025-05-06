using FluentValidation;
using CMCS_Auto.Models;

public class ClaimValidator : AbstractValidator<Claim>
{
    public ClaimValidator()
    {
        RuleFor(c => c.HoursWorked)
            .GreaterThan(0).WithMessage("Hours Worked must be greater than 0");

        RuleFor(c => c.HourlyRate)
            .GreaterThan(0).WithMessage("Hourly Rate must be greater than 0");

        RuleFor(c => c.Status)
            .NotEmpty().WithMessage("Status is required");

        RuleFor(c => c.SubmissionDate)
            .LessThanOrEqualTo(DateTime.Now).WithMessage("Submission date cannot be in the future");
    }
}
