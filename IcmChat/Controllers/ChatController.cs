using IcmChat.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.AI;

namespace IcmChat.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController(IChatClient chatClient) : ControllerBase
    {
        private readonly IChatClient _chatClient = chatClient;
      
        [HttpPost]
        public async Task<Models.ChatResponse> SendQuery([FromBody] ChatRequest request)
        {
            List<ChatMessage> chatMessages = [
                new ChatMessage(ChatRole.System, "You're Chat system"),
                new ChatMessage(ChatRole.User, request.Query)
                ];
           var response=await _chatClient.GetResponseAsync(chatMessages,new ChatOptions());

            // For demonstration purposes, we will just echo the query back in the response.
            return new Models.ChatResponse()
            {
                Message =response.Text,
                status = "success"
            };
          
        }
    }
}
