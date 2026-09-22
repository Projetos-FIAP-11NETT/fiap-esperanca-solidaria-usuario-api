using FiapEsperancaSolidaria.Usuario.Contract.Dto.Response;
using MediatR;

namespace FiapEsperancaSolidaria.Usuario.Application.UserFeature.Commands.UploadUserImage;

public sealed record UploadUserImageCommand(
    Stream Content,
    string FileName,
    string ContentType) : IRequest<UploadUserImageResponse>;
