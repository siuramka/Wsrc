using System.ComponentModel.DataAnnotations;

namespace Wsrc.Domain.Models.Chatmessages.Parameters;

public class MessageGetAllParameters
{
    public required string Channel { get; set; }

    public string? SenderUsername { get; set; }
}