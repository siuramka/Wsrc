using Wsrc.Api.Business.Mappings;
using Wsrc.Domain.Models.Chatmessages;
using Wsrc.Domain.Repositories;

namespace Wsrc.Api.Business.Services;

public class MessageService(MessageRepository messageRepository)
{
    public async Task<IEnumerable<MessageDto>> GetAllAsync(string? channelName, string? senderUsername)
    {
        var messages = await messageRepository.GetAllAsync(channelName, senderUsername);

        return MessageMapper.ToDtoList(messages);
    }
}