using Vogel.Rentals.Application.Contracts;
using Vogel.Rentals.Domain.Entities;

namespace Vogel.Rentals.Application.Abstractions;

public interface ICourierValidator
{
    Courier ValidateAndNormalizeCreate(CreateCourierRequest? req);
    void ValidateUploadCnhImage(string id, UpdateCourierCnhImageRequest? req);
}