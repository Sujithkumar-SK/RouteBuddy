using FluentValidation;
using Kanini.RouteBuddy.Application.Dto.BusPhoto;

namespace Kanini.RouteBuddy.Application.Validators;

public class CreateBusPhotoDtoValidator : AbstractValidator<CreateBusPhotoDto>
{
    public CreateBusPhotoDtoValidator()
    {
        RuleFor(x => x.BusId)
            .GreaterThan(0)
            .WithMessage("Bus ID must be a positive number");

        RuleFor(x => x.Photo)
            .NotNull()
            .WithMessage("Photo is required")
            .Must(BeValidImageFile)
            .WithMessage("Photo must be a valid image file (JPEG, PNG, or GIF)")
            .Must(BeValidFileSize)
            .WithMessage("Photo size cannot exceed 5MB");

        RuleFor(x => x.Caption)
            .MaximumLength(100)
            .WithMessage("Caption cannot exceed 100 characters");
    }

    private bool BeValidImageFile(Microsoft.AspNetCore.Http.IFormFile? file)
    {
        if (file == null) return false;

        var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/gif" };
        return allowedTypes.Contains(file.ContentType.ToLower());
    }

    private bool BeValidFileSize(Microsoft.AspNetCore.Http.IFormFile? file)
    {
        if (file == null) return false;

        const long maxSizeBytes = 5 * 1024 * 1024; // 5MB
        return file.Length <= maxSizeBytes;
    }
}

public class UpdateBusPhotoDtoValidator : AbstractValidator<UpdateBusPhotoDto>
{
    public UpdateBusPhotoDtoValidator()
    {
        RuleFor(x => x.Caption)
            .MaximumLength(100)
            .WithMessage("Caption cannot exceed 100 characters");
    }
}