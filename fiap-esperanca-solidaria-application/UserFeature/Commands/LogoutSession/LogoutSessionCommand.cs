using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace FiapEsperancaSolidaria.Usuario.Application.UserFeature.Commands.LogoutSession;

public sealed record class LogoutSessionCommand(Guid SessionId) : IRequest<bool>;
