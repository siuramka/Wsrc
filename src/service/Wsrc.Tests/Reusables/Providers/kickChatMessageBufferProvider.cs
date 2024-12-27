namespace Wsrc.Tests.Reusables.Providers;

public class kickChatMessageBufferProvider
{
    public string ProvideDeserialized(string channelId)
    {
        var randomMessageContent = Guid.NewGuid().ToString();
        var randomUser = Guid.NewGuid().ToString();

        return
            """{"event":"App\\Events\\ChatMessageEvent","data":"{\"id\":\"cf8905aa-9f75-4a91-96c7-712f628891d6\",\"chatroom_id\":${channelId},\"content\":\"${messageContent}\",\"type\":\"message\",\"created_at\":\"2024-12-27T14:57:16+00:00\",\"sender\":{\"id\":10559454,\"username\":\"${user}\",\"slug\":\"${user}\",\"identity\":{\"color\":\"#75FD46\",\"badges\":[{\"type\":\"moderator\",\"text\":\"Moderator\"},{\"type\":\"verified\",\"text\":\"Verified channel\"}]}}}","channel":"chatrooms.${channelId}.v2"}"""
                .Replace("${channelId}", channelId)
                .Replace("${user}", randomUser)
                .Replace("${messageContent}", randomMessageContent);
    }
}