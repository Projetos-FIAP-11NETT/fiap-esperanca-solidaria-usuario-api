using FiapEsperancaSolidaria.Usuario.Contract.Dto.Response;
using FiapEsperancaSolidaria.Usuario.Domain.Contracts.Storage;
using MediatR;

namespace FiapEsperancaSolidaria.Usuario.Application.UserFeature.Commands.UploadUserImage;

public class UploadUserImageCommandHandler(
        IImageStorageService imageStorageService
    ) : IRequestHandler<UploadUserImageCommand, UploadUserImageResponse>
{
    public async Task<UploadUserImageResponse> Handle(
        UploadUserImageCommand request,
        CancellationToken cancellationToken)
    {
        var url = await imageStorageService.UploadAsync(
            request.Content,
            request.FileName,
            request.ContentType,
            cancellationToken);

        return new UploadUserImageResponse { Url = url };
    }
}
